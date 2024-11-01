using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace _CSRF_Library.Utilities
{
    public static class ConversionExtension
    {
        public static string ForceToString(this object input)
        {
            if (input == DBNull.Value || input == null)
            {
                return string.Empty;
            }
            else
            {
                return input.ToString();
            }
        }

        public static string ForceToStringNull(this object input)
        {
            if (input == DBNull.Value || input == null)
            {
                return null;
            }
            else
            {
                return input.ToString();
            }
        }

        public static byte[] ForceToByte(this object input)
        {
            if (input == DBNull.Value || input == null)
            {
                return null;
            }
            else
            {
                return (byte[])input;
            }
        }

        public static DateTime ForceToDateTime(this object input)
        {
            return Convert.ToDateTime(input);
        }

        public static DateTime? ForceToDateTimeNull(this object input)
        {
            try
            {
                return Convert.ToDateTime(input);
            }
            catch (Exception)
            {
                return null;
            }
        }

        //public static object ForceToDateTimeSql(this DateTime? input)
        //{
        //    var output = ForceToDateTimeNull(input);
        //    if (output == null)
        //        return DBNull.Value;
        //    else
        //        return output.Value;
        //}

        public static bool ForceToBoolean(this object input)
        {
            return Convert.ToBoolean(input);
        }

        public static bool? ForceToBooleanNull(this object input)
        {
            try
            {
                return Convert.ToBoolean(input);
            }
            catch
            {
                return null;
            }
        }

        //public static object ForceToBooleanSql(this bool? input)
        //{
        //    var output = ForceToBooleanNull(input);
        //    if (output == null)
        //        return DBNull.Value;
        //    else
        //        return output.Value;
        //}

        public static int ForceToInt32(this object input)
        {
            return Convert.ToInt32(input);
        }

        public static int? ForceToInt32Null(this object input)
        {
            try
            {
                return Convert.ToInt32(input);
            }
            catch
            {
                return null;
            }
        }

        //public static object ForceToInt32Sql(this int? input)
        //{
        //    var output = ForceToInt32Null(input);
        //    if (output == null)
        //        return DBNull.Value;
        //    else
        //        return output.Value;
        //}

        public static decimal ForceToDecimal(this object input)
        {
            return Convert.ToDecimal(input);
        }

        public static decimal? ForceToDecimalNull(this object input)
        {
            if (input == DBNull.Value || input == null)
            {
                return null;
            }
            else
            {
                return Convert.ToDecimal(input);
            }
        }

        //public static object ForceToDecimalSql(this decimal? input)
        //{
        //    var output = ForceToDecimalNull(input);
        //    if (output == null)
        //        return DBNull.Value;
        //    else
        //        return output.Value;
        //}

        public static Guid ForceToGUID(this object reader)
        {
            if (reader == DBNull.Value || reader == null)
            {
                return Guid.Empty;
            }
            else
            {
                return new Guid(reader.ToString());
            }
        }

        public static Guid? ForceToGUIDNull(this object reader)
        {
            if (reader == DBNull.Value || reader == null)
            {
                return null;
            }
            else
            {
                return new Guid(reader.ToString());
            }
        }

        public static void AddWithDBNullableValue(this SqlParameterCollection input, string parameterName, object value)
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

        public static DataTable ToDataTable<T>(this T[] items)
        {
            var dataTable = new DataTable(typeof(T).Name);
            var colCount = 0;
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance).OrderBy(x => x.MetadataToken).ToArray();
            foreach (PropertyInfo prop in Props)
            {
                var type = (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) ? Nullable.GetUnderlyingType(prop.PropertyType) : prop.PropertyType);
                if (!type.FullName.StartsWith("System.Collections.Generic.List")
                    && !type.FullName.StartsWith("Thalamo"))
                {
                    colCount++;
                    dataTable.Columns.Add(prop.Name, type);
                }
            }
            foreach (T item in items)
            {
                var values = new object[colCount];
                for (int i = 0; i < colCount; i++)
                {
                    if (!Props[i].PropertyType.FullName.StartsWith("System.Collections.Generic.List")
                        && !Props[i].PropertyType.FullName.StartsWith("Thalamo"))
                    {
                        values[i] = Props[i].GetValue(item, null);
                    }
                }
                dataTable.Rows.Add(values);

                //var dataRow = dataTable.Rows.Add();
                //foreach (PropertyInfo prop in Props)
                //{
                //    var type = (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) ? Nullable.GetUnderlyingType(prop.PropertyType) : prop.PropertyType);
                //    if (!type.FullName.StartsWith("System.Collections.Generic.List")
                //        && !type.FullName.StartsWith("Thalamo.PTT.BSAManagement"))
                //    {
                //        dataRow[prop.Name] = prop.GetValue(item, null) ?? DBNull.Value;
                //    }
                //}
            }

            return dataTable;
        }

        // DataTable
        public static DataTable ToDataTable2<T>(this T[] items)
        {
            var dataTable = new DataTable(typeof(T).Name);
            var colCount = 0;
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance).OrderBy(x => x.MetadataToken).ToArray();
            foreach (PropertyInfo prop in Props)
            {
                var type = (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) ? Nullable.GetUnderlyingType(prop.PropertyType) : prop.PropertyType);
                if (!type.FullName.StartsWith("System.Collections.Generic.List")
                    && !type.FullName.StartsWith("Ideamani"))
                {
                    colCount++;
                    dataTable.Columns.Add(prop.Name, type);
                }
            }
            foreach (T item in items)
            {
                int i = 0;
                var values = new object[colCount];
                foreach (PropertyInfo prop in Props)
                {
                    if (!prop.PropertyType.FullName.StartsWith("System.Collections.Generic.List")
                        && !prop.PropertyType.FullName.StartsWith("Ideamani"))
                    {
                        values[i] = prop.GetValue(item, null);
                        i++;
                    }
                }
                dataTable.Rows.Add(values);
            }

            return dataTable;
        }

        public static int? TryParseInt32Null(this string input, int? defaultValue = null)
        {
            if (int.TryParse(input, NumberStyles.AllowDecimalPoint, null, out int value))
            {
                return value;
            }
            else
            {
                return defaultValue;
            }
        }

        public static int TryParseInt32(this string input, int defaultValue = 0)
        {
            return TryParseInt32Null(input, (int?)defaultValue).Value;
        }

        public static decimal? TryParseDecimalNull(this string input, decimal? defaultValue = null)
        {
            decimal value;
            if (decimal.TryParse(input, out value))
            {
                return value;
            }
            else
            {
                return defaultValue;
            }
        }

        public static decimal TryParseDecimal(this string input, decimal defaultValue = 0)
        {
            return TryParseDecimalNull(input, (decimal?)defaultValue).Value;
        }

        public static DateTime? TryParseDateTimeNull(this string input, DateTime? defaultValue = null)
        {
            if (DateTime.TryParse(input, out DateTime value))
            {
                return value;
            }
            else
            {
                return defaultValue;
            }
        }

        public static DateTime TryParseDateTime(this string input, DateTime defaultValue)
        {
            return TryParseDateTimeNull(input, (DateTime?)defaultValue).Value;
        }

        public static bool? TryParseBooleanNull(this string input, bool? defaultValue = null)
        {
            if (bool.TryParse(input, out bool value))
            {
                return value;
            }
            else
            {
                return defaultValue;
            }
        }

        public static bool TryParseBoolean(this string input, bool defaultValue = false)
        {
            return TryParseBooleanNull(input, (bool?)defaultValue).Value;
        }        
    }
}
