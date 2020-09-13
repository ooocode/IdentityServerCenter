using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ManagerCenter.UserManager.Abstractions
{
    public interface IEmailSender
    {
        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="email">对方邮箱</param>
        /// <param name="subject">主题</param>
        /// <param name="htmlMessage">消息</param>
        /// <returns></returns>
        Task SendEmailAsync(string[] toEmails, string subject, string htmlMessage);
    }
}
