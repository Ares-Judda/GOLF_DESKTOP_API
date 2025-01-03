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
using Microsoft.Win32;
using PdfSharpCore.Pdf;
using PdfSharpCore.Drawing;

namespace GOLF_DESKTOP.Views.Pages {
    /// <summary>
    /// Lógica de interacción para InventoryPage.xaml
    /// </summary>
    public partial class InventoryPage : Page {

        public ObservableCollection<Clothe> Clothes { get; set; }

        public InventoryPage() {
            InitializeComponent();
            getArticles();
        }

        private void ClickPDFClothesPage(object sender, MouseButtonEventArgs e) {
            CreatePdfWithClothes();
        }

        private async void getArticles() {
            var user = UserSingleton.GetInstance();
            string idUser = user.IdUser;
            var articles = await ApiServiceRest.GetInventoryAsync(idUser);
            if (articles != null && articles.Any()) {
                Clothes = new ObservableCollection<Clothe>(articles);
                listaArticulos.ItemsSource = Clothes;

            } else {
                MessageBox.Show("No hay artículos en el inventario.", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void CreatePdfWithClothes() {
            try {
                SaveFileDialog saveFileDialog = new SaveFileDialog {
                    Filter = "Archivos PDF (*.pdf)|*.pdf",
                    DefaultExt = ".pdf",
                    FileName = "Inventario_" + DateTime.Now.ToString("yyyyMMdd") 
                };

                if (saveFileDialog.ShowDialog() == true) {
                    string filePath = saveFileDialog.FileName;

                    PdfDocument pdfDoc = new PdfDocument();
                    PdfPage page = pdfDoc.AddPage();
                    XGraphics gfx = XGraphics.FromPdfPage(page);
                    XFont font = new XFont("Verdana", 12, XFontStyle.Regular);

                    double yPosition = 20;

                    gfx.DrawString("Inventario de Artículos", new XFont("Verdana", 14, XFontStyle.Bold), XBrushes.Black, 40, yPosition);
                    yPosition += 30;

                    gfx.DrawString("Fecha de generación: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), font, XBrushes.Black, 40, yPosition);
                    yPosition += 30;

                    foreach (var clothe in Clothes) {
                        gfx.DrawString($"Nombre: {clothe.Name}", font, XBrushes.Black, 130, yPosition);
                        yPosition += 20;
                        gfx.DrawString($"Categoría: {clothe.ClotheCategory}", font, XBrushes.Black, 130, yPosition);
                        yPosition += 20;
                        gfx.DrawString($"Talla: {clothe.Size}", font, XBrushes.Black, 130, yPosition);
                        yPosition += 20;
                        gfx.DrawString($"Existencias: {clothe.Quota}", font, XBrushes.Black, 130, yPosition);
                        yPosition += 20;
                        gfx.DrawString($"Código: {clothe.ID_Clothes}", font, XBrushes.Black, 130, yPosition);

                        yPosition += 40;

                        gfx.DrawString($"Precio: {clothe.Price}", new XFont("Verdana", 12, XFontStyle.Bold), XBrushes.Black, 330, yPosition);
                        yPosition += 30;
                    }

                    pdfDoc.Save(filePath);

                    MessageBox.Show("PDF creado con éxito ", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            } catch (System.Exception ex) {
                MessageBox.Show("Error al generar el PDF: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
