using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Homer.Authorization.Interfaces;
using Homer.Authorization.Models;
using Homer.Authorization.Requirements;
using Homer.Models.Domain;
using Homer.Models.DTO;
using Homer.Models.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace Homer.Api.Controllers
{

    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class StoresController : ControllerBase
    {
        private readonly IStoreRepository<IMongoDatabase> _repo;
        private readonly IMapper _mapper;
        private readonly IAuthorizationService _authService;


        public StoresController(IStoreRepository<IMongoDatabase> repo,IMapper mapper, IAuthorizationService authService)
        {
            _authService = authService;
            _repo = repo;
            _mapper = mapper;
        }
        [HttpGet("all")]
        [Authorize("MustBeStoreOwner")]
        public async Task<IActionResult> GetAllAsync(CancellationToken cancellationToken=default)
        {
            //var res = await _repo.GetAllAsync(cancellationToken);
            var userId = User.Claims.SingleOrDefault(x => x.Type == HomerClaimConstants.UserId);
            var company = User.Claims.SingleOrDefault(x => x.Type == HomerClaimConstants.Company);
            var propertedProperties = new ProtectedProperties { CompanyName = company.Value, OwnerId = userId.Value };
            var res = await _repo.GetByProtectedResource(propertedProperties, cancellationToken);
            var resDto = _mapper.Map<IEnumerable<StoreDto>>(res);
            return Ok(resDto);
        }
        [HttpGet("{Id}")]
        [Authorize(Policy = "MustBeStoreOwner")]
        public async Task<IActionResult> GetByIdAsync([FromRoute]string Id,CancellationToken cancellationToken=default)
        {
            if(string.IsNullOrEmpty(Id))
            {
                return BadRequest();
            }
            var res = await _repo.GetByIdAsync(Id, cancellationToken);
            //var authres = await _authService.AuthorizeAsync(User, res);
            var authres = await _authService.AuthorizeAsync(User, res, "SameCompany");
            if (authres.Failed())
            {
                return Forbid();
            }


            var resDto = _mapper.Map<StoreDto>(res);
            return Ok(resDto);
        }
        [HttpGet]
        [Authorize(Roles = HomerRoleConstants.Admin)]
        public async Task<IActionResult> GetByAddress([FromQuery(Name = "address")] string address,
                                                      [FromQuery(Name = "city")] string city,
                                                      [FromQuery(Name = "state")] string state,
                                                      [FromQuery(Name = "zip")] string zip,
                                                      CancellationToken cancellationToken = default)
        {
            var addressObj = new AddressInfo {
                Address = address,
                City = city,
                State = state,
                Zip = zip
            };
            var res = await _repo.GetByAddressAsync(addressObj, cancellationToken);
            var resDto = _mapper.Map<IEnumerable<StoreDto>>(res);
            return Ok(resDto);
        }
        [HttpPost]
        [Authorize(Policy = "MustBeStoreOwner")]
        public async Task<IActionResult> CreateAsync([FromBody] StoreDto store, CancellationToken cancellationToken= default)
        {
            if(store==null)
            {
                return BadRequest();
            }
            var data = _mapper.Map<Store>(store);
            data.BusinessHours.TryAdd("Monday", new BusinessHour { FromTime = "8:00am", ToTime = "8:00pm" });
            data.BusinessHours.TryAdd("Tuesday", new BusinessHour { FromTime = "8:00am", ToTime = "8:00pm" });
            data.BusinessHours.TryAdd("Wednesday", new BusinessHour { FromTime = "8:00am", ToTime = "8:00pm" });
            data.BusinessHours.TryAdd("Thursday", new BusinessHour { FromTime = "8:00am", ToTime = "8:00pm" });
            data.BusinessHours.TryAdd("Friday", new BusinessHour { FromTime = "8:00am", ToTime = "8:00pm" });
            data.BusinessHours.TryAdd("Saturday", new BusinessHour { FromTime = "8:00am", ToTime = "8:00pm" });


            data.OwnerId = User.Claims.SingleOrDefault(x => x.Type == HomerClaimConstants.UserId)?.Value;
            data.CompanyName = User.Claims.SingleOrDefault(x => x.Type == HomerClaimConstants.Company)?.Value;
            var res = await _repo.UpsertAsync(data, cancellationToken);
            var resDto = _mapper.Map<StoreDto>(res);
            return new CreatedResult("/api/stores",new { Id = resDto.Id });
        }
        [HttpPut]
        [Authorize(Policy = "MustBeStoreOwner")]
        public async Task<IActionResult> UpdateAsync([FromBody] StoreDto store, CancellationToken cancellationToken = default)
        {
            if(store==null)
            {
                return BadRequest();
            }
            var data = _mapper.Map<Store>(store);
            var res = await _repo.UpsertAsync(data, cancellationToken);
            var resDto = _mapper.Map<StoreDto>(res);
            return Ok(resDto);
        }
        [HttpDelete("{Id}")]
        [Authorize(Policy = "MustBeStoreOwner")]
        public async Task<IActionResult> DeleteAsync([FromRoute] string Id, CancellationToken cancellationToken=default)
        {
            if(string.IsNullOrEmpty(Id))
            {
                return BadRequest();
            }
            var store = await _repo.GetByIdAsync(Id);
            //var auth = await _authService.AuthorizeAsync(User, store);
            var auth = await _authService.AuthorizeAsync(User, store, "SameCompany");
            if(auth.Failed())
            {
                return Forbid();
            }
            var res = await _repo.DeleteAsync(Id, cancellationToken);
            if(res)
            {
                return NoContent();
            }else
            {
                return NotFound();
            }
        }
    }
}