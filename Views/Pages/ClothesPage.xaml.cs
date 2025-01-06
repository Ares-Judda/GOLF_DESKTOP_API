using GOLF_DESKTOP.Model.Entities;
using GOLF_DESKTOP.Model.Utilities;
using GOLF_DESKTOP.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Lógica de interacción para ClothesPage.xaml
    /// </summary>
    public partial class ClothesPage : Page {
        public ObservableCollection<Clothe> Clothes { get; set; }

        public ClothesPage() {
            InitializeComponent();
            getArticles();
        }

        private async void MouseDownSearch(object sender, MouseButtonEventArgs e)
        {
            try
            {
                string articleName = txbSearchArticle.Text.Trim();

                if (string.IsNullOrWhiteSpace(articleName))
                {
                    MessageBox.Show("Por favor, ingrese un nombre de artículo para buscar.", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var articles = await ArticulosServiceGrpc.GetArticulosBySellingAndNameAsync(UserSingleton.GetInstance().IdUser, articleName);

                if (articles.Any())
                {
                    foreach (var clothe in articles)
                    {
                        if (!string.IsNullOrEmpty(clothe.Image))
                        {
                            clothe.ImageSource = await LoadImageFromUrlAsync(clothe.Image);
                        }
                        else
                        {
                            clothe.ImageSource = GetDefaultImage();
                        }
                    }

                    listaArticulosVendedor.ItemsSource = articles;
                }
                else
                {
                    MessageBox.Show("No se encontraron artículos con un nombre similar.", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
                    listaArticulosVendedor.ItemsSource = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ocurrió un error al buscar el artículo: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void CategorySelection(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                ComboBoxItem selectedItem = cbxCategory.SelectedItem as ComboBoxItem;
                string category = selectedItem?.Content.ToString();

                var articles = await ArticulosServiceGrpc.GetArticulosBySellingAndCategoryAsync(UserSingleton.GetInstance().IdUser, category);

                if (articles.Any())
                {
                    foreach (var clothe in articles)
                    {
                        if (!string.IsNullOrEmpty(clothe.Image))
                        {
                            clothe.ImageSource = await LoadImageFromUrlAsync(clothe.Image);
                        }
                        else
                        {
                            clothe.ImageSource = GetDefaultImage();
                        }
                    }

                    listaArticulosVendedor.ItemsSource = articles;
                }
                else
                {
                    MessageBox.Show("No se encontraron artículos con esa categoría.", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
                    listaArticulosVendedor.ItemsSource = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ocurrió un error al buscar el artículo: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private async void getArticles()
        {
            var articulos = await ArticulosServiceGrpc.GetArticulosBySellingAsync(UserSingleton.GetInstance().IdUser);

            Clothes = new ObservableCollection<Clothe>(articulos);

            if (Clothes.Any())
            {
                foreach (var clothe in Clothes)
                {
                    if (!string.IsNullOrEmpty(clothe.Image))
                    {
                        clothe.ImageSource = await LoadImageFromUrlAsync(clothe.Image);
                    }
                    else
                    {
                        clothe.ImageSource = GetDefaultImage();
                    }
                }

                listaArticulosVendedor.ItemsSource = Clothes;
            }

        }

        private async Task<BitmapImage> LoadImageFromUrlAsync(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return GetDefaultImage(); // Usar imagen predeterminada si la URL es nula o vacía
            }

            try
            {
                var bitmap = new BitmapImage();
                using (var webClient = new System.Net.WebClient())
                {
                    var imageData = await webClient.DownloadDataTaskAsync(new Uri(url));
                    using (var stream = new MemoryStream(imageData))
                    {
                        bitmap.BeginInit();
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.StreamSource = stream;
                        bitmap.EndInit();
                    }
                }
                return bitmap;
            }
            catch (Exception)
            {
                return GetDefaultImage(); // Usar imagen predeterminada en caso de error
            }
        }

        private BitmapImage GetDefaultImage()
        {
            return new BitmapImage(new Uri("pack://application:,,,/Resources/Images/ClotheIcon.png"));
        }

        private void MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var selectedClothe = (Clothe)listaArticulosVendedor.SelectedItem;

            if (selectedClothe != null)
            {
                NavigationService.Navigate(new UpdateClothes(selectedClothe));
            }
        }

    }
}
