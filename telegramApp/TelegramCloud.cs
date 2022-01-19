
using System.Diagnostics;
using System.IO;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Extensions.Polling;
using System.Threading.Tasks;
using System;
using System.Threading;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Collections.Generic;
using Telegram.Bot.Types.ReplyMarkups;

namespace HomeWork10._5.telegramApp
{
    public class TelegramCloud
    {

        private TelegramBotClient Bot { get; set; }


        public TelegramCloud(string pathToken)
        {
            Bot = new TelegramBotClient(System.IO.File.ReadAllText(pathToken));
            var cts = new CancellationTokenSource();
            Bot.ReceiveAsync(new DefaultUpdateHandler(HandleUpdateAsync, HandleErrorAsync));
        }

        private static Task HandleErrorAsync(ITelegramBotClient bot, Exception exception, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private async Task HandleUpdateAsync(ITelegramBotClient bot, Update update, CancellationToken cancellationToken)
        {

            try
            {
                if (update.Message is Message)
                {
                    await BotOnMessageReceived(update.Message);
                    Debug.WriteLine(update.Type);
                }
            }
            catch (Exception exception)
            {
                await HandleErrorAsync(bot, exception, cancellationToken);
            }
        }

        protected virtual async Task BotOnMessageReceived(Message message)
        {

            foreach (var fileAddress in await GetFileMessagePathAsync(message))
            {
                BotSaveFile(fileAddress, AppContext.BaseDirectory, message.From);
                Bot.DeleteMessageAsync(message.Chat.Id, message.MessageId);
            }

            if (message.Text is string) ;
            BotCommand botCommand = new BotCommand();

            if (message.Entities is MessageEntity[] entities)
                for (int i = 0; i < entities.Length; i++)
                    if (entities[i].Type == Telegram.Bot.Types.Enums.MessageEntityType.BotCommand)
                    {
                        BotCommand command = new BotCommand() { Command = message.Text.Remove(0,1) };
                        BotDoCommand(command, message.Chat.Id, message.From);
                        Debug.WriteLine($"Command reed: {message.Text}");
                    }



        }

        protected virtual async Task<List<Telegram.Bot.Types.File>> GetFileMessagePathAsync(Message message)
        {
            List<Telegram.Bot.Types.File> listPathMediaFiles = new List<Telegram.Bot.Types.File>();
            List<FileBase> listMediaFiles = new List<FileBase>();
            if (message.Audio is Audio audio)
                listMediaFiles.Add(audio);
            if (message.Document is Document document)
                listMediaFiles.Add(document);
            if (message.Video is Video video)
                listMediaFiles.Add(video);
            if (message.Voice is Voice voice)
                listMediaFiles.Add(voice);
            if (message.Photo is PhotoSize[] photoSizes)
                foreach (var photoSize in photoSizes)
                    listMediaFiles.Add(photoSize);
            foreach (var file in listMediaFiles)
                listPathMediaFiles.Add(await Bot.GetFileAsync(file.FileId));
            return listPathMediaFiles;
        }

        protected virtual async Task BotSaveFile(Telegram.Bot.Types.File file, string mainDirecto, User user)
        {
            var pathTelegramFIle = file.FilePath.Split('/');
            var nameDirectory = pathTelegramFIle[0];
            var nameFile = pathTelegramFIle[1];
            string pathLocalFile = $@"{mainDirecto}users\{user.Id}\{nameDirectory}\";
            Directory.CreateDirectory(pathLocalFile);
            pathLocalFile += nameFile;
            Debug.WriteLine($"Path: {pathLocalFile}");

            using (Stream stream = new FileStream(pathLocalFile, FileMode.OpenOrCreate))
            {
                await Bot.DownloadFileAsync(file.FilePath, stream);
            }

        }

        protected virtual async Task BotSendTextMessageAsync(ChatId chatId, string text)
        {
            await Bot.SendTextMessageAsync(chatId, text);
        }

        protected virtual async Task BotDoCommand(BotCommand command, long chatId, User user)
        {
            string nameDirectory = string.Empty;
            Debug.WriteLine($"Command in func: {command.Command} vs");
            foreach (var key in CommandHelper.BotCommands.Keys)
                if (key.Command == command.Command)
                    if (CommandHelper.BotCommands.TryGetValue(key, out nameDirectory))
                    {
                        await Bot.SendTextMessageAsync
                             (chatId,
                             nameDirectory,
                             null,
                             null,
                             null,
                             null,
                             null,
                             null,
                             await BildingButton(GetSpecificFileList(user, nameDirectory))
                             );
                    }
        }

        private string[] GetSpecificFileList(User user, string nameDirectory)
        {
            string[] fileList = new string[0];
            try
            {
                fileList = Directory.GetFiles($@"{Directory.GetCurrentDirectory()}\users\{user.Id}\{nameDirectory}\");
            }
            catch (DirectoryNotFoundException) { }
            catch (ArgumentException) { }
            catch (UnauthorizedAccessException) { }
            catch (PathTooLongException) { };
            for (int i = 0; i < fileList.Length; i++)
            {
                var splitAddres = fileList[i].Split('\\');
                fileList[i] = splitAddres[splitAddres.Length - 1];
            }
            return fileList;
        }

        protected virtual async Task<InlineKeyboardMarkup> BildingButton(string[] textButton)
        {
            int lengthButton = textButton.Length;
            InlineKeyboardButton[][] inlineKeyboardButtons = new InlineKeyboardButton[lengthButton][];
            for (int i = 0; i < lengthButton; i++)
            {
                inlineKeyboardButtons[i] = new InlineKeyboardButton[1];
                inlineKeyboardButtons[i][0] = new InlineKeyboardButton(textButton[i]) { CallbackData = textButton[i] };
            }
            return new InlineKeyboardMarkup(inlineKeyboardButtons);
        }

    }
}
