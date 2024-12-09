using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GOLF_DESKTOP.Views.Windows {
    /// <summary>
    /// Lógica de interacción para SellingWindow.xaml
    /// </summary>
    public partial class SellingWindow : Window {
        public SellingWindow() {
            InitializeComponent();
        }

        private void ClickClose(object sender, RoutedEventArgs e) {
            Close();
        }

        private void ClickRestore(object sender, RoutedEventArgs e) {
            if (WindowState == WindowState.Normal) {
                WindowState = WindowState.Maximized;
            } else {
                WindowState = WindowState.Normal;
            }
        }

        private void ClickMinimize(object sender, RoutedEventArgs e) {
            WindowState = WindowState.Minimized;
        }

        private void ClickSales(object sender, RoutedEventArgs e) {
            SalesOptions.Visibility = Visibility.Visible;
        }

        private void ClickSell(object sender, RoutedEventArgs e) {
            SalesOptions.Visibility = Visibility.Collapsed;
            fraPages.Navigate(new System.Uri("/Views/Pages/SellClothesPage.xaml", UriKind.RelativeOrAbsolute));
        }

        private void ClickModify(object sender, RoutedEventArgs e) {
            SalesOptions.Visibility = Visibility.Collapsed;
            fraPages.Navigate(new System.Uri("/Views/Pages/ClothesPage.xaml", UriKind.RelativeOrAbsolute));
        }

        private void ClickEarnings(object sender, RoutedEventArgs e) {
            SalesOptions.Visibility = Visibility.Collapsed;
            fraPages.Navigate(new System.Uri("/Views/Pages/EarningsPage.xaml", UriKind.RelativeOrAbsolute));
        }

        private void ClickInventory(object sender, RoutedEventArgs e) {
            SalesOptions.Visibility = Visibility.Collapsed;
            fraPages.Navigate(new System.Uri("/Views/Pages/InventoryPage.xaml", UriKind.RelativeOrAbsolute));
        }

        private void ClickHome(object sender, RoutedEventArgs e) {
            SalesOptions.Visibility = Visibility.Collapsed;
            fraPages.Navigate(new System.Uri("/Views/Pages/HomePage.xaml", UriKind.RelativeOrAbsolute));
        }

        private void ClickProfile(object sender, RoutedEventArgs e) {
            SalesOptions.Visibility = Visibility.Collapsed;
            fraPages.Navigate(new System.Uri("/Views/Pages/ProfilePage.xaml", UriKind.RelativeOrAbsolute));
        }

        private void MouseDownLogout(object sender, MouseButtonEventArgs e) {
            Login login = new Login();
            login.Show();
            Close();
        }
    }
}
