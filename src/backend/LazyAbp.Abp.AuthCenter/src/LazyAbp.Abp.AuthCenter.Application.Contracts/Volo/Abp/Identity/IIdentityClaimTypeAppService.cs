using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Identity.Dtos;

namespace Volo.Abp.Identity
{
    public interface IIdentityClaimTypeAppService :
        ICrudAppService<ClaimTypeDto, Guid, GetIdentityClaimTypesInput, CreateClaimTypeDto, UpdateClaimTypeDto>
    {
        Task<List<ClaimTypeDto>> GetAllListAsync();
    }
}
