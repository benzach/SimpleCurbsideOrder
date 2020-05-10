using System;
using System.Collections.Generic;
using System.Text;

namespace Dat.Domain
{
    public class UserProfileEdit:UserProfile
    {
        public string oldPassword { get; set; }
        public string confirmedPassword { get; set; }
        public bool isStoreOwner { get; set; }
        public bool isAdmin { get; set; }
        public bool isCustomer { get; set; }
    }
}
