using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GMS.Framework.Contract;
using GMS.Framework.Utility;
using GMS.OA.Contract;
using System.Linq;

namespace GMS.Crm.Contract
{
    [Table("Customer")]
    public class Customer : ModelBase
    {
        public Customer()
        {
            this.Cooperations = new List<Cooperations>();
            this.CustomerCooperationsIds = new List<int>();
        }
        [StringLength(50, ErrorMessage = "客户名不能超过50个字")]
        [Required(ErrorMessage = "客户名不能为空")]
        public string Name { get; set; }

        public string Contacter { get; set; }
        public int? CityId { get; set; }

        public virtual City City { get; set; }

        [StringLength(50, ErrorMessage = "电话不能超过50个字")]
        public string Tel { get; set; }

        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}", ErrorMessage = "电子邮件地址无效")]
        public string Email { get; set; }
        /// <summary>
        /// 通讯地址
        /// </summary>
        [StringLength(100, ErrorMessage = "地址不能超过100个字")]
        public string Address { get; set; }
        public int Gender { get; set; }
        public int Category { get; set; }
        public int? ContacterType { get; set; }
        public int? CooperationOrNot { get; set; }

        [NotMapped]
        public float? AvePayment { get; set; }

        [NotMapped]
        public float? PredictPayment { get; set; }

        [NotMapped]
        public float? CurrentPayment { get; set; }
        public string Channel { get; set; }

        [NotMapped]
        public string ShowChannel
        {
            get
            {
                if (string.IsNullOrEmpty(BusinessType) == false)
                {
                    string[] strs = BusinessType.Split(",".ToCharArray());
                    if (strs != null && strs.Count() > 0)
                    {
                        return string.Join(",", strs.Select(p => int.Parse(p)).Cast<EnumChannel>().Select(p => EnumHelper.GetEnumTitle(p)));
                    }
                }
                return "";
            }
        }

        public string BusinessType { get; set; }
        [NotMapped]
        public string ShowBusinessType
        {
            get
            {
                if (string.IsNullOrEmpty(BusinessType) == false)
                {
                    string[] strs = BusinessType.Split(",".ToCharArray());
                    if (strs != null && strs.Count() > 0)
                    {
                        return string.Join(",", strs.Select(p => int.Parse(p)).Cast<EnumBusinessType>().Select(p => EnumHelper.GetEnumTitle(p)));
                    }
                }
                return "";
            }
        }

        [RegularExpression(@"[0-9]{1,4}", ErrorMessage = "输入0-1000的数字")]
        public int? ChainCount { get; set; }
        public int? ChainType { get; set; }

        private int? staffId;
        public int? StaffID
        {
            get
            {
                return staffId;
            }
            set
            {
                if (staffId == null)
                {
                    Staff = null;
                }
                staffId = value;
            }
        }
        public virtual Staff Staff { get; set; }

        [NotMapped]
        public List<int> CustomerCooperationsIds { get; set; }
        [NotMapped]
        public string CustomerCooperShow
        {
            get
            {
                if (Cooperations == null || Cooperations.Count > 0)
                {
                    return string.Join(",", Cooperations.Select(p => p.Name));
                }
                else
                {
                    return "没有";
                }
            }
        }

        /// <summary>
        /// 角色列表
        /// </summary>
        public virtual List<Cooperations> Cooperations { get; set; }


        //public virtual ICollection<Business> Business { get; set; }

    }


    public enum EnumAgeGroup
    {
        [EnumTitle("无", IsDisplay = false)]
        None = 0,

        [EnumTitle("30以下")]
        Below30 = 1,

        [EnumTitle("31-35")]
        From31To35 = 2,

        [EnumTitle("36-40")]
        From36To40 = 3,

        [EnumTitle("41-45")]
        From41To45 = 4,

        [EnumTitle("46-50")]
        From46To50 = 5,

        [EnumTitle("50以上")]
        Above50 = 6
    }

    public enum EnumProfession
    {
        [EnumTitle("无", IsDisplay = false)]
        None = 0,

        [EnumTitle("政府机关")]
        Government = 1,

        [EnumTitle("事业单位")]
        Institution = 2,

        [EnumTitle("金融业")]
        BankingBusiness = 3,

        [EnumTitle("个体私营")]
        PrivateEnterprises = 4,

        [EnumTitle("服务业")]
        ServicingBusiness = 5,

        [EnumTitle("广告传媒")]
        NewElement = 6,

        [EnumTitle("制造业")]
        Manufacturing = 7,

        [EnumTitle("运输业")]
        TransportService = 8,

        [EnumTitle("商贸")]
        Trade = 9,

        [EnumTitle("军警")]
        MilitaryPolice = 10,

        [EnumTitle("退休")]
        Retirement = 11,

        [EnumTitle("IT通讯业")]
        Komuniko = 12,

        [EnumTitle("医疗卫生教育")]
        MedicalTreatment = 13,

        [EnumTitle("房地产建筑业")]
        Realty = 14,

        [EnumTitle("其他职业")]
        Others = 15
    }

    /// <summary>
    /// 称谓
    /// </summary>
    public enum EnumGender
    {
        [EnumTitle("先生")]
        Sir = 1,

        [EnumTitle("女士")]
        Miss = 2
    }

    /// <summary>
    /// 客户类型
    /// </summary>
    public enum EnumCategory
    {
        [EnumTitle("无", IsDisplay = false)]
        None = 0,

        [EnumTitle("商业")]
        Single = 1,

        [EnumTitle("连锁")]
        Married = 2,

        [EnumTitle("其他")]
        MarriedButChild = 3
    }

    /// <summary>
    /// 渠道
    /// </summary>
    [Flags]
    public enum EnumChannel
    {
        [EnumTitle("一终端")]
        Channel1 = 1,

        [EnumTitle("二终端")]
        Channel2 = 2,

        [EnumTitle("三终端")]
        Channel = 3
    }
    /// <summary>
    /// 商业类型
    /// </summary>
    [Flags]
    public enum EnumBusinessType
    {
        [EnumTitle("配送型")]
        Delivery = 1,

        [EnumTitle("代理型")]
        Agency = 2,

        [EnumTitle("自由人")]
        FreeMan = 3
    }

    /// <summary>
    /// 连锁合作方式
    /// </summary>
    [Flags]
    public enum EnumChainType
    {
        [EnumTitle("无", IsDisplay = false)]
        None = 0,

        [EnumTitle("公司直供")]
        Direct = 1,

        [EnumTitle("代理商供")]
        Agent = 2,

        [EnumTitle("业务员供")]
        Salesman = 3
    }
}
