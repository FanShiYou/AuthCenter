using EasyAbp.IdentityServerAdmin.IdentityResources.Dtos;
using EasyAbp.IdentityServerAdmin.Permissions;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.IdentityServer.IdentityResources;

namespace EasyAbp.IdentityServerAdmin.IdentityResources
{
    [RemoteService(false)]
    [Authorize(IdentityServerAdminPermissions.IdentityResource.Default)]
    public class IdentityResourceAppService : ReadOnlyAppService<IdentityResource, IdentityResourceDto, Guid, GetIdentityResourceListInputDto>, IIdentityResourceAppService
    {
        private readonly IIdentityResourceRepository _repository;

        public IdentityResourceAppService(IIdentityResourceRepository repository) : base((IReadOnlyRepository<IdentityResource, Guid>)repository)
        {
            _repository = repository;
        }

        protected override IQueryable<IdentityResource> CreateFilteredQuery(GetIdentityResourceListInputDto input)
        {
            return base.ReadOnlyRepository
                .AsQueryable()
                .WhereIf(!input.Keywords.IsNullOrWhiteSpace(),
                         e => e.Name.Contains(input.Keywords) ||
                              e.DisplayName.Contains(input.Keywords));
        }

        public override Task<PagedResultDto<IdentityResourceDto>> GetListAsync(GetIdentityResourceListInputDto input)
        {
            return base.GetListAsync(input);
        }
        public override Task<IdentityResourceDto> GetAsync(Guid id)
        {
            return base.GetAsync(id);
        }

        [Authorize(IdentityServerAdminPermissions.IdentityResource.Create)]
        public async Task<IdentityResourceDto> CreateAsync(CreateIdentityResourceInputDto input)
        {
            var nameExist = await _repository.CheckNameExistAsync(input.Name.Trim());

            if (nameExist)
            {
                throw new IdentityResourceNameNotAllowedDuplicateException(input.Name);
            }
            var identityResource = new IdentityResource(GuidGenerator.Create(), input.Name.Trim())
            {
                DisplayName = input.DisplayName?.Trim(),
                Description = input.Description?.Trim(),
                Enabled = input.Enabled,
                Required = input.Required,
                Emphasize = input.Emphasize,
                ShowInDiscoveryDocument = input.ShowInDiscoveryDocument,
                Properties = new Dictionary<string, string>()
            };
            input.UserClaims.ForEach(x => identityResource.AddUserClaim(x));

            if (input.Properties != null)
            {
                input.Properties.ForEach(x =>
                {
                    if (!identityResource.Properties.ContainsKey(x.Key))
                    {
                        identityResource.Properties.Add(x.Key, x.Value);
                    }
                });
            }

            identityResource = await _repository.InsertAsync(identityResource);
            return MapToGetOutputDto(identityResource);
        }
        [Authorize(IdentityServerAdminPermissions.IdentityResource.Update)]
        public async Task<IdentityResourceDto> UpdateAsync(Guid id, UpdateIdentityResourceInputDto input)
        {
            var nameExist = await _repository.CheckNameExistAsync(input.Name, id);

            if (nameExist)
            {
                throw new IdentityResourceNameNotAllowedDuplicateException(input.Name);
            }
            var identityResource = await _repository.FindAsync(id);
            identityResource = ObjectMapper.Map<UpdateIdentityResourceInputDto, IdentityResource>(input, identityResource);

            // userClaim
            identityResource.RemoveAllUserClaims();
            if (input.UserClaims != null)
            {
                input.UserClaims.ForEach(e =>
                {
                    identityResource.AddUserClaim(e);
                });
            }

            identityResource = await _repository.UpdateAsync(identityResource);
            return MapToGetOutputDto(identityResource);
        }
        [Authorize(IdentityServerAdminPermissions.IdentityResource.Delete)]
        public async Task DeleteAsync(Guid id)
        {
            var identityResource = await _repository.FindAsync(id, true);
            await _repository.DeleteAsync(identityResource);
        }
    }
}
