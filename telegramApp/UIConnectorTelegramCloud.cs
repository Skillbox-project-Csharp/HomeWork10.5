using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace HomeWork10._5.telegramApp
{
    public partial class UIConnectorTelegramCloud : TelegramCloud
    {
        private MainWindow Window { get; set; }
        public ObservableCollection<TelegramUser> BotMessageLog { get; set; }
        public UIConnectorTelegramCloud(MainWindow window, string pathToken) : base(pathToken)
        {
            Window = window;
            BotMessageLog = new ObservableCollection<TelegramUser>();


        }

        protected override Task BotOnMessageReceived(Message message)
        {
            Window.Dispatcher.Invoke(() =>
            {
                var person = new TelegramUser(message.From, message.Chat);
                Debug.WriteLine($"!BotMessageLog.Contains(person): {!BotMessageLog.Contains(person)}");
                if (!BotMessageLog.Contains(person)) BotMessageLog.Add(person);
                if (message.Text is string)
                    BotMessageLog[BotMessageLog.IndexOf(person)].Messages.Add(new TelegramSimpleMessage(message.Text, message.From.IsBot));
                foreach (var fileName in GetNamesFiles(message))
                    BotMessageLog[BotMessageLog.IndexOf(person)].Messages.Add(new TelegramSimpleMessage($"Файл {fileName} загружен", message.From.IsBot));
            }
            );
            return base.BotOnMessageReceived(message);
        }

        public static List<string> GetNamesFiles(Message message)
        {
            List<string> nameFiles = new List<string>();
            Debug.WriteLine($"Message type: {message.Type}");
            switch (message.Type)
            {
                case MessageType.Audio:
                    nameFiles.Add(message.Audio.FileName);
                    break;
                case MessageType.Video:
                    nameFiles.Add(message.Video.FileName);
                    break;
                case MessageType.Voice:
                    nameFiles.Add(message.Voice.FileId);
                    break;
                case MessageType.Document:
                    nameFiles.Add(message.Document.FileName);
                    break;
                case MessageType.Photo:
                    foreach (var photo in message.Photo)
                        nameFiles.Add(photo.FileId);
                    break;
            }
            return nameFiles;
        }

        protected async override Task<Message> BotSendMediaFileAsync(long chatId, string filePath, string typeFile)
        {
            Message message = await base.BotSendMediaFileAsync(chatId, filePath, typeFile);
            if (message != null)
            {
                Window.Dispatcher.Invoke(() =>
                {
                    var person = new TelegramUser(message.From, message.Chat);
                    Debug.WriteLine($"!BotMessageLog.Contains(person): {!BotMessageLog.Contains(person)}");
                    if (!BotMessageLog.Contains(person)) BotMessageLog.Add(person);
                    if (message.Text is string)
                        BotMessageLog[BotMessageLog.IndexOf(person)].Messages.Add(new TelegramSimpleMessage(message.Text, message.From.IsBot));

                    foreach (var fileName in GetNamesFiles(message))
                        BotMessageLog[BotMessageLog.IndexOf(person)].Messages.Add(new TelegramSimpleMessage($"{typeFile} файл {fileName} отправлен", message.From.IsBot));
                }
                );
            }
            return message;
        }

        public async override Task<Message> BotSendTextMessageAsync(ChatId chatId, string text)
        {
            Message message = await base.BotSendTextMessageAsync(chatId, text);
            if (message != null)
                Window.Dispatcher.Invoke(() =>
                {
                    var person = new TelegramUser(message.From, message.Chat);
                    Debug.WriteLine($"!BotMessageLog.Contains(person): {!BotMessageLog.Contains(person)}");
                    if (!BotMessageLog.Contains(person)) BotMessageLog.Add(person);
                    if (message.Text is string)
                        BotMessageLog[BotMessageLog.IndexOf(person)].Messages.Add(new TelegramSimpleMessage(message.Text, message.From.IsBot));
                }
                );
            return message;
        }

        public async override Task<Message> BotSendTextMessageAsync(ChatId chatId, string text, InlineKeyboardMarkup buttons)
        {
            Message message = await base.BotSendTextMessageAsync(chatId, text, buttons);
            if (message is Message)
            {
                Window.Dispatcher.Invoke(() =>
                {
                    var person = new TelegramUser(message.From, message.Chat);
                    StringBuilder textSendMessage = new StringBuilder();
                    if (!BotMessageLog.Contains(person)) BotMessageLog.Add(person);
                    if (message.Text is string)
                        textSendMessage.AppendLine(message.Text);
                    if (message?.ReplyMarkup?.InlineKeyboard?.ToList() is List<IEnumerable<InlineKeyboardButton>> ListButton)
                    {
                        foreach (var rowButton in ListButton)
                            foreach (var button in rowButton)
                                textSendMessage.AppendLine($"\tButton: {button.Text}");
                    }
                    if (textSendMessage.Length != 0)
                        BotMessageLog[BotMessageLog.IndexOf(person)].Messages.Add(new TelegramSimpleMessage(textSendMessage.ToString(), message.From.IsBot));
                }
                );
                return message;
            }
            else return null;
        }
    }
}
