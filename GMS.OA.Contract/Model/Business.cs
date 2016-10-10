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
    [Table("Business")]
    public class Business : ModelBase
    {
        public int? StaffID { get; set; }
        public virtual Staff Staff { get; set; }
        public int? CustomerID { get; set; }
        public virtual Customer Customer { get; set; }

        [DataType(DataType.MultilineText)]
        [StringLength(200, ErrorMessage = "细节不能超过200个字符")]
        public string Message { get; set; }
        public string Log { get; set; }

        [NotMapped]
        public string Show_CreateDate
        {
            get
            {
                return CreateTime.ToString("MM/dd/yyyy");
            }
        }
    }

    public class BusinessVM
    {
        private Customer customer;
        public Customer Customer
        {
            get
            {
                return customer;
            }
            set
            {
                customer = value;
            }
        }

        private IEnumerable<Business> business;
        public IEnumerable<Business> Business
        {
            get
            {
                return business;
            }
            set
            {
                business = value;
            }
        }
        public BusinessVM()
        {

        }
        public BusinessVM(Customer cust, IEnumerable<Business> bis)
        {
            this.customer = cust;
            this.business = bis;
        }
        //public IEnumerable<Business> 
    }
}
