using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace HomeWork10._5.telegramApp
{
    public partial class UIConnectorTelegramCloud :  TelegramCloud
    {
        private MainWindow Window { get; set; }
        public ObservableCollection<Message> BotMessageLog { get; set; }
        public UIConnectorTelegramCloud(MainWindow window, string pathToken) : base(pathToken)
        {
            Window = window;
            BotMessageLog = new ObservableCollection<Message>();
        }

        protected override Task BotOnMessageReceived(Message message)
        {
            Window.Dispatcher.Invoke(() =>
            {
                BotMessageLog.Add(message);
            }
);
            return base.BotOnMessageReceived(message);
        }
    }
}
