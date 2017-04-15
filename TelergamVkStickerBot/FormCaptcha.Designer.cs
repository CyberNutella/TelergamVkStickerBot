namespace TelergamVkStickerBot
{
  partial class FormCaptcha
  {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.picCaptcha = new System.Windows.Forms.PictureBox();
      this.groupBox = new System.Windows.Forms.GroupBox();
      this.txtCaptcha = new System.Windows.Forms.TextBox();
      this.btnSubmit = new System.Windows.Forms.Button();
      ((System.ComponentModel.ISupportInitialize)(this.picCaptcha)).BeginInit();
      this.groupBox.SuspendLayout();
      this.SuspendLayout();
      // 
      // picCaptcha
      // 
      this.picCaptcha.Dock = System.Windows.Forms.DockStyle.Top;
      this.picCaptcha.Image = global::TelergamVkStickerBot.Properties.Resources.cgsg;
      this.picCaptcha.Location = new System.Drawing.Point(0, 0);
      this.picCaptcha.Name = "picCaptcha";
      this.picCaptcha.Size = new System.Drawing.Size(284, 150);
      this.picCaptcha.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
      this.picCaptcha.TabIndex = 0;
      this.picCaptcha.TabStop = false;
      // 
      // groupBox
      // 
      this.groupBox.Controls.Add(this.btnSubmit);
      this.groupBox.Controls.Add(this.txtCaptcha);
      this.groupBox.Dock = System.Windows.Forms.DockStyle.Bottom;
      this.groupBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.groupBox.Location = new System.Drawing.Point(0, 153);
      this.groupBox.Margin = new System.Windows.Forms.Padding(0);
      this.groupBox.Name = "groupBox";
      this.groupBox.Padding = new System.Windows.Forms.Padding(0);
      this.groupBox.Size = new System.Drawing.Size(284, 108);
      this.groupBox.TabIndex = 1;
      this.groupBox.TabStop = false;
      // 
      // txtCaptcha
      // 
      this.txtCaptcha.Dock = System.Windows.Forms.DockStyle.Top;
      this.txtCaptcha.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
      this.txtCaptcha.Location = new System.Drawing.Point(0, 13);
      this.txtCaptcha.Name = "txtCaptcha";
      this.txtCaptcha.Size = new System.Drawing.Size(284, 30);
      this.txtCaptcha.TabIndex = 0;
      // 
      // btnSubmit
      // 
      this.btnSubmit.Dock = System.Windows.Forms.DockStyle.Bottom;
      this.btnSubmit.Location = new System.Drawing.Point(0, 49);
      this.btnSubmit.Name = "btnSubmit";
      this.btnSubmit.Size = new System.Drawing.Size(284, 59);
      this.btnSubmit.TabIndex = 1;
      this.btnSubmit.Text = "Submit";
      this.btnSubmit.UseVisualStyleBackColor = true;
      this.btnSubmit.Click += new System.EventHandler(this.btnSubmit_Click);
      // 
      // FormCaptcha
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(284, 261);
      this.Controls.Add(this.groupBox);
      this.Controls.Add(this.picCaptcha);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "FormCaptcha";
      this.Text = "Captcha";
      ((System.ComponentModel.ISupportInitialize)(this.picCaptcha)).EndInit();
      this.groupBox.ResumeLayout(false);
      this.groupBox.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.PictureBox picCaptcha;
    private System.Windows.Forms.GroupBox groupBox;
    private System.Windows.Forms.Button btnSubmit;
    private System.Windows.Forms.TextBox txtCaptcha;
  }
}