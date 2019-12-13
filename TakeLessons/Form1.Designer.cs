namespace TakeLessons
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.ID = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.Pwd = new System.Windows.Forms.TextBox();
            this.Login = new System.Windows.Forms.Button();
            this.VerifyCode = new System.Windows.Forms.TextBox();
            this.CodeImg = new System.Windows.Forms.PictureBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.loginMsg = new System.Windows.Forms.Label();
            this.notice = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.CodeImg)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // ID
            // 
            this.ID.Location = new System.Drawing.Point(127, 36);
            this.ID.Name = "ID";
            this.ID.Size = new System.Drawing.Size(154, 21);
            this.ID.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(80, 40);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "学号：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(80, 80);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(44, 12);
            this.label2.TabIndex = 3;
            this.label2.Text = "密码：";
            // 
            // Pwd
            // 
            this.Pwd.Location = new System.Drawing.Point(127, 76);
            this.Pwd.Name = "Pwd";
            this.Pwd.PasswordChar = '*';
            this.Pwd.Size = new System.Drawing.Size(154, 21);
            this.Pwd.TabIndex = 1;
            // 
            // Login
            // 
            this.Login.BackColor = System.Drawing.Color.Transparent;
            this.Login.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Login.FlatAppearance.BorderColor = System.Drawing.Color.Azure;
            this.Login.FlatAppearance.BorderSize = 0;
            this.Login.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.Login.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.Login.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Login.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Login.ForeColor = System.Drawing.Color.Transparent;
            this.Login.Image = ((System.Drawing.Image)(resources.GetObject("Login.Image")));
            this.Login.Location = new System.Drawing.Point(143, 151);
            this.Login.Name = "Login";
            this.Login.Size = new System.Drawing.Size(79, 28);
            this.Login.TabIndex = 3;
            this.Login.TabStop = false;
            this.Login.Text = "登录";
            this.Login.UseVisualStyleBackColor = false;
            this.Login.Click += new System.EventHandler(this.Login_Click);
            // 
            // VerifyCode
            // 
            this.VerifyCode.Location = new System.Drawing.Point(120, 9);
            this.VerifyCode.Name = "VerifyCode";
            this.VerifyCode.Size = new System.Drawing.Size(100, 21);
            this.VerifyCode.TabIndex = 2;
            // 
            // CodeImg
            // 
            this.CodeImg.BackColor = System.Drawing.Color.Transparent;
            this.CodeImg.Cursor = System.Windows.Forms.Cursors.Hand;
            this.CodeImg.Image = ((System.Drawing.Image)(resources.GetObject("CodeImg.Image")));
            this.CodeImg.InitialImage = ((System.Drawing.Image)(resources.GetObject("CodeImg.InitialImage")));
            this.CodeImg.Location = new System.Drawing.Point(225, 0);
            this.CodeImg.Name = "CodeImg";
            this.CodeImg.Size = new System.Drawing.Size(124, 34);
            this.CodeImg.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.CodeImg.TabIndex = 6;
            this.CodeImg.TabStop = false;
            this.CodeImg.Click += new System.EventHandler(this.CodeImg_Click);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Transparent;
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.VerifyCode);
            this.panel1.Controls.Add(this.CodeImg);
            this.panel1.Location = new System.Drawing.Point(7, 106);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(352, 37);
            this.panel1.TabIndex = 7;
            this.panel1.Visible = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(60, 13);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(57, 12);
            this.label3.TabIndex = 7;
            this.label3.Text = "验证码：";
            // 
            // loginMsg
            // 
            this.loginMsg.AutoSize = true;
            this.loginMsg.BackColor = System.Drawing.Color.Transparent;
            this.loginMsg.Font = new System.Drawing.Font("新宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.loginMsg.ForeColor = System.Drawing.Color.Red;
            this.loginMsg.Location = new System.Drawing.Point(137, 189);
            this.loginMsg.Name = "loginMsg";
            this.loginMsg.Size = new System.Drawing.Size(57, 12);
            this.loginMsg.TabIndex = 8;
            this.loginMsg.Text = "登陆信息";
            this.loginMsg.Visible = false;
            // 
            // notice
            // 
            this.notice.AutoSize = true;
            this.notice.BackColor = System.Drawing.Color.Transparent;
            this.notice.Font = new System.Drawing.Font("黑体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.notice.ForeColor = System.Drawing.Color.DodgerBlue;
            this.notice.Location = new System.Drawing.Point(333, 4);
            this.notice.Name = "notice";
            this.notice.Size = new System.Drawing.Size(0, 12);
            this.notice.TabIndex = 9;
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 30;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // Form1
            // 
            this.AcceptButton = this.Login;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.ClientSize = new System.Drawing.Size(360, 210);
            this.Controls.Add(this.notice);
            this.Controls.Add(this.loginMsg);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.Login);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.Pwd);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ID);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "登录";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Shown += new System.EventHandler(this.Form1_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.CodeImg)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.TextBox ID;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox Pwd;
        private System.Windows.Forms.Button Login;
        private System.Windows.Forms.TextBox VerifyCode;
        private System.Windows.Forms.PictureBox CodeImg;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label loginMsg;
        private System.Windows.Forms.Label notice;
        private System.Windows.Forms.Timer timer1;
    }
}

