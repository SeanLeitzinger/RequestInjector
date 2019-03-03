using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RequestInjector.NetCore
{
    public class QueryModelBinder : IModelBinder
    {
        public QueryModelBinder()
        {
        }

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var modelInstance = bindingContext.HttpContext.RequestServices.GetRequiredService(bindingContext.ModelType);
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
