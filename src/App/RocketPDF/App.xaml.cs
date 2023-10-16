namespace RocketPDF
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();
        }

        protected override async void OnStart()
        {
            if (Preferences.Get(PreferenceKeys.FirstRun, true))
            {
                SecureStorage.RemoveAll();
                Preferences.Set(PreferenceKeys.FirstRun, false);
            }

            var userInfo = await SecureStorage.GetAsync(StorageKeys.UserInfo);
            if (!string.IsNullOrWhiteSpace(userInfo))
            {
                UserStore.Value.IsLoggedIn = true;
                //UserStore.Value.UserInfo = JsonHelper.Deserialize<UserInfoDto>(userInfo);
            }
            UserStore.Value.AccessToken = await SecureStorage.GetAsync(StorageKeys.AccessToken);

            base.OnStart();
        }
    }
}