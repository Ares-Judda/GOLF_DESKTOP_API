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
    /// Lógica de interacción para ClientWindow.xaml
    /// </summary>
    public partial class ClientWindow : Window {
        public ClientWindow() {
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

        private void ClickHome(object sender, RoutedEventArgs e) {
            fraPages.Navigate(new System.Uri("/Views/Pages/HomePage.xaml", UriKind.RelativeOrAbsolute));
        }


        private void ClickProfile(object sender, RoutedEventArgs e) {
            fraPages.Navigate(new System.Uri("/Views/Pages/ProfilePage.xaml", UriKind.RelativeOrAbsolute));
        }

        private void MouseDownLogout(object sender, MouseButtonEventArgs e) {
                Login login = new Login();
                login.Show();
                Close();        
        }
    }
}
