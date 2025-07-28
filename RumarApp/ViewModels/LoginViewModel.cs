using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Windows.Storage;
using RumarApp.ApiClient;
using RumarApp.Helpers;
using RumarApp.Views;
using Shared.Application.Common.Exceptions;
using Shared.Application.Identity.Tokens;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using ApplicationData = Windows.Storage.ApplicationData;

namespace RumarApp.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        private readonly IApiService<TokenRequest, TokenResponse> _loginService;

        private string _email = string.Empty;
        private string _password = string.Empty;
        private bool _isBusy;
        private string _errorMessage = string.Empty;
        private string _jwtToken = string.Empty;
        private bool _isEmailValid = true;

        public string Email
        {
            get => _email;
            set
            {
                if (_email != value)
                {
                    _email = value;
                    OnPropertyChanged();
                    ValidateEmail();
                    (LoginCommand as RelayCommand)?.RaiseCanExecuteChanged();
                }
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                if (_password != value)
                {
                    _password = value;
                    OnPropertyChanged();
                    (LoginCommand as RelayCommand)?.RaiseCanExecuteChanged();
                }
            }
        }

        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                if (_isBusy != value)
                {
                    _isBusy = value;
                    OnPropertyChanged();
                    (LoginCommand as RelayCommand)?.RaiseCanExecuteChanged();
                }
            }
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                if (_errorMessage != value)
                {
                    _errorMessage = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(HasError));
                }
            }
        }

        public bool HasError => !string.IsNullOrEmpty(ErrorMessage);

        public string JwtToken
        {
            get => _jwtToken;
            private set
            {
                if (_jwtToken != value)
                {
                    _jwtToken = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(IsLoggedIn));
                }
            }
        }

        public bool IsLoggedIn => !string.IsNullOrEmpty(JwtToken);

        public bool IsEmailValid
        {
            get => _isEmailValid;
            private set
            {
                if (_isEmailValid != value)
                {
                    _isEmailValid = value;
                    OnPropertyChanged();
                }
            }
        }

        public ICommand LoginCommand { get; }

        private readonly Action _onLoginSuccess;

        public LoginViewModel(IApiService<TokenRequest, TokenResponse> loginService, Action onLoginSuccess)
        {
            _loginService = loginService;
            _onLoginSuccess = onLoginSuccess; 
            LoginCommand = new RelayCommand(async () => await LoginAsync(), CanLogin);

            // Check if user is already logged in
            var savedToken = GetSavedToken();
            if (!string.IsNullOrEmpty(savedToken))
            {
                JwtToken = savedToken;
            }
        }

        private bool CanLogin()
        {
            return !IsBusy &&
                   !string.IsNullOrWhiteSpace(Email) &&
                   !string.IsNullOrWhiteSpace(Password) &&
                   IsEmailValid;
        }

        private void ValidateEmail()
        {
            if (string.IsNullOrWhiteSpace(Email))
            {
                IsEmailValid = true; // Don't show error for empty email
                return;
            }

            var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            IsEmailValid = emailRegex.IsMatch(Email);
        }

        private async Task LoginAsync()
        {
            if (IsBusy) return;

            IsBusy = true;
            ErrorMessage = string.Empty;

            try
            {
                var request = new TokenRequest(Email, Password);
                var response = await _loginService.ExecuteAsync(request);

                if (!string.IsNullOrEmpty(response.Token))
                {
                    JwtToken = response.Token;
                    SaveToken(JwtToken);

                    // Save refresh token if available
                    if (!string.IsNullOrEmpty(response.RefreshToken))
                    {
                        SaveRefreshToken(response.RefreshToken);
                    }

                    _onLoginSuccess?.Invoke(); 
                }
                else
                {
                    ErrorMessage = "Credenciales inválidas. Por favor, verifica tu email y contraseña.";
                }
            }
            catch (UnauthorizedException)
            {
                ErrorMessage = "Credenciales inválidas. Por favor, verifica tu email y contraseña.";
            }
            catch (Exception ex)
            {
                ErrorMessage = "Error de conexión. Por favor, intenta nuevamente.";
                Debug.WriteLine($"Login error: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        public void Logout()
        {
            JwtToken = string.Empty;
            ClearTokens();
            Email = string.Empty;
            Password = string.Empty;
            ErrorMessage = string.Empty;
        }

        private void SaveToken(string token)
        {
            ApplicationData.Current.LocalSettings.Values["jwt_token"] = token;
        }

        private void SaveRefreshToken(string refreshToken)
        {
            ApplicationData.Current.LocalSettings.Values["refresh_token"] = refreshToken;
        }

        private string? GetSavedToken()
        {
            return ApplicationData.Current.LocalSettings.Values["jwt_token"] as string;
        }

        private void ClearTokens()
        {
            ApplicationData.Current.LocalSettings.Values.Remove("jwt_token");
            ApplicationData.Current.LocalSettings.Values.Remove("refresh_token");
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
