using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
//using System.Web.Mvc;
using Homer.Models.DTO;
using Homer.UI.Models;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Newtonsoft.Json;

namespace Homer.UI.Controllers
{
    [Authorize]
    public class StoresController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public StoresController(IHttpClientFactory httpCientFactory)
        {
            _httpClientFactory = httpCientFactory;
        }
        
        public async Task<IActionResult> Index(CancellationToken cancellation=default)
        {
            await WriteOutIdentityInformation();
            var request = new HttpRequestMessage(HttpMethod.Get, "all");
            var client = _httpClientFactory.CreateClient("HomerApiClient");
            var resp = await client.SendAsync(request);
            resp.EnsureSuccessStatusCode();
            var stream = await resp.Content.ReadAsStringAsync();
            var resDto=JsonConvert.DeserializeObject<IEnumerable<StoreDto>>(stream);
            return View(resDto);            
        }
        public async Task<IActionResult> Edit([FromRoute] string Id,CancellationToken cancellationToken=default)
        {
            if(string.IsNullOrEmpty(Id) )
            {
                return BadRequest();
            }
            var client = _httpClientFactory.CreateClient("HomerApiClient");
            var request = new HttpRequestMessage(HttpMethod.Get,Id);

            var resp = await client.SendAsync(request, cancellationToken);
            resp.EnsureSuccessStatusCode();
            var stream = await resp.Content.ReadAsStringAsync();
            var resDto = JsonConvert.DeserializeObject<StoreDto>(stream);
            if (resDto==null)
            {
                return NotFound();
            }
            return View(resDto);
        }
        [HttpPost]
        public async Task<IActionResult>Edit([Bind("Id,StoreName,PhoneNumber,Category,SubCategory,Address,OwnerId,CompanyName")] StoreDto storeDto,CancellationToken cancellationToken=default)
        {
            if (ModelState.IsValid)
            {
                var client = _httpClientFactory.CreateClient("HomerApiClient");
                var content = new JsonContent(storeDto);
                var resp = await client.PutAsync("", content);
                resp.EnsureSuccessStatusCode();
                var stream = await resp.Content.ReadAsStringAsync();
                var res = JsonConvert.DeserializeObject<StoreDto>(stream);
                return RedirectToAction("Index");
            }
            return View(storeDto);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(StoreDto storeDto, CancellationToken cancellationToken=default)
        {
            if (ModelState.IsValid)
            {
                var client = _httpClientFactory.CreateClient("HomerApiClient");
                var resp = await client.PostAsync("", new JsonContent(storeDto));
                resp.EnsureSuccessStatusCode();
                var stream = await resp.Content.ReadAsStringAsync();
                var store = JsonConvert.DeserializeObject<StoreDto>(stream);
                return RedirectToAction("Index");
            }else
            {
                return View(storeDto);
            }
            
        }
        //[Authorize(Roles ="Admin")]
        public async Task<IActionResult> Details(string Id, CancellationToken cancellationToken=default)
        {
            if(string.IsNullOrEmpty(Id))
            {
                return BadRequest();
            }

            var client = _httpClientFactory.CreateClient("HomerApiClient");
            var request = new HttpRequestMessage(HttpMethod.Get, Id);

            var resp = await client.SendAsync(request, cancellationToken);
            resp.EnsureSuccessStatusCode();
            var stream = await resp.Content.ReadAsStringAsync();
            var resDto = JsonConvert.DeserializeObject<StoreDto>(stream);

            return View(resDto);
        }
        public async Task<IActionResult> Delete(string Id, CancellationToken cancellationToken=default)
        {
            if(string.IsNullOrEmpty(Id))
            {
                return BadRequest();
            }
            var client = _httpClientFactory.CreateClient("HomerApiClient");
            var request = new HttpRequestMessage(HttpMethod.Get, Id);

            var resp = await client.SendAsync(request, cancellationToken);
            resp.EnsureSuccessStatusCode();
            var stream = await resp.Content.ReadAsStringAsync();
            var resDto = JsonConvert.DeserializeObject<StoreDto>(stream);

            return View(resDto);
        }
        [HttpPost]
        public async Task<IActionResult> DeleteConfirmation(string Id, CancellationToken cancellationToken=default)
        {
            if(string.IsNullOrEmpty(Id))
            {
                return BadRequest();
            }
            var client = _httpClientFactory.CreateClient("HomerApiClient");
            var resp = await client.DeleteAsync(Id, cancellationToken);
            resp.EnsureSuccessStatusCode();
            return RedirectToAction("Index");
        }
        public async Task WriteOutIdentityInformation()
        {
            var identityToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.IdToken);
            Debug.WriteLine($"Identity token: {identityToken}");
            foreach(var claim in User.Claims)
            {
                Debug.WriteLine($"claim type: { claim.Type} - claim value {claim.Value}");
            }
        }
        public async Task Logout(CancellationToken cancellationToken=default)
        {
            var client = _httpClientFactory.CreateClient("IDPClient");
            var discoverDoc = await client.GetDiscoveryDocumentAsync();
            if(discoverDoc.IsError)
            {
                throw new Exception(discoverDoc.Error);
            }
            var revokeAccessResp = await client.RevokeTokenAsync(new TokenRevocationRequest {
                Address=discoverDoc.RevocationEndpoint,
                ClientId="homerClient",
                ClientSecret="eric.pham",
                Token=await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken)
            },cancellationToken);
            if(revokeAccessResp.IsError)
            {
                throw new Exception(revokeAccessResp.Error);
            }
            var revokeRefreshResp = await client.RevokeTokenAsync(new TokenRevocationRequest {
                Address=discoverDoc.RevocationEndpoint,
                ClientId="homerClient",
                ClientSecret="eric.pham",
                Token = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.RefreshToken)
            }, cancellationToken);
            if(revokeRefreshResp.IsError)
            {
                throw new Exception(revokeRefreshResp.Error);
            }
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);

        }
    }
}