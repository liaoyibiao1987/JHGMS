using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GMS.OA.Contract.Model
{
    public class V_BusinessBase
    {
        public virtual Int64 ID { get; set; }
        public virtual int BranchID { get; set; }
        public virtual double? SumPredictPayment { get; set; }
    }
}
