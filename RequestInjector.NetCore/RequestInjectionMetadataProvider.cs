using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

namespace RequestInjector.NetCore
{
    public class RequestInjectionMetadataProvider : IMetadataDetailsProvider, IDisplayMetadataProvider
    {
        public void CreateDisplayMetadata(DisplayMetadataProviderContext context)
        {
            if (context.Key.MetadataKind == ModelMetadataKind.Type)
            {
                context.DisplayMetadata.ConvertEmptyStringToNull = false;
            }
        }
    }
}
