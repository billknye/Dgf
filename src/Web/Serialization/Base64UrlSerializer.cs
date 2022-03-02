using Dgf.Framework.States;
using Dgf.Framework.States.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Dgf.Web.Serialization
{
    public class Base64UrlSerializer : IGameStateSerializer
    {
        public IGameState Deserialize(Type expectedType, string urlEncoded)
        {
            using var ms = new MemoryStream(Microsoft.AspNetCore.WebUtilities.Base64UrlTextEncoder.Decode(urlEncoded));
            using var rdr = new BinaryReaderEx(ms);
            var state = Activator.CreateInstance(expectedType) as IGameState;
            state.Read(rdr);
            return state;            
        }

        public string Serialize(IGameState obj)
        {
            using var ms = new MemoryStream();
            using var writer = new BinaryWriterEx(ms);                
            obj.Write(writer);
            return Microsoft.AspNetCore.WebUtilities.Base64UrlTextEncoder.Encode(ms.ToArray());
        }
    }
}
