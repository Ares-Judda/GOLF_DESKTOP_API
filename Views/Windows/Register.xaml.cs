using GOLF_DESKTOP.Model.Validations;
using GOLF_DESKTOP.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Win32;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using GOLF_DESKTOP.Model.Utilities;
using GOLF_DESKTOP.Services;
using System.Diagnostics;


namespace GOLF_DESKTOP.Views.Windows
{
    /// <summary>
    /// Lógica de interacción para Register.xaml
    /// </summary>
    public partial class Register : Window {
        public Register() {
            InitializeComponent();
            
        }

        private void OnFieldChanged(object sender, RoutedEventArgs e) {
            bool isImageValid = ProfileImage.Source != null &&
                                 !ProfileImage.Source.Equals(new BitmapImage(new Uri("/Resources/Images/UserIcon.png", UriKind.Relative)));

            bool isEmailValid = !string.IsNullOrWhiteSpace(txbEmailUser.Text);
            bool isNameValid = !string.IsNullOrWhiteSpace(txbName.Text);
            bool isUsernameValid = !string.IsNullOrWhiteSpace(txbUsername.Text);
            bool isLastNameValid = !string.IsNullOrWhiteSpace(txbLastName.Text);
            bool isPasswordValid = !string.IsNullOrWhiteSpace(psbPassword.Password);
            bool isUserTypeValid = cbxUserType.SelectedItem != null;

            bool areFieldsValid = FieldValidator.AreFieldsValid(
                txbEmailUser.Text,
                txbName.Text,
                txbUsername.Text,
                txbLastName.Text,
                psbPassword.Password,
                cbxUserType.SelectedItem,
                isImageValid
            );

            btnRegister.IsEnabled = areFieldsValid;
        }

        private void ClickReturnToLogin(object sender, MouseButtonEventArgs e) {
            Login login = new Login();
            login.Show();
            this.Close();
        }

        private void ClickProfileImage(object sender, MouseButtonEventArgs e) {
            e.Handled = true; 
            OpenFileDialog openFileDialog = new OpenFileDialog {
                Filter = "Archivos de imagen (*.jpg;*.png;*.jpeg)|*.jpg;*.png;*.jpeg"
            };

            if (openFileDialog.ShowDialog() == true) {
                string selectedImagePath = openFileDialog.FileName;
                FileInfo fileInfo = new FileInfo(selectedImagePath);

                if (fileInfo.Length > 10 * 1024 * 1024) {
                    MessageBox.Show("La imagen seleccionada excede los 10 MB. Por favor, selecciona una imagen más pequeña.",
                                    "Tamaño Excedido", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                try {
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(selectedImagePath, UriKind.Absolute);
                    bitmap.EndInit();
                    ProfileImage.Source = bitmap;
                } catch (Exception ex) {
                    MessageBox.Show($"Error al cargar la imagen: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
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

        private async void ClickRegister(object sender, RoutedEventArgs e) {
            var user = new User {
                email = txbEmailUser.Text,
                password = psbPassword.Password,
                role = cbxUserType.SelectedIndex == 0 ? "CLIENT_ROLE" : "SELLING_ROLE",
                name = txbName.Text,
                lastname = txbLastName.Text,
                userName = txbUsername.Text
            };

            var profileImageBytes = ImageHandler.ConvertImageToBytes((BitmapImage)ProfileImage.Source);
            if (profileImageBytes != null) {
                string imageUrl = await ApiServiceRest.UploadImageAsync(profileImageBytes);
                if (string.IsNullOrEmpty(imageUrl)) {
                    MessageBox.Show("No se pudo subir la imagen. Intenta nuevamente.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                user.imagen = imageUrl;
            }

            var response = await ApiServiceRest.RegisterUserAsync(user);
            if (response.IsSuccessStatusCode) {
                MessageBox.Show("Usuario registrado correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                Login login = new Login();
                login.Show();
                Close();
            } else {
                string error = await response.Content.ReadAsStringAsync();
                MessageBox.Show($"Error al registrar usuario: {error}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
