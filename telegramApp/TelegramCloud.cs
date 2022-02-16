
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
using System.Linq;

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
            Bot.SetMyCommandsAsync(CommandHelper.BotCommands.Keys);
        }

        private static Task HandleErrorAsync(ITelegramBotClient bot, Exception exception, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private async Task HandleUpdateAsync(ITelegramBotClient bot, Update update, CancellationToken cancellationToken)
        {

            try
            {

                Debug.WriteLine(update.Type);
                if (update.Message is Message)
                {
                    await BotOnMessageReceived(update.Message);
                }
                if (update.CallbackQuery is CallbackQuery)
                    await BotOnCallbackQueryReceived(update.CallbackQuery);
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
                await BotSaveFile(fileAddress, AppContext.BaseDirectory, message.From);
                await Bot.DeleteMessageAsync(message.Chat.Id, message.MessageId);
            }


            if (message.Entities is MessageEntity[] entities)
                for (int i = 0; i < entities.Length; i++)
                    if (entities[i].Type == Telegram.Bot.Types.Enums.MessageEntityType.BotCommand)
                    {
                        BotCommand command = new BotCommand() { Command = message.Text.Remove(0, 1) };
                        await BotDoCommand(command, message.Chat, message.From);
                        Debug.WriteLine($"Command reed: {message.Text}");
                    }
        }

        protected virtual async Task BotOnCallbackQueryReceived(CallbackQuery callbackQuery)
        {
            if (callbackQuery.Message is Message message)
                if (message.Text is string text)
                {
                    string pathFile = $@"{AppContext.BaseDirectory}users\{callbackQuery.From.Id}\{text}\{callbackQuery.Data}";
                    Debug.WriteLine($"\tAFilePath: {pathFile}");
                    await BotSendMediaFileAsync(callbackQuery.From.Id, pathFile, text);
                    await Bot.DeleteMessageAsync(message.Chat.Id, message.MessageId);
                }
        }
        protected async Task<List<Telegram.Bot.Types.File>> GetFileMessagePathAsync(Message message)
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

        protected async Task BotSaveFile(Telegram.Bot.Types.File file, string mainDirecto, User user)
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

        public virtual async Task<Message> BotSendTextMessageAsync(ChatId chatId, string text)
        {
            return await Bot.SendTextMessageAsync(chatId, text);
        }

        public virtual async Task<Message> BotSendTextMessageAsync(ChatId chatId, string text, InlineKeyboardMarkup buttons)
        {
            return await Bot.SendTextMessageAsync(chatId, text, null, null, null, null, null, null, buttons);
        }
        protected async Task BotDoCommand(BotCommand command, Chat chat, User user)
        {
            string nameDirectory = string.Empty;
            Debug.WriteLine($"Command in func: {command.Command}");
            foreach (var key in CommandHelper.BotCommands.Keys)
                if (key.Command == command.Command)
                    if (CommandHelper.BotCommands.TryGetValue(key, out nameDirectory))
                    {
                        await BotSendTextMessageAsync(chat.Id, nameDirectory, BildingButton(GetSpecificFileList(user, nameDirectory)));
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

        protected virtual InlineKeyboardMarkup BildingButton(string[] textButton)
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

        protected virtual async Task<Message> BotSendAudioFileAsync(long chatId, string filePath)
        {
            using (Stream stream = System.IO.File.OpenRead(filePath))
            {
                var fileName = filePath.Split('\\').Last();
                var message = await Bot.SendAudioAsync(
                    chatId: chatId,
                    audio: new InputMedia(stream, fileName)
                );
                return message;
            }
        }
        protected virtual async Task<Message> BotSendVoiceFileAsync(long chatId, string filePath)
        {
            using (Stream stream = System.IO.File.OpenRead(filePath))
            {
                var fileName = filePath.Split('\\').Last();
                var message = await Bot.SendVoiceAsync(
                    chatId: chatId,
                    voice: new InputMedia(stream, fileName)
                );
                return message;
            }
        }

        protected virtual async Task<Message> BotSendDocumentFileAsync(long chatId, string filePath)
        {
            using (Stream stream = System.IO.File.OpenRead(filePath))
            {
                var fileName = filePath.Split('\\').Last();
                var message = await Bot.SendDocumentAsync(
                    chatId: chatId,
                    document: new InputMedia(stream, fileName)
                );
                return message;
            }
        }
        protected virtual async Task<Message> BotSendVideoFileAsync(long chatId, string filePath)
        {
            using (Stream stream = System.IO.File.OpenRead(filePath))
            {
                var fileName = filePath.Split('\\').Last();
                var message = await Bot.SendVideoAsync(
                    chatId: chatId,
                    video: new InputMedia(stream, fileName)
                );
                return message;
            }
        }

        protected virtual async Task<Message> BotSendPhotoFileAsync(long chatId, string filePath)
        {
            using (Stream stream = System.IO.File.OpenRead(filePath))
            {
                var fileName = filePath.Split('\\').Last();
                var message = await Bot.SendPhotoAsync(
                    chatId: chatId,
                    photo: new InputMedia(stream, fileName)
                );
                return message;
            }
        }

        protected virtual async Task<Message> BotSendMediaFileAsync(long chatId, string filePath, string typeFile)
        {
            Message message = null;
            switch (typeFile)
            {
                case "music":
                    message = await BotSendAudioFileAsync(chatId, filePath);
                    break;
                case "documents":
                    message = await BotSendDocumentFileAsync(chatId, filePath);
                    break;
                case "voice":
                    message = await BotSendVoiceFileAsync(chatId, filePath);
                    break;
                case "video":
                    message = await BotSendVideoFileAsync(chatId, filePath);
                    break;
                case "photo":
                    message = await BotSendPhotoFileAsync(chatId, filePath);
                    break;
            }
            return message;
        }

    }
}
