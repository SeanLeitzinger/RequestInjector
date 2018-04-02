using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace RequestInjector.NetCore
{
    public class RequestInjectionHandler<T> : CustomCreationConverter<T>
    {
        IServiceProvider provider;

        public RequestInjectionHandler(IServiceProvider provider)
        {
            this.provider = provider;
        }

        public override T Create(Type objectType)
        {
            var httpContext = provider.GetRequiredService<IHttpContextAccessor>();
            var scope = (IServiceScope)httpContext.HttpContext.Items["scope"];

            return (T)scope.ServiceProvider.GetRequiredService(objectType);
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
