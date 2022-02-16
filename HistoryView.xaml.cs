using HomeWork10._5.telegramApp;
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
using System.Windows.Shapes;

namespace HomeWork10._5
{
    /// <summary>
    /// Логика взаимодействия для HistoryView.xaml
    /// </summary>
    public partial class HistoryView : Window
    {
        public ObservableCollection<TelegramUser> UsersHistory { get; set; }
        public HistoryView(ObservableCollection<TelegramUser> usersHistory)
        {
            InitializeComponent();
            UsersHistory = usersHistory;
            LBHistoryUser.ItemsSource = UsersHistory;
        }
    }
}
