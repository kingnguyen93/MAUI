namespace RocketPDF.Core.Extensions
{
    public static class ViewModelMapper
    {
        public static string GetPageTypeName(this Type viewModelType)
        {
            return viewModelType.AssemblyQualifiedName
                .Replace("ViewModel", "Page")
                .Replace("Pages", "Views");
        }

        public static string GetViewModelTypeName(this Type viewType)
        {
            return viewType.AssemblyQualifiedName
                .Replace("Views", "ViewModels")
                .Replace("Page", "ViewModel");
        }
    }
}