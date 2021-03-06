﻿using GMS.Crm.Contract;
using GMS.Framework.Contract;
using System;
using System.Collections.Generic;

namespace GMS.Account.Contract
{
    public interface IAccountService
    {
        LoginInfo GetLoginInfo(Guid token);
        LoginInfo Login(string loginName, string password);
        void Logout(Guid token);
        void ModifyPwd(User user);

        User GetUser(int id);
        bool UpdateUserLoginName(int id, string name);
        bool ResetPW(int id, string password);

        IEnumerable<User> GetUserList(UserRequest request = null);
        IEnumerable<User> GetActivedUserList(int staffid = 0);
        void SaveUser(User user);
        void DeleteUser(List<int> ids);

        Role GetRole(int id);
        IEnumerable<Role> GetRoleList(RoleRequest request = null);
        void SaveRole(Role role);
        void DeleteRole(List<int> ids);

        Guid SaveVerifyCode(string verifyCodeText);
        bool CheckVerifyCode(string verifyCodeText, Guid guid);


        bool AddProvinceByName(string name);
        bool AddCityByName(int provinceId, string name);
        bool AddAreaByName(int cityId, string name);

        IEnumerable<City> GetCityList(Request request = null);
        IEnumerable<Area> GetAreaList(Request request = null);
        IEnumerable<Province> GetProvinceList(Request request = null);


        IEnumerable<Cooperations> GetCooperationsList(Request request = null);

        bool DeleteCooperations(List<int> ids);
        bool AddOrEidtCooperation(int id, string name);

    }
}
