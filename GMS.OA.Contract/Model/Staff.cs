using System;
using System.Linq;
using GMS.Framework.Contract;
using System.Collections.Generic;
using GMS.Framework.Utility;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using GMS.Account.Contract;

namespace GMS.OA.Contract
{
    [Serializable]
    [Table("Staff")]
    public class Staff : ModelBase
    {
        public Staff()
        {
            this.BranchId = 0;
        }

        [StringLength(100)]
        [Required]
        public string Name { get; set; }

        [StringLength(300)]
        public string CoverPicture { get; set; }

        public int Position { get; set; }

        [StringLength(50, ErrorMessage = "电话不能超过50个字")]
        public string Tel { get; set; }

        private int? userID;
        public int? UserID
        {
            get
            {
                return userID;
            }
            set
            {
                if (value == null)
                {
                    User = null;
                }
                userID = value;
            }
        }

        [ForeignKey("UserID")]
        public virtual User User { get; set; }

        public int? BranchId { get; set; }

        public virtual Branch Branch { get; set; }

    }

    /// <summary>
    /// 职位
    /// </summary>
    public enum EnumPosition
    {
        [EnumTitle("无", IsDisplay = false)]
        None = 0,

        [EnumTitle("开发工程师")]
        Development = 1,

        [EnumTitle("高级开发工程师")]
        SDE = 2,

        [EnumTitle("测试工程师")]
        Testing = 3,

        [EnumTitle("项目经理")]
        PM = 4,
    }

}
