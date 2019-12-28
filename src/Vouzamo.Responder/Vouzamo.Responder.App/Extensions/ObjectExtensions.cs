using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Vouzamo.Responder.App.Extensions
{
    public static class ObjectExtensions
    {
        public static T DeepClone<T>(this T source)
        {
            var formatter = new BinaryFormatter();
            var stream = new MemoryStream();

            using (stream)
            {
                formatter.Serialize(stream, source);
                stream.Seek(0, SeekOrigin.Begin);
                return (T)formatter.Deserialize(stream);
            }
        }
    }
}
