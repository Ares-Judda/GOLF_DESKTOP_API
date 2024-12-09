﻿using GOLF_DESKTOP.Views.Windows;
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
    /// Lógica de interacción para HomePage.xaml
    /// </summary>
    public partial class HomePage : Page {
        public HomePage() {
            InitializeComponent();
        }

        private void MouseDownShopingCar(object sender, MouseButtonEventArgs e) {
            NavigationService.Navigate(new ShopingCarPage());
        }
    }
}
