using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _CSRF_Library.Utilities
{
    public static class SqlCommandExtension
    {
        public static void ExecuteReader(this SqlCommand comm, Action<SqlDataReader> callback)
        {
            var reader = comm.ExecuteReader();

            while (reader.Read())
            {
                callback(reader);
            }

            reader.Close();
        }

        public static void FillDataAdapter(this SqlCommand comm, DataSet ds)
        {
            using (var adp = new SqlDataAdapter(comm))
            {
                adp.Fill(ds);
            }
        }

        public static void FillDataAdapter(this SqlCommand comm, DataTable dt)
        {
            using (var adp = new SqlDataAdapter(comm))
            {
                adp.Fill(dt);
            }
        }

        public static void AddWithValueWithCheckDbNull(this SqlParameterCollection input, string parameterName, object value)
        {
            if (value == null)
            {
                input.AddWithValue(parameterName, DBNull.Value);
            }
            else
            {
                input.AddWithValue(parameterName, value);
            }
        }
    }
}
