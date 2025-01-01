using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GOLF_DESKTOP.Model.Utilities {
    public class ClotheSingleton {
        private static ClotheSingleton _instance;

        public int ?ID_Clothes { get; set; }
        public int Quota { get; set; }

        private ClotheSingleton() { }

        public static ClotheSingleton GetInstance() {
            if (_instance == null) {
                _instance = new ClotheSingleton();
            }
            return _instance;
        }
    }
}
