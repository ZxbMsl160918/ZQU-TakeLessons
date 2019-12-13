using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Web;
using System.Net;
using System.Diagnostics;
using System.Data.SqlClient;

namespace TakeLessons
{
    public partial class main : Form
    {
        // 打印信息线程，防止界面假死
        Thread t = null;
        // 是否开始抢课
        bool isStart = false;
        readonly Form1 form1;
        //需要提交的数据
        List<string> postList;
        //课程的列表
        private readonly List<string[]> coursesList;
        //注册的是多少天的注册码
        int day;
        //抢课开始时间
        DateTime Start_T;
        public main(Form1 form1, List<string[]> list)
        {
            CheckForIllegalCrossThreadCalls = false;
            InitializeComponent();
            this.form1 = form1;
            //获取课程信息的list
            coursesList = list;
            foreach (var course in coursesList)
            {
                ListViewItem lvi = new ListViewItem(course[3]);
                lvi.SubItems.Add(course[0]);
                lvi.SubItems.Add(course[1]);
                lvi.SubItems.Add(course[2]);
                listView1.Items.Add(lvi);
            }
            ID.Text += form1.ID.Text;
            //获取开始抢课时间
            DateTime dateTime = Convert.ToDateTime(form1.lessonUtils.json["startTime"]);
            StartTime.Text += dateTime.ToString("MM/dd HH:mm");
            Total.Text += coursesList.Count.ToString() + " 门";
            if (form1.lessonUtils.json["free"] == "0")
            {
                //获取到期时间
                dateTime = Convert.ToDateTime(GetLeftTime()).AddDays(day);
                if (DateTime.Now >= dateTime)
                {
                    MessageBox.Show("您的使用期限已到，请联系QQ:1028911514续费！", "温馨提示");
                    Close();
                    form1.isOverDue = true;
                    return;
                }
                TimeSpan left = dateTime - DateTime.Now;
                LeftTime.Text += Convert.ToInt32(left.TotalDays) + "天";
                LeftTime.Visible = true;
            }
            Msg.Text = "          欢迎您使用本软件\r\n\r\n";
            Msg.Text += "   请先选择课程，然后调整抢课顺序再点一键抢课让软件挂机。多线程、捡课模式能持续发送需要抢的课程；抢课时间不准确也能持续等到抢课开始；如错过最佳抢课时间，如果有人退选，立马抢到！\r\n";
            ConsoleFocus();
            timer1.Start();
        }

        //执行sql查询操作
        private string executeSql(string sql)
        {
            //远程数据库
            string conStr = "Data Source=43.226.43.226;Initial Catalog=mssql1811240b5f_db;User ID=mssql1811240b5f;Password=Zxb9416173296";
            using (SqlConnection con = new SqlConnection(conStr))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@id", form1.ID.Text);
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        string temp = reader[0].ToString();
                        reader.Close();
                        return temp;
                    }
                    return "";
                }
            }
        }

        //获取账号的剩余使用时间
        private string GetLeftTime()
        {
            string time;
            time = executeSql("select time from 注册码表10 where registerID=@id");
            if (time != "")
            {
                day = 365;
                return time;
            }
            else
            {
                time = executeSql("select time from 注册码表5 where registerID=@id");
                day = 180;
                return time;
            }
        }

        //获取百度时间，不获取教务系统时间是为了避免教务系统卡死而程序无响应
        public string GetNetWorkTime()
        {
            WebRequest request = null;
            WebResponse response = null;
            WebHeaderCollection headerCollection = null;
            string datetime = string.Empty;
            try
            {
                request = WebRequest.Create("https://www.baidu.com/");
                request.Timeout = 3000;
                request.Credentials = CredentialCache.DefaultCredentials;
                response = request.GetResponse();
                headerCollection = response.Headers;
                foreach (var h in headerCollection.AllKeys)
                {
                    if (h == "Date")
                    {
                        datetime = headerCollection[h];
                    }
                }
                return datetime;
            }
            catch (Exception)
            {
                return datetime;
            }
            finally
            {
                if (request != null)
                    request.Abort();
                if (response != null)
                    response.Close();
                if (headerCollection != null)
                    headerCollection.Clear();
            }
        }

        //将黑框框调到最后
        private void ConsoleFocus()
        {
            Msg.Focus();
            Msg.Select(Msg.TextLength, 0);
            Msg.ScrollToCaret();
        }

        // 设置本机电脑的年月日
        public void SetLocalDate(int year, int month, int day)
        {
            //实例一个Process类，启动一个独立进程
            Process p = new Process();
            //Process类有一个StartInfo属性
            //设定程序名
            p.StartInfo.FileName = "cmd.exe";
            //设定程式执行参数 “/C”表示执行完命令后马上退出
            p.StartInfo.Arguments = string.Format("/c date {0}-{1}-{2}", year, month, day);
            //关闭Shell的使用
            p.StartInfo.UseShellExecute = false;
            //重定向标准输入
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = true;
            //重定向错误输出
            p.StartInfo.RedirectStandardError = true;
            //设置不显示doc窗口
            p.StartInfo.CreateNoWindow = true;
            //启动
            p.Start();
            //从输出流取得命令执行结果
            p.StandardOutput.ReadToEnd();
            p.Close();
        }
        
        // 设置本机电脑的时分秒
        public void SetLocalTime(int hour, int min, int sec)
        {
            //实例一个Process类，启动一个独立进程
            Process p = new Process();
            //Process类有一个StartInfo属性
            //设定程序名
            p.StartInfo.FileName = "cmd.exe";
            //设定程式执行参数 “/C”表示执行完命令后马上退出
            p.StartInfo.Arguments = string.Format("/c time {0}:{1}:{2}", hour, min, sec);
            //关闭Shell的使用
            p.StartInfo.UseShellExecute = false;
            //重定向标准输入
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = true;
            //重定向错误输出
            p.StartInfo.RedirectStandardError = true;
            //设置不显示doc窗口
            p.StartInfo.CreateNoWindow = true;
            //启动
            p.Start();
            //从输出流取得命令执行结果
            p.StandardOutput.ReadToEnd();
            p.Close();
        }

        //校准本地时间
        private void setTime(DateTime netTime)
        {
            SetLocalDate(netTime.Year, netTime.Month, netTime.Day);
            SetLocalTime(netTime.Hour, netTime.Minute, netTime.Second);
        }

        //是否已到抢课时间
        private void isTimeTo()
        {
            //获取当前网络时间
            DateTime now;
            //抢课开始时间
            string t = dateTimePicker1.Value.ToString("yyyy-MM-dd HH:mm");
            DateTime time = Convert.ToDateTime(t);
            Start_T = time;
            try
            {
                now = Convert.ToDateTime(GetNetWorkTime());
                //同步电脑时间
                setTime(now);
            }
            catch (Exception)
            {
                MessageBox.Show("同步网络时间出错，请手动同步本机的网络时间或者重启软件！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);                
            }
            // 实际开发不要使用死循环
            while (true)
            {
                try
                {
                    now = DateTime.Now;
                    if (time <= now)
                        break;
                    Msg.Text += "等待抢课开始 " + now.ToString("HH:mm:ss") + "\r\n";
                    ConsoleFocus();
                    Thread.Sleep(1000);

                }
                catch (Exception err)
                {
                    Debug.WriteLine(err.Message);
                }
            }
        }

        //打印实时信息
        public void printMsg()
        {
            //卡在这，当时间到达执行抢课
            isTimeTo();
            Msg.Text += "\r\n          ๑乛v乛๑嘿嘿 抢课开始！！！\r\n\r\n";
            isStart = true;
            ConsoleFocus();
            string temp;
            var i = 1;
            // 不是多线程
            if (multiThread.Checked == false)
            {
                //循环发送数据一个个发
                foreach (var data in postList)
                {                    
                    Msg.Text += "正在抢第" + i.ToString() + "门课...\r\n";
                    ConsoleFocus();
                    //重试两次
                    for (int j = 1; j <= 3; j++)
                    {
                        temp = form1.lessonUtils.Send(form1.lessonUtils.url + "xsxklist!getAdd.action", "POST", data, form1.lessonUtils.url + "xsxklist!xsmhxsxk.action");
                        if (temp == "1")
                        {
                            temp = "抢课成功！";
                            Msg.Text += "第" + i.ToString() + "门课，" + temp + "\r\n";
                            ConsoleFocus();
                            break;
                        }
                        else
                        {
                            temp = "抢课失败，第" + j.ToString() + "次重试";
                            Msg.Text += "第" + i.ToString() + "门课，" + temp + "\r\n";
                            ConsoleFocus();
                        }
                    }
                    i++;
                }
                Msg.Text += "请到教务系统查看最终选课结果。\r\n";
                ConsoleFocus();
            }
            else
            {
                //循环多线程发送
                foreach (var data in postList)
                {
                    Msg.Text += "开始执行第" + i.ToString() + "门抢课线程！\r\n";
                    ConsoleFocus();
                    // 抢课线程
                    Thread thread = null;
                    thread = new Thread(new ParameterizedThreadStart(form1.lessonUtils.Send));
                    thread.Start(data);
                    i++;
                }
                Msg.Text += "\r\n正在进行抢课...请耐心等待弹窗提示！\r\n";
                Msg.Text += "如教务系统坑爹时间不正确，将一直请求，直到抢课开始~\r\n";
                ConsoleFocus();
            }
        }

        //点击开始抢课
        private void Execute_Click(object sender, EventArgs e)
        {
            if (listView1.CheckedItems.Count < 1)
            {
                MessageBox.Show("您还未选择课程！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            postList = new List<string>();
            string courses = "";
            //将选中的课程数据收集起来
            foreach (SelectedCourses item in listCourseSort.Items)
            {
                courses += "《" + item.name + " " + item.time + "》\r\n";
                postList.Add("kcrwdm=" + item.code + "&kcmc=" + HttpUtility.UrlEncode(item.name));
            }
            //点击是则开始等待抢课开始
            if (MessageBox.Show("您选择的课程为：\r\n" + courses + "是否提交？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk) == DialogResult.Yes)
            {
                stopTakeLesson.Visible = true;
                Execute.Visible = false;
                // 创建新线程，负责打印实时信息并执行选课
                t = new Thread(new ThreadStart(printMsg));
                t.Start();
                listView1.Enabled = false;
                multiThread.Enabled = false;
                dateTimePicker1.Enabled = false;
            }
        }

        private void main_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("确定退出吗？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            {
                Environment.Exit(0);
            }
            else
                e.Cancel = true;
        }

        //5分钟刷新一次抢课列表，保持与教务系统的连接
        private void timer1_Tick(object sender, EventArgs e)
        {
            //距离抢课小于15分钟，停止刷新列表
            if ((Start_T - DateTime.Now).Minutes <= 15)
                timer1.Stop();
            else
                try
                {
                    form1.lessonUtils.getCourseList();
                }
                catch (Exception)
                {

                }
        }

        public class SelectedCourses
        {
            // 课程代码
            public string code;
            // 课程名称
            public string name;
            // 上课时间
            public string time;
            public SelectedCourses(string code, string name, string time)
            {
                this.code = code;
                this.name = name;
                this.time = time;
            }
            // 重写ToString方法
            public override string ToString()
            {
                return name + " " + time;
            }

            public override bool Equals(object obj)
            {
                if (obj == null)
                {
                    return false;
                }
                if ((obj.GetType().Equals(this.GetType())) == false)
                {
                    return false;
                }
                SelectedCourses tmp;
                tmp = (SelectedCourses)obj;
                return code.Equals(tmp.code);
            }

            public override int GetHashCode()
            {   
                return code.GetHashCode();
            }
        }

        private void listView1_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            // 课程代码
            string code = listView1.Items[e.Index].SubItems[0].Text;
            // 课程名称
            string name = listView1.Items[e.Index].SubItems[1].Text;
            // 上课时间
            string time = listView1.Items[e.Index].SubItems[3].Text;
            SelectedCourses course = new SelectedCourses(code, name, time);
            // 如果存在则删除
            if (listCourseSort.Items.Contains(course))
                listCourseSort.Items.Remove(course);      
            // 不存在则增加
            else
                listCourseSort.Items.Add(course);
        }

        // 上移
        private void button1_Click(object sender, EventArgs e)
        {
           int index = listCourseSort.SelectedIndex;
            // 未选中
            if (index == -1)
            {
                MessageBox.Show("未选中选项！");
                return;
            }
            var tmp = listCourseSort.Items[index];
            int old = index;
            index -= 1;
            if (index < 0)
                index = 0;
            listCourseSort.Items[old] = listCourseSort.Items[index];
            listCourseSort.Items[index] = tmp;
            listCourseSort.SelectedIndex = index;
        }

        // 下移
        private void button2_Click(object sender, EventArgs e)
        {
            int index = listCourseSort.SelectedIndex;
            // 未选中
            if (index == -1)
            {
                MessageBox.Show("未选中选项！");
                return;
            }
            var tmp = listCourseSort.Items[index];
            int old = index;
            index += 1;
            if (index >= listCourseSort.Items.Count)
                index = listCourseSort.Items.Count - 1;
            listCourseSort.Items[old] = listCourseSort.Items[index];
            listCourseSort.Items[index] = tmp;
            listCourseSort.SelectedIndex = index;
        }

        private void StopTakeLesson_Click(object sender, EventArgs e)
        {
            if (isStart == true)
            {
                if (MessageBox.Show("抢课已经开始，强制停止将造成未知隐患，是否退出软件？", "警告", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    Application.Exit();
                }
                else
                    return;
            }
            if (MessageBox.Show("是否停止抢课？", "警告", MessageBoxButtons.YesNo) == DialogResult.No)
                return;
            t.Abort();
            Msg.Text += "\r\n～(￣▽￣～)~已停止执行. . .~\r\n";
            ConsoleFocus();
            listView1.Enabled = true;
            multiThread.Enabled = true;
            dateTimePicker1.Enabled = true;
            stopTakeLesson.Visible = false;
            Execute.Visible = true;
        }

        private void Main_Shown(object sender, EventArgs e)
        {
            MessageBox.Show("-(๑ᵔ⌔ᵔ๑) 在点击一键选课之前，请先填写选课时间哦！", "温馨提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}
