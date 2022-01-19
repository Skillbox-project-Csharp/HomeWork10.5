using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace HomeWork10._5.telegramApp
{
    class CommandHelper
    {
        static public Dictionary<BotCommand,string> BotCommands { get; set; } = new Dictionary<BotCommand, string>()
        {
            { new BotCommand(){Command = "getlistaudio", Description = "Показать список аудиофайлов"}, "music" },
            { new BotCommand(){Command = "getlistdocument", Description = "Показать список документов"}, "documents" },
            { new BotCommand(){Command = "getlistphoto", Description = "Показать список фото"}, "photo" },
            {new BotCommand(){Command = "getlistvideo", Description = "Показать список видео"}, "video" },
            {new BotCommand(){Command = "getlistvoice", Description = "Показать список голосовых сообщений"}, "" }
        };


    }
}
