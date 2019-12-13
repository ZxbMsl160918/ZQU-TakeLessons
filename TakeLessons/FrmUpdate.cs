using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace TakeLessons
{
    public partial class FrmUpdate : Form
    {
        string version;
        Update update;
        public FrmUpdate(string v)
        {
            version = v;
            InitializeComponent();
            update = new Update() { DownloadFileCompleted = DownloadFileCompleted, DownloadProgressChanged = DownloadProgressChanged };
            try
            {
                update.Download("http://zxb.zxbmsl.top/zxb/ZQU抢课" + version + ".exe", "ZQU抢课" + version + ".exe");
            }
            catch
            {
                //MessageBox.Show(Properties.Resources.DownLoadFail, Properties.Resources.NewVersion, MessageBoxButtons.OK);
                Environment.Exit(0);
            }
        }

        private void FrmUpdate_Load(object sender, EventArgs e)
        {
            
        }

        /// <summary>
        /// 更新下载进度条
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            progressBar1.Minimum = 0;
            progressBar1.Maximum = (int)e.TotalBytesToReceive;
            progressBar1.Value = (int)e.BytesReceived;
            label1.Text = e.ProgressPercentage + "%";
        }

        /// <summary>
        /// 更新包下载完成，开始更新操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message + "请手动在浏览器输入：http://zxb.zxbmsl.top/zxb/ZQU抢课" + version + ".exe", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Environment.Exit(0);
                return;
            }
            else if ((e.Cancelled == true))
            {
                MessageBox.Show("下载文件操作被取消！", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Environment.Exit(0);
                return;
            }
            else
            {
                MessageBox.Show("下载文件操作完成，即将打开新版本软件", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                try
                {
                    // 获取程序运行路径
                    var appPath = Directory.GetCurrentDirectory();
                    Process p = new Process();
                    p.StartInfo.FileName = $@"{appPath}\ZQU抢课{version}.exe";
                    p.Start();
                    // 调试的时候因为DeleteOwn.vshost.exe文件一直被vs占用所以删除会失效，直接执行就可以了
                    using (FileStream fs = new FileStream("del.bat", FileMode.Create))
                    {
                        using (StreamWriter sw = new StreamWriter(fs, Encoding.ASCII))
                        {
                            sw.WriteLine("@echo off");
                            sw.WriteLine(string.Concat("del", " \"", AppDomain.CurrentDomain.FriendlyName, "\""));//删除主程序，引号是保证文件名中包含空格也可以删除，FriendlyName修改名称后可以删除
                            sw.WriteLine("del %0");//删除自己
                        }
                    }
                    Process proc = new Process();
                    proc.StartInfo.FileName = "del.bat";
                    proc.StartInfo.UseShellExecute = false;//不显示命令行
                    proc.StartInfo.CreateNoWindow = false;//不在窗体展示
                    proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    proc.Start();
                }
                catch (Exception err)
                {
                    MessageBox.Show(err.Message);
                }
                Dispose();
            }
        }

        private void FrmUpdate_FormClosed(object sender, FormClosedEventArgs e)
        {

        }
    }
}
