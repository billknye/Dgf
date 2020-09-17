using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Dgf.Framework;
using Dgf.Framework.States.Serialization;
using Dgf.Web.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;

namespace Dgf.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddSingleton<IGameStateSerializer, Base64UrlSerializer>();

            // Make sure that all assemblies in the bin path are loaded so we can search for IGame classes
            // This is a pretty terrible hack, I should find a good extensions dependency injection scanning solution            
            LoadAllBinDirectoryAssemblies();
            var games = AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(n => n.GetTypes())
                .Where(n => typeof(IGame).IsAssignableFrom(n) && !n.IsInterface && !n.IsAbstract);

            foreach (var game in games)
            {
                services.AddSingleton(typeof(IGame), game);
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IEnumerable<IGame> games)
        {
            // None of this code or state is secret or private, show all the details to all the people
            app.UseDeveloperExceptionPage();            

            app.UseStaticFiles();

            // TODO make some real support for games providing their own styling
            foreach (var game in games)
            {
                var embeddedProvider = new EmbeddedFileProvider(game.GetType().Assembly, $"{game.GetType().Namespace}.Assets");

                app.UseStaticFiles(new StaticFileOptions
                {
                    FileProvider = embeddedProvider,
                    RequestPath = $"/{game.Slug}/Assets"
                });
            }
            

            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }

        // This is not great
        private static void LoadAllBinDirectoryAssemblies()
        {
            string binPath = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory);
            foreach (string dll in Directory.GetFiles(binPath, "*.dll", SearchOption.AllDirectories))
            {
                try
                {
                    Assembly loadedAssembly = Assembly.LoadFrom(dll);
                }
                catch (FileLoadException loadEx)
                { } // The Assembly has already been loaded.
                catch (BadImageFormatException imgEx)
                { } // If a BadImageFormatException exception is thrown, the file is not an assembly.
            }
        }

    }
}
