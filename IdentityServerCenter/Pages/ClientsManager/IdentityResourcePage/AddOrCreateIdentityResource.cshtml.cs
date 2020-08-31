using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace IdentityServerCenter.Pages.ClientsManager.IdentityResourcePage
{
    public class AddOrCreateIdentityResourceModel : PageModel
    {
        private readonly ConfigurationDbContext configurationDbContext;

        [FromRoute]
        public int? Id  { get; set; }

        public CreateOrUpdateIdentityResourceViewModel ViewModel  { get; set; }

        public AddOrCreateIdentityResourceModel(ConfigurationDbContext configurationDbContext)
        {
            this.configurationDbContext = configurationDbContext ?? throw new ArgumentNullException(nameof(configurationDbContext));
        }

        public async Task<IActionResult> OnGetAsync()
        {
            if (Id.HasValue)
            {
                var identityResource = await configurationDbContext.IdentityResources.FirstOrDefaultAsync(e => e.Id == Id).ConfigureAwait(false);
                if(identityResource == null)
                {
                    return NotFound();
                }

                ViewModel = new CreateOrUpdateIdentityResourceViewModel
                {
                    Id = identityResource.Id,
                    Description = identityResource.Description,
                    DisplayName = identityResource.DisplayName,
                    Name = identityResource.Name
                };
            }
            else
            {
                ViewModel = new CreateOrUpdateIdentityResourceViewModel();
            }
            return Page();
        }

        /// <summary>
        /// POST 创建或者更新
        /// </summary>
        /// <param name="ViewModel"></param>
        /// <returns></returns>
        public async Task<IActionResult> OnPostAsync(CreateOrUpdateIdentityResourceViewModel ViewModel)
        {
            if (ViewModel is null)
            {
                throw new ArgumentNullException(nameof(ViewModel));
            }

            if (!TryValidateModel(ViewModel))
            {
                return BadRequest(ModelState);
            }

            if (ViewModel.Id.HasValue)
            {
                await UpdateIdentityResourceAsync(ViewModel).ConfigureAwait(false);
            }
            else
            {
                await CreateIdentityResourceAsync(ViewModel).ConfigureAwait(false);
            }

            try
            {
                await configurationDbContext.SaveChangesAsync().ConfigureAwait(false);
            }
            catch (DbUpdateException ex)
            {
                ModelState.AddModelError(string.Empty, ex.InnerException?.Message ?? ex.Message);
                return BadRequest(ModelState);
            }

            return new OkResult();
        }


        private async Task CreateIdentityResourceAsync(CreateOrUpdateIdentityResourceViewModel model)
        {
            if (model is null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            var resource = new IdentityServer4.EntityFramework.Entities.IdentityResource
            {
                Name = model.Name,
                Description = model.Description,
                DisplayName = model.DisplayName,
                ShowInDiscoveryDocument = true,
                Enabled = true,
            };

            await configurationDbContext.IdentityResources.AddAsync(resource).ConfigureAwait(false);
        }
        
        private async Task UpdateIdentityResourceAsync(CreateOrUpdateIdentityResourceViewModel model)
        {
            if (model is null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (!model.Id.HasValue)
            {
                throw new ArgumentNullException(nameof(model.Id));
            }

            var resource = await configurationDbContext.IdentityResources.FirstOrDefaultAsync(e => e.Id == model.Id.Value).ConfigureAwait(false);
            if(resource == null)
            {
                throw new Exception("身份资源不存在");
            }

            resource.Name = model.Name;
            resource.Description = model.Description;
            resource.DisplayName = model.DisplayName;
        }

        /// <summary>
        /// 创建或更新身份资源视图模型
        /// </summary>
        public class CreateOrUpdateIdentityResourceViewModel
        {
            public int? Id  { get; set; }

            //public int Id { get; set; }
            //public bool Enabled { get; set; }
            [Required(ErrorMessage = "{0}不能为空")]
            [DisplayName("(必填)身份资源名称")]
            public string Name { get; set; }


            [DisplayName("显示名称")]
            public string DisplayName { get; set; }


            [DisplayName("描述")]
            public string Description { get; set; }


            //public bool Required { get; set; }

            /// <summary>
            /// 强调
            /// </summary>
            // public bool Emphasize { get; set; }

            /// <summary>
            /// 显示在发现文档
            /// </summary>
            /// 
            //public bool ShowInDiscoveryDocument { get; set; }

            //public List<IdentityClaim> UserClaims { get; set; }
            //public List<IdentityResourceProperty> Properties { get; set; }
            //public DateTime Created { get; set; }
            //public DateTime? Updated { get; set; }
            //public bool NonEditable { get; set; }
        }
    }
}