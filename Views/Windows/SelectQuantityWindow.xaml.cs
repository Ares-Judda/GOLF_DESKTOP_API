using GOLF_DESKTOP.Model.Entities;
using GOLF_DESKTOP.Model.Utilities;
using GOLF_DESKTOP.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GOLF_DESKTOP.Views.Windows {
    /// <summary>
    /// Lógica de interacción para SelectQuantityWindow.xaml
    /// </summary>
    public partial class SelectQuantityWindow : Window {
        public int SelectedQuantity { get; private set; } = 0;

        public SelectQuantityWindow(int maxQuantity) {
            InitializeComponent();
            for (int i = 1; i <= maxQuantity; i++) {
                QuantityComboBox.Items.Add(i);
            }
            QuantityComboBox.SelectedIndex = 0;
        }

        private async void AcceptButton_Click(object sender, RoutedEventArgs e) {
            if (QuantityComboBox.SelectedItem != null) {
                SelectedQuantity = (int)QuantityComboBox.SelectedItem;

                var clothe = ClotheSingleton.GetInstance();
                var user = UserSingleton.GetInstance();

                try {
                   
                    string idUser = user.IdUser;
                    var response = await ApiServiceRest.AddClotheToShoppingCarAsync(clothe.ID_Clothes, SelectedQuantity, idUser);

                    if (response.IsSuccessStatusCode) {
                        MessageBox.Show("Artículo añadido al carrito exitosamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                        this.DialogResult = true; 
                    } else {
                        string errorMessage = await response.Content.ReadAsStringAsync();
                        MessageBox.Show($"Error al agregar al carrito: {errorMessage}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                } catch (Exception ex) {
                    MessageBox.Show($"Ocurrió un error al procesar la solicitud: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            } else {
               
                MessageBox.Show("Por favor selecciona una cantidad antes de continuar.", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
