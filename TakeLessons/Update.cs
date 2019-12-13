using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Security.Principal;

namespace TakeLessons
{
    public class Update
    {
        WebClient _client;
        public Update()
        {
            _client = new WebClient();
            _client.DownloadFileCompleted += (sender, args) => { DownloadFileCompleted?.Invoke(sender, args); };
            _client.DownloadProgressChanged += (sender, args) => { DownloadProgressChanged?.Invoke(sender, args); };
        }
        /// <summary>
        /// 在异步文件下载操作完成时发生。
        /// </summary>
        public Action<object, AsyncCompletedEventArgs> DownloadFileCompleted { get; set; }
        /// <summary>
        /// 在异步下载操作成功转换部分或全部数据后发生
        /// </summary>
        public Action<object, DownloadProgressChangedEventArgs> DownloadProgressChanged { get; set; }
        /// <summary>
        /// 根据名字 关闭进程
        /// </summary>
        /// <param name="ProcessName"></param>
        /// <returns></returns>
        public bool CloseProcess(string ProcessName)
        {
            bool result = false;
            var temp = System.Diagnostics.Process.GetProcessesByName(ProcessName);
            foreach (var item in temp)
            {
                try
                {
                    item.Kill();
                    result = true;
                }
                catch
                {
                }
            }
            return result;
        }

        /// <summary>
        /// 异步下载文件
        /// </summary>
        /// <param name="url">文件下载路径</param>
        /// <param name="fileName">文件名</param>
        /// <param name="savePath">保存路径，如为空默认当前路径</param>
        public void Download(String url, String fileName, String savePath = "")
        {
            if (savePath == "")
                savePath = AppDomain.CurrentDomain.BaseDirectory;
            _client.DownloadFileAsync(new Uri(url), savePath + fileName);
        }
    }
}
