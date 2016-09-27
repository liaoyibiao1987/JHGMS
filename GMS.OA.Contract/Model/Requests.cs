using System;
using System.Collections.Generic;
using GMS.Framework.Contract;

namespace GMS.OA.Contract
{
    public class StaffRequest : Request
    {
        public string Name { get; set; }
        public int BranchId { get; set; }
    }

    public class BranchRequest : Request
    {
        public string Name { get; set; }
    }
}

namespace GMS.Account.Contract
{
    public class UserRequest : Request
    {
        public string LoginName { get; set; }
        public string Mobile { get; set; }
    }

    public class RoleRequest : Request
    {
        public string RoleName { get; set; }
    }
}

namespace GMS.Cms.Contract
{
    public class ArticleRequest : Request
    {

        public string Title { get; set; }
        public int ChannelId { get; set; }
        public bool? IsActive { get; set; }
    }

    public class ChannelRequest : Request
    {
        public string Name { get; set; }
        public bool? IsActive { get; set; }
    }

    public class TagRequest : Request
    {
        public Orderby Orderby { get; set; }
    }

    public enum Orderby
    {
        ID = 0,
        Hits = 1
    }
}
namespace GMS.Crm.Contract
{
    public class ProjectRequest : Request
    {
        public string Name { get; set; }
    }

    public class CustomerRequest : Request
    {
        public CustomerRequest()
        {
            this.Customer = new Customer();
        }

        public Customer Customer { get; set; }
    }

    public class VisitRecordRequest : Request
    {
        public VisitRecordRequest()
        {
            this.VisitRecord = new VisitRecord();
        }

        public int? StartHour { get; set; }
        public int? EndHour { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public VisitRecord VisitRecord { get; set; }
    }

    public class UserAnalysis
    {
        public string UserName { get; set; }
        public int VisitRecordCount { get; set; }
        public int CustomerCount { get; set; }
    }

    public class VisitStatistics
    {
        public int Hour { get; set; }
        public int VisitRecordCount { get; set; }
        public int VisitCount { get; set; }
        public int TelCount { get; set; }
    }
}