using GOLF_DESKTOP.Model.Entities;
using GOLF_DESKTOP.Model.Utilities;
using GOLF_DESKTOP.Services;
using GOLF_DESKTOP.Views;
using GOLF_DESKTOP.Views.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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
                listaArticulos.ItemsSource = Clothes;
                CalculatePago();
            } else {
                MessageBox.Show("No hay artículos en el carrito.", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
            }
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
