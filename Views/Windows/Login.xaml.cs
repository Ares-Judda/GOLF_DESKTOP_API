using GOLF_DESKTOP.Model.Validations;
using GOLF_DESKTOP.Model;
using GOLF_DESKTOP.Model.Utilities;
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
using GOLF_DESKTOP.Model.Entities;
using Newtonsoft.Json;
using GOLF_DESKTOP.Services;
using Grpc.Core;
using System.Net.Http;

namespace GOLF_DESKTOP.Views.Windows {
    /// <summary>
    /// Lógica de interacción para Login.xaml
    /// </summary>
    public partial class Login : Window {
        public Login() {
            InitializeComponent();
            var emailValidationRule = new EmailValidationRule();
            EmailValidationRule.ErrorTextBlock = tbkEmailValidationMessage;
        }

        private async void ClickLogin(object sender, RoutedEventArgs e) {
            var auth = new Auth {
                email = txtEmailUser.Text,
                password = psbUser.Password
            };
                var userData = await ApiService.LoginUserAsync(auth);
            if (userData != null) {
                LblLoginError.Visibility = Visibility.Collapsed;
                var userSingleton = UserSingleton.GetInstance();
                userSingleton.IdUser = userData.idUser;
                userSingleton.Email = userData.email;
                userSingleton.Role = userData.role;
                InicialiceSesion();
            } else {
                LblLoginError.Visibility = Visibility.Visible;
            }
            
        }



        private void InicialiceSesion() {
            var user = UserSingleton.GetInstance();
            if (user.Role == "CLIENT_ROLE") {
                ClientWindow client = new ClientWindow();
                client.Show();
                this.Close();
            } else {
                SellingWindow selling = new SellingWindow();
                selling.Show();
                this.Close();
            }

        }

        private void TextChangedValidateTextBox(object sender, TextChangedEventArgs e) {
            bool isEmailValid = !Validation.GetHasError(txtEmailUser) && !string.IsNullOrEmpty(txtEmailUser.Text);
            if (isEmailValid) {
                btnLogin.IsEnabled = true;
                tbkEmailValidationIcon.Visibility = Visibility.Collapsed;
            } else {
                tbkEmailValidationIcon.Visibility = Visibility.Visible;
                tbkEmailValidationMessage.Visibility = Visibility.Visible;
                btnLogin.IsEnabled = false;
            }
        }
        private void KeyDownLoginPasswordBox(object sender, KeyEventArgs e) {
            if (e.Key == Key.Enter && btnLogin.IsEnabled) {
                ClickLogin(sender, e);
            }
        }
        private void KeyDownLoginTextBox(object sender, KeyEventArgs e) {
            if (e.Key == Key.Enter && btnLogin.IsEnabled) {
                ClickLogin(sender, e);
            }
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

        private void ClickRegisterHere(object sender, RoutedEventArgs e) {
            Register register = new Register();
            register.Show();
            this.Close();
        }
    }
}
