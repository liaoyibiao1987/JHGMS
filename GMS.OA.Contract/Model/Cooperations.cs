using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using GMS.Framework.Contract;

namespace GMS.Crm.Contract
{
    [Table("Cooperations")]
    public class Cooperations : ModelBase
    {
        [Required(ErrorMessage = "名称不能为空")]
        [StringLength(50)]
        public string Name { get; set; }

        public virtual List<Customer> Customers { get; set; }
    }

}
