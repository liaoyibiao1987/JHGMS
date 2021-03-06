﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GMS.OA.Contract.Model
{
    public class BusinessPostParameter
    {
        public DateTime? startdate { get; set; }
        public DateTime? enddate { get; set; }
        /// <summary>
        /// DataTable请求服务器端次数
        /// </summary>       
        public int draw { get; set; }

        /// <summary>
        /// 过滤文本
        /// </summary>
        public dynamic search { get; set; }

        /// <summary>
        /// 每页显示的数量
        /// </summary>
        public int length { get; set; }

        public int start { get; set; }

        /// <summary>
        /// 类别
        /// </summary>
        public int? Category { get; set; }
        /// <summary>
        /// 渠道
        /// </summary>
        public int? Channel { get; set; }
        public int? BusinessType { get; set; }
        /// <summary>
        /// 业务员
        /// </summary>
        public int? StaffID { get; set; }
        /// <summary>
        /// 业务员类型
        /// </summary>
        public int? EnumPosition { get; set; }

        /// <summary>
        /// 公司名称
        /// </summary>
        public int? CustomerId { get; set; }

        /// <summary>
        /// 分管领导
        /// </summary>
        public int? Leaders { get; set; }

        /// <summary>
        /// 办事处
        /// </summary>
        public int? Suboffice { get; set; }

        /// <summary>
        /// 合作方式
        /// </summary>
        public int? ChainType { get; set; }
        /// <summary>
        /// 合作品种
        /// </summary>
        public int? CooperationKinds { get; set; }

        /// <summary>
        /// 地市
        /// </summary>
        public int? SelectCity { get; set; }
        /// <summary>
        ///是否合作
        /// </summary>
        public bool? CooperationOrNot { get; set; }
        /// <summary>
        ///是否有业务
        /// </summary>
        public bool? HasBusiness { get; set; }

        /// <summary>
        /// 排序列的数量
        /// </summary>
        public List<OrderParm> order { get; set; }

        /// <summary>
        /// 逗号分割所有的列
        /// </summary>
        public dynamic columns { get; set; }

        public int startpage
        {
            get
            {
                if (length == 0) return 0;
                return start / length + 1;
            }
        }


        public override string ToString()
        {
            return string.Format("startdate {0} enddate {1} Category {2} Channel {3} BusinessType {4} StaffID {5} EnumPosition {6} CustomerId {7} Leaders {8} Suboffice {9} ChainType {10} CooperationKinds {11} CooperationOrNot {12}", startdate, enddate, Category, Channel, BusinessType, StaffID, EnumPosition, CustomerId, Leaders, Suboffice, ChainType, CooperationKinds, CooperationOrNot);
        }
    }

    public class OrderParm
    {
        public int column { get; set; }
        public string dir { get; set; }
    }
}
