﻿using System;
using System.Linq;
using GMS.Framework.Contract;
using System.Collections.Generic;
using GMS.Framework.Utility;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace GMS.OA.Contract
{
    [Serializable]
    [Table("Branch")]
    public class Branch : ModelBase
    {
        public Branch()
        {
            ParentId = 1;
        }

        [StringLength(100)]
        [Required]
        public string Name { get; set; }

        [StringLength(300)]
        public string Desc { get; set; }

        public virtual List<Staff> Staffs { get; set; }

        public int ParentId { get; set; }

        public virtual Branch ParentBranch { get; set; }

        [ForeignKey("ParentId")]
        //[JsonIgnore]
        public virtual List<Branch> Embranchment { get; set; }
    }
}
