using System;

namespace Dat.Database.Abstract
{
    public interface IEntity
    {
        string Id { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string UpdateBy { get; set; }

    }
}
