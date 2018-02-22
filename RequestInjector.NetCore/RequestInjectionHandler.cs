using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace RequestInjector.NetCore
{
    public class RequestInjectionHandler<T> : CustomCreationConverter<T>
    {
        IServiceCollection collection;

        public RequestInjectionHandler(IServiceCollection collection)
        {
            this.collection = collection;
        }

        public override T Create(Type objectType)
        {
            var provider = collection.BuildServiceProvider();

            return (T)provider.GetService(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return null;

            var obj = Create(objectType);
            serializer.Populate(reader, obj);

            return obj;
        }
    }
}
