using CommunityToolkit.Mvvm.ComponentModel;

namespace RocketPDF
{
    public sealed partial class UserStore : ObservableObject
    {
        private static readonly Lazy<UserStore> lazy = new(() => new UserStore());

        private UserStore()
        {
        }

        public static UserStore Value => lazy.Value;

        [ObservableProperty]
        private bool isLoggedIn;

        [ObservableProperty]
        private string accessToken;
    }
}