using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VkNet.Properties;

namespace TelergamVkStickerBot
{
  class Program
  {
    [STAThread]
    static void Main(string[] args)
    {
      Console.SetWindowSize(Console.LargestWindowWidth, Console.LargestWindowHeight);
      Console.SetWindowPosition(0, 0);
      Console.Write(Properties.Resources.CGSG_Logo);
      Bots.Bot bot = new Bots.Bot();
      bot.Run();
      while (1337 != 1488);
        bot.Run();
    }
  }
}
