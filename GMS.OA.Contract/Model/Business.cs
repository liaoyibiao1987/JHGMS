using GMS.Framework.Contract;
using GMS.OA.Contract;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace GMS.Crm.Contract
{
    [Table("Customer")]
    public class Business : ModelBase
    {
        public int? StaffID { get; set; }
        public int? CustomerID { get; set; }
        [StringLength(200, ErrorMessage = "细节不能超过200个字符")]
        public string Message { get; set; }
        public string Log { get; set; }
        public virtual Staff Staff { get; set; }
    }
}
