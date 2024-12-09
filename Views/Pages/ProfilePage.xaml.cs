using GOLF_DESKTOP.Model.Utilities;
using GOLF_DESKTOP.Services;
using Microsoft.Win32;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace GOLF_DESKTOP.Views.Pages {
    public partial class ProfilePage : Page {
        public ProfilePage() {
            InitializeComponent();
            Loaded += ProfilePage_Loaded;
        }

        private async void ProfilePage_Loaded(object sender, RoutedEventArgs e) {
            var userReference = UserSingleton.GetInstance();
            string userId = userReference.IdUser;
            MessageBox.Show(userId);
            var user = await ApiService.GetUsuarioAsync(userId);

            if (user != null) {
                // Aquí puedes actualizar la interfaz con los datos del usuario
                // Por ejemplo, mostrar el nombre, correo y la imagen del perfil
                txtCorreo.Text = user.email;
                

                // Cargar la imagen de perfil
                ProfileImage.Source = await LoadImageFromUrlAsync(user.imagen);
            }
        }

        private async Task<BitmapImage> LoadImageFromUrlAsync(string url) {
            try {
                var bitmap = new BitmapImage();
                using (var webClient = new System.Net.WebClient()) {
                    var imageData = await webClient.DownloadDataTaskAsync(url);
                    using (var stream = new MemoryStream(imageData)) {
                        bitmap.BeginInit();
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.StreamSource = stream;
                        bitmap.EndInit();
                    }
                }
                return bitmap;
            } catch (Exception ex) {
                MessageBox.Show($"Error al cargar la imagen: {ex.Message}",
                                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        private void ClickUpdate(object sender, RoutedEventArgs e) {
            // Lógica para actualizar la información del usuario
        }

        private void ClickProfileImage(object sender, MouseButtonEventArgs e) {
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
    }
}
