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
    [Table("Payment")]
    public class Payment : ModelBase
    {
        public int CustomerID { get; set; }
        public string Durring { get; set; }
        public double? CurrentPayment { get; set; }
        public double? PredictPayment { get; set; }
        public string Logs { get; set; }
    }
}
