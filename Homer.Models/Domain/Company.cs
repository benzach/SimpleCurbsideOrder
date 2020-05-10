using System;
using System.Collections.Generic;
using System.Text;

namespace Homer.Models.Domain
{
    public class Company:BaseEntity
    {
        public Guid CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string PrimaryContact { get; set; }
        public string Title { get; set; }
        public IList<Store> Stores  => new List<Store>();
    }
}
