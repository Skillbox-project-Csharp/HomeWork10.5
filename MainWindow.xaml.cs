using HomeWork10._5.telegramApp;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
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

        private void SendTextBotUI()
        {
            var textForSend = TextForSend.Text;
            if (UsersChatInfo.SelectedItem is TelegramUser User)
                Client.BotSendTextMessageAsync(User.ChatInfo.Id, textForSend);
            TextForSend.Text = "";
            TextForSend.Focus();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SendTextBotUI();
        }

        private void TextForSend_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (TextForSend.Focusable)
                if (e.Key is Key.Enter)
                    SendTextBotUI();

        }

        private void Open(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = "|*.json";
            var result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK || result == System.Windows.Forms.DialogResult.Yes)
            {
                ObservableCollection<TelegramUser> readHistori = JsonConvert.DeserializeObject<ObservableCollection<TelegramUser>>(File.ReadAllText(dialog.FileName));
                HistoryView historyView = new HistoryView(readHistori);
                historyView.Show();
            }
        }
        private void Save(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog();
            dialog.Filter = "|*.json";
            var result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK || result == System.Windows.Forms.DialogResult.Yes )
            {
                File.WriteAllText(dialog.FileName, JsonConvert.SerializeObject(Client.BotMessageLog));
            }
        }

        private void Load(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = "|*.json";
            var result = dialog.ShowDialog();
            if(result == System.Windows.Forms.DialogResult.OK || result == System.Windows.Forms.DialogResult.Yes)
            {
                ObservableCollection<TelegramUser> readHistori = JsonConvert.DeserializeObject<ObservableCollection<TelegramUser>>(File.ReadAllText(dialog.FileName));
                foreach (var element in Client.BotMessageLog)
                    if (readHistori.Contains(element))
                        foreach (var message in element.Messages)
                            readHistori[readHistori.IndexOf(element)].Messages.Add(message);
                    else readHistori.Add(element);
                Client.BotMessageLog = readHistori;
                UsersChatInfo.ItemsSource = Client.BotMessageLog;
                UsersChatInfo.SelectedIndex = -1;
            }
        }

    }
}
