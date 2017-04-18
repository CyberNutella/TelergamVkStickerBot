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
    VkBot vkBot = new VkBot();
    TelergammBot tgBot = new TelergammBot();

    public class Association
    {
      public string BethesdaName;
      public long UniqueId;
      public long VkId;
      public long TgId;
    }

    List<Association> Associations = new List<Association>(); // List for slow search, when checking group name while creating unique id, 
    List<Association> HalfLife = new List<Association>();     // List for storing half-live(one-side) associations, which are in state of pairing
    Hashtable VkToTg = new Hashtable(); // Hashtables for adamantine-cyber-extra-supreme-mega-gold-ultra-fast Id translations
    Hashtable TgToVk = new Hashtable();
    Random IdGen = new Random(); // Random instance

    public Bot()
    {
      /* Associations file example */
/*
test#1337; Vk:02, Tg:39
test#1477; Vk:59, Tg:30
cake#1477; Vk:11, Tg:47
 */
      Directory.CreateDirectory("Data");
      FileStream a = new FileStream("Data/Associations.txt", FileMode.OpenOrCreate);
      a.Close();

      string []AssTexts = File.ReadAllLines("Data/Associations.txt", System.Text.Encoding.UTF8);

      foreach (string AssText in AssTexts)
      {
        if (AssText == "")
          continue;
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

      vkBot.MessageCallblack = VkMessageCallblack;
      tgBot.MessageCallblack = TgMessageCallblack;
      // <3 C#
    }

    public void VkMessageCallblack(Message message)
    {
      if (message.Text.StartsWith("/start ") && message.Text.Contains('#'))
      {
        string GroupName = message.Text.Substring(message.Text.LastIndexOf("/start ") + "/start ".Length).Split('#')[0];
        long UniqueId = Int32.Parse(message.Text.Substring(message.Text.LastIndexOf("/start ") + "/start ".Length).Split('#')[1]);

        Message M = new Message();
        M.ChatId = message.ChatId;
        M.From = "";

        foreach (Association Ass in HalfLife)
          if (Ass.BethesdaName == GroupName && Ass.UniqueId == UniqueId)
          {
            HalfLife.Remove(Ass);
            if (Ass.VkId != -1)
            {
              M.Text = "Can't make association of two Vk groups";
              vkBot.SendMessage(M);
            }
            Ass.VkId = message.ChatId;
            Associations.Add(Ass);
            string Lines = File.ReadAllText("Data/Associations.txt", System.Text.Encoding.UTF8);
            Lines += "\n" + Ass.BethesdaName + "#" + Ass.UniqueId + "; Vk:" + Ass.VkId + ", Tg:" + Ass.TgId;
            File.WriteAllText("Data/Associations.txt", Lines, System.Text.Encoding.UTF8);

            VkToTg[Ass.VkId] = Ass;
            TgToVk[Ass.TgId] = Ass;

            M.Text = "Successfully paired";
            vkBot.SendMessage(M);
            M.ChatId = Ass.TgId;
            tgBot.SendMessage(M);
            return;
          }

        M.Text = "Pair not found";
        vkBot.SendMessage(M);
      }

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
      if (VkToTg.ContainsKey(message.ChatId))
      {
        message.ChatId = ((Association)VkToTg[message.ChatId]).TgId;
        tgBot.SendMessage(message);
      }
      //if (msg.Attachments.First().Type == AttachmentsType.Sticker)
      //  vkBot.GetImage(msg.Attachments.First()).Save("ALLAH.PNG");
    }

    public void TgMessageCallblack(Message msg)
    {
      if (msg.Text.StartsWith("/start ") && !msg.Text.Contains('#'))
      {
        Message M = new Message();
        M.ChatId = msg.ChatId;
        M.From = "";

        Association NewAss = new Association();
        NewAss.BethesdaName = msg.Text.Substring(msg.Text.LastIndexOf("/start ") + "/start ".Length);
        if (NewAss.BethesdaName.Trim() == "")
        {
          M.Text = "Associaion name shouldn't be empty";
          tgBot.SendMessage(M);
          return;
        }

        NewAss.TgId = msg.ChatId;
        NewAss.VkId = -1;

        bool found = false;
        long ddos_defence = 0;
        while (!found && ddos_defence < 5000)
        {
          NewAss.UniqueId = IdGen.Next(0, 10000);

          bool sovpadenie = false;
          foreach (Association Ass in Associations)
            if (Ass.BethesdaName == NewAss.BethesdaName && Ass.UniqueId == NewAss.UniqueId)
              sovpadenie = true;

          if (!sovpadenie)
            found = true;
          ddos_defence++;
        }

        if (found)
          HalfLife.Add(NewAss);

        if (found)
          M.Text = "Creating group association with name: " + NewAss.BethesdaName + "#" + NewAss.UniqueId;
        else
          M.Text = "Failed. Try another name.";

        tgBot.SendMessage(M);
        return;
      }

      if (msg.Text.StartsWith("/start ") && msg.Text.Contains('#'))
      {
        
      }

      if (TgToVk.ContainsKey(msg.ChatId))
      {
        msg.ChatId = ((Association)TgToVk[msg.ChatId]).VkId;
        msg.Text = msg.From + "\n" + msg.Text;
        vkBot.SendMessage(msg);
      }
    }

    public void Run()
    {
      while (true)
      {
        try
        {
          vkBot.Response();
          tgBot.Responce();
        }
        catch (Exception e)
        {
          Console.WriteLine(e);
        }
      }
    }
  }
}