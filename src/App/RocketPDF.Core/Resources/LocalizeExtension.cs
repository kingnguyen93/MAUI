using Microsoft.Extensions.Localization;
using RocketPDF.Core.Helpers;
using RocketPDF.Core.Resources;

namespace RocketPDF.Core
{
    [ContentProperty(nameof(Key))]
    public class LocalizeExtension : IMarkupExtension
    {
        readonly IStringLocalizer<Localize> _localizer;

        public string Key { get; set; } = string.Empty;

        public LocalizeExtension()
        {
            _localizer = ServiceHelper.GetRequiredService<IStringLocalizer<Localize>>();
        }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            return (string)_localizer[Key];
        }

        object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider) => ProvideValue(serviceProvider);
    }
}
