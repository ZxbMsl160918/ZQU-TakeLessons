namespace TakeLessons
{
    partial class main
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(main));
            this.ID = new System.Windows.Forms.Label();
            this.Execute = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.StartTime = new System.Windows.Forms.Label();
            this.LeftTime = new System.Windows.Forms.Label();
            this.Total = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.multiThread = new System.Windows.Forms.CheckBox();
            this.stopTakeLesson = new System.Windows.Forms.Button();
            this.Msg = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.listCourseSort = new System.Windows.Forms.ListBox();
            this.listSort = new System.Windows.Forms.ListView();
            this.listView1 = new System.Windows.Forms.ListView();
            this.select = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.kcmc = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tea = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.time = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // ID
            // 
            this.ID.AutoSize = true;
            this.ID.Location = new System.Drawing.Point(6, 20);
            this.ID.Name = "ID";
            this.ID.Size = new System.Drawing.Size(41, 12);
            this.ID.TabIndex = 0;
            this.ID.Text = "学号：";
            // 
            // Execute
            // 
            this.Execute.BackColor = System.Drawing.Color.Transparent;
            this.Execute.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Execute.FlatAppearance.BorderSize = 0;
            this.Execute.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.Execute.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.Execute.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Execute.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Execute.ForeColor = System.Drawing.Color.White;
            this.Execute.Image = ((System.Drawing.Image)(resources.GetObject("Execute.Image")));
            this.Execute.Location = new System.Drawing.Point(58, 40);
            this.Execute.Name = "Execute";
            this.Execute.Size = new System.Drawing.Size(97, 33);
            this.Execute.TabIndex = 0;
            this.Execute.Text = "一键抢课";
            this.toolTip1.SetToolTip(this.Execute, "选中课程后点击抢课");
            this.Execute.UseVisualStyleBackColor = false;
            this.Execute.Click += new System.EventHandler(this.Execute_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.White;
            this.groupBox1.Controls.Add(this.dateTimePicker1);
            this.groupBox1.Controls.Add(this.StartTime);
            this.groupBox1.Controls.Add(this.LeftTime);
            this.groupBox1.Controls.Add(this.Total);
            this.groupBox1.Controls.Add(this.ID);
            this.groupBox1.Location = new System.Drawing.Point(464, 5);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(262, 119);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "信息";
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.CalendarForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.dateTimePicker1.CustomFormat = "yyyy-MM-dd HH:mm";
            this.dateTimePicker1.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePicker1.Location = new System.Drawing.Point(92, 43);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.Size = new System.Drawing.Size(136, 21);
            this.dateTimePicker1.TabIndex = 0;
            this.toolTip1.SetToolTip(this.dateTimePicker1, "选择抢课时间");
            // 
            // StartTime
            // 
            this.StartTime.AutoSize = true;
            this.StartTime.Location = new System.Drawing.Point(6, 46);
            this.StartTime.Name = "StartTime";
            this.StartTime.Size = new System.Drawing.Size(89, 12);
            this.StartTime.TabIndex = 3;
            this.StartTime.Text = "编辑抢课时间：";
            // 
            // LeftTime
            // 
            this.LeftTime.AutoSize = true;
            this.LeftTime.Location = new System.Drawing.Point(6, 98);
            this.LeftTime.Name = "LeftTime";
            this.LeftTime.Size = new System.Drawing.Size(65, 12);
            this.LeftTime.TabIndex = 2;
            this.LeftTime.Text = "剩余时间：";
            this.LeftTime.Visible = false;
            // 
            // Total
            // 
            this.Total.AutoSize = true;
            this.Total.Location = new System.Drawing.Point(6, 72);
            this.Total.Name = "Total";
            this.Total.Size = new System.Drawing.Size(65, 12);
            this.Total.TabIndex = 1;
            this.Total.Text = "课程总数：";
            // 
            // groupBox2
            // 
            this.groupBox2.BackColor = System.Drawing.Color.White;
            this.groupBox2.Controls.Add(this.multiThread);
            this.groupBox2.Controls.Add(this.Execute);
            this.groupBox2.Controls.Add(this.stopTakeLesson);
            this.groupBox2.Location = new System.Drawing.Point(6, 5);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(215, 119);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "操作";
            // 
            // multiThread
            // 
            this.multiThread.AutoSize = true;
            this.multiThread.Checked = true;
            this.multiThread.CheckState = System.Windows.Forms.CheckState.Checked;
            this.multiThread.Cursor = System.Windows.Forms.Cursors.Hand;
            this.multiThread.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.multiThread.Location = new System.Drawing.Point(30, 95);
            this.multiThread.Name = "multiThread";
            this.multiThread.Size = new System.Drawing.Size(155, 16);
            this.multiThread.TabIndex = 1;
            this.multiThread.Text = "多线程+ 捡课（建议）";
            this.multiThread.UseVisualStyleBackColor = true;
            // 
            // stopTakeLesson
            // 
            this.stopTakeLesson.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.stopTakeLesson.Cursor = System.Windows.Forms.Cursors.Hand;
            this.stopTakeLesson.FlatAppearance.BorderSize = 0;
            this.stopTakeLesson.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.stopTakeLesson.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Red;
            this.stopTakeLesson.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.stopTakeLesson.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.stopTakeLesson.ForeColor = System.Drawing.Color.Transparent;
            this.stopTakeLesson.Location = new System.Drawing.Point(59, 43);
            this.stopTakeLesson.Name = "stopTakeLesson";
            this.stopTakeLesson.Size = new System.Drawing.Size(97, 33);
            this.stopTakeLesson.TabIndex = 2;
            this.stopTakeLesson.Text = "停止抢课";
            this.stopTakeLesson.UseVisualStyleBackColor = false;
            this.stopTakeLesson.Visible = false;
            this.stopTakeLesson.Click += new System.EventHandler(this.StopTakeLesson_Click);
            // 
            // Msg
            // 
            this.Msg.BackColor = System.Drawing.Color.Black;
            this.Msg.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.Msg.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.Msg.ForeColor = System.Drawing.Color.Green;
            this.Msg.Location = new System.Drawing.Point(227, 16);
            this.Msg.Multiline = true;
            this.Msg.Name = "Msg";
            this.Msg.ReadOnly = true;
            this.Msg.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.Msg.Size = new System.Drawing.Size(232, 106);
            this.Msg.TabIndex = 5;
            this.Msg.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(227, 2);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 6;
            this.label1.Text = "实时输出";
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 300000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.button2);
            this.groupBox3.Controls.Add(this.button1);
            this.groupBox3.Controls.Add(this.listCourseSort);
            this.groupBox3.Controls.Add(this.listSort);
            this.groupBox3.Location = new System.Drawing.Point(540, 130);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(186, 241);
            this.groupBox3.TabIndex = 3;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "抢课顺序";
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.Color.Transparent;
            this.button2.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button2.FlatAppearance.BorderSize = 0;
            this.button2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button2.Image = ((System.Drawing.Image)(resources.GetObject("button2.Image")));
            this.button2.Location = new System.Drawing.Point(128, 207);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(38, 23);
            this.button2.TabIndex = 1;
            this.toolTip1.SetToolTip(this.button2, "下移");
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.Transparent;
            this.button1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button1.FlatAppearance.BorderSize = 0;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Image = ((System.Drawing.Image)(resources.GetObject("button1.Image")));
            this.button1.Location = new System.Drawing.Point(19, 207);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(38, 23);
            this.button1.TabIndex = 0;
            this.toolTip1.SetToolTip(this.button1, "上移");
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // listCourseSort
            // 
            this.listCourseSort.AllowDrop = true;
            this.listCourseSort.FormattingEnabled = true;
            this.listCourseSort.ItemHeight = 12;
            this.listCourseSort.Location = new System.Drawing.Point(6, 20);
            this.listCourseSort.Name = "listCourseSort";
            this.listCourseSort.Size = new System.Drawing.Size(172, 184);
            this.listCourseSort.TabIndex = 2;
            // 
            // listSort
            // 
            this.listSort.HideSelection = false;
            this.listSort.Location = new System.Drawing.Point(6, 20);
            this.listSort.Name = "listSort";
            this.listSort.Size = new System.Drawing.Size(174, 215);
            this.listSort.TabIndex = 0;
            this.listSort.UseCompatibleStateImageBehavior = false;
            // 
            // listView1
            // 
            this.listView1.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.listView1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("listView1.BackgroundImage")));
            this.listView1.BackgroundImageTiled = true;
            this.listView1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.listView1.CheckBoxes = true;
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.select,
            this.kcmc,
            this.tea,
            this.time});
            this.listView1.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.listView1.ForeColor = System.Drawing.Color.White;
            this.listView1.GridLines = true;
            this.listView1.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listView1.HideSelection = false;
            this.listView1.ImeMode = System.Windows.Forms.ImeMode.On;
            this.listView1.Location = new System.Drawing.Point(0, 126);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(534, 245);
            this.listView1.TabIndex = 2;
            this.listView1.TabStop = false;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            this.listView1.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.listView1_ItemCheck);
            // 
            // select
            // 
            this.select.Text = "";
            this.select.Width = 20;
            // 
            // kcmc
            // 
            this.kcmc.Text = "课程名称";
            this.kcmc.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.kcmc.Width = 156;
            // 
            // tea
            // 
            this.tea.Text = "教师姓名";
            this.tea.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tea.Width = 131;
            // 
            // time
            // 
            this.time.Text = "上课时间";
            this.time.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.time.Width = 208;
            // 
            // main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.GhostWhite;
            this.ClientSize = new System.Drawing.Size(730, 372);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.Msg);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.listView1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "main";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "zqu抢课";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.main_FormClosing);
            this.Shown += new System.EventHandler(this.Main_Shown);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label ID;
        private System.Windows.Forms.Button Execute;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader kcmc;
        private System.Windows.Forms.ColumnHeader tea;
        private System.Windows.Forms.ColumnHeader time;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label Total;
        private System.Windows.Forms.Label LeftTime;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox multiThread;
        private System.Windows.Forms.Label StartTime;
        private System.Windows.Forms.TextBox Msg;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ColumnHeader select;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.DateTimePicker dateTimePicker1;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.ListView listSort;
        private System.Windows.Forms.ListBox listCourseSort;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button stopTakeLesson;
    }
}