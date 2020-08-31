using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace IdentityServerCenter.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class FilesController : ControllerBase
    {
        private readonly IConfiguration configuration;

        public FilesController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        /// <summary>
        /// 获取头像
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Get([FromQuery]string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return NotFound();
            }

            var saveDir = configuration["AvatarSavePath"];
            return PhysicalFile(System.IO.Path.Combine(saveDir, name), "application/octet-stream", name);
        }
    }
}
