using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace RequestInjector.NetCore
{
    public class RequestInjectionHandler<T> : CustomCreationConverter<T>
    {
        IServiceCollection collection;
        IServiceProvider provider;

        public RequestInjectionHandler(IServiceCollection collection)
        {
            this.collection = collection;
            provider = collection.BuildServiceProvider();
        }

        public override T Create(Type objectType)
        {
            var httpContext = provider.GetService<IHttpContextAccessor>();

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
