using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VkNet.Utils.AntiCaptcha;

namespace TelergamVkStickerBot
{
  public partial class FormCaptcha : Form, ICaptchaSolver
  {
    public FormCaptcha()
    {
      InitializeComponent();
    }

    public void CaptchaIsFalse()
    {
      /// НИХУЯ НЕ ПОНЯТНЫЙ ИНТЕРФЕЙС
      /// НАХУЯ ЭТА ФУНКЦИЯ ?!
    }

    public string Solve(string url)
    {
      WebRequest request = WebRequest.Create(url);
      WebResponse response = request.GetResponse();
      Stream img = response.GetResponseStream();
      if (img == null)
        throw new ZeigHeil();
      picCaptcha.Image = Image.FromStream(img);
      ShowDialog();
      return txtCaptcha.Text;
    }

    private void btnSubmit_Click(object sender, EventArgs e)
    {
      Close();
    }
  }
}
