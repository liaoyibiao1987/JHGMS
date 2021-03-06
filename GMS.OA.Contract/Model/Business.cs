﻿using GMS.Framework.Contract;
using GMS.Framework.Utility;
using GMS.OA.Contract;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace GMS.Crm.Contract
{
    [Auditable]
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

        [StringLength(2000, ErrorMessage = "细节不能超过2000个字符")]
        public string Message { get; set; }

        [NotMapped]
        public string Show_CreateDate
        {
            get
            {
                return CreateTime.ToString("MM/dd/yyyy");
            }
        }

        [NotMapped]
        public double? CurrentPayment { get; set; }

        [NotMapped]
        public double? PredictPayment { get; set; }

        public void UpatePayment(Payment pment)
        {
            if (pment != null)
            {
                this.PredictPayment = pment.PredictPayment;
                this.CurrentPayment = pment.CurrentPayment;

            }
        }


        private string log = "";
        public string Log
        {
            get
            {
                if (Logs != null)
                {
                    log = SerializationHelper.XmlSerialize(Logs);
                    return log;
                }
                else
                {
                    return "";
                }
            }
            set
            {
                if (string.IsNullOrEmpty(value) == false)
                {
                    Logs = SerializationHelper.XmlDeserialize(typeof(List<BusinessLog>), value) as List<BusinessLog>;
                }
                log = value;
            }
        }


        [NotMapped]
        public List<BusinessLog> Logs { get; set; }

        /// <summary>
        /// 自动记录上一条跟单信息
        /// </summary>
        public void IncreaseLog()
        {
            if (Logs == null)
            {
                Logs = new List<BusinessLog>();

            }
            Logs.Add(new BusinessLog
            {
                Create = this.CreateTime,
                CurrentPayment = this.CurrentPayment.HasValue ? this.CurrentPayment.Value : 0,
                Message = this.Message,
                PredictPayment = this.PredictPayment.HasValue ? this.PredictPayment.Value : 0,
                StaffID = this.StaffID.HasValue ? this.StaffID.Value : 0,
            });
        }

    }

    [Serializable]
    public class BusinessLog
    {
        public DateTime Create { get; set; }

        public int StaffID { get; set; }
        public string Message { get; set; }

        public double CurrentPayment { get; set; }

        public double PredictPayment { get; set; }
    }
    public class BusinessVM
    {
        private Staff staff;
        public Staff Staff
        {
            get
            {
                return staff;
            }
            set
            {
                staff = value;
            }
        }

        public string PerPayment { get; set; }

        private string cityName;
        public string CityName
        {
            get
            {
                return cityName;
            }
            set
            {
                cityName = value;
            }
        }

        private string provienc;
        public string Provienc
        {
            get
            {
                return provienc;
            }
            set
            {
                provienc = value;
            }
        }

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
    }

    public class CreateBusinessEntity
    {
        public int? CustomerID { get; set; }
        public int? StaffID { get; set; }
        public string Message { get; set; }
        public DateTime CreateTime { get; set; }
        public bool? IsSpecial { get; set; }
        public float? CurrentPayment { get; set; }
        public float? PredictPayment { get; set; }

    }
}
