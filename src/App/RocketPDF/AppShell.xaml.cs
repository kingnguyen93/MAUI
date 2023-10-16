using System.Reflection;

namespace RocketPDF
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            RoutingHelper.RegisterRoutes(new[] { Assembly.GetExecutingAssembly() }, ignored: new[]
            {
                typeof(MainPage)
            });
        }
    }
}