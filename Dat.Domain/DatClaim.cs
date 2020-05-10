using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dat.Domain
{
    public class DatClaim
    {
        //[Key]
        //public Guid Id { get; set; }
        //[ForeignKey("FK_UserProfile")]
        //public Guid SubjectId { get; set; }
        //public string Type { get; set; }
        //public string Value { get; set; }
        //public UserProfile UserProfile { get; set; }
        public string given_name { get; set; }
        public string family_name { get; set; }
        public string address { get; set; }
        public List<string> roles { get; set; } 
        public string company { get; set; }
    }
}
