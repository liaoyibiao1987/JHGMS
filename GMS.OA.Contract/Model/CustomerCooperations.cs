using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GMS.Framework.Contract;
using GMS.Framework.Utility;
using GMS.OA.Contract;

namespace GMS.Crm.Contract
{
    [Table("CustomerCooperations")]
    public class CustomerCooperations : ModelBase
    {
        public int? CustomerID { get; set; }

        public int? CooperationsID { get; set; }

        [NotMapped]
        public virtual string CooperationsName { get; set; }

        public virtual List<Cooperations> Cooperations { get; set; }

    }
}
