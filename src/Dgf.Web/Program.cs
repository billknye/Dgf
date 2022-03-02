using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Dgf.Framework;
using Dgf.Framework.States.Serialization;
using Dgf.Web.Serialization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();


builder.Services.AddSingleton<IGameStateSerializer, Base64UrlSerializer>();

// Make sure that all assemblies in the bin path are loaded so we can search for IGame classes
// This is a pretty terrible hack, I should find a good extensions dependency injection scanning solution            
LoadAllBinDirectoryAssemblies();

var games = new List<Type>();

foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
{
    try
    {
        foreach (var type in assembly.GetTypes())
        {
            try
            {
                if (typeof(IGame).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
                {
                    games.Add(type);
                }
            }
            catch { }
        }
    }
    catch { }
}

foreach (var game in games)
{
    builder.Services.AddSingleton(typeof(IGame), game);
}

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();
app.UseStaticFiles();


// TODO make some real support for games providing their own styling
foreach (var game in app.Services.GetServices<IGame>())
{
    var embeddedProvider = new EmbeddedFileProvider(game.GetType().Assembly, $"{game.GetType().Namespace}.Assets");

    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = embeddedProvider,
        RequestPath = $"/{game.Slug}/Assets"
    });
}

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();


// This is not great
static void LoadAllBinDirectoryAssemblies()
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