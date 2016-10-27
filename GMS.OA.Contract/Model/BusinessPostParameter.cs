using System;
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
        public object search { get; set; }

        /// <summary>
        /// 每页显示的数量
        /// </summary>
        public int length { get; set; }

        public int start { get; set; }

        public int? Category { get; set; }
        public int? Channel { get; set; }
        public int? BusinessType { get; set; }
        public int? EnumPosition { get; set; }

        /// <summary>
        /// 排序列的数量
        /// </summary>
        public object order { get; set; }

        /// <summary>
        /// 逗号分割所有的列
        /// </summary>
        public object columns { get; set; }

        public int startpage
        {
            get
            {
                return start / length + 1;
            }
        }
    }
}
