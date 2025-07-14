using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using Windows.Storage;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace RumarApp.Views
{
    public sealed partial class LoginControl : UserControl
    {
        public LoginControl()
        {
            InitializeComponent();

            InvalidEmailError.Visibility = Visibility.Collapsed;
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            // Very basic mock authentication
            if (EmailBox.Text == "admin" && PasswordBox.Password == "1234")
            {
                // You could use local settings or a secure store
                ApplicationData.Current.LocalSettings.Values["IsLoggedIn"] = true;

                //LoginSucceeded?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                ContentDialog dialog = new ContentDialog
                {
                    Title = "Login failed",
                    Content = "Invalid credentials.",
                    CloseButtonText = "OK",
                    XamlRoot = this.XamlRoot
                };

                _ = dialog.ShowAsync();
            }
        }

        private void EmailBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var email = EmailBox.Text.Trim();
            // Simple email validation
            if (string.IsNullOrEmpty(email) || !email.Contains("@") || !email.Contains("."))
            {
                EmailBox.BorderBrush = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Red);
                InvalidEmailError.Visibility = Visibility.Visible;
            }
            else
            {
                EmailBox.BorderBrush = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Green);
                InvalidEmailError.Visibility = Visibility.Collapsed;
            }
        }
    }
}
