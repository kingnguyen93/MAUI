using CommunityToolkit.Mvvm.Messaging;
using Polly;
using Polly.Retry;
using Refit;
using RocketPDF.Core.Helpers;
using RocketPDF.Core.Services;
using System.Net;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RocketPDF.Core.Extensions
{
    public static class ServiceExtension
    {
        public static IServiceCollection RegisterServices(this IServiceCollection services, Assembly[] assemblies, ServiceLifetime lifetime = ServiceLifetime.Transient, string pattern = "Service")
        {
            assemblies.SelectMany(a => a.GetTypes().Where(a => a.Name.EndsWith(pattern) && !a.IsAbstract && !a.IsInterface))
                .Select(a => new { AssignedType = a, ServiceTypes = a.GetInterfaces().ToList() }).ToList()
                .ForEach(typesToRegister => typesToRegister.ServiceTypes.ForEach(typeToRegister => services.Add(new ServiceDescriptor(typeToRegister, typesToRegister.AssignedType, lifetime))));
            return services;
        }

        public static IServiceCollection RegisterApis(this IServiceCollection services, Assembly[] assemblies, string pattern = "Api")
        {
            services.AddScoped<AuthHeaderHandler>();
            var refitSettings = new RefitSettings
            {
                ContentSerializer = new SystemTextJsonContentSerializer(new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles,
                    Converters = { new JsonStringEnumConverter() }
                })
                // AuthorizationHeaderValueGetter = (_, _) => SecureStorage.GetAsync(StorageKeys.AccessToken)
            };
            foreach (var type in assemblies.SelectMany(a => a.GetTypes().Where(a => a.Name.EndsWith(pattern) && a.Namespace != "Refit.Implementation" && a.IsAbstract && a.IsInterface)))
            {
                services.AddRefitClient(type, refitSettings)
                    .ConfigureHttpClient(c => c.BaseAddress = new Uri(AppSettings.BaseUrl))
                    .AddHttpMessageHandler<AuthHeaderHandler>()
                    .AddPolicyHandler(GetUnauthPolicy)
                    .SetHandlerLifetime(TimeSpan.FromSeconds(60));
            }
            return services;
        }

        private static bool newTokenFetched;

        private static AsyncRetryPolicy<HttpResponseMessage> GetUnauthPolicy(IServiceProvider serviceProvider, HttpRequestMessage httpRequest)
        {
            return Policy.HandleResult<HttpResponseMessage>(r => r.StatusCode == HttpStatusCode.Unauthorized)
             .WaitAndRetryAsync(1, retryAttempt => TimeSpan.FromSeconds(retryAttempt), onRetryAsync: async (_, _, _, _) =>
             {
                 await using var _ = await LockHelper.LockAsync("RefreshToken");
                 if (newTokenFetched)
                 {
                     httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", UserStore.Value.AccessToken);
                     return;
                 }
                 //var authApi = serviceProvider.GetRequiredService<IAuthApi>();
                 //var oldAccessToken = await SecureStorage.GetAsync(StorageKeys.AccessToken);
                 //var refreshToken = await SecureStorage.GetAsync(StorageKeys.RefreshToken);
                 //var refreshTokenResponse = await authApi.RefreshTokenAsync(new RefreshTokenRequestDto
                 //{
                 //    AccessToken = oldAccessToken,
                 //    RefreshToken = refreshToken
                 //});
                 //if (refreshTokenResponse.IsSuccessStatusCode)
                 //{
                 //    await SecureStorage.SetAsync(StorageKeys.AccessToken, refreshTokenResponse.Content.Data.AccessToken);
                 //    await SecureStorage.SetAsync(StorageKeys.RefreshToken, refreshTokenResponse.Content.Data.RefreshToken);
                 //    UserStore.Value.AccessToken = refreshTokenResponse.Content.Data.AccessToken;
                 //    httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", refreshTokenResponse.Content.Data.AccessToken);
                 //}
                 //else
                 //{
                 //    WeakReferenceMessenger.Default.Send(new AccessTokenExpiredMessage(true));
                 //}
                 newTokenFetched = true;
             });
        }

        public static IServiceCollection RegisterPageViewModels(this IServiceCollection services, Assembly[] assemblies, string pattern = "Page")
        {
            var pages = assemblies.SelectMany(a => a.GetTypes().Where(a => a.Name.EndsWith(pattern) && !a.IsAbstract && !a.IsInterface))
                .Select(page =>
                {
                    var viewModelName = page.GetViewModelTypeName();
                    var viewModel = Type.GetType(viewModelName);
                    return new { Page = page, ViewModel = viewModel };
                });
            foreach (var page in pages)
            {
                if (page.Page != null && page.ViewModel != null)
                {
                    services.RegisterPage(page.Page, page.ViewModel);
                }
            }
            return services;
        }

        public static IServiceCollection RegisterPage<TPage, TViewModel>(this IServiceCollection services)
            where TPage : BasePage where TViewModel : BaseViewModel
        {
            return RegisterPage(services, typeof(TPage), typeof(TViewModel));
        }

        public static IServiceCollection RegisterPage(this IServiceCollection services, Type page, Type viewModel)
        {
            services.AddTransient(page);
            services.AddTransient(viewModel);
            return services;
        }
    }
}