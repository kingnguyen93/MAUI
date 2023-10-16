using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RocketPDF.Core.Extensions;
using RocketPDF.Core.Views;
using System.Diagnostics;

namespace RocketPDF.Core
{
    public abstract partial class BaseViewModel : ObservableObject, IQueryAttributable, IDisposable
    {
        [ObservableProperty]
        private bool isBusy;

        [ObservableProperty]
        private string title = string.Empty;

        private bool isLoading;

        public bool IsLoading
        {
            get => isLoading;
            set
            {
                isLoading = value;
                if (isLoading)
                {
                    ShowLoading();
                }
                else
                {
                    HideLoading();
                }
            }
        }

        ~BaseViewModel()
        {
            Dispose(false);
        }

        protected ResourceDictionary ColorResource => Application.Current.Resources.MergedDictionaries.FirstOrDefault();

        public Page CurrentPage { get; set; }

        // true: do nothing
        public virtual bool OnBackButtonPressed() => false;

        public virtual Task OnInitAsync(IDictionary<string, object> query) => Task.CompletedTask;

        public virtual Task OnInitAsync(object value) => Task.CompletedTask;

        public virtual Task OnBackAsync(IDictionary<string, object> query) => Task.CompletedTask;

        public virtual Task OnBackAsync(object value) => Task.CompletedTask;

        // Order: 1
        public virtual void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue(ParameterKeys.IsGoBack, out _))
            {
                OnBackAsync(query.GetValueOrDefault(ParameterKeys.Default).As<IDictionary<string, object>>() ?? new Dictionary<string, object>());
                if (query.TryGetValue(ParameterKeys.Default, out object value))
                {
                    OnBackAsync(value);
                }
                query.Clear();
            }
            else
            {
                OnInitAsync(query);
                if (query.TryGetValue(ParameterKeys.Default, out object value))
                {
                    OnInitAsync(value);
                }
            }
            Debug.WriteLine($"{GetType().Name}.{nameof(ApplyQueryAttributes)}");
        }

        // Order: 2
        public virtual Task OnAppearingAsync()
        {
            Debug.WriteLine($"{GetType().Name}.{nameof(OnAppearingAsync)}");
            return Task.CompletedTask;
        }

        private bool isAppeared;

        // Order:
        public virtual Task OnFirstAppearingAsync()
        {
            Debug.WriteLine($"{GetType().Name}.{nameof(OnFirstAppearingAsync)}");
            if (!isAppeared)
            {
                OnFirstAppearingAsync();
                isAppeared = true;
            }
            return Task.CompletedTask;
        }

        // Order: 3
        public virtual Task OnNavigatedToAsync()
        {
            Debug.WriteLine($"{GetType().Name}.{nameof(OnNavigatedToAsync)}");
            return Task.CompletedTask;
        }

        // Order: 4
        public virtual Task OnNavigatingFromAsync()
        {
            Debug.WriteLine($"{GetType().Name}.{nameof(OnNavigatingFromAsync)}");
            return Task.CompletedTask;
        }

        // Order: 5
        public virtual Task OnDisappearingAsync()
        {
            Debug.WriteLine($"{GetType().Name}.{nameof(OnDisappearingAsync)}");
            return Task.CompletedTask;
        }

        // Order: 6
        public virtual Task OnNavigatedFromAsync()
        {
            Debug.WriteLine($"{GetType().Name}.{nameof(OnNavigatedFromAsync)}");
            return Task.CompletedTask;
        }

        [RelayCommand]
        protected Task GoToAsync(string route)
        {
            return GoToAsync(route, true);
        }

        protected Task GoToAsync<TPage>(bool animate = true)
        {
            return GoToAsync(typeof(TPage).Name, animate);
        }

        protected Task GoToAsync(string route, bool animate)
        {
            return Shell.Current.GoToAsync(route, animate);
        }

        // Navigate with value
        protected Task GoToAsync<TValue>(string route, TValue value) where TValue : class, IComparable<TValue>
        {
            return GoToAsync(route, value, true);
        }

        protected Task GoToAsync<TPage, TValue>(TValue value, bool animate = true) where TValue : class, IComparable<TValue>
        {
            return GoToAsync(typeof(TPage).Name, value, animate);
        }

        protected Task GoToAsync<TValue>(string route, TValue value, bool animate) where TValue : class, IComparable<TValue>
        {
            return Shell.Current.GoToAsync(route, animate, new Dictionary<string, object>
            {
                { ParameterKeys.Default, value }
            });
        }

        // Navigate with parameters
        protected Task GoToAsync<TPage>(IDictionary<string, object> parameters, bool animate = true)
        {
            return GoToAsync(typeof(TPage).Name, parameters, animate);
        }

        protected Task GoToAsync(string route, IDictionary<string, object> parameters, bool animate = true)
        {
            return Shell.Current.GoToAsync(route, animate, parameters);
        }

        [RelayCommand]
        protected async Task GoBackAsync()
        {
            await Shell.Current.GoToAsync("..", true, new Dictionary<string, object>()
            {
                { ParameterKeys.IsGoBack, true }
            });
        }

        protected async Task GoBackAsync<TValue>(TValue value, bool animate = true) where TValue : class, IComparable<TValue>
        {
            await Shell.Current.GoToAsync("..", animate, new Dictionary<string, object>
            {
                { ParameterKeys.IsGoBack, true },
                { ParameterKeys.Default, value }
            });
        }

        protected async Task GoBackAsync(IDictionary<string, object> parameters, bool animate = true)
        {
            parameters.TryAdd(ParameterKeys.IsGoBack, true);
            await Shell.Current.GoToAsync("..", animate, parameters);
        }

        protected Task DisplayAlert(string title, string message, string cancel)
        {
            return CurrentPage?.DisplayAlert(title, message, cancel) ?? Task.CompletedTask;
        }

        protected Task<bool> DisplayAlert(string title, string message, string accept, string cancel)
        {
            return CurrentPage?.DisplayAlert(title, message, accept, cancel) ?? Task.FromResult(false);
        }

        protected Task<string> DisplayActionSheet(string title, string cancel, string destruction, params string[] buttons)
        {
            return CurrentPage?.DisplayActionSheet(title, cancel, destruction, buttons) ?? Task.FromResult(string.Empty);
        }

        protected Task ShowShortToast(string message)
        {
            return Toast.Make(message, ToastDuration.Short).Show();
        }

        protected Task ShowLongToast(string message)
        {
            return Toast.Make(message, ToastDuration.Long).Show();
        }

        protected async Task ShowPopup<TP>() where TP : Popup
        {
            var popup = Activator.CreateInstance(typeof(TP)).As<Popup>();
            await Shell.Current.ShowPopupAsync(popup);
        }

        private Popup loadingPage;

        protected void ShowLoading()
        {
            loadingPage ??= new LoadingPopup();
            Shell.Current.ShowPopup(loadingPage);
        }

        protected void HideLoading()
        {
            if (loadingPage != null)
            {
                loadingPage.Close();
                loadingPage = null;
            }
        }

        private bool disposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    //dispose only
                }
                disposed = true;
            }
        }
    }
}