using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using RumarApp.Views.Pages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace RumarApp.Views.Layout
{
    public sealed partial class LayoutControl : UserControl
    {
        public LayoutControl()
        {
            InitializeComponent();

            MainFrame.Navigate(typeof(DashboardPage));
        }

        private void sideNav_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.SelectedItem is NavigationViewItem selectedItem)
            {
                string tag = selectedItem.Tag.ToString() ?? "dash";

                switch (tag)
                {
                    case "dash":
                        MainFrame.Navigate(typeof(DashboardPage));
                        break;

                    case "loan":
                        MainFrame.Navigate(typeof(DashboardPage));
                        break;
                }
            }
        }
    }
}
