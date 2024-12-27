using GOLF_DESKTOP.Model.Entities;
using System;
using System.Collections.Generic;
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
    /// Lógica de interacción para ClothesDetails.xaml
    /// </summary>
    public partial class ClothesDetails : Page {
        private Clothe article;
        public ClothesDetails(Clothe clothe) {
            InitializeComponent();
            article = clothe;
            SetUpArticleInformation(article);
        }

        private void ClickAddToTheCar(object sender, RoutedEventArgs e) {

        }

        private void ClickReturnToHomePage(object sender, RoutedEventArgs e) {
            NavigationService.Navigate(new HomePage());
        }

        private void SetUpArticleInformation(Clothe article)
        {
            txtNombre.Text = article.Name;
            txtCantidad.Text = article.Quota.ToString();
            txtPrecio.Text = article.Price.ToString();

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

    }
}
