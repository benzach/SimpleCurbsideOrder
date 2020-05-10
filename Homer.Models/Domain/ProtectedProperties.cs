using Homer.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Homer.Models.Domain
{
    public class ProtectedProperties : IOwnerResource, ICompanyResource
    {
        public string CompanyName { get; set; }
        public string OwnerId { get; set; }
    }
}
