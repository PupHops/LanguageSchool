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
using System.Windows.Shapes;

namespace LanguageSchool
{
    /// <summary>
    /// Логика взаимодействия для Admin_wh.xaml
    /// </summary>
    public partial class Admin_wh : Window
    {
        public Admin_wh()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (Pass.Text == "0000")
            {
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show(sender);
                this.Close();
                mainWindow.ShowDialog();
            }
        }
    }
}
