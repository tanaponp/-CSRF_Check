using _CSRF_Library.Common;
using _CSRF_Library.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace _CSRF_Check.Controllers
{
    public class CertificateController : ApiController
    {
        [HttpGet]
        [Route("GetEmployeeByName/{EmployeeName}")]
        public CallResult<List<PISAPIManager.SearchDirectoryInfoEmpData>> GetEmployeeByName(string EmployeeName)
        {           
            try
            {
                PISAPIManager manager = new PISAPIManager();
                var result = new CallResult<List<PISAPIManager.SearchDirectoryInfoEmpData>>
                {
                    Status = 1,
                    Data = manager.GetEmployeeByName(EmployeeName)
                };
                return result;
            }
            catch (Exception ex)
            {
                return new CallResult<List<PISAPIManager.SearchDirectoryInfoEmpData>>
                {
                    Status = -1,
                    Message = ex.Message,
                    StackTrace = ex.StackTrace
                };
            }
        }
    }
}
