using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using System;

namespace RequestInjector.NetCore
{
    public class RequestInjectorModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context?.BindingInfo?.BindingSource == BindingSource.Query)
                return new QueryModelBinder();

            return null;
        }
    }
}
