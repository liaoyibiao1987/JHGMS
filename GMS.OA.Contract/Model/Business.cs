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
        [Required]
        public int? StaffID { get; set; }

        public virtual Staff Staff { get; set; }
        [Required]
        public int? CustomerID { get; set; }
        public bool? IsSpecial { get; set; }
        public virtual Customer Customer { get; set; }

        [Required]
        [StringLength(2000, ErrorMessage = "细节不能超过2000个字符")]
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

    public class CreateBusinessEntity
    {
        public int? CustomerID { get; set; }
        public int? StaffID { get; set; }
        public string Message { get; set; }
        public DateTime CreateTime { get; set; }
        public bool? IsSpecial { get; set; }

    }
}
