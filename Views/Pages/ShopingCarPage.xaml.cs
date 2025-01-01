using GOLF_DESKTOP.Model.Entities;
using GOLF_DESKTOP.Model.Utilities;
using GOLF_DESKTOP.Services;
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
            NavigationService.GoBack();
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
    }
}
