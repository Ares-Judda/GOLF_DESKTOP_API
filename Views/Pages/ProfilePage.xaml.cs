using GOLF_DESKTOP.Model.Entities;
using GOLF_DESKTOP.Model.Utilities;
using GOLF_DESKTOP.Services;
using Microsoft.Win32;
using Newtonsoft.Json;
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


        private async void ClickUpdate(object sender, RoutedEventArgs e) {
            try {
                string imageUrl = await ProcessProfileImage();
                var updates = CreateUpdateDictionary(imageUrl);

                if (updates.Count == 0) {
                    MessageBox.Show("No se proporcionaron datos para actualizar.", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                string json = JsonConvert.SerializeObject(new { actualizaciones = updates });

                var userReference = UserSingleton.GetInstance();
                string userId = userReference.IdUser;

                var response = await ApiServiceRest.UpdateUserAsync(json, userId);

                if (response.IsSuccessStatusCode) {
                    MessageBox.Show("Usuario actualizado exitosamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                } else {
                    string errorResponse = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Error al actualizar: {errorResponse}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            } catch (Exception ex) {
                MessageBox.Show($"Error inesperado: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task<string> ProcessProfileImage() {
            if (ProfileImage.Source == null ||
                ProfileImage.Source.Equals(new BitmapImage(new Uri("/Resources/Images/UserIcon.png", UriKind.Relative)))) {
                return null; // No hay nueva imagen seleccionada
            }

            var profileImageBytes = ImageHandler.ConvertImageToBytes((BitmapImage)ProfileImage.Source);
            if (profileImageBytes == null) {
                return null;
            }

            string imageUrl = await ApiServiceRest.UploadImageAsync(profileImageBytes);
            if (string.IsNullOrEmpty(imageUrl)) {
                MessageBox.Show("No se pudo subir la imagen. Intenta nuevamente.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                throw new Exception("Error al subir la imagen");
            }

            return imageUrl;
        }

        private Dictionary<string, object> CreateUpdateDictionary(string imageUrl) {
            var updates = new Dictionary<string, object> {
                    { "name", GetValueOrNull(txtNombres.Text) },
                    { "lastname", GetValueOrNull(txtApellidos.Text) },
                    { "cellphone", GetValueOrNull(txtTelefono.Text) },
                    { "address", GetValueOrNull(txtDireccion.Text) },
                    { "datebirth", dpFechaNacimiento.SelectedDate?.ToString("yyyy-MM-dd") },
                    { "zipcode", GetValueOrNull(txtCodigoPostal.Text) },
                    { "imagen", imageUrl }
            };
            return updates.Where(pair => pair.Value != null)
                          .ToDictionary(pair => pair.Key, pair => pair.Value);
        }

        private string GetValueOrNull(string input) {
            return !string.IsNullOrWhiteSpace(input) ? input.Trim() : null;
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
