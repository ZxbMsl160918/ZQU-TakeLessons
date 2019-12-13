﻿namespace TakeLessons
{
    partial class FrmLoading
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
            this.PnlImage = new System.Windows.Forms.Panel();
            this.LblMessage = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // PnlImage
            // 
            this.PnlImage.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.PnlImage.BackColor = System.Drawing.Color.Transparent;
            this.PnlImage.Location = new System.Drawing.Point(100, 17);
            this.PnlImage.Name = "PnlImage";
            this.PnlImage.Size = new System.Drawing.Size(200, 200);
            this.PnlImage.TabIndex = 1;
            this.PnlImage.Paint += new System.Windows.Forms.PaintEventHandler(this.PnlImage_Paint);
            this.PnlImage.Resize += new System.EventHandler(this.PnlImage_Resize);
            // 
            // LblMessage
            // 
            this.LblMessage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.LblMessage.BackColor = System.Drawing.Color.Transparent;
            this.LblMessage.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold);
            this.LblMessage.ForeColor = System.Drawing.Color.White;
            this.LblMessage.Location = new System.Drawing.Point(36, 224);
            this.LblMessage.Name = "LblMessage";
            this.LblMessage.Size = new System.Drawing.Size(328, 64);
            this.LblMessage.TabIndex = 0;
            this.LblMessage.Text = "正在处理中，请稍候……";
            this.LblMessage.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.LblMessage.Click += new System.EventHandler(this.LblMessage_Click);
            // 
            // FrmLoading
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(400, 300);
            this.Controls.Add(this.LblMessage);
            this.Controls.Add(this.PnlImage);
            this.Font = new System.Drawing.Font("微软雅黑", 12F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(5);
            this.Name = "FrmLoading";
            this.Opacity = 0.5D;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "FrmLoading";
            this.Load += new System.EventHandler(this.FrmLoading_Load);
            this.Shown += new System.EventHandler(this.FrmLoading_Shown);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel PnlImage;
        public System.Windows.Forms.Label LblMessage;
    }
}