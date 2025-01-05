using GOLF_DESKTOP.Model.Entities;
using GOLF_DESKTOP.Model.Utilities;
using GOLF_DESKTOP.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Lógica de interacción para HistoryPurchasePage.xaml
    /// </summary>
    public partial class HistoryPurchasePage : Page {
        public ObservableCollection<Clothe> Clothes { get; set; }
        public HistoryPurchasePage() {
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
            var articles = await ApiServiceRest.GetPurchaseHistoryAsync(idUser);

            if (articles != null && articles.Any()) {
                Clothes = new ObservableCollection<Clothe>(articles);
                listaArticulos.ItemsSource = Clothes;
            } else {
                MessageBox.Show("No hay artículos en el historial.", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
