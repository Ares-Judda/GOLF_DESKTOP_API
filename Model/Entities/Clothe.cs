using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace GOLF_DESKTOP.Model.Entities
{
    public class Clothe
    {
        public Clothe() { }
        public int ID_Clothes { get; set; }
        public string ?Name { get; set; }
        public string ?ClotheCategory { get; set; }
        public int Price { get; set; }
        public int Quota { get; set; }
        public string ?Size { get; set; }
        public string ?Image {  get; set; }
        public BitmapImage ImageSource { get; set; }
    }
}
