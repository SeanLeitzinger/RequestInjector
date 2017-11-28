using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace RequestInjector.NetCore
{
    public class RequestInjectionHandler<T> : CustomCreationConverter<T>
    {
        IServiceProvider container;

        public RequestInjectionHandler(IServiceProvider serviceProvider)
        {
            this.container = serviceProvider;
        }

        public override T Create(Type objectType)
        {
            return (T)container.GetService(objectType);
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
