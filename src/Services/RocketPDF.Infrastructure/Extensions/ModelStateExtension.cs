using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace RocketPDF.Infrastructure.Extensions
{
    public static class ModelStateExtension
    {
        public static string GetStringError(this ModelStateDictionary modelState)
        {
            return modelState.GetStringErrors().FirstOrDefault();
        }

        public static IEnumerable<string> GetStringErrors(this ModelStateDictionary modelState)
        {
            return modelState.Values.SelectMany(v => v.Errors).Select(v => v.ErrorMessage + " " + v.Exception).ToList();
        }
    }
}