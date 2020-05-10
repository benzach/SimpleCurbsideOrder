using Homer.Models.Domain;
using System;
using System.Collections.Generic;

namespace Homer.Models.DTO
{
    public class StoreDto:BaseEntity
    {
        public StoreDto()
        {
            BusinessHours = new Dictionary<string, BusinessHour>();
        }
        public AddressInfoDto Address { get; set; }
        public string StoreName { get; set; }
        public LocationDto Location{get;set;}
        public string PhoneNumber { get; set; }
        public Dictionary<string, BusinessHour> BusinessHours { get; set; }
        public bool IsActive { get; set; }
        public string Category { get; set; }
        public string SubCategory { get; set; }
        public string OwnerId { get; set; }
        public string CompanyName { get; set; }
    }
}
