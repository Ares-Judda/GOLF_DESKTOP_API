using GOLF_DESKTOP.Model.Entities;
using GOLF_DESKTOP.Model.Utilities;
using GOLF_DESKTOP.Services;
using GOLF_DESKTOP.Views.Windows;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace GOLF_DESKTOP.Views.Pages {
    /// <summary>
    /// Lógica de interacción para UpdateClothes.xaml
    /// </summary>
    public partial class UpdateClothes : Page {
        private Clothe articleToUpdate;

        public UpdateClothes(Clothe clothe) {
            InitializeComponent();
            articleToUpdate = clothe;
            SetUpArticleInformation(articleToUpdate);
        }

        private void CuadroTextoNumero_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            if (textBox != null)
            {
                int cursorPos = textBox.SelectionStart;

                string textoOriginal = textBox.Text;
                string textoFiltrado = new string(textoOriginal.Where(c => char.IsDigit(c)).ToArray());

                if (textoOriginal != textoFiltrado)
                {
                    textBox.Text = textoFiltrado;

                    textBox.SelectionStart = cursorPos > textBox.Text.Length ? textBox.Text.Length : cursorPos;
                }
            }
        }

        private async void ClickUpdate(object sender, RoutedEventArgs e)
        {
            string name = txtNombre.Text.Trim();
            string precioTexto = txtPrecio.Text.Trim();
            string quotaTxt = txtCantidad.Text.Trim();
            string results, imageUrl;
            ComboBoxItem selectedItem = cbxTalla.SelectedItem as ComboBoxItem;
            string size = selectedItem?.Content.ToString();
            ComboBoxItem selectedItemArt = cbxTipoArticulo.SelectedItem as ComboBoxItem;
            string clotheCategory = selectedItemArt?.Content.ToString();

            if (!string.IsNullOrEmpty(name) &&
                !string.IsNullOrEmpty(precioTexto) &&
                !string.IsNullOrEmpty(quotaTxt) &&
                !string.IsNullOrEmpty(size) &&
                !string.IsNullOrEmpty(clotheCategory) &&
                ClotheImage.Source != null ||
                ClotheImage.Source is BitmapImage bitmapImage &&
                bitmapImage.UriSource != null &&
                bitmapImage.UriSource.OriginalString != "/Resources/Images/ClotheIcon.png")
            {
                int.TryParse(precioTexto, out int price);
                int.TryParse(quotaTxt, out int quota);

                try
                {
                    var profileImageBytes = ImageHandler.ConvertImageToBytes((BitmapImage)ClotheImage.Source);
                    if (profileImageBytes != null)
                    {
                        imageUrl = await ApiServiceRest.UploadImageAsync(profileImageBytes);
                        if (string.IsNullOrEmpty(imageUrl))
                        {
                            MessageBox.Show("No se pudo subir la imagen. Intenta nuevamente.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                        else
                        {
                            var actualizaciones = new Dictionary<string, object>
                            {
                                { "name", name },
                                { "price", price },
                                { "quota", quota },
                                { "size", size },
                                { "clothecategory", clotheCategory },
                                { "pick", imageUrl },
                            };

                            results = await ArticulosServiceGrpc.UpdateArticuloAsync(articleToUpdate.ID_Clothes, actualizaciones);

                            if (results != null)
                            {
                                MessageBox.Show("Articulo actualizado correctamente", "Resultado de la Actualización", MessageBoxButton.OK, MessageBoxImage.Information);

                                if (!string.IsNullOrEmpty(articleToUpdate.Image))
                                {
                                    bool isDeleted = await ApiServiceRest.DeleteImageAsync(System.IO.Path.GetFileName(articleToUpdate.Image));
                                    if (!isDeleted)
                                    {
                                        MessageBox.Show("No se pudo eliminar la imagen anterior.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                                    }
                                }
                            }
                            else
                            {
                                MessageBox.Show("No se pudo actualizar el articulo", "Error al actualizar", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Faltan datos necesarios.", "Datos faltantes", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al actualizar el artículo: {ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Faltan datos necesarios.",
                                "Datos Faltantes", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void ClickReturnToClothesPage(object sender, MouseButtonEventArgs e) {
            NavigationService.GoBack();
        }

        private async void ClickDelete(object sender, RoutedEventArgs e)
        {
            bool resultado = await ArticulosServiceGrpc.DeleteArticuloAsync(articleToUpdate.ID_Clothes);

            if (resultado)
            {
                if (!string.IsNullOrEmpty(articleToUpdate.Image))
                {
                    MessageBox.Show("Imagen: " + articleToUpdate.Image);
                    bool imageDeleted = await ApiServiceRest.DeleteImageAsync(System.IO.Path.GetFileName(articleToUpdate.Image));
                    if (!imageDeleted)
                    {
                        MessageBox.Show("Artículo eliminado, pero no se pudo eliminar la imagen asociada.",
                                        "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }

                MessageBox.Show("Artículo eliminado correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                NavigationService.Navigate(new ClothesPage());
            }
            else
            {
                MessageBox.Show("No se pudo eliminar el artículo.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SetUpArticleInformation(Clothe article)
        {
            txtNombre.Text = article.Name;
            txtCantidad.Text = article.Quota.ToString();
            txtPrecio.Text = article.Price.ToString();
            ClotheImage.Source = article.ImageSource;

            foreach (ComboBoxItem item in cbxTipoArticulo.Items)
            {
                if (item.Content.ToString() == article.ClotheCategory)
                {
                    cbxTipoArticulo.SelectedItem = item;
                    break;
                }
            }

            foreach (ComboBoxItem item in cbxTalla.Items)
            {
                if (item.Content.ToString() == article.Size)
                {
                    cbxTalla.SelectedItem = item;
                    break;
                }
            }
        }

        private void ClickClotheImage(object sender, MouseButtonEventArgs e) {
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
                    ClotheImage.Source = bitmap;
                } catch (Exception ex) {
                    MessageBox.Show($"Error al cargar la imagen: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
