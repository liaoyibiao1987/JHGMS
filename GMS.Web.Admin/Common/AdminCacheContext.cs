﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GMS.Core.Cache;
using GMS.Framework.Utility;
using GMS.Crm.Contract;
using GMS.OA.Contract;

namespace GMS.Web.Admin.Common
{
    public class AdminCacheContext
    {
        public static AdminCacheContext Current
        {
            get
            {
                return CacheHelper.GetItem<AdminCacheContext>();
            }
        }

        public Dictionary<int, City> CityDic
        {
            get
            {
                return CacheHelper.Get<Dictionary<int, City>>("CityDic", () =>
                {
                    return ServiceContext.Current.CrmService.GetCityList().ToDictionary(a => a.ID);
                });
            }
        }


        public Dictionary<int, Area> AreaDic
        {
            get
            {
                return CacheHelper.Get<Dictionary<int, Area>>("AreaDic", () =>
                {
                    return ServiceContext.Current.CrmService.GetAreaList().ToDictionary(a => a.ID);
                });
            }
        }
        public Dictionary<int, Province> ProvinceDic
        {
            get
            {
                return CacheHelper.Get<Dictionary<int, Province>>("ProvinceDic", () =>
                {
                    return ServiceContext.Current.CrmService.GetProvinceList().ToDictionary(a => a.ID);
                });
            }
        }
        public Dictionary<int, Branch> BranchDic
        {
            get
            {
                return CacheHelper.Get<Dictionary<int, Branch>>("BranchDic", () =>
                {
                    return ServiceContext.Current.OAService.GetBranchList().ToDictionary(a => a.ID);
                });
            }
        }


        public IEnumerable<Cooperations> Cooperations
        {
            get
            {
                return CacheHelper.Get<IEnumerable<Cooperations>>("Cooperations", () =>
                {
                    return ServiceContext.Current.AccountService.GetCooperationsList().ToList();
                });
            }
        }
    }
}
