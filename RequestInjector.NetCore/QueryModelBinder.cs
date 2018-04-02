using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RequestInjector.NetCore
{
    public class QueryModelBinderProvider : IModelBinderProvider
    {
        IServiceProvider provider;

        public QueryModelBinderProvider(IServiceProvider provider)
        {
            this.provider = provider;
        }

        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context?.BindingInfo?.BindingSource == BindingSource.Query)
                return new QueryModelBinder(provider);

            return null;
        }
    }

    public class QueryModelBinder : IModelBinder
    {
        IServiceProvider provider;

        public QueryModelBinder(IServiceProvider provider)
        {
            this.provider = provider;
        }

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var scope = (IServiceScope)bindingContext.HttpContext.Items["scope"];
            var modelInstance = scope.ServiceProvider.GetRequiredService(bindingContext.ModelType);
            var nameValuePairs = bindingContext.ActionContext.HttpContext.Request.Query.ToDictionary(m => m.Key, m => m.Value.FirstOrDefault());

            var json = JsonConvert.SerializeObject(nameValuePairs);

            JsonConvert.PopulateObject(json, modelInstance, new JsonSerializerSettings
            {
                Error = HandleDeserializationError
            });

            bindingContext.Result = ModelBindingResult.Success(modelInstance);

            return Task.CompletedTask;
        }

        private void HandleDeserializationError(object sender, Newtonsoft.Json.Serialization.ErrorEventArgs errorArgs)
        {
            var currentError = errorArgs.ErrorContext.Error.Message;
            errorArgs.ErrorContext.Handled = true;
        }
    }
}
