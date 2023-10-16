using System.Reflection;

namespace RocketPDF.Core.Helpers
{
    public static class RoutingHelper
    {
        public static void RegisterRoutes(Assembly[] assemblies, string pattern = "Page", params Type[] ignored)
        {
            foreach (var page in assemblies.SelectMany(a => a.GetTypes().Where(t => t.Name.EndsWith(pattern) && !t.IsAbstract && !t.IsInterface && !ignored.Contains(t))))
            {
                Routing.RegisterRoute(page.Name, page);
            }
        }
    }
}