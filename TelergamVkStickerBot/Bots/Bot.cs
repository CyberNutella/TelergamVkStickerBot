using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace TelergamVkStickerBot.Bots
{
  public class Bot
  {
    VkBot vkBot = new VkBot();
    TelergammBot tgBot = new TelergammBot();

    public Bot()
    {
      vkBot.MessageCallblack = VkMessageCallblack;
      // и похуй что это не статический метод, // <3 C#
    }

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

    public List<Tuple<long, long>> Chats;

    public void TgMessageCallblack(Message msg)
    {
      //if ()
    }

    public void Run()
    {
      while (true)
      {
        try
        {
          vkBot.Response();
        }
        catch (Exception e)
        {
          Console.WriteLine(e);
        }
      }
    }
  }
}