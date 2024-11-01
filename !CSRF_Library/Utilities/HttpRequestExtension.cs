using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace _CSRF_Library.Utilities
{
    public static class HttpRequestExtension
    {
        public static bool CheckSecretKey(this HttpRequestMessage request)
        {
            var result = false;

            try
            {
                var secretKey = request.Headers.First(i => string.Equals(i.Key, "secretkey", StringComparison.OrdinalIgnoreCase) == true).Value.ForceToString();
                var origin = request.Headers.FirstOrDefault(i => string.Equals(i.Key, "origin", StringComparison.OrdinalIgnoreCase) == true).Value.ForceToString();

                if (secretKey.ToUpper() == "20DAB759-D06A-451F-8322-56141D2462C8".ToUpper())
                {
                    origin = "UNIT_TEST";
                }

                CommonUtility.Instance.SqlExec(comm =>
                {
                    comm.CommandText = "usp_SecretKey_IsActive";
                    comm.Parameters.AddWithValue("@Origin", origin);
                    comm.Parameters.AddWithValue("@SecretKey", secretKey);

                    result = comm.ExecuteScalar().ForceToBoolean();
                });
            }
            catch (Exception ex)
            {
                throw new Exception("Required SecretKey");
            }

            if (result == false)
            {
                throw new Exception("SecretKey Invalid");
            }

            return result;
        }
    }
}
