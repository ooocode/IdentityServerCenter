using AutoMapper;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.EntityFramework.Interfaces;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using IdentityServerCenter.Identity;
using IdentityServerCenter.Identity.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServerCenter.Services.ClientService
{
    /// <summary>
    /// 客户端服务
    /// </summary>
    public class ClientService : ServiceBase, IClientService
    {
        private readonly ConfigurationDbContext configurationDbContext;
        private readonly IMapper mapper;

        public ClientService(ConfigurationDbContext configurationDbContext,
            IMapper mapper,
            IIdentityServerInteractionService identityServerInteractionService, IClientStore clientStore)
        {
            this.configurationDbContext = configurationDbContext ?? throw new ArgumentNullException(nameof(configurationDbContext));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        private IQueryable<Client> Clients
        {
            get
            {
                var clients = configurationDbContext.Clients
                .Include(e => e.AllowedGrantTypes)
                .Include(e => e.RedirectUris)
                .Include(e => e.AllowedScopes)
                .Include(e => e.PostLogoutRedirectUris);

                return clients;
            }
        }


        /// <summary>
        /// 获取示例grantType
        /// </summary>
        /// <returns></returns>
        public List<ExampleGrantType> GetExampleGrantTypes()
        {
            return new List<ExampleGrantType>
            {
                new ExampleGrantType(nameof(IdentityServer4.Models.GrantTypes.Implicit),IdentityServer4.Models.GrantTypes.Implicit),
                new ExampleGrantType(nameof( IdentityServer4.Models.GrantTypes.ImplicitAndClientCredentials),IdentityServer4.Models.GrantTypes.ImplicitAndClientCredentials),
                new ExampleGrantType(nameof(IdentityServer4.Models.GrantTypes.Code),IdentityServer4.Models.GrantTypes.Code),
                new ExampleGrantType(nameof( IdentityServer4.Models.GrantTypes.CodeAndClientCredentials), IdentityServer4.Models.GrantTypes.CodeAndClientCredentials),
                new ExampleGrantType(nameof(IdentityServer4.Models.GrantTypes.Hybrid),IdentityServer4.Models.GrantTypes.Hybrid),
                new ExampleGrantType( nameof(IdentityServer4.Models.GrantTypes.HybridAndClientCredentials),IdentityServer4.Models.GrantTypes.HybridAndClientCredentials),
                new ExampleGrantType(nameof(IdentityServer4.Models.GrantTypes.ClientCredentials),IdentityServer4.Models.GrantTypes.ClientCredentials),
                new ExampleGrantType(nameof(IdentityServer4.Models.GrantTypes.ResourceOwnerPassword),IdentityServer4.Models.GrantTypes.ResourceOwnerPassword),
                new ExampleGrantType(nameof(IdentityServer4.Models.GrantTypes.ResourceOwnerPasswordAndClientCredentials),IdentityServer4.Models.GrantTypes.ResourceOwnerPasswordAndClientCredentials),
                new ExampleGrantType(nameof(IdentityServer4.Models.GrantTypes.DeviceFlow),IdentityServer4.Models.GrantTypes.DeviceFlow)
            };
        }

        /// <summary>
        /// 获取客户端列表
        /// </summary>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <param name="search"></param>
        /// <returns></returns>
        public async Task<PaginationResult<ClientDto>> GetClientsAsync(int? skip, int? take, string search)
        {
            IQueryable<Client> clients = Clients;
            if (!string.IsNullOrEmpty(search))
            {
                search = search.Trim();
                clients = clients.Where(e => e.ClientId.Contains(search)
                || e.ClientName.Contains(search)
                || e.Description.Contains(search));
            }

            int total = await clients.CountAsync().ConfigureAwait(false);
            skip ??= 0;
            take ??= 10;

            skip = skip < 0 ? 0 : skip;
            take = take < 0 ? 0 : take;

            List<Client> ls = await clients.Skip(skip.Value).Take(take.Value).ToListAsync().ConfigureAwait(false);

            List<ClientDto> result = new List<ClientDto>();
            foreach (var client in ls)
            {
                var model = client.ToModel();
                ClientDto clientDto = mapper.Map<ClientDto>(model);
                clientDto.Id = client.Id;
                result.Add(clientDto);
            }

            return new PaginationResult<ClientDto>
            {
                Succeeded = true,
                Total = total,
                Rows = result,
                PageSize = take.Value
            };
        }


        /// <summary>
        /// 根据主键id获取客户端
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ClientDto> GetClientByIdAsync(int id)
        {
            var entity = await Clients.FirstOrDefaultAsync(e => e.Id == id).ConfigureAwait(false);
            if (entity == null)
            {
                return null;
            }

            var model = entity.ToModel();
            ClientDto dto = mapper.Map<ClientDto>(model);
            dto.Id = entity.Id;
            return dto;
        }


        /// <summary>
        /// 获取所有可用作用域
        /// </summary>
        /// <returns></returns>
        public async Task<List<EnabledScopeDto>> GetAllEnabledScopesAsync()
        {
            var identityResourceNames = await configurationDbContext.IdentityResources
                .Where(e => e.Enabled)
                .Select(e => new EnabledScopeDto
                {
                    Name = e.Name,
                    Description = e.Description,
                    DisplayName = e.DisplayName,
                    Emphasize = e.Emphasize,
                    Required = e.Required,
                    ShowInDiscoveryDocument = e.ShowInDiscoveryDocument,
                    TypeTag = "身份"
                }).ToListAsync()
                .ConfigureAwait(false);

            var apiScopeNames = await configurationDbContext.ApiScopes
                .Where(e => e.Enabled)
                .Select(e => new EnabledScopeDto
                {
                    Name = e.Name,
                    Description = e.Description,
                    DisplayName = e.DisplayName,
                    Emphasize = e.Emphasize,
                    Required = e.Required,
                    ShowInDiscoveryDocument = e.ShowInDiscoveryDocument,
                    TypeTag = "API"
                })
                .ToListAsync().ConfigureAwait(false);
            identityResourceNames.AddRange(apiScopeNames);
            return identityResourceNames;
        }

        /// <summary>
        /// 删除客户端
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Result> DeleteClientAsync(int id)
        {
            var client = await configurationDbContext.Clients.FirstOrDefaultAsync(e => e.Id == id).ConfigureAwait(false);
            if (client == null)
            {
                return FailedResult("客户端不存在");
            }

            configurationDbContext.Clients.Remove(client);
            try
            {
                await configurationDbContext.SaveChangesAsync().ConfigureAwait(false);
                return OkResult();
            }
            catch (DbUpdateException ex)
            {
                return FailedResult(ex);
            }
        }

        /// <summary>
        /// 添加或者更新客户端
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<Result> CreateOrUpdateClientAsync(ClientDto dto)
        {
            //if(!GetExampleGrantTypes().Any(e=>e.GrantType == dto.AllowedGrantTypes))
            //{
            //    return Failed($"无效的grantType【{dto.AllowedGrantTypes}】");
            //}


            IdentityServer4.Models.Client model = mapper.Map<IdentityServer4.Models.Client>(dto);
            var entity = model.ToEntity();

            if (!dto.Id.HasValue || dto.Id == 0)
            {
                //创建
                await configurationDbContext.Clients.AddAsync(entity).ConfigureAwait(false);
            }
            else
            {
                entity.Id = dto.Id.Value;

                Client client = await Clients.FirstOrDefaultAsync(e => e.Id == dto.Id).ConfigureAwait(false);
                if (client == null)
                {
                    return FailedResult($"客户端[{dto.ClientId}]不存在");
                }


                client = mapper.Map(entity, client);
            }

            try
            {
                int row = await configurationDbContext.SaveChangesAsync().ConfigureAwait(false);
                if (row > 0)
                {
                    return OkResult();
                }

                return FailedResult("已执行，但未更新数据");
            }
            catch (DbUpdateException ex)
            {
                return FailedResult(ex);
            }

            //如果Id有值，先删除
            //if (dto.Id.HasValue && dto.Id > 0)
            //{
            //    //先删除
            //    var existClient = await configurationDbContext.Clients.FirstOrDefaultAsync(e => e.ClientId == dto.ClientId).ConfigureAwait(false);
            //    if (existClient != null)
            //    {
            //        configurationDbContext.Clients.Remove(existClient);
            //    }
            //}


            //IdentityServer4.Models.Client client = new IdentityServer4.Models.Client
            //{
            //    ClientName = dto.ClientName,
            //    Description = dto.Description,
            //    ClientId = dto.ClientId,
            //    Enabled = true,
            //    RequireClientSecret = false,//不需要密码
            //    AllowOfflineAccess = true, //允许离线访问
            //    AllowAccessTokensViaBrowser = true,  //允许令牌通过浏览器

            //    AlwaysSendClientClaims = true,//允许发送客户端声明
            //    AlwaysIncludeUserClaimsInIdToken = false, //允许userClaims 包含在 token 中

            //    //授权确认同意页面，false则跳过
            //    RequireConsent = dto.RequireConsent,

            //    RequirePkce = false,

            //    RedirectUris = dto.RedirectUris.Split(';')?.ToList(), //登录后重定位路径
            //    PostLogoutRedirectUris = dto.PostLogoutRedirectUris.Split(';')?.ToList(), //后端注销url
            //    FrontChannelLogoutUri = dto.FrontChannelLogoutUri,  //前端频道注销uri

            //    AllowedGrantTypes = IdentityServer4.Models.GrantTypes.Implicit, //添加简化模式授权

            //    AllowedScopes = dto.Scopes.Split(';')?.ToList()   //添加允许的作用域
            //};
            //var entity = client.ToEntity();
            //await configurationDbContext.Clients.AddAsync(entity).ConfigureAwait(false);

            //DataResult<int?> result = new DataResult<int?>();

            //try
            //{
            //    var row = await configurationDbContext.SaveChangesAsync().ConfigureAwait(false);
            //    if (row > 0)
            //    {
            //        var id = (await configurationDbContext.Clients.FirstOrDefaultAsync(e => e.ClientId == dto.ClientId).ConfigureAwait(false))?.Id;
            //        result.Succeeded = true;
            //        result.Data = id;
            //    }
            //}
            //catch (Exception ex)
            //{
            //    result.ErrorMessage = ex.InnerException?.Message ?? ex.Message;
            //}

            //return result;
        }
    }
}
