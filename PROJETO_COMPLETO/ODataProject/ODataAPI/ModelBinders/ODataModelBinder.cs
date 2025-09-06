using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.OData.Deltas;
using System.Text.Json;

namespace ODataAPI.ModelBinders
{
    public class ODataModelBinder : IModelBinder
    {
        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
                throw new ArgumentNullException(nameof(bindingContext));

            var request = bindingContext.HttpContext.Request;
            
            if (request.ContentType?.Contains("application/json") == true)
            {
                request.EnableBuffering();
                request.Body.Position = 0;
                
                using var reader = new StreamReader(request.Body);
                var json = await reader.ReadToEndAsync();
                
                if (!string.IsNullOrEmpty(json))
                {
                    var options = new JsonSerializerOptions 
                    { 
                        PropertyNameCaseInsensitive = true 
                    };
                    
                    // Verifica se é um tipo Delta para operações PATCH
                    if (bindingContext.ModelType.IsGenericType && 
                        bindingContext.ModelType.GetGenericTypeDefinition() == typeof(Delta<>))
                    {
                        var entityType = bindingContext.ModelType.GetGenericArguments()[0];
                        var deltaInstance = Activator.CreateInstance(bindingContext.ModelType, entityType);
                        var delta = (IDelta)deltaInstance!;
                        
                        // Processa o JSON diretamente para extrair apenas as propriedades presentes
                        var jsonDoc = JsonDocument.Parse(json);
                        foreach (var property in jsonDoc.RootElement.EnumerateObject())
                        {
                            var propInfo = entityType.GetProperty(property.Name, 
                                System.Reflection.BindingFlags.IgnoreCase | 
                                System.Reflection.BindingFlags.Public | 
                                System.Reflection.BindingFlags.Instance);
                            
                            if (propInfo != null && propInfo.CanWrite)
                            {
                                var value = JsonSerializer.Deserialize(property.Value.GetRawText(), propInfo.PropertyType, options);
                                delta.TrySetPropertyValue(propInfo.Name, value);
                            }
                        }
                        
                        bindingContext.Result = ModelBindingResult.Success(delta);
                        return;
                    }
                    else
                    {
                        // Comportamento normal para tipos não-Delta
                        var result = JsonSerializer.Deserialize(json, bindingContext.ModelType, options);
                        
                        // Converter DateTime para UTC se necessário
                        ConvertDateTimeToUtc(result);
                        
                        bindingContext.Result = ModelBindingResult.Success(result);
                        return;
                    }
                }
            }
            
            bindingContext.Result = ModelBindingResult.Failed();
        }
        
        private static void ConvertDateTimeToUtc(object? obj)
        {
            if (obj == null) return;
            
            var properties = obj.GetType().GetProperties()
                .Where(p => p.PropertyType == typeof(DateTime) && p.CanWrite);
                
            foreach (var prop in properties)
            {
                var value = (DateTime)prop.GetValue(obj)!;
                if (value.Kind == DateTimeKind.Unspecified)
                {
                    prop.SetValue(obj, DateTime.SpecifyKind(value, DateTimeKind.Utc));
                }
            }
        }
    }
}