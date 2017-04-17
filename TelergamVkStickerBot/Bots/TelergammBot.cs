using System;
using System.IO;
using System.Threading.Tasks;
using Telegram;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.InputMessageContents;
using Telegram.Bot.Types.ReplyMarkups;


namespace TelergamVkStickerBot.Bots
{
  public class TelergammBot
  {
    public Action<Message> MessageCallblack = ( Message m ) => { };

    private TelegramBotClient Api = new TelegramBotClient(Properties.Settings.Default.TelegrammToken);

    private void MessageCallbleck( object A, MessageEventArgs Args )
    {
      Message M = new Message();
      Telegram.Bot.Types.Message TgM = Args.Message;

      M.From = TgM.From.FirstName + " " + TgM.From.LastName + "(@" + TgM.From.Username + ")";
      M.Text = "";
      M.ChatId = TgM.Chat.Id;

      Attachment Att = new Attachment();

      switch (TgM.Type)
      {
        case MessageType.AudioMessage:
          Att.Type = AttachmentsType.Audio;
          Att.RepresentationTg = TgM.Audio.FileId;
          M.Attachments.Add(Att);
          M.Text = TgM.Audio.Performer + " - " + TgM.Audio.Title + " (" + TgM.Audio.Duration + ")";
          break;
        case MessageType.ContactMessage:
          M.Text = "has shared contact data of " + TgM.Contact.FirstName + " " + TgM.Contact.LastName + ":\n  Phone: " + TgM.Contact.PhoneNumber + "\n  Telegram: " + TgM.Contact.UserId;
          break;
        case MessageType.DocumentMessage:
          Att.Type = AttachmentsType.File;
          Att.RepresentationTg = TgM.Document.FileId;
          M.Attachments.Add(Att);
          M.Text = "has shared document: \"" + TgM.Document.FileName + "\" Type is \"" + TgM.Document.MimeType + "\"";
          break;
        case MessageType.LocationMessage:
          M.Text = "has shared location:\n  Latitude is " + TgM.Location.Latitude + "\n  Longtitude is " + TgM.Location.Longitude;
          break;
        case MessageType.PhotoMessage:
          Att.Type = AttachmentsType.Image;
          PhotoSize BestPhoto = TgM.Photo[0];
          foreach (PhotoSize Photo in TgM.Photo)
            if (Photo.Width > BestPhoto.Width || Photo.Height > BestPhoto.Height)
              BestPhoto = Photo;
          Att.RepresentationTg = BestPhoto.FileId;
          M.Attachments.Add(Att);
          M.Text = TgM.Caption;
          break;
        case MessageType.ServiceMessage:
          if (TgM.DeleteChatPhoto)
            M.Text = "Deleted chat photo((";
          /*



            A VERE HUTCH TU DU FOR SERVES MESADGES



            */

          break;
        case MessageType.StickerMessage:
          Att.Type = AttachmentsType.Sticker;
          Att.RepresentationTg = TgM.Sticker.FileId;
          M.Attachments.Add(Att);
          break;
        case MessageType.TextMessage:
          M.Text = TgM.Text;
          break;
        case MessageType.VideoMessage:
          Att.Type = AttachmentsType.Video;
          Att.RepresentationTg = TgM.Video.FileId;
          M.Attachments.Add(Att);
          M.Text = "has sent some video (" + TgM.Audio.Duration + ")";
          break;
        case MessageType.VoiceMessage:
          Att.Type = AttachmentsType.Audio;
          Att.RepresentationTg = TgM.Voice.FileId;
          M.Text = "has sent some voice message (" + TgM.Audio.Duration + ")";
          break;
      }

      MessageCallblack(M);

    }
    public TelergammBot()
    {
      Api.OnMessage += new EventHandler<MessageEventArgs>(MessageCallbleck);
      Api.StartReceiving();
    }

    public void Responce()
    {
    }

    public async Task SendMessage( Message M )
    {
      await Api.SendTextMessageAsync(M.ChatId, M.From + "\n" + M.Text);

      foreach (Attachment Att in M.Attachments)
      {
        switch (Att.Type)
        {
        }
      }
    }

    public async Task<Message> DownloadAttachments(Message M)
    {
      for (int i = 0; i < M.Attachments.Count; i++)
      {
        Telegram.Bot.Types.File TgFile = await Api.GetFileAsync(M.Attachments[i].RepresentationTg);
        string FileDir = "";
        string FilePath = "";

        if (M.Attachments[i].Type == AttachmentsType.Sticker)
          FileDir = "CachedStickers";
        else
          FileDir = "CachedData";

        System.IO.Directory.CreateDirectory(FileDir);

        FilePath = TgFile.FileId + "_" + TgFile.FilePath;

        try
        {
          FileStream File = new FileStream(FileDir + "/" + FilePath, FileMode.CreateNew);

          TgFile.FileStream.CopyTo(File);
          File.Close();

          await Api.SendTextMessageAsync(M.ChatId, "Successfully loaded");
        } catch
        {
          await Api.SendTextMessageAsync(M.ChatId, "Successfully not loaded, because cached");
        };
        //M.Attachments[i].FileName = FileDir + "/" + FilePath;
      }

      return M;
    }
  }
}