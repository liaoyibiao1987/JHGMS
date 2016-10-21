﻿using System;
using System.Collections.Generic;

namespace GMS.OA.Contract
{
    public interface IOAService
    {
        Staff GetStaff(int id);
        IEnumerable<Staff> GetStaffList(StaffRequest request = null);
        IEnumerable<Staff> GetAllStaffList();
        void SaveStaff(Staff staff);
        void DeleteStaff(List<int> ids);
        List<int> GetBelongsStaff(int id);
        List<Staff> GetBelongsStaffEntity(int id);
        List<Branch> GetBelongsToBranch(int BranchID);
        Branch GetBranch(int id);
        IEnumerable<Branch> GetBranchList(BranchRequest request = null);
        void SaveBranch(Branch branch);
        void DeleteBranch(List<int> ids);

        Branch GetOrg();
        void SaveOrg(Branch rootBranch);
    }
}
