using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Linq;
using System.Threading.Tasks;

namespace RequestInjector.NetCore
{
    public class QueryModelBinderProvider : IModelBinderProvider
    {
        IServiceCollection collection;

        public QueryModelBinderProvider(IServiceCollection collection)
        {
            this.collection = collection;
        }

        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context?.BindingInfo?.BindingSource == BindingSource.Query)
                return new QueryModelBinder(collection);

            return null;
        }
    }

    public class QueryModelBinder : IModelBinder
    {
        IServiceCollection collection;

        public QueryModelBinder(IServiceCollection collection)
        {
            this.collection = collection;
        }

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var provider = collection.BuildServiceProvider();
            var modelInstance = provider.GetService(bindingContext.ModelType);
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
