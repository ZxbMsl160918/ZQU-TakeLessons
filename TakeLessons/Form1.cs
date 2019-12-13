using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Diagnostics;

namespace TakeLessons
{
    public partial class Form1 : Form
    {
        public LessonUtils lessonUtils = null;
        // 版本号，用来判断是否需要更新
        public static string version = "0.44";
        //是否过期
        public bool isOverDue = false;
        // 加载中窗体工厂类
        readonly LoadingHelper loadingHelper = new LoadingHelper();
        public Form1()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
        }


        // 图片验证码委托
        public void SetCodeImg(Image image)
        {            
            if (notice.InvokeRequired)
            {
                Action<Image> action = new Action<Image>(SetCodeImg);
                Invoke(action, new object[] { image });
            }
            else
            {
                CodeImg.Image = image;
            }
        }


        // 滚动文字委托
        private void SetNotic(string msg)
        {
            if (notice.InvokeRequired)
            {
                Action<string> action = new Action<string>(SetNotic);
                Invoke(action, new object[] { msg });
            }
            else
            {
                notice.Text = msg;
            }
        }

        // 登陆信息文字委托
        private void SetLoginMsg(string msg)
        {
            if (loginMsg.InvokeRequired)
            {
                Action<string> action = new Action<string>(SetLoginMsg);
                Invoke(action, new object[] { msg });
            }
            else
            {
                loginMsg.Text = msg;
            }
        }

        // 验证码所在区域委托
        private void SetPanelVisible(bool visible)
        {
            if (panel1.InvokeRequired)
            {
                Action<bool> action = new Action<bool>(SetPanelVisible);
                Invoke(action, new object[] { visible });
            }
            else
            {
                panel1.Visible = visible;
            }
        }

        // 登陆信息显示委托
        private void SetLoginMsgVisible(bool visible)
        {
            if (loginMsg.InvokeRequired)
            {
                Action<bool> action = new Action<bool>(SetLoginMsgVisible);
                Invoke(action, new object[] { visible });
            }
            else
            {
                loginMsg.Visible = visible;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                lessonUtils = new LessonUtils();
                if (lessonUtils.json["isopen"] == "0")
                {
                    loadingHelper.SetMessage("软件暂未开放，3S后自动退出. . .");
                    Thread.Sleep(3 * 1000);
                    Environment.Exit(0);
                    return;
                }
                // 需要更新
                if (double.Parse(lessonUtils.json["version"]) > double.Parse(version))
                {
                    FrmUpdate frmUpdate = new FrmUpdate(lessonUtils.json["version"]);
                    frmUpdate.ShowDialog();
                    Environment.Exit(0);
                    return;
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message, "连接异常", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                //MessageBox.Show(err.Message);
                Environment.Exit(0);
                return;
            }
            loadingHelper.ShowLoading("正在获取验证码 . . .", this, o =>
            {
                SetNotic(lessonUtils.json["notice"]);
                int codeState;
                loadingHelper.SetMessage("检测是否需要验证码 . . .");
                codeState = lessonUtils.isNeedCode();
                if (codeState == -1)
                {
                    MessageBox.Show("获取验证码状态失败，请将dns设为114.114.114.114（只能内网使用）。", "连接异常", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                    Environment.Exit(0);
                    return;
                }
                // 需要验证码
                else if (codeState == 1)
                {
                    SetPanelVisible(true);
                    Image image = lessonUtils.getCodeImage();
                    if (image == null)
                    {
                        MessageBox.Show("验证码获取失败", "连接异常", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                        Environment.Exit(0);
                        return;
                    }
                    // 调用委托设置窗体验证码
                    SetCodeImg(image);
                }
                else
                    SetPanelVisible(false);
            });
            timer1.Start();
        }

        //点击图片刷新验证码
        private void CodeImg_Click(object sender, EventArgs e)
        {
            Image image;
            loadingHelper.ShowLoading("正在刷新 . . .", this, o =>
            {
                image = lessonUtils.getCodeImage();
                if (image == null)
                {
                    MessageBox.Show("验证码获取失败", "连接异常", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                     return;
                }
                SetCodeImg(image);
            });
        }

        private void Login_Click(object sender, EventArgs e)
        {
            GC.Collect();
            if (ID.Text == "")
            {
                MessageBox.Show("请输入学号！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ID.Focus();
                return;
            }
            if (Pwd.Text == "")
            {
                MessageBox.Show("请输入密码！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Pwd.Focus();
                return;
            }
            if (VerifyCode.Text == "" && panel1.Visible)
            {
                MessageBox.Show("请输入验证码！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                VerifyCode.Focus();
                return;
            }
            // 登陆失败结果
            dynamic json = null;
            // 课程列表
            List<string[]> list = null;
            loadingHelper.ShowLoading("正在登陆 . . .", this, o =>
            {
                //这里写处理耗时的代码，代码处理完成则自动关闭该窗口
                try
                {
                    //登陆
                    json = lessonUtils.executeLogin(ID.Text, Pwd.Text, VerifyCode.Text);
                }    
                catch(Exception err)
                {
                    MessageBox.Show(err.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                    return;
                }
                // 登陆成功
                if (json["status"] == "y")
                {
                    loadingHelper.SetMessage("登陆成功，正在获取课程信息 . . .");
                    list = lessonUtils.getCourseList();
                    if (list == null)
                    {
                        SetLoginMsg("课程信息获取失败！");
                        Image image;
                        loadingHelper.SetMessage(json["msg"] + "，正在刷新验证码");
                        SetPanelVisible(true);
                        image = lessonUtils.getCodeImage();
                        if (image == null)
                        {
                            MessageBox.Show("验证码获取失败", "连接异常", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                            return;
                        }
                        SetCodeImg(image);
                        return;
                    }
                }
                // 登陆失败
                else
                {
                    Image image;
                    SetLoginMsgVisible(true);
                    SetLoginMsg(json["msg"]);
                    //是否需要验证码
                    int codeState = lessonUtils.isNeedCode();
                    if (codeState == -1)
                    {
                        MessageBox.Show("获取验证码状态失败，请尝试刷新。", "连接异常", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                    }
                    else if (codeState == 1)
                    {
                        loadingHelper.SetMessage(json["msg"] + "，正在刷新验证码");
                        SetPanelVisible(true);
                        image = lessonUtils.getCodeImage();
                        if (image == null)
                        {
                            MessageBox.Show("验证码获取失败", "连接异常", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                            return;
                        }
                        SetCodeImg(image);
                    }
                    else
                        SetPanelVisible(false);
                }
            });
            if (json["status"] == "y")
            {
                //获取课程信息并创建主窗体
                main form2 = new main(this, list);
                //账号过期
                if (isOverDue == false)
                {
                    form2.Show();
                    Hide();
                    timer1.Stop();
                }
                else
                    Environment.Exit(0);
            }
        }

        //定时滚动公告
        private void timer1_Tick(object sender, EventArgs e)
        {
            notice.Left--;
            if (notice.Right <= 0)
                notice.Left = this.Width;
        }

        private void Form1_Shown(object sender, EventArgs e)
        {

        }
    }
}
