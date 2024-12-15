using GOLF_DESKTOP.Model.Entities;
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
            var user = await ApiServiceRest.GetUsuarioAsync(userId);
            if (user != null) {
                LoadFields(user);
                if (!string.IsNullOrEmpty(user.imagen)) {
                    MessageBox.Show(user.imagen);
                    BitmapImage bitmapImage = await LoadImageFromUrlAsync(user.imagen);

                    if (bitmapImage != null) {
                        ProfileImage.Source = bitmapImage;
                    }
                }
            } else {
                MessageBox.Show("No se pudo obtener la información del usuario.");
            }
        }

        private void LoadFields(User user) {
            txtNombres.Text = user.name;
            txtApellidos.Text = user.lastname;
            txtTelefono.Text = user.phone;
            txtCorreo.Text = user.email;
            txtDireccion.Text = user.address;
            txtCodigoPostal.Text = user.postalCode;
            dpFechaNacimiento.SelectedDate = user.birthDate;
            if (user.role == "CLIENT_ROLE") {
                cbxTipoCuenta.SelectedItem = cbxTipoCuenta.Items.Cast<ComboBoxItem>()
                    .FirstOrDefault(item => (item.Content as string) == "Cliente");
            } else if (user.role == "SELLING_ROLE") {
                cbxTipoCuenta.SelectedItem = cbxTipoCuenta.Items.Cast<ComboBoxItem>()
                    .FirstOrDefault(item => (item.Content as string) == "Vendedor");
            }
        }


        private async Task<BitmapImage> LoadImageFromUrlAsync(string url) {
            if (string.IsNullOrEmpty(url)) {
                MessageBox.Show("La URL de la imagen está vacía.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return GetDefaultImage(); // Usar imagen predeterminada si la URL es nula o vacía
            }

            try {
                var bitmap = new BitmapImage();
                using (var webClient = new System.Net.WebClient()) {
                    webClient.DownloadDataCompleted += (sender, e) => {
                        if (e.Error != null) {
                            MessageBox.Show($"Error al cargar la imagen: {e.Error.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }

                        if (e.Result == null || e.Result.Length == 0) {
                            MessageBox.Show("La imagen está vacía o no se pudo descargar correctamente.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            ProfileImage.Source = GetDefaultImage(); // Fallback a la imagen predeterminada
                            return;
                        }

                        var imageData = e.Result;
                        using (var stream = new MemoryStream(imageData)) {
                            bitmap.BeginInit();
                            bitmap.CacheOption = BitmapCacheOption.OnLoad;
                            bitmap.StreamSource = stream;
                            bitmap.EndInit();
                        }

                        // Asignar la imagen al control en el hilo de UI
                        Dispatcher.Invoke(() => ProfileImage.Source = bitmap);
                    };

                    // Descargar la imagen de manera asincrónica
                    webClient.DownloadDataAsync(new Uri(url));
                }
            } catch (Exception ex) {
                MessageBox.Show($"Error al cargar la imagen: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return GetDefaultImage(); // Usar imagen predeterminada en caso de fallo
            }

            return null;
        }

        private BitmapImage GetDefaultImage() {
            return new BitmapImage(new Uri("pack://application:,,,/Images/default_profile.png"));
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
