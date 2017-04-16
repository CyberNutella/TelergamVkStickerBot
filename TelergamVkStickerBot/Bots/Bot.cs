using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace TelergamVkStickerBot.Bots
{
  public class Bot
  {
//    VkBot vkBot = new VkBot();
    TelergammBot tgBot = new TelergammBot();

    public class Association
    {
      public string BethesdaName;
      public long UniqueId;
      public long VkId;
      public long TgId;
    }

    List<Association> Associations = new List<Association>(); // List for slow search, when checking group name while creating unique id, 
    Hashtable VkToTg = new Hashtable(); // Hashtables for adamantine-cyber-extra-supreme-mega-gold-ultra-fast Id translations
    Hashtable TgToVk = new Hashtable();

    public Bot()
    {
      /* 413r7 (ALERT) if you have not this file, code will throw some exceptions at you */
      /* No time to fix */

      /* Associations file example */
/*
test#1337; Vk:02, Tg:39
test#1477; Vk:59, Tg:30
cake#1477; Vk:11, Tg:47
 */
      string []AssTexts = File.ReadAllLines("Data/Associations.txt", System.Text.Encoding.UTF8);

      foreach (string AssText in AssTexts)
      {
        Association Ass = new Association();

        Ass.BethesdaName = AssText.Split('#')[0];
        string x = AssText.Split('#')[1];

        Ass.UniqueId = Int32.Parse(x.Split(';')[0]);
        x = x.Split(';')[1];

        Ass.VkId = Int32.Parse(x.Split(',')[0].Split(':')[1]);
        Ass.TgId = Int32.Parse(x.Split(',')[1].Split(':')[1]);

        Associations.Add(Ass);
        VkToTg.Add(Ass.VkId, Ass);
        TgToVk.Add(Ass.TgId, Ass);
      }

//      vkBot.MessageCallblack = VkMessageCallblack;
      tgBot.MessageCallblack = TgMessageCallblack;
      // и похуй что это не статический метод, // <3 C#
    }
/*
    public void VkMessageCallblack(Message message)
    {
      if (message.Text.Contains("!pic"))
      {
        OpenFileDialog ofd = new OpenFileDialog();
        ofd.ShowDialog();
        Image img = Image.FromFile(ofd.FileName);
        vkBot.SendMessage(new Message()
        {
          Attachments = new List<Attachment>()
          {
            vkBot.CreateAttachment(img, message.ChatId)
          },
          ChatId = message.ChatId,
          From = message.From
        });
      }
      message.Text = message.From + " сказал:\r\n" + message.Text;
      vkBot.SendMessage(message);
      //if (msg.Attachments.First().Type == AttachmentsType.Sticker)
      //  vkBot.GetImage(msg.Attachments.First()).Save("ALLAH.PNG");
    }
*/

    public List<Tuple<long, long>> Chats;

    public void TgMessageCallblack(Message msg)
    {
      //if ()
    }

    public async void Run()
    {
      while (true)
      {
        try
        {
          // vkBot.Response();
          await tgBot.Responce();
        }
        catch (Exception e)
        {
          Console.WriteLine(e);
        }
      }
    }
  }
}