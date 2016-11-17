using GMS.Account.Contract;
using GMS.Core.Cache;
using GMS.Crm.Contract;
using GMS.Framework.Contract;
using GMS.Framework.Utility;
using GMS.OA.Contract;
using GMS.OA.Contract.Model;
using GMS.Web.Admin.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace GMS.Web.Admin.Areas.Crm.Controllers
{

    [Permission(EnumBusinessPermission.CrmManage_Customer)]

    public class BusinessController : AdminControllerBase
    {
        //
        // GET: /Crm/Business/
        public ActionResult Index()
        {
            //RenderMyViewData(rquester);
            RenderMyViewData();
            CacheHelper.Clear(LoginInfo.LoginName + "__Customers");
            //rquester.StartDate = DateTime.Now.AddDays(-7);
            //rquester.EndDate = DateTime.Now;
            //int currentstaffid = UserContext.LoginInfo.StaffID.HasValue ? UserContext.LoginInfo.StaffID.Value : -1;
            //IEnumerable<BusinessVM> list = CrmService.GetBusinessList(rquester, GetCurrentUserStaffs(currentstaffid));
            //return View(list);
            return View();
        }
        protected override JsonResult Json(object data, string contentType, Encoding contentEncoding, JsonRequestBehavior behavior)
        {
            return new JsonNetResult
            {
                Data = data,
                ContentType = contentType,
                ContentEncoding = contentEncoding,
                JsonRequestBehavior = behavior
            };
        }

        [HttpPost]
        public ActionResult DeleteCustomers(List<int> ids)
        {
            if (ids != null && ids.Count > 0)
            {
                this.CrmService.DeleteCustomer(ids);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public JsonResult GetAllBranch()
        {
            IEnumerable<Branch> all = OAService.GetBranchList();
            return Json(all);
        }

        [HttpPost]
        public JsonResult GetPayment(int ID, string BDate)
        {
            Payment p = CrmService.GetPayment(ID, BDate);
            return Json(new { Payment = p });
        }


        [HttpPost]
        public JsonResult GetBusinessByAjax(BusinessPostParameter aoData)
        {
            //Fetch.Post("");
            int currentstaffid = UserContext.LoginInfo.StaffID.HasValue ? UserContext.LoginInfo.StaffID.Value : -1;
            List<int> staffids = GetCurrentUserStaffs(currentstaffid);

            PagedList<BusinessVM> list = CrmService.GetBusinessList(aoData, staffids);

            return new JsonResult()
            {
                Data = new
                {
                    iDraw = aoData.draw,
                    iTotalRecords = list.TotalItemCount,
                    iTotalDisplayRecords = list.TotalItemCount,
                    Data = list
                },
                JsonRequestBehavior = JsonRequestBehavior.DenyGet,
                MaxJsonLength = Int32.MaxValue
            };
        }

        private void RenderMyViewData()
        {
            ViewData.Add("Category", new SelectList(EnumHelper.GetItemValueList<EnumCategory>(), "Key", "Value", 0));
            ViewData.Add("Channel", new SelectList(EnumHelper.GetItemValueList<EnumChannel>(), "Key", "Value", 0));
            ViewData.Add("BusinessType", new SelectList(EnumHelper.GetItemValueList<EnumBusinessType>(), "Key", "Value", 0));
            ViewData.Add("EnumPosition", new SelectList(EnumHelper.GetItemValueList<EnumPosition>(), "Key", "Value", 0));

            //普通业务员，客户不会太多
            if (GMS.Web.Admin.Common.AdminUserContext.Current.LoginInfo.BusinessPermissionList.Select(p => p.ToString()).Contains(GMS.Account.Contract.EnumBusinessPermission.CrmManage_Belongs.ToString()) == false)
            {
                int currentstaffid = UserContext.LoginInfo.StaffID.HasValue ? UserContext.LoginInfo.StaffID.Value : -1;
                var customerList = CrmService.GetCustomerList(GetCurrentUserStaffs(currentstaffid)).ToList();
                customerList.ForEach(c => c.Name = string.Format("{0}({1})", c.Name, c.Contacter));
                ViewData.Add("CustomerId", new SelectList(customerList, "Id", "Name"));
            }


            List<Staff> liststaff = GetCurrentUserStaffs();
            ViewData.Add("Staffs", new SelectList(liststaff.Select(c => new { Id = c.ID, Name = c.Name }), "Id", "Name"));

            ViewData.Add("ChainType", new SelectList(EnumHelper.GetItemValueList<EnumChainType>(), "Key", "Value"));

            IEnumerable<Cooperations> listCoop = AccountService.GetCooperationsList(new Request { PageIndex = 0, PageSize = int.MaxValue });
            ViewData.Add("CooperationKinds", new SelectList(listCoop, "ID", "Name"));

            ViewData.Add("Coops", listCoop);
            RenderLeader();
        }
        private void RenderLeader()
        {
            BranchRequest br = new BranchRequest { PageIndex = 0, PageSize = 64 };
            IEnumerable<Branch> selected = OAService.GetBranchList(br);
            List<Branch> leader = new List<Branch>();
            List<Branch> suboffice = new List<Branch>();
            if (selected != null && selected.Count() > 0)
            {
                foreach (var item in selected)
                {
                    if (item.ID == 0) continue;
                    if (selected.Where(p => p.ParentId == item.ID).Count() > 0)
                    {
                        leader.Add(item);
                    }
                    else
                    {
                        suboffice.Add(item);
                    }
                }
            }
            ViewData.Add("Leaders", new SelectList(leader.Select(c => new { Id = c.ID, Name = c.Name }), "Id", "Name"));
            ViewData.Add("Suboffice", new SelectList(suboffice.Select(c => new { Id = c.ID, Name = c.Name }), "Id", "Name"));
        }

        /// <summary>
        /// 日历菜单
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            return View();
        }
        public ActionResult EditByID(int id)
        {
            ModelState.Clear();
            Business business = CrmService.GetBusinessById(id);
            if (business != null)
            {
                RenderMyViewData(business.CustomerID.Value);
            }
            else
            {
                RenderMyViewData(-1);
            }
            return View("Edit", business);
        }

        public ActionResult AddByDate(DateTime CreateDate)
        {
            Business business = new Business();
            business.CreateTime = CreateDate;
            RenderMyViewData(-1);
            return View("Edit", business);
        }
        public ActionResult AddByCustomerId(int CustomerId)
        {
            Business business = new Business();
            business.CreateTime = DateTime.Now;
            RenderMyViewData(CustomerId);
            return View("Edit", business);
        }

        [HttpPost]
        public ActionResult AddByDate(FormCollection collection)
        {
            if (ModelState.IsValid)
            {
                collection.Remove("");
                CreateBusinessEntity entity = new CreateBusinessEntity();
                this.TryUpdateModel<CreateBusinessEntity>(entity, collection.AllKeys);
                entity.StaffID = UserContext.LoginInfo.StaffID.HasValue ? UserContext.LoginInfo.StaffID.Value : -1;
                try
                {
                    this.CrmService.CreateBusiness(entity);
                }
                catch (BusinessException e)
                {
                    this.ModelState.AddModelError(e.Name, e.Message);
                    RenderMyViewData(-1);
                    return View("Edit");
                }
            }
            return this.RefreshParent();
        }

        [HttpPost]
        public ActionResult EditByID(int ID, FormCollection collection)
        {
            if (ModelState.IsValid)
            {
                Business business = CrmService.GetBusinessById(ID);
                business.IncreaseLog();
                if (business != null)
                {
                    this.TryUpdateModel<Business>(business, collection.AllKeys);
                    business.StaffID = UserContext.LoginInfo.StaffID.HasValue ? UserContext.LoginInfo.StaffID.Value : -1;
                    try
                    {
                        this.CrmService.UpdateBusiness(business);
                    }
                    catch (BusinessException e)
                    {
                        this.ModelState.AddModelError(e.Name, e.Message);
                        RenderMyViewData(-1);
                        return View("Edit", business);
                    }
                }

            }
            return this.RefreshParent();
        }

        [HttpPost]
        public ActionResult AddByCustomerId(int CustomerId, FormCollection collection)
        {
            Customer customer = CrmService.GetCustomer(CustomerId);

            if (ModelState.IsValid && customer != null)
            {
                CreateBusinessEntity business = new CreateBusinessEntity();
                this.TryUpdateModel<CreateBusinessEntity>(business);
                business.StaffID = UserContext.LoginInfo.StaffID.HasValue ? UserContext.LoginInfo.StaffID.Value : -1;
                try
                {
                    this.CrmService.CreateBusiness(business);
                }
                catch (BusinessException e)
                {
                    this.ModelState.AddModelError(e.Name, e.Message);
                    RenderMyViewData(-1);
                    return View("Edit");
                }
            }

            return this.RefreshParent();
        }
        [HttpPost]
        public ActionResult MoveEvent(int ID, DateTime to)
        {
            Business business = CrmService.GetBusinessById(ID);
            business.CreateTime = to;
            CrmService.UpdateBusiness(business);
            return new EmptyResult();
        }
        /// <summary>
        /// 业务员获取自己一个月的业务信息。
        /// </summary>
        /// <param name="dstart"></param>
        /// <param name="dend"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetBusiness(DateTime dstart, DateTime dend)
        {
            BusinessRequest rquester = new BusinessRequest();
            rquester.StartDate = dstart;
            rquester.EndDate = dend;
            IEnumerable<Business> list = CrmService.GetBusinessList(rquester, UserContext.LoginInfo.StaffID.HasValue ? UserContext.LoginInfo.StaffID.Value : -1);
            return Json(list);
        }

        [HttpPost]
        public JsonResult ModifyStaffs(List<int> customerids, int newstaffid)
        {
            bool result = CrmService.ModifyStaffs(customerids, newstaffid);
            return Json(result);
        }
        private void RenderMyViewData(int customid)
        {
            var request = new CustomerRequest();
            int currentstaffid = UserContext.LoginInfo.StaffID.HasValue ? UserContext.LoginInfo.StaffID.Value : -1;
            request.Customer.StaffID = currentstaffid;

            var customerList = this.CrmService.GetCustomerList(GetCurrentUserStaffs(currentstaffid), request).ToList();

            customerList.ForEach(c => c.Name = string.Format("{0}({1})", c.Name, c.Tel));
            customid = customerList.Exists(p => p.ID == customid) ? customid : -1;

            ViewData.Add("CustomerId", new SelectList(customerList, "Id", "Name", customid));
        }

        private string GetCoopString(List<int> ids)
        {
            var liststr = this.Cooperations.Where(p => ids.Contains(p.ID));
            if (liststr != null && liststr.Count() > 0)
            {
                return string.Join(",", liststr.Select(x => x.Name));
            }
            else
            {
                return "";
            }
        }

        [HttpPost]
        public JsonResult GentExcel(BusinessPostParameter aoData)
        {
            int days = (aoData.enddate.Value - aoData.startdate.Value).Days;
            int currentstaffid = UserContext.LoginInfo.StaffID.HasValue ? UserContext.LoginInfo.StaffID.Value : -1;
            List<int> staffids = GetCurrentUserStaffs(currentstaffid);

            string temFilepath = Path.Combine(Request.PhysicalApplicationPath, "Template" + ".xlsx");
            string filename = Guid.NewGuid().ToString() + ".xlsx";
            string outputFilepath = Path.Combine(Request.PhysicalApplicationPath, @"download\" + filename);
            FileInfo temFile = new FileInfo(temFilepath);
            FileInfo outputFil = new FileInfo(outputFilepath);
            List<BusinessVM> list = CrmService.GetBusinessDownload(aoData, staffids);
            bool returnval = false;
            if (list != null && list.Count > 0)
            {
                try
                {
                    using (FastExcel fastExcel = new FastExcel(temFile, outputFil))
                    {
                        Worksheet worksheet = new Worksheet();
                        List<Row> rows = new List<Row>();
                        List<Cell> fristcells = new List<Cell>();

                        fristcells.Add(new Cell(1, "分管领导"));
                        fristcells.Add(new Cell(2, "办事处"));
                        fristcells.Add(new Cell(3, "省份"));
                        fristcells.Add(new Cell(4, "地市"));
                        fristcells.Add(new Cell(5, "公司名称"));
                        fristcells.Add(new Cell(6, "类别"));
                        fristcells.Add(new Cell(7, "联系人"));
                        fristcells.Add(new Cell(8, "联系方式"));
                        fristcells.Add(new Cell(9, "渠道"));
                        fristcells.Add(new Cell(10, "商业类型"));
                        fristcells.Add(new Cell(11, "连锁门店数量"));
                        fristcells.Add(new Cell(12, "连锁合作方式"));
                        fristcells.Add(new Cell(13, "业务员"));
                        fristcells.Add(new Cell(14, "业务员类型"));
                        fristcells.Add(new Cell(15, "是否合作"));
                        fristcells.Add(new Cell(16, "合作品种"));
                        fristcells.Add(new Cell(17, "预计回款"));
                        for (int i = 0; i < days; i++)
                        {
                            DateTime now = aoData.startdate.Value.AddDays(i);
                            fristcells.Add(new Cell(18 + i, now.ToString("MM月dd日")));
                        }
                        rows.Add(new Row(1, fristcells));

                        for (int rowNumber = 0; rowNumber < list.Count; rowNumber++)
                        {
                            List<Cell> cells = new List<Cell>();
                            int branch = list[rowNumber].Staff.BranchId.HasValue ? list[rowNumber].Staff.BranchId.Value : -1;
                            int cityid = list[rowNumber].Customer.CityId.HasValue ? list[rowNumber].Customer.CityId.Value : -1;
                            cells.Add(new Cell(1, GetParentBranch(branch)));
                            cells.Add(new Cell(2, GetBranch(branch)));
                            cells.Add(new Cell(3, GetProvinceName(cityid)));
                            cells.Add(new Cell(4, GetCityName(cityid)));
                            cells.Add(new Cell(5, string.IsNullOrEmpty(list[rowNumber].Customer.Name) ? "" : list[rowNumber].Customer.Name));
                            cells.Add(new Cell(6, GetCategory(list[rowNumber].Customer.Category)));
                            cells.Add(new Cell(7, StringHelper.XmlStringReplace((list[rowNumber].Customer.Contacter))));
                            cells.Add(new Cell(8, StringHelper.XmlStringReplace(list[rowNumber].Customer.Tel)));
                            cells.Add(new Cell(9, list[rowNumber].Customer.ShowChannel));
                            cells.Add(new Cell(10, list[rowNumber].Customer.ShowBusinessType));

                            object chaincout;
                            if (list[rowNumber].Customer.ChainCount.HasValue == false || list[rowNumber].Customer.ChainCount.Value < 1)
                            {
                                chaincout = null;
                            }
                            else
                            {
                                chaincout = list[rowNumber].Customer.ChainCount.Value;
                            }
                            cells.Add(new Cell(11, chaincout));

                            int chainType = list[rowNumber].Customer.ChainType.HasValue ? list[rowNumber].Customer.ChainType.Value : 0;
                            cells.Add(new Cell(12, EnumHelper.GetEnumTitle((EnumChainType)chainType)));

                            cells.Add(new Cell(13, list[rowNumber].Staff != null ? list[rowNumber].Staff.Name : ""));
                            cells.Add(new Cell(14, list[rowNumber].Staff != null ? EnumHelper.GetEnumTitle((EnumPosition)list[rowNumber].Staff.Position) : ""));

                            bool iscop = list[rowNumber].Customer.CooperationOrNot.HasValue ? list[rowNumber].Customer.CooperationOrNot.Value : false;
                            cells.Add(new Cell(15, iscop == true ? "是" : "否"));

                            cells.Add(new Cell(16, GetCoopString(list[rowNumber].Customer.CooperationsIds)));

                            string strperpayment = list[rowNumber].PerPayment;
                            if (string.IsNullOrEmpty(strperpayment) == false)
                            {
                                double dperpay = 0d;
                                double.TryParse(strperpayment, out dperpay);
                                cells.Add(new Cell(17, dperpay));
                            }
                            else
                            {
                                cells.Add(new Cell(17, strperpayment));
                            }


                            for (int i = 0; i < days; i++)
                            {
                                DateTime now = aoData.startdate.Value.AddDays(i);
                                if (list[rowNumber].Business != null && list[rowNumber].Business.Count() > 0)
                                {
                                    var x = list[rowNumber].Business.FirstOrDefault(p => p.CreateTime == now);
                                    if (x != null)
                                    {
                                        cells.Add(new Cell(18 + i, StringHelper.XmlStringReplace(x.Message)));
                                    }
                                    else
                                    {
                                        cells.Add(new Cell(18 + i, ""));
                                    }
                                }
                                else
                                {
                                    cells.Add(new Cell(18 + i, ""));
                                }
                            }
                            rows.Add(new Row(rowNumber + 2, cells));
                        }
                        worksheet.Rows = rows;
                        fastExcel.Write(worksheet, "sheet1");
                    }
                    returnval = true;
                }
                catch (Exception es)
                {
                    returnval = false;
                }

                return Json(new { result = returnval, filepath = filename });
            }
            else
            {
                return Json(new { result = returnval, message = "没有可导出数据." });
            }

        }

        public FileResult Downloadfile(string strFile)
        {
            string fullPathUrl = Path.Combine(Request.PhysicalApplicationPath, @"download/" + strFile);
            try
            {
                return File(fullPathUrl, "application/vnd.ms-excel", "客户管理表.xlsx");

            }
            catch (Exception e)
            {
                Console.Write(e.ToString());
                return null;
            }
            finally
            {
                //System.IO.File.Delete(fullPathUrl);
            }
        }

        private string GetCategory(int categoryType)
        {
            return EnumHelper.GetEnumTitle((EnumCategory)categoryType);
        }

        private string GetParentBranch(int branchID)
        {
            if (BranchDic[branchID] != null)
            {
                return StringHelper.XmlStringReplace(BranchDic[branchID].ParentBranch.Name);
            }
            else
            {
                return "";
            }
        }
        private string GetBranch(int branchID)
        {
            if (BranchDic[branchID] != null)
            {
                return StringHelper.XmlStringReplace(BranchDic[branchID].Name);
            }
            else
            {
                return "";
            }
        }


        #region 动态获取客户
        [HttpPost]
        public JsonResult GetCustomers(string term)
        {
            if (string.IsNullOrEmpty(term) == false)
            {
                var x = CacheCustomers.Where(n => ((string.IsNullOrEmpty(n.Name) == false && n.Name.Contains(term))
                    || (string.IsNullOrEmpty(n.Contacter) == false && n.Contacter.Contains(term))));
                if (x == null || x.Count() == 0)
                {
                    return null;
                }
                else
                {
                    return Json(x.Select(p => new { id = p.ID, text = p.Name }));
                }
            }
            else
            {
                return null;
            }
        }

        public IEnumerable<Customer> CacheCustomers
        {
            get
            {
                return CacheHelper.Get<IEnumerable<Customer>>(LoginInfo.LoginName + "_Customers", () =>
                {
                    int currentstaffid = UserContext.LoginInfo.StaffID.HasValue ? UserContext.LoginInfo.StaffID.Value : -1;
                    var customerList = CrmService.GetCustomerList(GetCurrentUserStaffs(currentstaffid)).ToList();
                    customerList.ForEach(c => c.Name = string.Format("{0}({1})", c.Name, c.Contacter));
                    return customerList;
                });
            }
        }
        #endregion
        private string GetCityName(int cityid)
        {
            if (CityDic.ContainsKey(cityid) && CityDic[cityid] != null)
            {
                return StringHelper.XmlStringReplace(CityDic[cityid].Name);
            }
            else
            {
                return "";
            }
        }
        private string GetProvinceName(int cityid)
        {
            if (CityDic.ContainsKey(cityid) && CityDic[cityid] != null)
            {
                return CityDic[cityid].Province == null ? "" : StringHelper.XmlStringReplace(CityDic[cityid].Province.Name);
            }
            else
            {
                return "";
            }
        }

        public Dictionary<int, City> CityDic
        {
            get
            {
                return AdminCacheContext.Current.CityDic;
            }
        }

        public Dictionary<int, Area> AreaDic
        {
            get
            {
                return AdminCacheContext.Current.AreaDic;
            }
        }

        public Dictionary<int, Province> ProvinceDic
        {
            get
            {
                return AdminCacheContext.Current.ProvinceDic;
            }
        }

        public Dictionary<int, Branch> BranchDic
        {
            get
            {
                return AdminCacheContext.Current.BranchDic;
            }
        }

        public IEnumerable<Cooperations> Cooperations
        {
            get
            {
                return AdminCacheContext.Current.Cooperations;
            }
        }

    }
}
