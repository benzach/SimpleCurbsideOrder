using Homer.Models.Interfaces;
using System;
using System.Collections.Generic;

namespace Homer.Models.Domain
{
    public class Store:BaseEntity,IOwnerResource,ICompanyResource
    {
        public Store()
        {
            BusinessHours = new Dictionary<string, BusinessHour>();
        }
        public AddressInfo Address { get; set; }
        public string StoreName { get; set; }
        public Location Location{get;set;}
        public string PhoneNumber { get; set; }
        public Dictionary<string, BusinessHour> BusinessHours { get; set; }
        public bool IsActive { get; set; }
        public string Category { get; set; }
        public string SubCategory { get; set; }
        public string OwnerId { get; set; }
        public string CompanyName { get; set; }
    }
}
