using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Dat.Data;
using Dat.Domain;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace Dat.IDP.Quickstart.Account
{
    public class UserProfileController : Controller
    {
        private readonly IUserProfileRepository<IMongoDatabase> _userProfileRepository;
        private readonly IMapper _mapper;
        public UserProfileController(IUserProfileRepository<IMongoDatabase> userProfileRepo, IMapper mapper)
        {
            _userProfileRepository = userProfileRepo;
            _mapper = mapper;
        }
        public async Task<IActionResult> Index([FromRoute] string Id, CancellationToken cancellationToken=default)
        {
            var res = await _userProfileRepository.GetByIdAsync(Id, cancellationToken);
            if(res==null)
            {
                return NotFound();
            }
            var ret = _mapper.Map<UserProfileEdit>(res);
            return View(ret) ;
        }
        public async Task<IActionResult> Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult>Create(UserProfileEdit userProfile,CancellationToken cancellationToken=default)
        {
            if(userProfile==null)
            {
                return BadRequest();
            }
            if (ModelState.IsValid)
            {
                var uProfile = _mapper.Map<UserProfile>(userProfile);
                var res = await _userProfileRepository.UpsertAsync(uProfile, cancellationToken);
                //var userProfileDTO = _mapper.Map<UserProfileDTO>(userProfile);
                return new CreatedResult("/userprofile", new { Id = res.Id });
            }else
            {
                return View(userProfile);
            }
        }
        [HttpPost]
        public async Task<IActionResult> ChangePassword([Bind("Username,oldPassword,confirmedPassword,Password")]UserProfileEdit userProfile,CancellationToken cancellationToken)
        {
            var user = await _userProfileRepository.FindByUsernameAsync(userProfile.Username, cancellationToken);
            if(user==null)
            {
                ModelState.AddModelError(nameof(UserProfileEdit.Username), "User not found");
                return View(userProfile);
            }
            var oldPasswordHashed = userProfile.oldPassword.Sha256();
            if (oldPasswordHashed != user.Password)
            {
                ModelState.AddModelError(nameof(UserProfileEdit.oldPassword), "old password does not match");
            }
            if (userProfile.Password != userProfile.confirmedPassword)
            {
                ModelState.AddModelError(nameof(UserProfileEdit.confirmedPassword), "confirmed password does not match password");
            }
            if (ModelState.IsValid)
            {
                var dmUser = _mapper.Map<UserProfile>(userProfile);
                dmUser.Id = user.Id;
                dmUser.SubjectId = user.SubjectId;
                dmUser.UpdateDate = DateTime.UtcNow;
                var res = await _userProfileRepository.UpsertAsync(dmUser, cancellationToken);
                return new OkObjectResult(res);
            }

            return View(userProfile);
        }
        public async Task<IActionResult> ChangePassword()
        {
            return View(new UserProfileEdit());
        }
        public  async Task<IActionResult> Edit([FromRoute] string Id,CancellationToken cancellationToken)
        {
            var res = await _userProfileRepository.GetByIdAsync(Id, cancellationToken);
            var userProfileEdit = _mapper.Map<UserProfileEdit>(res); 
            return View(userProfileEdit);
        }
        [HttpPost]
        public async Task<IActionResult> Edit([Bind("Id,SubjectId,Username,Claims,isActive,isAdmin,isStoreOwner,isCustomer")]UserProfileEdit userProfile, CancellationToken cancellationToken=default)//oldPassword,confirmedPassword,Password,
        {
            //var user = await _userProfileRepository.GetByIdAsync(userProfile.Id, cancellationToken);
            //var oldPasswordHashed = userProfile.oldPassword.Sha256();
            //if (oldPasswordHashed != user.Password)
            //{
            //    ModelState.AddModelError(nameof(UserProfileEdit.oldPassword), "old password does not match");
            //}
            //if(userProfile.Password!=userProfile.confirmedPassword)
            //{
            //    ModelState.AddModelError(nameof(UserProfileEdit.confirmedPassword), "confirmed password does not match password");
            //}
            if (ModelState.IsValid)
            {
                var dmUser = _mapper.Map<UserProfile>(userProfile);
                var res = await _userProfileRepository.UpsertAsync(dmUser, cancellationToken);
                return new OkObjectResult(res);
            }

            return View(userProfile);
           
        }
    }
}