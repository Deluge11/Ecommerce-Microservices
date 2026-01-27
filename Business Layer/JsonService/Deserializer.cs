using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Business_Layer.JsonService
{
    public static class Deserializer
    {
        public static T DeserializeBytesArray<T>(byte[] bytesArray)
        {
            var message = Encoding.UTF8.GetString(bytesArray);
            return JsonSerializer.Deserialize<T>(message);
        }
    }
}
