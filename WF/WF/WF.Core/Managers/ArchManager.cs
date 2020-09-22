using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WF.Core.Data;
using WF.Core.Models;

namespace WF.Core.Managers
{
    public class ArchManager<TArch> where TArch : Arch
    {
        private readonly ArchDbContext<TArch> archDbContext;

        public ArchManager(ArchDbContext<TArch> archDbContext)
        {
            this.archDbContext = archDbContext;
        }

        /// <summary>
        /// 根据业务key获取arch
        /// </summary>
        /// <param name="businessKey"></param>
        /// <returns></returns>
        public virtual async Task<TArch> GetArchByBusinessKeyAsync(string businessKey)
        {
            TArch arch = await archDbContext.Arches.FirstOrDefaultAsync(e => e.BusinessKey == businessKey);
            return arch;
        }

        public static string SaveDirectory = "C:/保存文件";

        /// <summary>
        /// 添加附件
        /// </summary>
        /// <param name="arch"></param>
        /// <param name="formFile"></param>
        /// <returns></returns>
        public virtual async Task<ArchAttachment> AddArchAttachmentAsync(TArch arch, IFormFile formFile)
        {
            var fileName = $"{DateTimeOffset.Now.ToString("yyyyMMddHHssmm")}{Guid.NewGuid().ToString("N")}";
            var savePath = System.IO.Path.Combine(SaveDirectory, fileName);

            //先保存到物理磁盘，再进行数据库操作
            using (FileStream stream = new FileStream(savePath, FileMode.Create))
            {
                await formFile.CopyToAsync(stream);
            }

            ArchAttachment attachment = new ArchAttachment
            {
                BusinessKey = arch.BusinessKey,
                CreateDateTime = DateTimeOffset.Now,
                LastUploadDateTime = DateTimeOffset.Now,
                OrignFileName = formFile.FileName,
                ContentType = formFile.ContentType,
                FileName = fileName
            };

            await archDbContext.ArchAttachments.AddAsync(attachment);
            await archDbContext.SaveChangesAsync();

            return attachment;
        }

        /// <summary>
        /// 删除附件
        /// </summary>
        /// <param name="arch"></param>
        /// <param name=""></param>
        /// <returns></returns>
        public virtual async Task DeleteArchAttachments(TArch arch, List<int> attachmentIds)
        {
            //先删除数据库，再删物理磁盘
            var attachments = archDbContext.ArchAttachments.Where(e => attachmentIds.Contains(e.Id));
            archDbContext.ArchAttachments.RemoveRange(attachments);
            await archDbContext.SaveChangesAsync();

            foreach (var item in attachments)
            {
                var savePath = System.IO.Path.Combine(SaveDirectory, item.FileName);
                File.Delete(savePath);
            }
        }

        /// <summary>
        /// 获取附件
        /// </summary>
        /// <param name="arch"></param>
        /// <returns></returns>
        public virtual async Task<List<ArchAttachment>> GetArchAttachmentsAsync(TArch arch)
        {
            if(arch == null || string.IsNullOrEmpty(arch.BusinessKey))
            {
                return null;
            }

            var attachments = await archDbContext.ArchAttachments.Where(e => arch.BusinessKey == e.BusinessKey).ToListAsync();
            return attachments;
        }
    }
}
