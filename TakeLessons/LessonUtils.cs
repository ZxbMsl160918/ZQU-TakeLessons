using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using System.Web.Script.Serialization;
using System.Diagnostics;
using System.Web;
using System.Threading;
using TakeLessons.common;

namespace TakeLessons
{  
    public class LessonUtils
    {
        private HttpWebRequest request;
        private HttpWebResponse response;
        private CookieContainer cookies;
        public static bool codeState;
        public dynamic json = null;

        public LessonUtils()
        {
            StreamReader reader;
            string jsonStr;
            cookies = null;
            //从配置文件读入
            //url = ConfigurationManager.AppSettings["url"];
            request = (HttpWebRequest)WebRequest.Create(Parameter.HostUrl + "/Setting.ashx");
            request.Method = "GET";
            request.KeepAlive = false;
            try
            {
                response = (HttpWebResponse)request.GetResponse();
                reader = new StreamReader(response.GetResponseStream());
                jsonStr = reader.ReadToEnd();
                response.Close();
                reader.Close();
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                json = serializer.Deserialize<dynamic>(jsonStr);
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
            finally
            {
                request.Abort();
            }
        }

        /// <summary>
        /// 是否需要验证码并保存请求的cookies
        /// </summary>
        /// <returns>-1获取是否需要失败，1需要验证码 0不需要验证码</returns>
        public int isNeedCode()
        {
            StreamReader streamReader;
            string result;
            request = (HttpWebRequest)WebRequest.Create(Parameter.eduSysUrl + "login!isNeedVerify.action?_=" + DateTime.Now.Millisecond);
            request.Method = "GET";
            //第一次请求
            if (cookies == null)
            { 
                request.CookieContainer = new CookieContainer();
                cookies = request.CookieContainer;
            }
            else
                request.CookieContainer = cookies;
            request.KeepAlive = true;
            try
            {
                response = (HttpWebResponse)request.GetResponse();
                streamReader = new StreamReader(response.GetResponseStream());
                //如果返回1表示需要验证码
                result = streamReader.ReadToEnd();
                streamReader.Close();
                response.Close();
            }
            catch (Exception)
            {
                return -1;
            }
            finally
            {
                request.Abort();
            }
            return result == "1" ? 1 : 0;
        }

        //获取验证码图片
        public Image getCodeImage()
        {
            Image image;
            string ms = DateTime.Now.Millisecond + "";
            request = (HttpWebRequest)WebRequest.Create(Parameter.eduSysUrl + "yzm?d=" + ms);
            request.ServicePoint.Expect100Continue = false;
            request.Method = "GET";
            request.KeepAlive = true;
            request.CookieContainer = cookies;
            request.Accept = "image/webp,image/apng,image/*,*/*;q=0.8";
            try
            {
                response = (HttpWebResponse)request.GetResponse();
                Stream stream = response.GetResponseStream();
                image = Image.FromStream(stream);
                stream.Close();
                response.Close();
            }
            catch (Exception)
            {                
                return null;
            }
            finally
            {
                request.Abort();
            }
            return image;
        }


        /// <summary>
        /// 执行模拟登陆教务系统
        /// </summary>
        /// <param name="id">账号</param>
        /// <param name="pwd">密码</param>
        /// <param name="code">验证码</param>
        /// <returns>服务器返回的json</returns>
        private dynamic login(string id, string pwd, string code)
        {
            request = (HttpWebRequest)WebRequest.Create(Parameter.eduSysUrl + "login!doLogin.action");
            request.KeepAlive = true;
            request.Method = "POST";
            request.CookieContainer = cookies;
            //登陆参数 账号、密码、验证码
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("account=" + id);
            stringBuilder.Append("&pwd=" + pwd);
            stringBuilder.Append("&verifycode=" + code);
            byte[] postData = new UTF8Encoding().GetBytes(stringBuilder.ToString());
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = postData.Length;
            try
            {
                request.GetRequestStream().Write(postData, 0, postData.Length);
                response = request.GetResponse() as HttpWebResponse;
                StreamReader rspReader = new StreamReader(response.GetResponseStream());
                string result = rspReader.ReadToEnd();
                rspReader.Close();
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                return serializer.Deserialize<dynamic>(result);
            }
            catch (WebException err)
            {
                Dictionary<string, string> msg = new Dictionary<string, string>
                {
                    { "status", "n" }
                };
                msg["msg"] = err.Message;
                return msg;
            }
        }

        /// <summary>
        /// 登陆，需要判断账号是否购买使用权
        /// </summary>
        /// <param name="id">学号</param>
        /// <param name="pwd">密码</param>
        /// <param name="code">验证码</param>
        /// <returns></returns>
        public dynamic executeLogin(string id, string pwd, string code)
        {
            Dictionary<string, string> msg = new Dictionary<string, string>
            {
                { "status", "n" }
            };
            // 请求结果
            string result;
            // 内部版设置为1，注释则需要收费
            //json["free"] = "1";
            //不免费使用
            if (json["free"] == "0")
            {
                try
                {
                    //请求获得注册情况
                    request = (HttpWebRequest)WebRequest.Create(Parameter.HostUrl + "/IsRegister.ashx?id=" + id);
                    request.Method = "GET";
                    request.KeepAlive = false;
                    response = (HttpWebResponse)request.GetResponse();
                    StreamReader reader = new StreamReader(response.GetResponseStream());
                    result = reader.ReadToEnd();
                    reader.Close();
                    response.Close();
                }
                catch (Exception err)
                {
                    MessageBox.Show("获取注册情况失败，" + err.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                    msg["msg"] = "获取注册情况失败";
                    return msg;
                }
                finally
                {
                    request.Abort();
                }
                //未注册购买
                if (result == "0")
                {
                    if (MessageBox.Show("学号：\"" + id + "\" 未激活，是否前往激活？", "温馨提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question,  MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification) == DialogResult.Yes)
                    {
                        Process.Start(Parameter.HostUrl + "/Register.aspx");
                        msg.Add("msg", "请完成激活. . .");
                    }
                    else
                    {
                        msg.Add("msg", "激活失败");
                    }
                    return msg;
                }
                //已注册购买 过期到主对话框获得
                else
                {
                    //执行登陆
                    return login(id, pwd, code);
                }
            }
            //免费使用
            else
                return login(id, pwd, code);
        }

        //获取课程信息
        //返回的为string的list，0为课程名称  1为老师姓名 2为上课时间 3为课程编号
        public List<string[]> getCourseList()
        {
            string[] data;
            List<string[]> courses = new List<string[]>();
            //测试使用
            //string jsonStr = "{\"total\":141,\"rows\":[{\"kcrwdm\":\"1060091\",\"pkrs\":\"84\",\"jxbdm\":\"1074974\",\"kcptdm\":\"105523072\",\"xmmc\":\"星期一晚,9-10节,1-108\",\"kcdm\":\"201095\",\"kcmc\":\"环境与资源\",\"rwdm\":\"1039234\",\"xbyqdm\":\"\",\"rs1\":\"\",\"rs2\":\"\",\"wyfjdm\":\"\",\"wyyzdm\":\"\",\"kkxqdm\":\"1\",\"zxs\":\"32\",\"xf\":\"2\",\"kcflmc\":\"通识教育课\",\"teaxm\":\"熊德信\",\"jxbrs\":\"84\"},{\"kcrwdm\":\"1060092\",\"pkrs\":\"92\",\"jxbdm\":\"1074975\",\"kcptdm\":\"105523072\",\"xmmc\":\"星期二晚,9-10节,2-205\",\"kcdm\":\"201095\",\"kcmc\":\"环境与资源\",\"rwdm\":\"1039234\",\"xbyqdm\":\"\",\"rs1\":\"\",\"rs2\":\"\",\"wyfjdm\":\"\",\"wyyzdm\":\"\",\"kkxqdm\":\"1\",\"zxs\":\"32\",\"xf\":\"2\",\"kcflmc\":\"通识教育课\",\"teaxm\":\"熊德信\",\"jxbrs\":\"92\"},{\"kcrwdm\":\"1060093\",\"pkrs\":\"92\",\"jxbdm\":\"1074976\",\"kcptdm\":\"105523072\",\"xmmc\":\"星期三晚,9-10节,2-205\",\"kcdm\":\"201095\",\"kcmc\":\"环境与资源\",\"rwdm\":\"1039234\",\"xbyqdm\":\"\",\"rs1\":\"\",\"rs2\":\"\",\"wyfjdm\":\"\",\"wyyzdm\":\"\",\"kkxqdm\":\"1\",\"zxs\":\"32\",\"xf\":\"2\",\"kcflmc\":\"通识教育课\",\"teaxm\":\"吴燕妮\",\"jxbrs\":\"92\"},{\"kcrwdm\":\"1060094\",\"pkrs\":\"92\",\"jxbdm\":\"1074977\",\"kcptdm\":\"105523072\",\"xmmc\":\"星期四晚,9-10节,2-205\",\"kcdm\":\"201095\",\"kcmc\":\"环境与资源\",\"rwdm\":\"1039234\",\"xbyqdm\":\"\",\"rs1\":\"\",\"rs2\":\"\",\"wyfjdm\":\"\",\"wyyzdm\":\"\",\"kkxqdm\":\"1\",\"zxs\":\"32\",\"xf\":\"2\",\"kcflmc\":\"通识教育课\",\"teaxm\":\"\",\"jxbrs\":\"92\"},{\"kcrwdm\":\"1060095\",\"pkrs\":\"92\",\"jxbdm\":\"1074978\",\"kcptdm\":\"105523073\",\"xmmc\":\"星期二晚,9-10节,2-405\",\"kcdm\":\"201224\",\"kcmc\":\"数学文化\",\"rwdm\":\"1039268\",\"xbyqdm\":\"\",\"rs1\":\"\",\"rs2\":\"\",\"wyfjdm\":\"\",\"wyyzdm\":\"\",\"kkxqdm\":\"1\",\"zxs\":\"32\",\"xf\":\"2\",\"kcflmc\":\"通识教育课\",\"teaxm\":\"朱智伟\",\"jxbrs\":\"68\"},{\"kcrwdm\":\"1060096\",\"pkrs\":\"92\",\"jxbdm\":\"1074979\",\"kcptdm\":\"105523067\",\"xmmc\":\"星期一晚,9-10节,2-301\",\"kcdm\":\"051104\",\"kcmc\":\"中国文化概论\",\"rwdm\":\"1039220\",\"xbyqdm\":\"\",\"rs1\":\"\",\"rs2\":\"\",\"wyfjdm\":\"\",\"wyyzdm\":\"\",\"kkxqdm\":\"1\",\"zxs\":\"32\",\"xf\":\"2\",\"kcflmc\":\"通识教育课\",\"teaxm\":\"孟建安\",\"jxbrs\":\"92\"},{\"kcrwdm\":\"1060097\",\"pkrs\":\"92\",\"jxbdm\":\"1074980\",\"kcptdm\":\"105523067\",\"xmmc\":\"星期二晚,9-10节,2-301\",\"kcdm\":\"051104\",\"kcmc\":\"中国文化概论\",\"rwdm\":\"1039220\",\"xbyqdm\":\"\",\"rs1\":\"\",\"rs2\":\"\",\"wyfjdm\":\"\",\"wyyzdm\":\"\",\"kkxqdm\":\"1\",\"zxs\":\"32\",\"xf\":\"2\",\"kcflmc\":\"通识教育课\",\"teaxm\":\"朱为鸿\",\"jxbrs\":\"92\"},{\"kcrwdm\":\"1060098\",\"pkrs\":\"92\",\"jxbdm\":\"1074981\",\"kcptdm\":\"105523067\",\"xmmc\":\"星期三晚,9-10节,2-301\",\"kcdm\":\"051104\",\"kcmc\":\"中国文化概论\",\"rwdm\":\"1039220\",\"xbyqdm\":\"\",\"rs1\":\"\",\"rs2\":\"\",\"wyfjdm\":\"\",\"wyyzdm\":\"\",\"kkxqdm\":\"1\",\"zxs\":\"32\",\"xf\":\"2\",\"kcflmc\":\"通识教育课\",\"teaxm\":\"梁桂平\",\"jxbrs\":\"92\"},{\"kcrwdm\":\"1060099\",\"pkrs\":\"92\",\"jxbdm\":\"1074982\",\"kcptdm\":\"105523067\",\"xmmc\":\"星期四晚,9-10节,2-301\",\"kcdm\":\"051104\",\"kcmc\":\"中国文化概论\",\"rwdm\":\"1039220\",\"xbyqdm\":\"\",\"rs1\":\"\",\"rs2\":\"\",\"wyfjdm\":\"\",\"wyyzdm\":\"\",\"kkxqdm\":\"1\",\"zxs\":\"32\",\"xf\":\"2\",\"kcflmc\":\"通识教育课\",\"teaxm\":\"温艳\",\"jxbrs\":\"56\"},{\"kcrwdm\":\"1060100\",\"pkrs\":\"92\",\"jxbdm\":\"1074983\",\"kcptdm\":\"105523067\",\"xmmc\":\"星期三晚,9-10节,2-402\",\"kcdm\":\"051104\",\"kcmc\":\"中国文化概论\",\"rwdm\":\"1039220\",\"xbyqdm\":\"\",\"rs1\":\"\",\"rs2\":\"\",\"wyfjdm\":\"\",\"wyyzdm\":\"\",\"kkxqdm\":\"1\",\"zxs\":\"32\",\"xf\":\"2\",\"kcflmc\":\"通识教育课\",\"teaxm\":\"康庄\",\"jxbrs\":\"2\"},{\"kcrwdm\":\"1060101\",\"pkrs\":\"52\",\"jxbdm\":\"1074984\",\"kcptdm\":\"105523064\",\"xmmc\":\"星期一晚,9-10节,2-218\",\"kcdm\":\"201252\",\"kcmc\":\"西方哲学智慧\",\"rwdm\":\"1039253\",\"xbyqdm\":\"\",\"rs1\":\"\",\"rs2\":\"\",\"wyfjdm\":\"\",\"wyyzdm\":\"\",\"kkxqdm\":\"1\",\"zxs\":\"32\",\"xf\":\"2\",\"kcflmc\":\"通识教育课\",\"teaxm\":\"黎玉琴\",\"jxbrs\":\"52\"},{\"kcrwdm\":\"1060102\",\"pkrs\":\"92\",\"jxbdm\":\"1074985\",\"kcptdm\":\"105523064\",\"xmmc\":\"星期二晚,9-10节,2-113\",\"kcdm\":\"201252\",\"kcmc\":\"西方哲学智慧\",\"rwdm\":\"1039253\",\"xbyqdm\":\"\",\"rs1\":\"\",\"rs2\":\"\",\"wyfjdm\":\"\",\"wyyzdm\":\"\",\"kkxqdm\":\"1\",\"zxs\":\"32\",\"xf\":\"2\",\"kcflmc\":\"通识教育课\",\"teaxm\":\"汪聂才\",\"jxbrs\":\"36\"},{\"kcrwdm\":\"1060103\",\"pkrs\":\"92\",\"jxbdm\":\"1074986\",\"kcptdm\":\"105523064\",\"xmmc\":\"星期三晚,9-10节,2-113\",\"kcdm\":\"201252\",\"kcmc\":\"西方哲学智慧\",\"rwdm\":\"1039253\",\"xbyqdm\":\"\",\"rs1\":\"\",\"rs2\":\"\",\"wyfjdm\":\"\",\"wyyzdm\":\"\",\"kkxqdm\":\"1\",\"zxs\":\"32\",\"xf\":\"2\",\"kcflmc\":\"通识教育课\",\"teaxm\":\"汪聂才\",\"jxbrs\":\"5\"},{\"kcrwdm\":\"1060104\",\"pkrs\":\"92\",\"jxbdm\":\"1074987\",\"kcptdm\":\"105523064\",\"xmmc\":\"星期四晚,9-10节,2-113\",\"kcdm\":\"201252\",\"kcmc\":\"西方哲学智慧\",\"rwdm\":\"1039253\",\"xbyqdm\":\"\",\"rs1\":\"\",\"rs2\":\"\",\"wyfjdm\":\"\",\"wyyzdm\":\"\",\"kkxqdm\":\"1\",\"zxs\":\"32\",\"xf\":\"2\",\"kcflmc\":\"通识教育课\",\"teaxm\":\"陈伟\",\"jxbrs\":\"92\"},{\"kcrwdm\":\"1060106\",\"pkrs\":\"92\",\"jxbdm\":\"1074989\",\"kcptdm\":\"105523095\",\"xmmc\":\"星期一晚,9-10节,2-202\",\"kcdm\":\"201201\",\"kcmc\":\"生态与人类\",\"rwdm\":\"1039273\",\"xbyqdm\":\"\",\"rs1\":\"\",\"rs2\":\"\",\"wyfjdm\":\"\",\"wyyzdm\":\"\",\"kkxqdm\":\"1\",\"zxs\":\"32\",\"xf\":\"2\",\"kcflmc\":\"通识教育课\",\"teaxm\":\"赵则海\",\"jxbrs\":\"92\"},{\"kcrwdm\":\"1060107\",\"pkrs\":\"92\",\"jxbdm\":\"1074990\",\"kcptdm\":\"105523095\",\"xmmc\":\"星期二晚,9-10节,2-202\",\"kcdm\":\"201201\",\"kcmc\":\"生态与人类\",\"rwdm\":\"1039273\",\"xbyqdm\":\"\",\"rs1\":\"\",\"rs2\":\"\",\"wyfjdm\":\"\",\"wyyzdm\":\"\",\"kkxqdm\":\"1\",\"zxs\":\"32\",\"xf\":\"2\",\"kcflmc\":\"通识教育课\",\"teaxm\":\"赵则海\",\"jxbrs\":\"92\"},{\"kcrwdm\":\"1060108\",\"pkrs\":\"92\",\"jxbdm\":\"1074991\",\"kcptdm\":\"105523095\",\"xmmc\":\"星期三晚,9-10节,2-202\",\"kcdm\":\"201201\",\"kcmc\":\"生态与人类\",\"rwdm\":\"1039273\",\"xbyqdm\":\"\",\"rs1\":\"\",\"rs2\":\"\",\"wyfjdm\":\"\",\"wyyzdm\":\"\",\"kkxqdm\":\"1\",\"zxs\":\"32\",\"xf\":\"2\",\"kcflmc\":\"通识教育课\",\"teaxm\":\"郭玉娟\",\"jxbrs\":\"92\"},{\"kcrwdm\":\"1060109\",\"pkrs\":\"92\",\"jxbdm\":\"1074992\",\"kcptdm\":\"105523095\",\"xmmc\":\"星期四晚,9-10节,2-202\",\"kcdm\":\"201201\",\"kcmc\":\"生态与人类\",\"rwdm\":\"1039273\",\"xbyqdm\":\"\",\"rs1\":\"\",\"rs2\":\"\",\"wyfjdm\":\"\",\"wyyzdm\":\"\",\"kkxqdm\":\"1\",\"zxs\":\"32\",\"xf\":\"2\",\"kcflmc\":\"通识教育课\",\"teaxm\":\"许冬焱\",\"jxbrs\":\"92\"},{\"kcrwdm\":\"1060114\",\"pkrs\":\"91\",\"jxbdm\":\"1074997\",\"kcptdm\":\"105523098\",\"xmmc\":\"星期一晚,9-10节,2-206\",\"kcdm\":\"201271\",\"kcmc\":\"养生与健康\",\"rwdm\":\"1039225\",\"xbyqdm\":\"\",\"rs1\":\"\",\"rs2\":\"\",\"wyfjdm\":\"\",\"wyyzdm\":\"\",\"kkxqdm\":\"1\",\"zxs\":\"32\",\"xf\":\"2\",\"kcflmc\":\"通识教育课\",\"teaxm\":\"刘延莹\",\"jxbrs\":\"91\"},{\"kcrwdm\":\"1060115\",\"pkrs\":\"91\",\"jxbdm\":\"1074998\",\"kcptdm\":\"105523098\",\"xmmc\":\"星期二晚,9-10节,2-206\",\"kcdm\":\"201271\",\"kcmc\":\"养生与健康\",\"rwdm\":\"1039225\",\"xbyqdm\":\"\",\"rs1\":\"\",\"rs2\":\"\",\"wyfjdm\":\"\",\"wyyzdm\":\"\",\"kkxqdm\":\"1\",\"zxs\":\"32\",\"xf\":\"2\",\"kcflmc\":\"通识教育课\",\"teaxm\":\"刘延莹\",\"jxbrs\":\"91\"},{\"kcrwdm\":\"1060116\",\"pkrs\":\"92\",\"jxbdm\":\"1074999\",\"kcptdm\":\"105523098\",\"xmmc\":\"星期三晚,9-10节,2-206\",\"kcdm\":\"201271\",\"kcmc\":\"养生与健康\",\"rwdm\":\"1039225\",\"xbyqdm\":\"\",\"rs1\":\"\",\"rs2\":\"\",\"wyfjdm\":\"\",\"wyyzdm\":\"\",\"kkxqdm\":\"1\",\"zxs\":\"32\",\"xf\":\"2\",\"kcflmc\":\"通识教育课\",\"teaxm\":\"黄思敏\",\"jxbrs\":\"92\"},{\"kcrwdm\":\"1060117\",\"pkrs\":\"92\",\"jxbdm\":\"1075000\",\"kcptdm\":\"105523098\",\"xmmc\":\"星期四晚,9-10节,2-206\",\"kcdm\":\"201271\",\"kcmc\":\"养生与健康\",\"rwdm\":\"1039225\",\"xbyqdm\":\"\",\"rs1\":\"\",\"rs2\":\"\",\"wyfjdm\":\"\",\"wyyzdm\":\"\",\"kkxqdm\":\"1\",\"zxs\":\"32\",\"xf\":\"2\",\"kcflmc\":\"通识教育课\",\"teaxm\":\"肖琳\",\"jxbrs\":\"92\"},{\"kcrwdm\":\"1060118\",\"pkrs\":\"92\",\"jxbdm\":\"1075001\",\"kcptdm\":\"105523101\",\"xmmc\":\"星期一晚,9-10节,3-109\",\"kcdm\":\"201330\",\"kcmc\":\"中外音乐名作赏析\",\"rwdm\":\"1039242\",\"xbyqdm\":\"\",\"rs1\":\"\",\"rs2\":\"\",\"wyfjdm\":\"\",\"wyyzdm\":\"\",\"kkxqdm\":\"1\",\"zxs\":\"32\",\"xf\":\"2\",\"kcflmc\":\"通识教育课\",\"teaxm\":\"孙家国\",\"jxbrs\":\"92\"},{\"kcrwdm\":\"1060119\",\"pkrs\":\"92\",\"jxbdm\":\"1075002\",\"kcptdm\":\"105523101\",\"xmmc\":\"星期二晚,9-10节,3-109\",\"kcdm\":\"201330\",\"kcmc\":\"中外音乐名作赏析\",\"rwdm\":\"1039242\",\"xbyqdm\":\"\",\"rs1\":\"\",\"rs2\":\"\",\"wyfjdm\":\"\",\"wyyzdm\":\"\",\"kkxqdm\":\"1\",\"zxs\":\"32\",\"xf\":\"2\",\"kcflmc\":\"通识教育课\",\"teaxm\":\"王伽娜\",\"jxbrs\":\"59\"},{\"kcrwdm\":\"1060120\",\"pkrs\":\"92\",\"jxbdm\":\"1075003\",\"kcptdm\":\"105523101\",\"xmmc\":\"星期三晚,9-10节,3-109\",\"kcdm\":\"201330\",\"kcmc\":\"中外音乐名作赏析\",\"rwdm\":\"1039242\",\"xbyqdm\":\"\",\"rs1\":\"\",\"rs2\":\"\",\"wyfjdm\":\"\",\"wyyzdm\":\"\",\"kkxqdm\":\"1\",\"zxs\":\"32\",\"xf\":\"2\",\"kcflmc\":\"通识教育课\",\"teaxm\":\"范晓君\",\"jxbrs\":\"28\"},{\"kcrwdm\":\"1060121\",\"pkrs\":\"92\",\"jxbdm\":\"1075004\",\"kcptdm\":\"105523101\",\"xmmc\":\"星期四晚,9-10节,3-109\",\"kcdm\":\"201330\",\"kcmc\":\"中外音乐名作赏析\",\"rwdm\":\"1039242\",\"xbyqdm\":\"\",\"rs1\":\"\",\"rs2\":\"\",\"wyfjdm\":\"\",\"wyyzdm\":\"\",\"kkxqdm\":\"1\",\"zxs\":\"32\",\"xf\":\"2\",\"kcflmc\":\"通识教育课\",\"teaxm\":\"范晓君\",\"jxbrs\":\"38\"},{\"kcrwdm\":\"1060123\",\"pkrs\":\"92\",\"jxbdm\":\"1075006\",\"kcptdm\":\"105523068\",\"xmmc\":\"星期一晚,9-10节,2-302\",\"kcdm\":\"201194\",\"kcmc\":\"社交礼仪\",\"rwdm\":\"1039233\",\"xbyqdm\":\"\",\"rs1\":\"\",\"rs2\":\"\",\"wyfjdm\":\"\",\"wyyzdm\":\"\",\"kkxqdm\":\"1\",\"zxs\":\"32\",\"xf\":\"2\",\"kcflmc\":\"通识教育课\",\"teaxm\":\"曹和修\",\"jxbrs\":\"92\"},{\"kcrwdm\":\"1060124\",\"pkrs\":\"92\",\"jxbdm\":\"1075007\",\"kcptdm\":\"105523068\",\"xmmc\":\"星期二晚,9-10节,2-302\",\"kcdm\":\"201194\",\"kcmc\":\"社交礼仪\",\"rwdm\":\"1039233\",\"xbyqdm\":\"\",\"rs1\":\"\",\"rs2\":\"\",\"wyfjdm\":\"\",\"wyyzdm\":\"\",\"kkxqdm\":\"1\",\"zxs\":\"32\",\"xf\":\"2\",\"kcflmc\":\"通识教育课\",\"teaxm\":\"田杨群\",\"jxbrs\":\"92\"},{\"kcrwdm\":\"1060125\",\"pkrs\":\"92\",\"jxbdm\":\"1075008\",\"kcptdm\":\"105523068\",\"xmmc\":\"星期三晚,9-10节,2-302\",\"kcdm\":\"201194\",\"kcmc\":\"社交礼仪\",\"rwdm\":\"1039233\",\"xbyqdm\":\"\",\"rs1\":\"\",\"rs2\":\"\",\"wyfjdm\":\"\",\"wyyzdm\":\"\",\"kkxqdm\":\"1\",\"zxs\":\"32\",\"xf\":\"2\",\"kcflmc\":\"通识教育课\",\"teaxm\":\"王乐\",\"jxbrs\":\"92\"},{\"kcrwdm\":\"1060126\",\"pkrs\":\"92\",\"jxbdm\":\"1075009\",\"kcptdm\":\"105523068\",\"xmmc\":\"星期四晚,9-10节,2-302\",\"kcdm\":\"201194\",\"kcmc\":\"社交礼仪\",\"rwdm\":\"1039233\",\"xbyqdm\":\"\",\"rs1\":\"\",\"rs2\":\"\",\"wyfjdm\":\"\",\"wyyzdm\":\"\",\"kkxqdm\":\"1\",\"zxs\":\"32\",\"xf\":\"2\",\"kcflmc\":\"通识教育课\",\"teaxm\":\"曹和修\",\"jxbrs\":\"92\"},{\"kcrwdm\":\"1060127\",\"pkrs\":\"92\",\"jxbdm\":\"1075010\",\"kcptdm\":\"105523065\",\"xmmc\":\"星期一晚,9-10节,3-301\",\"kcdm\":\"201200\",\"kcmc\":\"生命伦理学\",\"rwdm\":\"1039241\",\"xbyqdm\":\"\",\"rs1\":\"\",\"rs2\":\"\",\"wyfjdm\":\"\",\"wyyzdm\":\"\",\"kkxqdm\":\"1\",\"zxs\":\"32\",\"xf\":\"2\",\"kcflmc\":\"通识教育课\",\"teaxm\":\"唐文武\",\"jxbrs\":\"92\"},{\"kcrwdm\":\"1060128\",\"pkrs\":\"92\",\"jxbdm\":\"1075011\",\"kcptdm\":\"105523065\",\"xmmc\":\"星期三晚,9-10节,3-301\",\"kcdm\":\"201200\",\"kcmc\":\"生命伦理学\",\"rwdm\":\"1039241\",\"xbyqdm\":\"\",\"rs1\":\"\",\"rs2\":\"\",\"wyfjdm\":\"\",\"wyyzdm\":\"\",\"kkxqdm\":\"1\",\"zxs\":\"32\",\"xf\":\"2\",\"kcflmc\":\"通识教育课\",\"teaxm\":\"李佩环\",\"jxbrs\":\"92\"},{\"kcrwdm\":\"1060129\",\"pkrs\":\"92\",\"jxbdm\":\"1075012\",\"kcptdm\":\"105523065\",\"xmmc\":\"星期四晚,9-10节,3-301\",\"kcdm\":\"201200\",\"kcmc\":\"生命伦理学\",\"rwdm\":\"1039241\",\"xbyqdm\":\"\",\"rs1\":\"\",\"rs2\":\"\",\"wyfjdm\":\"\",\"wyyzdm\":\"\",\"kkxqdm\":\"1\",\"zxs\":\"32\",\"xf\":\"2\",\"kcflmc\":\"通识教育课\",\"teaxm\":\"郑福庆\",\"jxbrs\":\"92\"},{\"kcrwdm\":\"1060130\",\"pkrs\":\"50\",\"jxbdm\":\"1075018\",\"kcptdm\":\"105523109\",\"xmmc\":\"星期一晚,9-10节,2-210\",\"kcdm\":\"201083\",\"kcmc\":\"国学经典选读\",\"rwdm\":\"1039254\",\"xbyqdm\":\"\",\"rs1\":\"\",\"rs2\":\"\",\"wyfjdm\":\"\",\"wyyzdm\":\"\",\"kkxqdm\":\"1\",\"zxs\":\"32\",\"xf\":\"2\",\"kcflmc\":\"通识教育课\",\"teaxm\":\"黄伟\",\"jxbrs\":\"50\"},{\"kcrwdm\":\"1060131\",\"pkrs\":\"50\",\"jxbdm\":\"1075019\",\"kcptdm\":\"105523109\",\"xmmc\":\"星期三晚,9-10节,2-210\",\"kcdm\":\"201083\",\"kcmc\":\"国学经典选读\",\"rwdm\":\"1039254\",\"xbyqdm\":\"\",\"rs1\":\"\",\"rs2\":\"\",\"wyfjdm\":\"\",\"wyyzdm\":\"\",\"kkxqdm\":\"1\",\"zxs\":\"32\",\"xf\":\"2\",\"kcflmc\":\"通识教育课\",\"teaxm\":\"刘挺颂\",\"jxbrs\":\"50\"},{\"kcrwdm\":\"1060132\",\"pkrs\":\"50\",\"jxbdm\":\"1075020\",\"kcptdm\":\"105523109\",\"xmmc\":\"星期四晚,9-10节,2-210\",\"kcdm\":\"201083\",\"kcmc\":\"国学经典选读\",\"rwdm\":\"1039254\",\"xbyqdm\":\"\",\"rs1\":\"\",\"rs2\":\"\",\"wyfjdm\":\"\",\"wyyzdm\":\"\",\"kkxqdm\":\"1\",\"zxs\":\"32\",\"xf\":\"2\",\"kcflmc\":\"通识教育课\",\"teaxm\":\"黄伟\",\"jxbrs\":\"50\"},{\"kcrwdm\":\"1060133\",\"pkrs\":\"50\",\"jxbdm\":\"1075021\",\"kcptdm\":\"105523110\",\"xmmc\":\"星期一晚,9-10节,2-416\",\"kcdm\":\"201072\",\"kcmc\":\"古典文学名著选读\",\"rwdm\":\"1039261\",\"xbyqdm\":\"\",\"rs1\":\"\",\"rs2\":\"\",\"wyfjdm\":\"\",\"wyyzdm\":\"\",\"kkxqdm\":\"1\",\"zxs\":\"32\",\"xf\":\"2\",\"kcflmc\":\"通识教育课\",\"teaxm\":\"丁楹\",\"jxbrs\":\"50\"},{\"kcrwdm\":\"1060134\",\"pkrs\":\"50\",\"jxbdm\":\"1075022\",\"kcptdm\":\"105523110\",\"xmmc\":\"星期二晚,9-10节,2-416\",\"kcdm\":\"201072\",\"kcmc\":\"古典文学名著选读\",\"rwdm\":\"1039261\",\"xbyqdm\":\"\",\"rs1\":\"\",\"rs2\":\"\",\"wyfjdm\":\"\",\"wyyzdm\":\"\",\"kkxqdm\":\"1\",\"zxs\":\"32\",\"xf\":\"2\",\"kcflmc\":\"通识教育课\",\"teaxm\":\"刘挺颂\",\"jxbrs\":\"50\"},{\"kcrwdm\":\"1060135\",\"pkrs\":\"50\",\"jxbdm\":\"1075023\",\"kcptdm\":\"105523110\",\"xmmc\":\"星期三晚,9-10节,2-416\",\"kcdm\":\"201072\",\"kcmc\":\"古典文学名著选读\",\"rwdm\":\"1039261\",\"xbyqdm\":\"\",\"rs1\":\"\",\"rs2\":\"\",\"wyfjdm\":\"\",\"wyyzdm\":\"\",\"kkxqdm\":\"1\",\"zxs\":\"32\",\"xf\":\"2\",\"kcflmc\":\"通识教育课\",\"teaxm\":\"杜云南\",\"jxbrs\":\"50\"},{\"kcrwdm\":\"1060136\",\"pkrs\":\"50\",\"jxbdm\":\"1075024\",\"kcptdm\":\"105523111\",\"xmmc\":\"星期二晚,9-10节,2-208\",\"kcdm\":\"201079\",\"kcmc\":\"国外文学名著选读\",\"rwdm\":\"1039228\",\"xbyqdm\":\"\",\"rs1\":\"\",\"rs2\":\"\",\"wyfjdm\":\"\",\"wyyzdm\":\"\",\"kkxqdm\":\"1\",\"zxs\":\"32\",\"xf\":\"2\",\"kcflmc\":\"通识教育课\",\"teaxm\":\"梁桂平\",\"jxbrs\":\"50\"},{\"kcrwdm\":\"1060137\",\"pkrs\":\"50\",\"jxbdm\":\"1075025\",\"kcptdm\":\"105523111\",\"xmmc\":\"星期三晚,9-10节,2-208\",\"kcdm\":\"201079\",\"kcmc\":\"国外文学名著选读\",\"rwdm\":\"1039228\",\"xbyqdm\":\"\",\"rs1\":\"\",\"rs2\":\"\",\"wyfjdm\":\"\",\"wyyzdm\":\"\",\"kkxqdm\":\"1\",\"zxs\":\"32\",\"xf\":\"2\",\"kcflmc\":\"通识教育课\",\"teaxm\":\"马娅\",\"jxbrs\":\"50\"},{\"kcrwdm\":\"1060138\",\"pkrs\":\"50\",\"jxbdm\":\"1075026\",\"kcptdm\":\"105523111\",\"xmmc\":\"星期四晚,9-10节,2-208\",\"kcdm\":\"201079\",\"kcmc\":\"国外文学名著选读\",\"rwdm\":\"1039228\",\"xbyqdm\":\"\",\"rs1\":\"\",\"rs2\":\"\",\"wyfjdm\":\"\",\"wyyzdm\":\"\",\"kkxqdm\":\"1\",\"zxs\":\"32\",\"xf\":\"2\",\"kcflmc\":\"通识教育课\",\"teaxm\":\"温洁霞\",\"jxbrs\":\"50\"},{\"kcrwdm\":\"1060144\",\"pkrs\":\"50\",\"jxbdm\":\"1075032\",\"kcptdm\":\"105523114\",\"xmmc\":\"星期三晚,9-10节,2-218\",\"kcdm\":\"201081\",\"kcmc\":\"国外政治法律名著选读\",\"rwdm\":\"1039227\",\"xbyqdm\":\"\",\"rs1\":\"\",\"rs2\":\"\",\"wyfjdm\":\"\",\"wyyzdm\":\"\",\"kkxqdm\":\"1\",\"zxs\":\"32\",\"xf\":\"2\",\"kcflmc\":\"通识教育课\",\"teaxm\":\"戴维\",\"jxbrs\":\"11\"},{\"kcrwdm\":\"1060145\",\"pkrs\":\"50\",\"jxbdm\":\"1075033\",\"kcptdm\":\"105523114\",\"xmmc\":\"星期四晚,9-10节,2-218\",\"kcdm\":\"201081\",\"kcmc\":\"国外政治法律名著选读\",\"rwdm\":\"1039227\",\"xbyqdm\":\"\",\"rs1\":\"\",\"rs2\":\"\",\"wyfjdm\":\"\",\"wyyzdm\":\"\",\"kkxqdm\":\"1\",\"zxs\":\"32\",\"xf\":\"2\",\"kcflmc\":\"通识教育课\",\"teaxm\":\"韩月香\",\"jxbrs\":\"50\"},{\"kcrwdm\":\"1060147\",\"pkrs\":\"92\",\"jxbdm\":\"1075035\",\"kcptdm\":\"105523116\",\"xmmc\":\"星期一晚,9-10节,2-415\",\"kcdm\":\"201022\",\"kcmc\":\"《易经》与中国文化\",\"rwdm\":\"1039275\",\"xbyqdm\":\"\",\"rs1\":\"\",\"rs2\":\"\",\"wyfjdm\":\"\",\"wyyzdm\":\"\",\"kkxqdm\":\"1\",\"zxs\":\"32\",\"xf\":\"2\",\"kcflmc\":\"通识教育课\",\"teaxm\":\"邢建勇\",\"jxbrs\":\"92\"},{\"kcrwdm\":\"1060148\",\"pkrs\":\"92\",\"jxbdm\":\"1075036\",\"kcptdm\":\"105523116\",\"xmmc\":\"星期二晚,9-10节,2-415\",\"kcdm\":\"201022\",\"kcmc\":\"《易经》与中国文化\",\"rwdm\":\"1039275\",\"xbyqdm\":\"\",\"rs1\":\"\",\"rs2\":\"\",\"wyfjdm\":\"\",\"wyyzdm\":\"\",\"kkxqdm\":\"1\",\"zxs\":\"32\",\"xf\":\"2\",\"kcflmc\":\"通识教育课\",\"teaxm\":\"邢建勇\",\"jxbrs\":\"92\"},{\"kcrwdm\":\"1060149\",\"pkrs\":\"92\",\"jxbdm\":\"1075037\",\"kcptdm\":\"105523117\",\"xmmc\":\"星期一晚,9-10节,2-213\",\"kcdm\":\"201323\",\"kcmc\":\"中国哲学智慧\",\"rwdm\":\"1039250\",\"xbyqdm\":\"\",\"rs1\":\"\",\"rs2\":\"\",\"wyfjdm\":\"\",\"wyyzdm\":\"\",\"kkxqdm\":\"1\",\"zxs\":\"32\",\"xf\":\"2\",\"kcflmc\":\"通识教育课\",\"teaxm\":\"周黄琴\",\"jxbrs\":\"27\"},{\"kcrwdm\":\"1060150\",\"pkrs\":\"92\",\"jxbdm\":\"1075038\",\"kcptdm\":\"105523117\",\"xmmc\":\"星期三晚,9-10节,2-213\",\"kcdm\":\"201323\",\"kcmc\":\"中国哲学智慧\",\"rwdm\":\"1039250\",\"xbyqdm\":\"\",\"rs1\":\"\",\"rs2\":\"\",\"wyfjdm\":\"\",\"wyyzdm\":\"\",\"kkxqdm\":\"1\",\"zxs\":\"32\",\"xf\":\"2\",\"kcflmc\":\"通识教育课\",\"teaxm\":\"何凯文\",\"jxbrs\":\"12\"},{\"kcrwdm\":\"1060152\",\"pkrs\":\"92\",\"jxbdm\":\"1075040\",\"kcptdm\":\"105523118\",\"xmmc\":\"星期一晚,9-10节,3-101\",\"kcdm\":\"201078\",\"kcmc\":\"国外名画赏析\",\"rwdm\":\"1039271\",\"xbyqdm\":\"\",\"rs1\":\"\",\"rs2\":\"\",\"wyfjdm\":\"\",\"wyyzdm\":\"\",\"kkxqdm\":\"1\",\"zxs\":\"32\",\"xf\":\"2\",\"kcflmc\":\"通识教育课\",\"teaxm\":\"刘晓慧\",\"jxbrs\":\"92\"},{\"kcrwdm\":\"1060153\",\"pkrs\":\"92\",\"jxbdm\":\"1075041\",\"kcptdm\":\"105523118\",\"xmmc\":\"星期二晚,9-10节,3-101\",\"kcdm\":\"201078\",\"kcmc\":\"国外名画赏析\",\"rwdm\":\"1039271\",\"xbyqdm\":\"\",\"rs1\":\"\",\"rs2\":\"\",\"wyfjdm\":\"\",\"wyyzdm\":\"\",\"kkxqdm\":\"1\",\"zxs\":\"32\",\"xf\":\"2\",\"kcflmc\":\"通识教育课\",\"teaxm\":\"韩靖\",\"jxbrs\":\"92\"},{\"kcrwdm\":\"1060154\",\"pkrs\":\"92\",\"jxbdm\":\"1075042\",\"kcptdm\":\"105523118\",\"xmmc\":\"星期三晚,9-10节,3-101\",\"kcdm\":\"201078\",\"kcmc\":\"国外名画赏析\",\"rwdm\":\"1039271\",\"xbyqdm\":\"\",\"rs1\":\"\",\"rs2\":\"\",\"wyfjdm\":\"\",\"wyyzdm\":\"\",\"kkxqdm\":\"1\",\"zxs\":\"32\",\"xf\":\"2\",\"kcflmc\":\"通识教育课\",\"teaxm\":\"韩靖\",\"jxbrs\":\"26\"},{\"kcrwdm\":\"1060155\",\"pkrs\":\"92\",\"jxbdm\":\"1075043\",\"kcptdm\":\"105523119\",\"xmmc\":\"星期一晚,9-10节,3-102\",\"kcdm\":\"201313\",\"kcmc\":\"中国历代名画赏析\",\"rwdm\":\"1039267\",\"xbyqdm\":\"\",\"rs1\":\"\",\"rs2\":\"\",\"wyfjdm\":\"\",\"wyyzdm\":\"\",\"kkxqdm\":\"1\",\"zxs\":\"32\",\"xf\":\"2\",\"kcflmc\":\"通识教育课\",\"teaxm\":\"陈文彦 \",\"jxbrs\":\"92\"},{\"kcrwdm\":\"1060156\",\"pkrs\":\"92\",\"jxbdm\":\"1075044\",\"kcptdm\":\"105523119\",\"xmmc\":\"星期二晚,9-10节,3-102\",\"kcdm\":\"201313\",\"kcmc\":\"中国历代名画赏析\",\"rwdm\":\"1039267\",\"xbyqdm\":\"\",\"rs1\":\"\",\"rs2\":\"\",\"wyfjdm\":\"\",\"wyyzdm\":\"\",\"kkxqdm\":\"1\",\"zxs\":\"32\",\"xf\":\"2\",\"kcflmc\":\"通识教育课\",\"teaxm\":\"任漫丛\",\"jxbrs\":\"92\"},{\"kcrwdm\":\"1060157\",\"pkrs\":\"92\",\"jxbdm\":\"1075045\",\"kcptdm\":\"105523119\",\"xmmc\":\"星期四晚,9-10节,3-102\",\"kcdm\":\"201313\",\"kcmc\":\"中国历代名画赏析\",\"rwdm\":\"1039267\",\"xbyqdm\":\"\",\"rs1\":\"\",\"rs2\":\"\",\"wyfjdm\":\"\",\"wyyzdm\":\"\",\"kkxqdm\":\"1\",\"zxs\":\"32\",\"xf\":\"2\",\"kcflmc\":\"通识教育课\",\"teaxm\":\"陈文彦 \",\"jxbrs\":\"92\"},{\"kcrwdm\":\"1060168\",\"pkrs\":\"92\",\"jxbdm\":\"1075056\",\"kcptdm\":\"105523121\",\"xmmc\":\"星期一晚,9-10节,3-308\",\"kcdm\":\"201260\",\"kcmc\":\"现代西方文学赏析\",\"rwdm\":\"1039265\",\"xbyqdm\":\"\",\"rs1\":\"\",\"rs2\":\"\",\"wyfjdm\":\"\",\"wyyzdm\":\"\",\"kkxqdm\":\"1\",\"zxs\":\"32\",\"xf\":\"2\",\"kcflmc\":\"通识教育课\",\"teaxm\":\"温洁霞\",\"jxbrs\":\"9\"},{\"kcrwdm\":\"1060169\",\"pkrs\":\"92\",\"jxbdm\":\"1075057\",\"kcptdm\":\"105523121\",\"xmmc\":\"星期二晚,9-10节,3-308\",\"kcdm\":\"201260\",\"kcmc\":\"现代西方文学赏析\",\"rwdm\":\"1039265\",\"xbyqdm\":\"\",\"rs1\":\"\",\"rs2\":\"\",\"wyfjdm\":\"\",\"wyyzdm\":\"\",\"kkxqdm\":\"1\",\"zxs\":\"32\",\"xf\":\"2\",\"kcflmc\":\"通识教育课\",\"teaxm\":\"陈爱香\",\"jxbrs\":\"50\"},{\"kcrwdm\":\"1060170\",\"pkrs\":\"92\",\"jxbdm\":\"1075058\",\"kcptdm\":\"105523121\",\"xmmc\":\"星期三晚,9-10节,3-308\",\"kcdm\":\"201260\",\"kcmc\":\"现代西方文学赏析\",\"rwdm\":\"1039265\",\"xbyqdm\":\"\",\"rs1\":\"\",\"rs2\":\"\",\"wyfjdm\":\"\",\"wyyzdm\":\"\",\"kkxqdm\":\"1\",\"zxs\":\"32\",\"xf\":\"2\",\"kcflmc\":\"通识教育课\",\"teaxm\":\"温洁霞\",\"jxbrs\":\"4\"},{\"kcrwdm\":\"1060172\",\"pkrs\":\"92\",\"jxbdm\":\"1075060\",\"kcptdm\":\"105523122\",\"xmmc\":\"星期一晚,9-10节,3-108\",\"kcdm\":\"201293\",\"kcmc\":\"影视名片赏析\",\"rwdm\":\"1039274\",\"xbyqdm\":\"\",\"rs1\":\"\",\"rs2\":\"\",\"wyfjdm\":\"\",\"wyyzdm\":\"\",\"kkxqdm\":\"1\",\"zxs\":\"32\",\"xf\":\"2\",\"kcflmc\":\"通识教育课\",\"teaxm\":\"陈明华\",\"jxbrs\":\"92\"},{\"kcrwdm\":\"1060173\",\"pkrs\":\"92\",\"jxbdm\":\"1075061\",\"kcptdm\":\"105523122\",\"xmmc\":\"星期二晚,9-10节,3-108\",\"kcdm\":\"201293\",\"kcmc\":\"影视名片赏析\",\"rwdm\":\"1039274\",\"xbyqdm\":\"\",\"rs1\":\"\",\"rs2\":\"\",\"wyfjdm\":\"\",\"wyyzdm\":\"\",\"kkxqdm\":\"1\",\"zxs\":\"32\",\"xf\":\"2\",\"kcflmc\":\"通识教育课\",\"teaxm\":\"赖翅萍\",\"jxbrs\":\"92\"},{\"kcrwdm\":\"1060174\",\"pkrs\":\"92\",\"jxbdm\":\"1075062\",\"kcptdm\":\"105523122\",\"xmmc\":\"星期三晚,9-10节,3-108\",\"kcdm\":\"201293\",\"kcmc\":\"影视名片赏析\",\"rwdm\":\"1039274\",\"xbyqdm\":\"\",\"rs1\":\"\",\"rs2\":\"\",\"wyfjdm\":\"\",\"wyyzdm\":\"\",\"kkxqdm\":\"1\",\"zxs\":\"32\",\"xf\":\"2\",\"kcflmc\":\"通识教育课\",\"teaxm\":\"徐法超\",\"jxbrs\":\"92\"},{\"kcrwdm\":\"1060175\",\"pkrs\":\"92\",\"jxbdm\":\"1075063\",\"kcptdm\":\"105523122\",\"xmmc\":\"星期四晚,9-10节,3-108\",\"kcdm\":\"201293\",\"kcmc\":\"影视名片赏析\",\"rwdm\":\"1039274\",\"xbyqdm\":\"\",\"rs1\":\"\",\"rs2\":\"\",\"wyfjdm\":\"\",\"wyyzdm\":\"\",\"kkxqdm\":\"1\",\"zxs\":\"32\",\"xf\":\"2\",\"kcflmc\":\"通识教育课\",\"teaxm\":\"罗彦\",\"jxbrs\":\"92\"},{\"kcrwdm\":\"1060176\",\"pkrs\":\"92\",\"jxbdm\":\"1075064\",\"kcptdm\":\"105523257\",\"xmmc\":\"星期二晚,9-10节,3-301\",\"kcdm\":\"201310\",\"kcmc\":\"中国古代诗词赏析\",\"rwdm\":\"1039248\",\"xbyqdm\":\"\",\"rs1\":\"\",\"rs2\":\"\",\"wyfjdm\":\"\",\"wyyzdm\":\"\",\"kkxqdm\":\"1\",\"zxs\":\"32\",\"xf\":\"2\",\"kcflmc\":\"通识教育课\",\"teaxm\":\"丁楹\",\"jxbrs\":\"92\"},{\"kcrwdm\":\"1060177\",\"pkrs\":\"92\",\"jxbdm\":\"1075065\",\"kcptdm\":\"105523257\",\"xmmc\":\"星期四晚,9-10节,3-307\",\"kcdm\":\"201310\",\"kcmc\":\"中国古代诗词赏析\",\"rwdm\":\"1039248\",\"xbyqdm\":\"\",\"rs1\":\"\",\"rs2\":\"\",\"wyfjdm\":\"\",\"wyyzdm\":\"\",\"kkxqdm\":\"1\",\"zxs\":\"32\",\"xf\":\"2\",\"kcflmc\":\"通识教育课\",\"teaxm\":\"丁楹\",\"jxbrs\":\"92\"},{\"kcrwdm\":\"1060178\",\"pkrs\":\"92\",\"jxbdm\":\"1075066\",\"kcptdm\":\"105523258\",\"xmmc\":\"星期一晚,9-10节,3-310\",\"kcdm\":\"201311\",\"kcmc\":\"中国古代戏曲赏析\",\"rwdm\":\"1039251\",\"xbyqdm\":\"\",\"rs1\":\"\",\"rs2\":\"\",\"wyfjdm\":\"\",\"wyyzdm\":\"\",\"kkxqdm\":\"1\",\"zxs\":\"32\",\"xf\":\"2\",\"kcflmc\":\"通识教育课\",\"teaxm\":\"刘炳辰\",\"jxbrs\":\"92\"},{\"kcrwdm\":\"1060179\",\"pkrs\":\"92\",\"jxbdm\":\"1075067\",\"kcptdm\":\"105523258\",\"xmmc\":\"星期二晚,9-10节,3-310\",\"kcdm\":\"201311\",\"kcmc\":\"中国古代戏曲赏析\",\"rwdm\":\"1039251\",\"xbyqdm\":\"\",\"rs1\":\"\",\"rs2\":\"\",\"wyfjdm\":\"\",\"wyyzdm\":\"\",\"kkxqdm\":\"1\",\"zxs\":\"32\",\"xf\":\"2\",\"kcflmc\":\"通识教育课\",\"teaxm\":\"刘炳辰\",\"jxbrs\":\"15\"},{\"kcrwdm\":\"1060182\",\"pkrs\":\"92\",\"jxbdm\":\"1075070\",\"kcptdm\":\"105523259\",\"xmmc\":\"星期二晚,9-10节,2-414\",\"kcdm\":\"201312\",\"kcmc\":\"中国古典园林文化与审美\",\"rwdm\":\"1039245\",\"xbyqdm\":\"\",\"rs1\":\"\",\"rs2\":\"\",\"wyfjdm\":\"\",\"wyyzdm\":\"\",\"kkxqdm\":\"1\",\"zxs\":\"32\",\"xf\":\"2\",\"kcflmc\":\"通识教育课\",\"teaxm\":\"杨盛\",\"jxbrs\":\"92\"},{\"kcrwdm\":\"1060183\",\"pkrs\":\"92\",\"jxbdm\":\"1075071\",\"kcptdm\":\"105523259\",\"xmmc\":\"星期三晚,9-10节,2-414\",\"kcdm\":\"201312\",\"kcmc\":\"中国古典园林文化与审美\",\"rwdm\":\"1039245\",\"xbyqdm\":\"\",\"rs1\":\"\",\"rs2\":\"\",\"wyfjdm\":\"\",\"wyyzdm\":\"\",\"kkxqdm\":\"1\",\"zxs\":\"32\",\"xf\":\"2\",\"kcflmc\":\"通识教育课\",\"teaxm\":\"杨盛\",\"jxbrs\":\"92\"},{\"kcrwdm\":\"1060184\",\"pkrs\":\"92\",\"jxbdm\":\"1075072\",\"kcptdm\":\"105523260\",\"xmmc\":\"星期一晚,9-10节,2-516\",\"kcdm\":\"201319\",\"kcmc\":\"中国现当代文学名著赏析\",\"rwdm\":\"1039256\",\"xbyqdm\":\"\",\"rs1\":\"\",\"rs2\":\"\",\"wyfjdm\":\"\",\"wyyzdm\":\"\",\"kkxqdm\":\"1\",\"zxs\":\"32\",\"xf\":\"2\",\"kcflmc\":\"通识教育课\",\"teaxm\":\"王少瑜\",\"jxbrs\":\"17\"},{\"kcrwdm\":\"1060185\",\"pkrs\":\"92\",\"jxbdm\":\"1075073\",\"kcptdm\":\"105523260\",\"xmmc\":\"星期二晚,9-10节,2-516\",\"kcdm\":\"201319\",\"kcmc\":\"中国现当代文学名著赏析\",\"rwdm\":\"1039256\",\"xbyqdm\":\"\",\"rs1\":\"\",\"rs2\":\"\",\"wyfjdm\":\"\",\"wyyzdm\":\"\",\"kkxqdm\":\"1\",\"zxs\":\"32\",\"xf\":\"2\",\"kcflmc\":\"通识教育课\",\"teaxm\":\"孙莹\",\"jxbrs\":\"22\"},{\"kcrwdm\":\"1060186\",\"pkrs\":\"92\",\"jxbdm\":\"1075074\",\"kcptdm\":\"105523260\",\"xmmc\":\"星期三晚,9-10节,2-516\",\"kcdm\":\"201319\",\"kcmc\":\"中国现当代文学名著赏析\",\"rwdm\":\"1039256\",\"xbyqdm\":\"\",\"rs1\":\"\",\"rs2\":\"\",\"wyfjdm\":\"\",\"wyyzdm\":\"\",\"kkxqdm\":\"1\",\"zxs\":\"32\",\"xf\":\"2\",\"kcflmc\":\"通识教育课\",\"teaxm\":\"杨红军\",\"jxbrs\":\"92\"},{\"kcrwdm\":\"1060187\",\"pkrs\":\"92\",\"jxbdm\":\"1075075\",\"kcptdm\":\"105523261\",\"xmmc\":\"星期一晚,9-10节,3-407\",\"kcdm\":\"201321\",\"kcmc\":\"中国新诗赏析\",\"rwdm\":\"1039239\",\"xbyqdm\":\"\",\"rs1\":\"\",\"rs2\":\"\",\"wyfjdm\":\"\",\"wyyzdm\":\"\",\"kkxqdm\":\"1\",\"zxs\":\"32\",\"xf\":\"2\",\"kcflmc\":\"通识教育课\",\"teaxm\":\"黎保荣\",\"jxbrs\":\"92\"},{\"kcrwdm\":\"1060188\",\"pkrs\":\"92\",\"jxbdm\":\"1075076\",\"kcptdm\":\"105523261\",\"xmmc\":\"星期二晚,9-10节,3-407\",\"kcdm\":\"201321\",\"kcmc\":\"中国新诗赏析\",\"rwdm\":\"1039239\",\"xbyqdm\":\"\",\"rs1\":\"\",\"rs2\":\"\",\"wyfjdm\":\"\",\"wyyzdm\":\"\",\"kkxqdm\":\"1\",\"zxs\":\"32\",\"xf\":\"2\",\"kcflmc\":\"通识教育课\",\"teaxm\":\"杨红军\",\"jxbrs\":\"92\"},{\"kcrwdm\":\"1060189\",\"pkrs\":\"92\",\"jxbdm\":\"1075077\",\"kcptdm\":\"105523261\",\"xmmc\":\"星期三晚,9-10节,3-407\",\"kcdm\":\"201321\",\"kcmc\":\"中国新诗赏析\",\"rwdm\":\"1039239\",\"xbyqdm\":\"\",\"rs1\":\"\",\"rs2\":\"\",\"wyfjdm\":\"\",\"wyyzdm\":\"\",\"kkxqdm\":\"1\",\"zxs\":\"32\",\"xf\":\"2\",\"kcflmc\":\"通识教育课\",\"teaxm\":\"鲍昌宝\",\"jxbrs\":\"92\"},{\"kcrwdm\":\"1060190\",\"pkrs\":\"92\",\"jxbdm\":\"1075078\",\"kcptdm\":\"105523262\",\"xmmc\":\"星期一晚,9-10节,3-410\",\"kcdm\":\"201332\",\"kcmc\":\"中西审美文化比较\",\"rwdm\":\"1039238\",\"xbyqdm\":\"\",\"rs1\":\"\",\"rs2\":\"\",\"wyfjdm\":\"\",\"wyyzdm\":\"\",\"kkxqdm\":\"1\",\"zxs\":\"32\",\"xf\":\"2\",\"kcflmc\":\"通识教育课\",\"teaxm\":\"卢永和\",\"jxbrs\":\"92\"},{\"kcrwdm\":\"1060191\",\"pkrs\":\"92\",\"jxbdm\":\"1075079\",\"kcptdm\":\"105523262\",\"xmmc\":\"星期二晚,9-10节,3-410\",\"kcdm\":\"201332\",\"kcmc\":\"中西审美文化比较\",\"rwdm\":\"1039238\",\"xbyqdm\":\"\",\"rs1\":\"\",\"rs2\":\"\",\"wyfjdm\":\"\",\"wyyzdm\":\"\",\"kkxqdm\":\"1\",\"zxs\":\"32\",\"xf\":\"2\",\"kcflmc\":\"通识教育课\",\"teaxm\":\"卢永和\",\"jxbrs\":\"92\"},{\"kcrwdm\":\"1060193\",\"pkrs\":\"92\",\"jxbdm\":\"1075081\",\"kcptdm\":\"105523263\",\"xmmc\":\"星期日晚,9-10节,2-206\",\"kcdm\":\"201093\",\"kcmc\":\"化学与社会\",\"rwdm\":\"1039243\",\"xbyqdm\":\"\",\"rs1\":\"\",\"rs2\":\"\",\"wyfjdm\":\"\",\"wyyzdm\":\"\",\"kkxqdm\":\"1\",\"zxs\":\"32\",\"xf\":\"2\",\"kcflmc\":\"通识教育课\",\"teaxm\":\"姚夙\",\"jxbrs\":\"92\"},{\"kcrwdm\":\"1060194\",\"pkrs\":\"92\",\"jxbdm\":\"1075082\",\"kcptdm\":\"105523263\",\"xmmc\":\"星期二晚,9-10节,2-213\",\"kcdm\":\"201093\",\"kcmc\":\"化学与社会\",\"rwdm\":\"1039243\",\"xbyqdm\":\"\",\"rs1\":\"\",\"rs2\":\"\",\"wyfjdm\":\"\",\"wyyzdm\":\"\",\"kkxqdm\":\"1\",\"zxs\":\"32\",\"xf\":\"2\",\"kcflmc\":\"通识教育课\",\"teaxm\":\"姚夙\",\"jxbrs\":\"92\"},{\"kcrwdm\":\"1060195\",\"pkrs\":\"92\",\"jxbdm\":\"1075083\",\"kcptdm\":\"105523263\",\"xmmc\":\"星期四晚,9-10节,2-213\",\"kcdm\":\"201093\",\"kcmc\":\"化学与社会\",\"rwdm\":\"1039243\",\"xbyqdm\":\"\",\"rs1\":\"\",\"rs2\":\"\",\"wyfjdm\":\"\",\"wyyzdm\":\"\",\"kkxqdm\":\"1\",\"zxs\":\"32\",\"xf\":\"2\",\"kcflmc\":\"通识教育课\",\"teaxm\":\"汪洪武\",\"jxbrs\":\"92\"},{\"kcrwdm\":\"1060196\",\"pkrs\":\"92\",\"jxbdm\":\"1075084\",\"kcptdm\":\"105523264\",\"xmmc\":\"星期一晚,9-10节,2-305\",\"kcdm\":\"201199\",\"kcmc\":\"生命科学与人类文明\",\"rwdm\":\"1039276\",\"xbyqdm\":\"\",\"rs1\":\"\",\"rs2\":\"\",\"wyfjdm\":\"\",\"wyyzdm\":\"\",\"kkxqdm\":\"1\",\"zxs\":\"32\",\"xf\":\"2\",\"kcflmc\":\"通识教育课\",\"teaxm\":\"姜玉霞\",\"jxbrs\":\"92\"},{\"kcrwdm\":\"1060197\",\"pkrs\":\"92\",\"jxbdm\":\"1075085\",\"kcptdm\":\"105523264\",\"xmmc\":\"星期二晚,9-10节,2-305\",\"kcdm\":\"201199\",\"kcmc\":\"生命科学与人类文明\",\"rwdm\":\"1039276\",\"xbyqdm\":\"\",\"rs1\":\"\",\"rs2\":\"\",\"wyfjdm\":\"\",\"wyyzdm\":\"\",\"kkxqdm\":\"1\",\"zxs\":\"32\",\"xf\":\"2\",\"kcflmc\":\"通识教育课\",\"teaxm\":\"姜玉霞\",\"jxbrs\":\"92\"},{\"kcrwdm\":\"1060198\",\"pkrs\":\"92\",\"jxbdm\":\"1075086\",\"kcptdm\":\"105523264\",\"xmmc\":\"星期三晚,9-10节,2-305\",\"kcdm\":\"201199\",\"kcmc\":\"生命科学与人类文明\",\"rwdm\":\"1039276\",\"xbyqdm\":\"\",\"rs1\":\"\",\"rs2\":\"\",\"wyfjdm\":\"\",\"wyyzdm\":\"\",\"kkxqdm\":\"1\",\"zxs\":\"32\",\"xf\":\"2\",\"kcflmc\":\"通识教育课\",\"teaxm\":\"姜玉霞\",\"jxbrs\":\"92\"},{\"kcrwdm\":\"1060199\",\"pkrs\":\"92\",\"jxbdm\":\"1075087\",\"kcptdm\":\"105523264\",\"xmmc\":\"星期四晚,9-10节,2-305\",\"kcdm\":\"201199\",\"kcmc\":\"生命科学与人类文明\",\"rwdm\":\"1039276\",\"xbyqdm\":\"\",\"rs1\":\"\",\"rs2\":\"\",\"wyfjdm\":\"\",\"wyyzdm\":\"\",\"kkxqdm\":\"1\",\"zxs\":\"32\",\"xf\":\"2\",\"kcflmc\":\"通识教育课\",\"teaxm\":\"姜玉霞\",\"jxbrs\":\"92\"},{\"kcrwdm\":\"1060200\",\"pkrs\":\"92\",\"jxbdm\":\"1075088\",\"kcptdm\":\"105523265\",\"xmmc\":\"星期一晚,9-10节,2-513\",\"kcdm\":\"201248\",\"kcmc\":\"物理学与人类文明\",\"rwdm\":\"1039252\",\"xbyqdm\":\"\",\"rs1\":\"\",\"rs2\":\"\",\"wyfjdm\":\"\",\"wyyzdm\":\"\",\"kkxqdm\":\"1\",\"zxs\":\"32\",\"xf\":\"2\",\"kcflmc\":\"通识教育课\",\"teaxm\":\"陈贵楚\",\"jxbrs\":\"92\"},{\"kcrwdm\":\"1060201\",\"pkrs\":\"92\",\"jxbdm\":\"1075089\",\"kcptdm\":\"105523265\",\"xmmc\":\"星期二晚,9-10节,2-513\",\"kcdm\":\"201248\",\"kcmc\":\"物理学与人类文明\",\"rwdm\":\"1039252\",\"xbyqdm\":\"\",\"rs1\":\"\",\"rs2\":\"\",\"wyfjdm\":\"\",\"wyyzdm\":\"\",\"kkxqdm\":\"1\",\"zxs\":\"32\",\"xf\":\"2\",\"kcflmc\":\"通识教育课\",\"teaxm\":\"李晓霞\",\"jxbrs\":\"92\"},{\"kcrwdm\":\"1060202\",\"pkrs\":\"92\",\"jxbdm\":\"1075090\",\"kcptdm\":\"105523265\",\"xmmc\":\"星期三晚,9-10节,2-513\",\"kcdm\":\"201248\",\"kcmc\":\"物理学与人类文明\",\"rwdm\":\"1039252\",\"xbyqdm\":\"\",\"rs1\":\"\",\"rs2\":\"\",\"wyfjdm\":\"\",\"wyyzdm\":\"\",\"kkxqdm\":\"1\",\"zxs\":\"32\",\"xf\":\"2\",\"kcflmc\":\"通识教育课\",\"teaxm\":\"李晓霞\",\"jxbrs\":\"92\"},{\"kcrwdm\":\"1060203\",\"pkrs\":\"92\",\"jxbdm\":\"1075091\",\"kcptdm\":\"16336\",\"xmmc\":\"星期三晚,9-10节,2-415\",\"kcdm\":\"1003663\",\"kcmc\":\"运筹智慧与社会生活\",\"rwdm\":\"1039272\",\"xbyqdm\":\"\",\"rs1\":\"\",\"rs2\":\"\",\"wyfjdm\":\"\",\"wyyzdm\":\"\",\"kkxqdm\":\"1\",\"zxs\":\"32\",\"xf\":\"2\",\"kcflmc\":\"通识教育课\",\"teaxm\":\"乔友付\",\"jxbrs\":\"10\"},{\"kcrwdm\":\"1060204\",\"pkrs\":\"92\",\"jxbdm\":\"1075092\",\"kcptdm\":\"16336\",\"xmmc\":\"星期四晚,9-10节,2-415\",\"kcdm\":\"1003663\",\"kcmc\":\"运筹智慧与社会生活\",\"rwdm\":\"1039272\",\"xbyqdm\":\"\",\"rs1\":\"\",\"rs2\":\"\",\"wyfjdm\":\"\",\"wyyzdm\":\"\",\"kkxqdm\":\"1\",\"zxs\":\"32\",\"xf\":\"2\",\"kcflmc\":\"通识教育课\",\"teaxm\":\"乔友付\",\"jxbrs\":\"15\"},{\"kcrwdm\":\"1060206\",\"pkrs\":\"92\",\"jxbdm\":\"1075094\",\"kcptdm\":\"105523267\",\"xmmc\":\"星期一晚,9-10节,2-515\",\"kcdm\":\"201234\",\"kcmc\":\"通信技术与社会进步\",\"rwdm\":\"1039259\",\"xbyqdm\":\"\",\"rs1\":\"\",\"rs2\":\"\",\"wyfjdm\":\"\",\"wyyzdm\":\"\",\"kkxqdm\":\"1\",\"zxs\":\"32\",\"xf\":\"2\",\"kcflmc\":\"通识教育课\",\"teaxm\":\"戴宏跃\",\"jxbrs\":\"92\"},{\"kcrwdm\":\"1060207\",\"pkrs\":\"92\",\"jxbdm\":\"1075095\",\"kcptdm\":\"105523267\",\"xmmc\":\"星期二晚,9-10节,2-515\",\"kcdm\":\"201234\",\"kcmc\":\"通信技术与社会进步\",\"rwdm\":\"1039259\",\"xbyqdm\":\"\",\"rs1\":\"\",\"rs2\":\"\",\"wyfjdm\":\"\",\"wyyzdm\":\"\",\"kkxqdm\":\"1\",\"zxs\":\"32\",\"xf\":\"2\",\"kcflmc\":\"通识教育课\",\"teaxm\":\"戴宏跃\",\"jxbrs\":\"92\"},{\"kcrwdm\":\"1060208\",\"pkrs\":\"92\",\"jxbdm\":\"1075096\",\"kcptdm\":\"105523267\",\"xmmc\":\"星期三晚,9-10节,2-515\",\"kcdm\":\"201234\",\"kcmc\":\"通信技术与社会进步\",\"rwdm\":\"1039259\",\"xbyqdm\":\"\",\"rs1\":\"\",\"rs2\":\"\",\"wyfjdm\":\"\",\"wyyzdm\":\"\",\"kkxqdm\":\"1\",\"zxs\":\"32\",\"xf\":\"2\",\"kcflmc\":\"通识教育课\",\"teaxm\":\"肖立梅\",\"jxbrs\":\"92\"},{\"kcrwdm\":\"1060209\",\"pkrs\":\"92\",\"jxbdm\":\"1075097\",\"kcptdm\":\"105523268\",\"xmmc\":\"星期一晚,9-10节,2-402\",\"kcdm\":\"201123\",\"kcmc\":\"经济学智慧\",\"rwdm\":\"1039237\",\"xbyqdm\":\"\",\"rs1\":\"\",\"rs2\":\"\",\"wyfjdm\":\"\",\"wyyzdm\":\"\",\"kkxqdm\":\"1\",\"zxs\":\"32\",\"xf\":\"2\",\"kcflmc\":\"通识教育课\",\"teaxm\":\"丁孝智\",\"jxbrs\":\"92\"},{\"kcrwdm\":\"1060210\",\"pkrs\":\"92\",\"jxbdm\":\"1075098\",\"kcptdm\":\"105523268\",\"xmmc\":\"星期二晚,9-10节,2-402\",\"kcdm\":\"201123\",\"kcmc\":\"经济学智慧\",\"rwdm\":\"1039237\",\"xbyqdm\":\"\",\"rs1\":\"\",\"rs2\":\"\",\"wyfjdm\":\"\",\"wyyzdm\":\"\",\"kkxqdm\":\"1\",\"zxs\":\"32\",\"xf\":\"2\",\"kcflmc\":\"通识教育课\",\"teaxm\":\"付华英\",\"jxbrs\":\"92\"},{\"kcrwdm\":\"1060211\",\"pkrs\":\"92\",\"jxbdm\":\"1075099\",\"kcptdm\":\"105523268\",\"xmmc\":\"星期四晚,9-10节,2-402\",\"kcdm\":\"201123\",\"kcmc\":\"经济学智慧\",\"rwdm\":\"1039237\",\"xbyqdm\":\"\",\"rs1\":\"\",\"rs2\":\"\",\"wyfjdm\":\"\",\"wyyzdm\":\"\",\"kkxqdm\":\"1\",\"zxs\":\"32\",\"xf\":\"2\",\"kcflmc\":\"通识教育课\",\"teaxm\":\"付华英\",\"jxbrs\":\"92\"},{\"kcrwdm\":\"1060212\",\"pkrs\":\"92\",\"jxbdm\":\"1075100\",\"kcptdm\":\"105523268\",\"xmmc\":\"星期四晚,9-10节,2-414\",\"kcdm\":\"201123\",\"kcmc\":\"经济学智慧\",\"rwdm\":\"1039237\",\"xbyqdm\":\"\",\"rs1\":\"\",\"rs2\":\"\",\"wyfjdm\":\"\",\"wyyzdm\":\"\",\"kkxqdm\":\"1\",\"zxs\":\"32\",\"xf\":\"2\",\"kcflmc\":\"通识教育课\",\"teaxm\":\"刘浏\",\"jxbrs\":\"92\"},{\"kcrwdm\":\"1060217\",\"pkrs\":\"92\",\"jxbdm\":\"1075105\",\"kcptdm\":\"13351\",\"xmmc\":\"星期一晚,9-10节,3-408\",\"kcdm\":\"1001692\",\"kcmc\":\"当代世界政治经济与国际关系\",\"rwdm\":\"1039221\",\"xbyqdm\":\"\",\"rs1\":\"\",\"rs2\":\"\",\"wyfjdm\":\"\",\"wyyzdm\":\"\",\"kkxqdm\":\"1\",\"zxs\":\"32\",\"xf\":\"2\",\"kcflmc\":\"通识教育课\",\"teaxm\":\"韩月香\",\"jxbrs\":\"92\"},{\"kcrwdm\":\"1060218\",\"pkrs\":\"92\",\"jxbdm\":\"1075106\",\"kcptdm\":\"13351\",\"xmmc\":\"星期二晚,9-10节,3-408\",\"kcdm\":\"1001692\",\"kcmc\":\"当代世界政治经济与国际关系\",\"rwdm\":\"1039221\",\"xbyqdm\":\"\",\"rs1\":\"\",\"rs2\":\"\",\"wyfjdm\":\"\",\"wyyzdm\":\"\",\"kkxqdm\":\"1\",\"zxs\":\"32\",\"xf\":\"2\",\"kcflmc\":\"通识教育课\",\"teaxm\":\"韩月香\",\"jxbrs\":\"92\"},{\"kcrwdm\":\"1060219\",\"pkrs\":\"92\",\"jxbdm\":\"1075107\",\"kcptdm\":\"13351\",\"xmmc\":\"星期三晚,9-10节,3-408\",\"kcdm\":\"1001692\",\"kcmc\":\"当代世界政治经济与国际关系\",\"rwdm\":\"1039221\",\"xbyqdm\":\"\",\"rs1\":\"\",\"rs2\":\"\",\"wyfjdm\":\"\",\"wyyzdm\":\"\",\"kkxqdm\":\"1\",\"zxs\":\"32\",\"xf\":\"2\",\"kcflmc\":\"通识教育课\",\"teaxm\":\"徐秋涛\",\"jxbrs\":\"23\"},{\"kcrwdm\":\"1060220\",\"pkrs\":\"92\",\"jxbdm\":\"1075108\",\"kcptdm\":\"13351\",\"xmmc\":\"星期四晚,9-10节,3-408\",\"kcdm\":\"1001692\",\"kcmc\":\"当代世界政治经济与国际关系\",\"rwdm\":\"1039221\",\"xbyqdm\":\"\",\"rs1\":\"\",\"rs2\":\"\",\"wyfjdm\":\"\",\"wyyzdm\":\"\",\"kkxqdm\":\"1\",\"zxs\":\"32\",\"xf\":\"2\",\"kcflmc\":\"通识教育课\",\"teaxm\":\"吴婷\",\"jxbrs\":\"10\"},{\"kcrwdm\":\"1060221\",\"pkrs\":\"91\",\"jxbdm\":\"1075109\",\"kcptdm\":\"105523270\",\"xmmc\":\"星期一晚,9-10节,2-306\",\"kcdm\":\"201141\",\"kcmc\":\"两性文化与两性关系\",\"rwdm\":\"1039232\",\"xbyqdm\":\"\",\"rs1\":\"\",\"rs2\":\"\",\"wyfjdm\":\"\",\"wyyzdm\":\"\",\"kkxqdm\":\"1\",\"zxs\":\"32\",\"xf\":\"2\",\"kcflmc\":\"通识教育课\",\"teaxm\":\"赖翅萍\",\"jxbrs\":\"91\"},{\"kcrwdm\":\"1060222\",\"pkrs\":\"92\",\"jxbdm\":\"1075110\",\"kcptdm\":\"105523270\",\"xmmc\":\"星期二晚,9-10节,2-306\",\"kcdm\":\"201141\",\"kcmc\":\"两性文化与两性关系\",\"rwdm\":\"1039232\",\"xbyqdm\":\"\",\"rs1\":\"\",\"rs2\":\"\",\"wyfjdm\":\"\",\"wyyzdm\":\"\",\"kkxqdm\":\"1\",\"zxs\":\"32\",\"xf\":\"2\",\"kcflmc\":\"通识教育课\",\"teaxm\":\"吴丹凤\",\"jxbrs\":\"92\"}]}";
            string jsonStr = Send(Parameter.eduSysUrl + "xsxklist!getDataList.action", "POST", "page=1&rows=200&sort=kcrwdm&order=asc", Parameter.eduSysUrl + "xsxklist!xsmhxsxk.action");
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            dynamic json;
            try
            {
                json = serializer.Deserialize<dynamic>(jsonStr);
            }
            catch (Exception)
            {
                return null;
            }
            foreach (var course in json["rows"])
            {
                data = new string[4];
                // 可能存在某一门课程不完整，键不存在捕获异常即可
                try
                {
                    data[0] = course["kcmc"];
                    data[1] = course["teaxm"];
                    data[2] = course["xmmc"];
                    data[3] = course["kcrwdm"];
                    courses.Add(data);
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
            }
            return courses;
        }
        
        //发送请求
        public string Send(string URL, string method, string data, string referer = "")
        {
            string json;
            request = WebRequest.Create(URL) as HttpWebRequest;
            request.Method = method;
            request.KeepAlive = true;
            request.CookieContainer = cookies;
            request.ServicePoint.Expect100Continue = false;
            if (referer != "")
                request.Referer = referer;
            byte[] sendData = new UTF8Encoding().GetBytes(data);
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = sendData.Length;
            try
            {
                Stream stream = request.GetRequestStream();
                stream.Write(sendData, 0, sendData.Length);
                response = request.GetResponse() as HttpWebResponse;
                StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                json = reader.ReadToEnd();
                reader.Close();
                response.Close();
                stream.Close();
            }
            catch (Exception)
            {                
                return "error";
            }
            finally
            {
                request.Abort();
            }
            return json;
        }

        //重载Send函数 用于多线程调用抢课
        public void Send(object p)
        {
            string temp = p.ToString();
            string json;
            HttpWebRequest request = WebRequest.Create(Parameter.eduSysUrl + "xsxklist!getAdd.action") as HttpWebRequest;
            request.Method = "POST";
            request.KeepAlive = true;
            request.CookieContainer = cookies;
            request.ServicePoint.Expect100Continue = false;
            request.Referer = Parameter.eduSysUrl + "xsxklist!xsmhxsxk.action";
            byte[] sendData = new UTF8Encoding().GetBytes(p.ToString());
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = sendData.Length;
            try
            {
                Stream stream = request.GetRequestStream();
                stream.Write(sendData, 0, sendData.Length);
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                // 读取返回结果
                json = reader.ReadToEnd();
                stream.Close();
                response.Close();
                reader.Close();
                if (json == "1")
                {
                    json = "抢课成功！";
                    //抢课信息
                    MessageBox.Show(HttpUtility.UrlDecode(temp.Substring(temp.IndexOf("%"))) + json, "恭喜抢课成功", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button3, MessageBoxOptions.ServiceNotification);
                    return;
                }
                //else if (json.IndexOf("选课人数超出，请选其他课程") != -1)
                //{
                //    //抢课信息
                //    MessageBox.Show("\"" + HttpUtility.UrlDecode(temp.Substring(temp.IndexOf("%"))) + "\" 选课失败，" + json, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);
                //    return;
                //}
                else
                {
                    // 等待500毫秒
                    Thread.Sleep(500);
                    //失败重试
                    Send(p);                                                                                                                                                                                                                                                                                                                                                               
                }
            }
            catch (WebException err)
            {
                Debug.WriteLine(err.Message);
                // 等待一秒
                Thread.Sleep(1000);
                //异常重试
                Send(p);
            }
            finally
            {
                request.Abort();
            }
        }
    }
}
