using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace GOLF_DESKTOP.Model.Entities
{
    public class Sale
    {
        public Sale() { }
        public int ID_Purchase { get; set; }
        public string? ID_Client { get; set; }
        public int ID_Clothes { get; set; }
        public int Quantity { get; set; }
        public string? Date { get; set; }
        public string? NameArticle { get; set; }
        public int PriceArticle { get; set; }
        public string? Selling {  get; set; }
        public string? Image { get; set; }
        public BitmapImage ImageSource { get; set; }
    }
}
