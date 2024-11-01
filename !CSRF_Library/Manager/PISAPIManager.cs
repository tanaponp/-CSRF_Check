using _CSRF_Library.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _CSRF_Library.Manager
{
    public class PISAPIManager
    {
        public SearchDirectoryInfoResponseData SearchDirectoryInfo(Dictionary<string, string> query)
        {
            return new SearchDirectoryInfoResponseData
            {
                Entries = new SearchDirectoryInfoResponseData.entries
                {
                    Entry = new List<SearchDirectoryInfoResponseData.entry>
                    {
                        new SearchDirectoryInfoResponseData.entry
                        {
                            AREA_ID = "1",
                            EmployeeNameTH = "นาย สมชาย ใจดี",
                            EmployeeNameENG = "Mr. Somchai JaiDee",
                            EmployeeId = "123456",
                            PositionCode = "001",
                            PositionNameTH = "ตำแหน่ง 1",
                            PositionNameENG = "Position 1",
                            PositionABBR = "P1",
                            DepartmentCode = "001",
                            DepartmentNameTH = "แผนก 1",
                            DepartmentNameENG = "Department 1",
                            DepartmentABBR = "D1",
                            EMailAddress = ""
                        }
                    }
                }
            };
        }
        public List<SearchDirectoryInfoEmpData> GetEmployeeByName(string EmployeeName)
        {
            List<SearchDirectoryInfoEmpData> result = new List<SearchDirectoryInfoEmpData>();
            try
            {
                var dic = new Dictionary<string, string>();
                dic.Add("EmployeeName", EmployeeName);
                var emp = this.SearchDirectoryInfo(dic);
                if (emp == null || emp.Entries == null || emp.Entries.Entry == null) return result;
                foreach (var e in emp.Entries.Entry)
                {
                    result.Add(new SearchDirectoryInfoEmpData
                    {
                        AREA_ID = e.AREA_ID.ForceToString(),
                        EmployeeNameTH = e.EmployeeNameTH.ForceToString(),
                        EmployeeNameENG = e.EmployeeNameENG.ForceToString(),
                        EmployeeId = e.EmployeeId.ForceToString(),
                        PositionCode = e.PositionCode.ForceToString(),
                        PositionNameTH = e.PositionNameTH.ForceToString(),
                        PositionNameENG = e.PositionNameENG.ForceToString(),
                        PositionABBR = e.PositionABBR.ForceToString(),
                        DepartmentCode = e.DepartmentCode.ForceToString(),
                        DepartmentNameTH = e.DepartmentNameTH.ForceToString(),
                        DepartmentNameENG = e.DepartmentNameENG.ForceToString(),
                        DepartmentABBR = e.DepartmentABBR.ForceToString(),
                        EMailAddress = e.EMailAddress.ForceToString(),
                        NickName = e.NickName.ForceToString(),
                        MobileNo = e.MobileNo.ForceToString(),
                        IntPhoneNo = e.IntPhoneNo.ForceToString()
                    });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }
        public class SearchDirectoryInfoEmpData
        {
            public string AREA_ID { get; set; }
            public string EmployeeNameTH { get; set; }
            public string EmployeeNameENG { get; set; }
            public string EmployeeId { get; set; }
            public string PositionCode { get; set; }
            public string PositionNameTH { get; set; }
            public string PositionNameENG { get; set; }
            public string PositionABBR { get; set; }
            public string DepartmentCode { get; set; }
            public string DepartmentNameTH { get; set; }
            public string DepartmentNameENG { get; set; }
            public string DepartmentABBR { get; set; }
            public string EMailAddress { get; set; }
            public string NickName { get; set; }
            public string MobileNo { get; set; }
            public string IntPhoneNo { get; set; }
            public string ImagePath { get; set; }

        }
        public class SearchDirectoryInfoResponseData
        {
            public entries Entries { get; set; }

            public class entries
            {
                public List<entry> Entry { get; set; }
            }

            public class entry
            {
                public string AREA_ID { get; set; }
                public string EmployeeNameTH { get; set; }
                public string EmployeeNameENG { get; set; }
                public string EmployeeId { get; set; }
                public string PositionCode { get; set; }
                public string PositionNameTH { get; set; }
                public string PositionNameENG { get; set; }
                public string PositionABBR { get; set; }
                public string DepartmentCode { get; set; }
                public string DepartmentNameTH { get; set; }
                public string DepartmentNameENG { get; set; }
                public string DepartmentABBR { get; set; }
                public string EMailAddress { get; set; }
                public string NickName { get; set; }
                public string MobileNo { get; set; }
                public string IntPhoneNo { get; set; }
            }
        }
    }
}
