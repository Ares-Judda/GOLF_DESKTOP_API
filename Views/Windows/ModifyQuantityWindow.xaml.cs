using GOLF_DESKTOP.Model.Entities;
using GOLF_DESKTOP.Model.Utilities;
using GOLF_DESKTOP.Services;
using GOLF_DESKTOP.Views.Pages;
using System;
using System.Text.Json;
using System.Windows;
using System.Windows.Navigation;

namespace GOLF_DESKTOP.Views.Windows {
    public partial class ModifyQuantityWindow : Window {
        private int _availableQuantity;
        private int _currentQuantity;
        private Clothe _clothe;

        public int SelectedQuantity { get; private set; } = 0;

        public ModifyQuantityWindow(int availableQuantity, int currentQuantity, Clothe clothe) {
            InitializeComponent();
            _availableQuantity = availableQuantity;
            _currentQuantity = currentQuantity;
            _clothe = clothe;
            var clotheSingleton = ClotheSingleton.GetInstance();
            clotheSingleton.ID_Clothes = clothe.ID_Clothes;
            clotheSingleton.Quota = clothe.Quota;

            for (int i = 1; i <= _availableQuantity; i++) {
                QuantityComboBox.Items.Add(i);
            }

            QuantityComboBox.SelectedItem = 1;
        }

        private async void IncreaseQuantity_Click(object sender, RoutedEventArgs e) {
            if (QuantityComboBox.SelectedItem != null) {
                int selectedQuantity = (int)QuantityComboBox.SelectedItem;

                if (selectedQuantity <= _availableQuantity && selectedQuantity > 0) {
                    int newQuantity = _currentQuantity + selectedQuantity;
                    var user = UserSingleton.GetInstance();
                    var article = ClotheSingleton.GetInstance();

                    try {
                        var response = await ApiServiceRest.UpdateQuantityAsync(article.ID_Clothes, newQuantity, user.IdUser);

                        if (response.IsSuccessStatusCode) {
                            MessageBox.Show("Artículo añadido al carrito exitosamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                            this.DialogResult = true;
                            this.Close();
                        } else {

                            string errorMessage = await response.Content.ReadAsStringAsync();
                            MessageBox.Show($"Error al agregar al carrito: {errorMessage}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    } catch (Exception ex) {
                        MessageBox.Show($"Ocurrió un error al procesar la solicitud: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            } else {
                MessageBox.Show("Por favor selecciona una cantidad válida.", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private async void DecreaseQuantity_Click(object sender, RoutedEventArgs e) {
            if (QuantityComboBox.SelectedItem != null) {
                int selectedQuantity = (int)QuantityComboBox.SelectedItem;
                int newQuantity = _currentQuantity - selectedQuantity;
                if (newQuantity >= 1) {
                    var user = UserSingleton.GetInstance();
                    var article = ClotheSingleton.GetInstance();

                    try {
                        var response = await ApiServiceRest.UpdateQuantityAsync(article.ID_Clothes, newQuantity, user.IdUser);

                        if (response.IsSuccessStatusCode) {
                            MessageBox.Show("Cantidad disminuida exitosamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                            this.DialogResult = true;
                        } else {
                            string errorMessage = await response.Content.ReadAsStringAsync();
                            MessageBox.Show($"Error al disminuir la cantidad: {errorMessage}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    } catch (Exception ex) {
                        MessageBox.Show($"Ocurrió un error al procesar la solicitud: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                } else {
                    MessageBox.Show("La cantidad no puede ser menor que 1. Si deseas eliminar el artículo, utiliza la opción de eliminar.", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            } else {
                MessageBox.Show("Por favor selecciona una cantidad válida.", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private async void DeleteItem_Click(object sender, RoutedEventArgs e) {
            var result = MessageBox.Show("¿Seguro que deseas eliminar este artículo del carrito?", "Eliminar artículo", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes) {
                var user = UserSingleton.GetInstance();
                var article = ClotheSingleton.GetInstance();
                try {
                    var response = await ApiServiceRest.RemoveClotheFromCarAsync(article.ID_Clothes, user.IdUser);

                    if (response.IsSuccessStatusCode) {
                        MessageBox.Show("Articulo eliminado del carrito con exito.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                        this.DialogResult = true;
                       
                    } else {
                        string errorMessage = await response.Content.ReadAsStringAsync();
                        MessageBox.Show($"Error al eliminar el articulo de tu carrito: {errorMessage}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                } catch (Exception ex) {
                    MessageBox.Show($"Ocurrió un error al procesar la solicitud: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
