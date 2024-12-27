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
    /// Lógica de interacción para EarningsPage.xaml
    /// </summary>
    public partial class EarningsPage : Page {
        public ObservableCollection<Sale> Sales { get; set; }
        private static string vendedor;

        public EarningsPage() {
            InitializeComponent();
        }

        private void OnDateChanged(object sender, SelectionChangedEventArgs e)
        {
            _ = HandleDateChangeAsync();
        }

        private async Task HandleDateChangeAsync()
        {
            if (dpInicio.SelectedDate.HasValue && dpCorte.SelectedDate.HasValue)
            {
                DateTime startDate = dpInicio.SelectedDate.Value;
                DateTime endDate = dpCorte.SelectedDate.Value;

                if (startDate > endDate)
                {
                    MessageBox.Show("La fecha de inicio no puede ser mayor que la fecha de corte.",
                                    "Rango de fechas inválido",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Warning);
                    return;
                }
                else if (endDate < startDate)
                {
                    MessageBox.Show("La fecha de corte no puede ser menor que la fecha de inicio.",
                                   "Rango de fechas inválido",
                                   MessageBoxButton.OK,
                                   MessageBoxImage.Warning);
                    return;
                }

                await BringSales(startDate, endDate);
                lbDates.Visibility = Visibility.Collapsed;
                listaArticulosVendidos.Visibility = Visibility.Visible;
            }
        }

        private async Task BringSales(DateTime startDate, DateTime endDate)
        {
            var articulos = await SalesServiceGrpc.GetAllSalesAsync(UserSingleton.GetInstance().IdUser, startDate.ToString("yyyy-MM-dd"), endDate.ToString("yyyy-MM-dd"));

            if (articulos.Any())
            {
                vendedor = articulos.FirstOrDefault().Selling;
                txtVendedor.Text = vendedor;
            }

            Sales = new ObservableCollection<Sale>(articulos);

            if (Sales.Any())
            {
                listaArticulosVendidos.ItemsSource = Sales;
                CalculateEarnings();
                CalculateSales();
            }
            else
            {
                listaArticulosVendidos.ItemsSource = null;
                txtGanancias.Text = "0";
                txtVentas.Text = "0";
            }

        }

        private void CalculateEarnings()
        {
            if (Sales == null || !Sales.Any())
            {
                txtGanancias.Text = "0";
                return;
            }

            var earnings = Sales.Sum(sale => sale.Quantity * sale.PriceArticle);
            txtGanancias.Text = earnings.ToString("C"); 
        }

        private void CalculateSales()
        {
            if (Sales == null || !Sales.Any())
            {
                txtVentas.Text = "0";
                return;
            }

            var ventas = Sales.Sum(sale => sale.Quantity);
            txtVentas.Text = ventas.ToString();
        }

    }
}
