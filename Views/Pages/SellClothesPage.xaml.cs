using GOLF_DESKTOP.Model.Utilities;
using GOLF_DESKTOP.Services;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
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

namespace GOLF_DESKTOP.Views.Pages {
    /// <summary>
    /// Lógica de interacción para SellClothesPage.xaml
    /// </summary>
    public partial class SellClothesPage : Page {
        public SellClothesPage() {
            InitializeComponent();
        }

        private void ClickPublish(object sender, RoutedEventArgs e) {
            string name = txtNombre.Text.Trim();
            string priceTxt = txtPrecio.Text.Trim();
            string quotaTxt = txtCantidad.Text.Trim();
            ComboBoxItem selectedItem = cbxTalla.SelectedItem as ComboBoxItem;
            string size = selectedItem?.Content.ToString();
            ComboBoxItem selectedItemArt = cbxTipoArticulo.SelectedItem as ComboBoxItem;
            string clotheCategory = selectedItemArt?.Content.ToString();

            if(name != null && priceTxt != null && quotaTxt != null && size != null && clotheCategory != null)
            {
                int quota, price;
                int.TryParse(quotaTxt, out quota);
                int.TryParse(priceTxt, out price);
                string idSelling = UserSingleton.GetInstance().IdUser;

                SaveArticle(name, clotheCategory, price, size, quota, idSelling);

            }
            else
            {
                MessageBox.Show("Faltan datos necesarios.",
                                    "Datos faltantes", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

        }

        private async void SaveArticle(string name, string clotheCategory, int price, string size, int quota, string idSelling)
        {


            bool results = await ArticulosServiceGrpc.SaveArticuloAsync(name, clotheCategory, price, size, quota, idSelling);

            if (results)
            {
                MessageBox.Show("Artículo guardado correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                NavigationService.Navigate(new HomePage());
            }
            else
            {
                MessageBox.Show("No se pudo guardar el artículo.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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

    }
}
