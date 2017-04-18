using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AngleSharp.Network.Default;
using Newtonsoft.Json.Linq;
using VkNet;
using VkNet.Enums;
using VkNet.Enums.Filters;
using VkNet.Model;
using VkNet.Model.Attachments;
using VkNet.Model.RequestParams;
using VkNet.Utils;
using VkNet.Utils.AntiCaptcha;

// Пишу коментарии на русском тому шо могу, здесь все в Юникоде 卐
namespace TelergamVkStickerBot.Bots
{
  public class VkBot
  {
    private static WebClient wc = new WebClient();

    private static string UploadFile(string url, string file)
    {
      string response = Encoding.ASCII.GetString(wc.UploadFile(url, file));
      string keyvals = "\"file\":\"";
      int start = response.IndexOf(keyvals) + keyvals.Length,
        end = response.IndexOf("\"", start + 1);
      return response.Substring(start, end - start);
    }

    // Я ниибу как эта функция работает, я её с левого форума скопировал
    private static string UploadPhoto(string url, string file)
    { 
      string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
      byte[] boundarybytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");

      HttpWebRequest wr = (HttpWebRequest)WebRequest.Create(url);
      wr.ContentType = "multipart/form-data; boundary=" + boundary;
      wr.Method = "POST";
      wr.KeepAlive = true;
      wr.Credentials = System.Net.CredentialCache.DefaultCredentials;

      Stream rs = wr.GetRequestStream();
      rs.Write(boundarybytes, 0, boundarybytes.Length);
      string header = "Content-Disposition: form-data; name=\"photo\"; filename=\"photo.png\"\r\nContent-Type: image/jpeg\r\n\r\n";
      byte[] headerbytes = System.Text.Encoding.UTF8.GetBytes(header);
      rs.Write(headerbytes, 0, headerbytes.Length);

      FileStream fileStream = new FileStream(file, FileMode.Open, FileAccess.Read);
      byte[] buffer = new byte[4096];
      int bytesRead = 0;
      while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
      {
        rs.Write(buffer, 0, bytesRead);
      }
      fileStream.Close();

      byte[] trailer = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
      rs.Write(trailer, 0, trailer.Length);
      rs.Close();

      WebResponse wresp = wr.GetResponse();
      Stream stream2 = wresp.GetResponseStream();
      StreamReader reader2 = new StreamReader(stream2);
      string s3 = reader2.ReadToEnd();
      return s3; 
    }

    private static Uri GetUri(Photo photo)
    {
      if (photo.Photo2560 != null)
        return photo.Photo2560;
      if (photo.Photo1280 != null)
        return photo.Photo1280;
      if (photo.Photo807 != null)
        return photo.Photo807;
      if (photo.Photo604 != null)
        return photo.Photo604;
      if (photo.Photo130 != null)
        return photo.Photo130;
      if (photo.Photo75 != null)
        return photo.Photo75;
      throw new ZeigHeil();
    }

    private static Uri GetUri(Sticker sticker)
    {
      if (sticker.Photo352 != null)
        return new Uri(sticker.Photo352);
      if (sticker.Photo256 != null)
        return new Uri(sticker.Photo256);
      if (sticker.Photo128 != null)
        return new Uri(sticker.Photo128);
      if (sticker.Photo64 != null)
        return new Uri(sticker.Photo64);
      throw new ZeigHeil();
    }

    private static Uri GetUri(VkNet.Model.Attachments.Attachment attachment)
    {
      switch (Types[attachment.Type])
      {
        case AttachmentsType.Image:
          return GetUri((Photo)attachment.Instance);
        case AttachmentsType.Sticker:
          return GetUri((Sticker)attachment.Instance);
      }
      throw new ZeigHeil();
    }

    private static Attachment TransformAttachment(VkNet.Model.Attachments.Attachment attachment)
    {
      Attachment a = new Attachment();
      a.Type = Types[attachment.Type];
      a.FileName = GetUri(attachment).OriginalString;
      return a;
    }

    private VkApi api = new VkApi(new FormCaptcha());

    public Action<Message> MessageCallblack = (Message m) => { };

    MessagesGetLongPollHistoryParams LongPollParams = new MessagesGetLongPollHistoryParams();

    public VkBot()
    {
      // Authorize
      api.Authorize(new ApiAuthParams()
      {
        ApplicationId = 5977261,
        Login = Properties.Settings.Default.VkLogin,
        Password = Properties.Settings.Default.VkPassword,
        Settings = Settings.All
      });
      // Get last message
      // пиздец извращение ? значит, что переменная может быть равна null
      // как id может быть null - непонятно
      long? lastId = api.Messages.Get(new MessagesGetParams()
        {
          Out = MessageType.Received,
          Count = 1
        })
        .Messages.First()
        .Id;
      // А вдруг...
      if (!lastId.HasValue)
        throw new ZeigHeil();

      LongPollServerResponse serv = api.Messages.GetLongPollServer(true, true);
      LongPollParams.Pts = serv.Pts;
      LongPollParams.Ts = serv.Ts;
      LongPollParams.PreviewLength = 0;
      LongPollParams.Onlines = false;
      LongPollParams.EventsLimit = null;
      UsersFields f = new UsersFields();
      LongPollParams.Fields = f;
      LongPollParams.MsgsLimit = null;
      LongPollParams.MaxMsgId = lastId;

    }

    private static Dictionary<Type, AttachmentsType> Types = new Dictionary<Type, AttachmentsType>()
    {
      {typeof(Photo), AttachmentsType.Image},
      {typeof(Video), AttachmentsType.Video},
      {typeof(Audio), AttachmentsType.Audio},
      {typeof(Document), AttachmentsType.File},
      {typeof(Link), AttachmentsType.Link},
      {typeof(Market), AttachmentsType.Link},
      {typeof(MarketAlbum), AttachmentsType.Link},
      {typeof(WallReply), AttachmentsType.Link},
      {typeof(Sticker), AttachmentsType.Sticker},
      {typeof(Gift), AttachmentsType.Sticker}
    };

    public void SendMessage(Message message)
    {
      MessagesSendParams msg = new MessagesSendParams();
      if (message.ChatId > 0)
        msg.ChatId = message.ChatId;
      else
        msg.UserId = -message.ChatId;
      msg.Message = message.Text;
      List<MediaAttachment> l = new List<MediaAttachment>();
      foreach (Attachment attachment in message.Attachments)
        l.Add(attachment.RepresentationVk);
      msg.Attachments = l;
      api.Messages.Send(msg);
    }

    private int N;

    public Attachment CreateSticker( Image image, long ChatId )
    {
      Attachment a = new Attachment();
      a.Type = AttachmentsType.Sticker;
      image.Save("temp_" + N.ToString() + ".png", ImageFormat.Png);
      string serverAddress = api.Call("docs.getUploadServer", new VkParameters()
        {
          {"type", "graffiti"},
          {"access_token", api.Token}
        })["upload_url"]
        .ToString();
      // Load file
      string file = UploadFile(serverAddress, "temp_" + N.ToString() + ".png");
      var graf = JObject.Parse(api.Call("docs.save", new VkParameters()
          {
            {"file", file},
            {"title", "graffiti.png"}
          })
          .RawJson)["response"]
        .ToArray()
        .First();
      List<MediaAttachment> att = new List<MediaAttachment>();

      Document graffiti = new Document();
      graffiti.Ext = graf["ext"].ToString();
      graffiti.Id = graf["id"].ToObject<long>();
      graffiti.OwnerId = graf["owner_id"].ToObject<long>();
      graffiti.Title = graf["title"].ToString();
      graffiti.Size = graf["size"].ToObject<long>();
      graffiti.Uri = graf["url"].ToString();
      N++;
      a.RepresentationVk = graffiti;
      a.FileName = new Uri(graffiti.Uri).OriginalString;
      return a;
    }

    public Attachment CreateAttachment(Image image, long ChatId)
    {
      Attachment a = new Attachment();
      a.Type = AttachmentsType.Image;
      UploadServerInfo serverInfo = api.Photo.GetMessagesUploadServer();
      image.Save("temp_" + N.ToString() + ".png", ImageFormat.Png);
      string file = UploadPhoto(serverInfo.UploadUrl, "temp_" + N.ToString() + ".png");
      N++;
      var blyat = api.Photo.SaveMessagesPhoto(file);
      a.RepresentationVk = blyat.First();
      a.FileName = GetUri((Photo)a.RepresentationVk).OriginalString;
      return a;
    }

    public Image GetImage(Attachment attachment)
    {
      if (attachment.Type != AttachmentsType.Image && attachment.Type != AttachmentsType.Sticker)
        throw new ZeigHeil();
      WebRequest request = WebRequest.Create(attachment.FileName);
      WebResponse response = request.GetResponse();
      Stream img = response.GetResponseStream();
      if (img == null)
        throw new ZeigHeil();
      return Image.FromStream(img);
    }

    private DateTime LastResponse = DateTime.Now;
    private DateTime func(VkNet.Model.Message m)
    {
      return m.Date.GetValueOrDefault();
    }

    public void Response()
    {
      while (DateTime.Now - LastResponse < TimeSpan.FromSeconds(1 / api.RequestsPerSecond + 0.01))
        ;
      LastResponse = DateTime.Now;
      api.Account.SetOnline(false);

      LongPollHistoryResponse longpoll = api.Messages.GetLongPollHistory(LongPollParams);

      LongPollParams.Pts = longpoll.NewPts;

      longpoll.Messages.OrderBy(new Func<VkNet.Model.Message, DateTime>(func));

      foreach (var message in longpoll.Messages)
      {
        if (message.Type == MessageType.Sended)
          continue;
        Message msg = new Message();
        msg.ChatId = message.ChatId.GetValueOrDefault(-message.UserId.GetValueOrDefault());
        msg.Text = message.Body;
        User from = api.Users.Get(message.UserId.GetValueOrDefault(), ProfileFields.All);
        msg.From = from.FirstName + " " + from.LastName;
        foreach (VkNet.Model.Attachments.Attachment attachment in message.Attachments)
          msg.Attachments.Add(TransformAttachment(attachment));
        if (LongPollParams.MaxMsgId < message.Id)
          LongPollParams.MaxMsgId = message.Id.GetValueOrDefault();
        MessageCallblack(msg);
        api.Messages.MarkAsRead(message.Id.GetValueOrDefault());
      }
    }
  }
}