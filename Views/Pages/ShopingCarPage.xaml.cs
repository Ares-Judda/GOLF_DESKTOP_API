using GOLF_DESKTOP.Model.Entities;
using GOLF_DESKTOP.Model.Utilities;
using GOLF_DESKTOP.Services;
using GOLF_DESKTOP.Views;
using GOLF_DESKTOP.Views.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace GOLF_DESKTOP.Views.Pages {
    /// <summary>
    /// Lógica de interacción para ShopingCarPage.xaml
    /// </summary>
    public partial class ShopingCarPage : Page, INotifyPropertyChanged {
        public ObservableCollection<Clothe> Clothes { get; set; }

        private decimal _pago;
        public decimal Pago {
            get { return _pago; }
            set {
                if (_pago != value) {
                    _pago = value;
                    OnPropertyChanged(nameof(Pago));
                }
            }
        }

        public ShopingCarPage() {
            InitializeComponent();
            DataContext = this;
            getArticles();
        }


        private void ClickReturnToHomePage(object sender, MouseButtonEventArgs e) {
            NavigationService.Navigate(new HomePage());
        }

        private async void getArticles() {
            var user = UserSingleton.GetInstance();
            string idUser = user.IdUser;
            var articles = await ApiServiceRest.GetShoppingCarAsync(idUser);

            if (articles != null && articles.Any()) {
                btnBuy.Visibility = Visibility.Visible;
                rtglTotal.Visibility = Visibility.Visible;
                txbBuy.Visibility = Visibility.Visible;
                Clothes = new ObservableCollection<Clothe>(articles);

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

                listaArticulos.ItemsSource = Clothes;
                CalculatePago();
            } else {
                MessageBox.Show("No hay artículos en el carrito.", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
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

        private void CalculatePago() {
            Pago = Clothes.Sum(c => c.Total ?? 0);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void MouseDoubleClick(object sender, MouseButtonEventArgs e) {
            var clothe = (Clothe)listaArticulos.SelectedItem;
            if (clothe != null) {
                int availableQuantity = clothe.Quantity ?? 0;
                if (availableQuantity > 0) {
                    ModifyQuantityWindow modifyQuantityWindow = new ModifyQuantityWindow(availableQuantity, (int)(clothe.Quantity ?? 0), clothe) {
                        Owner = Application.Current.MainWindow
                    };
                    modifyQuantityWindow.ShowDialog();
                    NavigationService.Navigate(new ShopingCarPage());

                } else {
                    MessageBox.Show("No hay suficientes existencias disponibles.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            } else {
                MessageBox.Show("No se ha seleccionado un artículo válido.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void btnBuy_Click(object sender, RoutedEventArgs e) {
            var user = UserSingleton.GetInstance();
            string userId = user.IdUser;
            var updatedClothes = Clothes.Select(c => new {
                ID_Clothes = c.ID_Clothes,
                newQuantity = c.Quantity
            }).Cast<dynamic>().ToList(); 
            try {
                var result = await ApiServiceRest.CheckAndUpdateCartAsync(userId, updatedClothes);

                if (result.IsSuccessStatusCode) {
                    MessageBox.Show("Compra realizada correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                    NavigationService.Navigate(new ShopingCarPage());
                } else {
                    string errorMessage = await result.Content.ReadAsStringAsync();
                    MessageBox.Show($"Error al procesar la compra: {errorMessage}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            } catch (Exception ex) {
                MessageBox.Show($"Ocurrió un error al procesar la solicitud: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnBuyHistory_Click(object sender, RoutedEventArgs e) {
            NavigationService.Navigate(new HistoryPurchasePage());
        }
    }
}
