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

namespace HomeWork10._5
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private telegramApp.UIConnectorTelegramCloud Client { get; set; }
        public MainWindow()
        {

            InitializeComponent();
            Client = new telegramApp.UIConnectorTelegramCloud(this, AppContext.BaseDirectory + @"\token.txt");
            
            UsersChatInfo.ItemsSource = Client.BotMessageLog;
        }
    }
}
