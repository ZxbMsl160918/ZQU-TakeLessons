using System.Dynamic;
using System.Threading;
using System.Windows.Forms;

namespace TakeLessons
{
    public class LoadingHelper
    {
        FrmLoading loadingForm;

        /// <summary>
        /// 开始加载
        /// </summary>
        /// <param name="message">消息</param>
        /// <param name="ownerForm">父窗体</param>
        /// <param name="work">待执行工作</param>
        /// <param name="workArg">工作参数</param>
       
        public  void ShowLoading(string message, Form ownerForm, ParameterizedThreadStart work, object workArg = null)
        {
            loadingForm = new FrmLoading(message);
            dynamic expandoObject = new ExpandoObject();
            expandoObject.Form = loadingForm;
            expandoObject.WorkArg = workArg;
            loadingForm.SetWorkAction(work, expandoObject);
            loadingForm.ShowDialog(ownerForm);
            if (loadingForm.WorkException != null)
            {
                throw loadingForm.WorkException;
            }
        }
        public void SetMessage(string msg)
        {
            loadingForm.SetMessage(msg);
        }
    }
}
