using Homer.Models.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Homer.Models.DTO
{
    public class CompanyDto:BaseEntity
    {
        public Guid CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string PrimaryContact { get; set; }
        public string Title { get; set; }
        public IList<StoreDto> Stores  => new List<StoreDto>();
    }
}
