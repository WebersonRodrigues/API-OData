using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.OData.Deltas;
using ODataAPI.Models;

namespace ODataAPI.ModelBinders
{
    public class ODataModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder? GetBinder(ModelBinderProviderContext context)
        {
            var modelType = context.Metadata.ModelType;
            const string NAMESPACE_MODEL = "ODataAPI.Models";

            if ((modelType.IsGenericType && modelType.GetGenericTypeDefinition() == typeof(Delta)) ||
                modelType.Namespace == NAMESPACE_MODEL)
                return new ODataModelBinder();
                        
            return null;
        }
    }
}