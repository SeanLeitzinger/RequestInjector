using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace RequestInjector.NetCore
{
    public class RequestInjectorHandler<T> : CustomCreationConverter<T>
    {
        IServiceProvider provider;

        public RequestInjectorHandler(IServiceProvider provider)
        {
            this.provider = provider;
        }

        public override T Create(Type objectType)
        {
            var httpContext = provider.GetRequiredService<IHttpContextAccessor>();

            return (T)httpContext.HttpContext.RequestServices.GetRequiredService(objectType);
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
