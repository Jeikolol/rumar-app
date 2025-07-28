using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using RumarApp.ApiClient;
using RumarApp.ViewModels;
using Shared.Application.Identity.Tokens;
using System;
using System.Diagnostics;
using Windows.Storage;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace RumarApp.Views
{
    public sealed partial class LoginControl : UserControl
    {
        public LoginControl(Action onLoginSuccess)
        {
            InitializeComponent();

            var loginService = App.Services.GetRequiredService<IApiService<TokenRequest, TokenResponse>>();

            this.DataContext = new LoginViewModel(loginService, onLoginSuccess);

            InvalidEmailError.Visibility = Visibility.Collapsed;
        }
    }
}
