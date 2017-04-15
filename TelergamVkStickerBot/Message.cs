using System;
using System.Collections.Generic;
using System.Windows.Forms;
using VkNet.Model.Attachments;

namespace TelergamVkStickerBot
{
  public enum AttachmentsType
  {
    Sticker,
    Image,
    Video,
    Audio,
    Link,
    File
  }

  public struct Attachment
  {
    public AttachmentsType Type;
    public string FileName;
    public string RepresentationTg;
    public MediaAttachment RepresentationVk;
  }

  public class Message
  {
    public string From;
    public long ChatId;
    public string Text;
    public List<Attachment> Attachments = new List<Attachment>();
  }
}