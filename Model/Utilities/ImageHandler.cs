using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace GOLF_DESKTOP.Model.Utilities {
    public static class ImageHandler {
        public static byte[] ConvertImageToBytes(BitmapImage bitmapImage) {
            if (bitmapImage == null) return null;
            using (var ms = new MemoryStream()) {
                var encoder = new JpegBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmapImage));
                encoder.Save(ms);
                return ms.ToArray();
            }
        }

        public static bool HasImage(BitmapImage image) {
            return image != null;
        }
        
    }
}
