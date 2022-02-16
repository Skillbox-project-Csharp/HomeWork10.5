using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace HomeWork10._5.telegramApp
{
    public partial class TelegramUser : IEquatable<TelegramUser>
    {
        public ObservableCollection<TelegramSimpleMessage> Messages { set; get; }
        public User UserInfo { set; get; }
        public Chat ChatInfo { set; get; }
       public TelegramUser(User userInfo, Chat chatInfo)
        {
            UserInfo = userInfo;
            ChatInfo = chatInfo;
            
            Messages = new ObservableCollection<TelegramSimpleMessage>();
        }

        public bool Equals(TelegramUser other) => other.ChatInfo.Id == ChatInfo.Id;
    }

    public partial class TelegramSimpleMessage
    {
        public string TextMessage { get; set; }
        public bool IsBot { get; set; }
        public TelegramSimpleMessage(string textMessage, bool isBot)
        {
            TextMessage = textMessage;
            IsBot = isBot;
        }
    }
}
