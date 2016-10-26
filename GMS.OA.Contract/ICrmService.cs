using System;
using System.Collections.Generic;
using GMS.Framework.Contract;
using GMS.OA.Contract.Model;

namespace GMS.Crm.Contract
{
    public interface ICrmService
    {
        Project GetProject(int id);
        IEnumerable<Project> GetProjectList(ProjectRequest request = null);
        void SaveProject(Project project);
        void DeleteProject(List<int> ids);

        Customer GetCustomer(int id);
        IEnumerable<Customer> GetCustomerList(List<int> staffids, CustomerRequest request = null);
        void SaveCustomer(Customer customer);
        void DeleteCustomer(List<int> ids);

        VisitRecord GetVisitRecord(int id);
        IEnumerable<VisitRecord> GetVisitRecordList(VisitRecordRequest request = null);
        void SaveVisitRecord(VisitRecord visitRecord);
        void DeleteVisitRecord(List<int> ids);

        PagedList<BusinessVM> GetBusinessList(BusinessPostParameter request, List<int> staffIDs);
        IEnumerable<BusinessVM> GetBusinessList(BusinessRequest request, List<int> staffIDs);
        IEnumerable<Business> GetBusinessList(BusinessRequest request, int staffID);
        //IEnumerable<Business> GetBusinessList(BusinessRequest request, int staffID);
        void CreateBusiness(CreateBusinessEntity entity);
        bool UpdateBusiness(Business entity);
        Business GetBusinessById(int businessID);

        Payment GetPayment(int customerid, string durring);

        IEnumerable<City> GetCityList(Request request = null);
        IEnumerable<Area> GetAreaList(Request request = null);
        IEnumerable<Province> GetProvinceList(Request request = null);

        IEnumerable<UserAnalysis> GetUserAnalysis(DateTime startDate, DateTime endDate);
        IEnumerable<VisitStatistics> GetVisitStatistics(DateTime startDate, DateTime endDate);

    }
}
