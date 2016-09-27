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
