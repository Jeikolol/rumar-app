using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using RumarApp.Views;
using RumarApp.Views.Layout;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace RumarApp
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public static MainWindow Instance { get; private set; }

        public MainWindow()
        {
            InitializeComponent();
            Instance = this;

            bool isLoggedIn = ApplicationData.Current.LocalSettings.Values["IsLoggedIn"] as bool? == true;

            if (isLoggedIn)
            {
                ShowDasboardControl();
            }
            else
            {
                ShowLoginControl();
            }
        }

        private void ShowLoginControl()
        {
            var loginControl = new LoginControl(ShowDasboardControl);
            RootGrid.Children.Clear();
            RootGrid.Children.Add(loginControl);
        }

        private void ShowDasboardControl()
        {
            RootGrid.Children.Clear();
            RootGrid.Children.Add(new LayoutControl());
        }
    }
}
