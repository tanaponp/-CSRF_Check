using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using Microsoft.SharePoint;
using System.Web;

namespace _CSRF_Library.Utilities
{
    public delegate void ExecSqlCallback(SqlConnection conn, SqlCommand comm);
    public delegate void ExecSqlCallbackCommandOnly(SqlCommand comm);

    public delegate void ExecCallback();

    public delegate void ReadDataReaderCallback(SqlDataReader reader);

    //public delegate void ExecSPCallback(SPSite site, SPWeb web);

    public partial class CommonUtility
    {
        static CommonUtility _instance;
        public static CommonUtility Instance
        {
            get
            {
                if (_instance == null) _instance = new CommonUtility();
                return _instance;
            }
        }


        static string _connectionString;
        public static string ConnectionString
        {
            get
            {
                if (_connectionString == null)
                {
                    _connectionString = ConfigurationManager.ConnectionStrings["CC_ConnectionString"].ConnectionString;
                }

                return _connectionString;
            }
        }        
        public static DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);

            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Defining type of data column gives proper data table 
                var type = (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) ? Nullable.GetUnderlyingType(prop.PropertyType) : prop.PropertyType);
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name, type);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }

        public void SqlExec(ExecSqlCallback callback, string connectionString = null)
        {
            if (string.IsNullOrEmpty(connectionString)) connectionString = CommonUtility.ConnectionString;

            using (var conn = new SqlConnection(connectionString))
            using (var comm = conn.CreateCommand())
            {
                if (conn.State == ConnectionState.Closed) conn.Open();
                comm.CommandType = CommandType.StoredProcedure;
                comm.CommandTimeout = 0;

                callback(conn, comm);
            }
        }

        public void SqlExec(ExecSqlCallbackCommandOnly callback, string connectionString = null)
        {
            if (string.IsNullOrEmpty(connectionString)) connectionString = CommonUtility.ConnectionString;

            using (var conn = new SqlConnection(connectionString))
            using (var comm = conn.CreateCommand())
            {
                if (conn.State == ConnectionState.Closed) conn.Open();
                comm.CommandType = CommandType.StoredProcedure;
                comm.CommandTimeout = 0;

                callback(comm);
            }
        }

        public void SqlExec(ExecSqlCallback callback, SqlConnection conn)
        {
            using (var comm = conn.CreateCommand())
            {
                comm.CommandType = CommandType.StoredProcedure;
                comm.CommandTimeout = 0;

                callback(conn, comm);
            }
        }        

        public Stream ResizeImage(Stream stream, int width, int height)
        {
            var image = Image.FromStream(stream);
            var bitmap = ResizeImage(image, width, height);
            var result = new MemoryStream();
            bitmap.Save(result, ImageFormat.Png);
            bitmap.Dispose();
            image.Dispose();

            return result;
        }

        public Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

        public UserData GetCurrentUser()
        {            
            var user = new UserData();
            user.EmailAddr = SPCurrentUser.Email;
            user.LoginName = SPCurrentUser.LoginName;
            user.FullName = SPCurrentUser.Name;
            var keyword = user.EmailAddr;
            if (string.IsNullOrEmpty(keyword) == true)
            {
                keyword = user.LoginName;
            }

            var tmp = GetUserData(keyword.Trim());

            if (tmp != null)
            {
                user.LoginName = tmp.LoginName;
                user.FullName = tmp.FullName;
                user.EmailAddr = tmp.EmailAddr;
                user.IsAdmin = CurrentUserIsAdminGroup(tmp.LoginName);
            }
            else
            {
                user.IsAdmin = CurrentUserIsAdminGroup(SPCurrentUser.LoginName.Replace("i:0#.w|", "").Trim());
            }

            return user;
        }

        public UserData GetUserData(string loginName)
        {
            UserData result = null;

            SqlExec((conn, comm) =>
            {
                comm.CommandType = CommandType.StoredProcedure;
                comm.CommandText = "uspGetUserData";

                comm.Parameters.AddWithValue("@email", loginName);

                using (var read = comm.ExecuteReader())
                {
                    while (read.Read())
                    {
                        result = new UserData
                        {
                            LoginName = read["LoginName"].ForceToString(),
                            FullName = read["DisplayName"].ForceToString(),
                            EmailAddr = read["Email"].ForceToString()
                        };
                    }
                }
            });

            return result;
        }
        public bool CurrentUserIsAdminGroup(string LoginName)
        {
            var result = false;
            var groupNames = "G1";
            var webURL = "W";
            var LoginName2 = LoginName;
            LoginName = "i:0#.w|" + LoginName;
            try
            {
                SPSecurity.RunWithElevatedPrivileges(() =>
                {
                    using (var site = new SPSite(webURL))
                    using (var web = site.OpenWeb())
                    {
                        var tempAllowUnsafeUpdates = web.AllowUnsafeUpdates;
                        web.AllowUnsafeUpdates = true;

                        //var spUser = web.EnsureUser(LoginName);
                        var spUser = web.AllUsers.Cast<SPUser>().Single(u => u.LoginName.ToLower().Equals(LoginName.ToLower()));
                        if (spUser == null)
                        {
                            spUser = web.AllUsers.Cast<SPUser>().Single(u => u.LoginName.ToLower().Equals(LoginName2.ToLower()));
                        }
                        foreach (var groupName in groupNames.Split(','))
                        {
                            //foreach (SPGroup g in web.CurrentUser.Groups)
                            foreach (SPGroup g in spUser.Groups)
                            {
                                if (string.Compare(g.Name, groupName, true) == 0)
                                {
                                    result = true;
                                    break;
                                }
                            }
                            if (result)
                            {
                                break;
                            }
                        }
                        web.AllowUnsafeUpdates = tempAllowUnsafeUpdates;
                    }
                });
            }
            catch (Exception ex)
            {
                throw new Exception("CurrentUserIsAdminGroup Error: " + ex.Message);
            }

            return result;
        }
        public bool IsInGroup(string GroupName)
        {
            var result = false;
            var webURL = "W";
            UserData currentUser = CommonUtility.Instance.GetCurrentUser();

            HttpContext context = HttpContext.Current;
            HttpContext.Current = null;

            SPSecurity.RunWithElevatedPrivileges(() =>
            {
                using (var site = new SPSite(webURL))
                using (var web = site.OpenWeb())
                {
                    var tempAllowUnsafeUpdates = web.AllowUnsafeUpdates;
                    web.AllowUnsafeUpdates = true;

                    var spUser = web.EnsureUser(currentUser.LoginName);
                    if (GroupName.IndexOf(",") == -1)
                    {
                        foreach (SPGroup g in spUser.Groups)
                        {
                            if (string.Compare(g.Name, GroupName, true) == 0)
                            {
                                result = true;
                                break;
                            }
                        }
                    }
                    else
                    {
                        var groups = GroupName.Split(',');
                        foreach (var group in groups)
                        {
                            foreach (SPGroup g in spUser.Groups)
                            {
                                if (string.Compare(g.Name, group, true) == 0)
                                {
                                    result = true;
                                    break;
                                }
                            }
                            if (result)
                            {
                                break;
                            }
                        }
                    }

                    web.AllowUnsafeUpdates = tempAllowUnsafeUpdates;
                }
            });
            HttpContext.Current = context;
            return result;
        }

        public bool HasGroup(string GroupName)
        {
            var result = false;
            var webURL = "W";

            HttpContext context = HttpContext.Current;
            HttpContext.Current = null;

            SPSecurity.RunWithElevatedPrivileges(() =>
            {
                using (var site = new SPSite(webURL))
                using (var web = site.OpenWeb())
                {
                    var tempAllowUnsafeUpdates = web.AllowUnsafeUpdates;
                    web.AllowUnsafeUpdates = true;

                    foreach (SPGroup g in web.Groups)
                    {
                        if (string.Compare(g.Name, GroupName, true) == 0)
                        {
                            result = true;
                            break;
                        }
                    }

                    web.AllowUnsafeUpdates = tempAllowUnsafeUpdates;
                }
            });
            HttpContext.Current = context;
            return result;
        }

        public bool IsDepartmentAdmin()
        {
            var result = false;
            var webURL = "W";
            var DepartmentAdminGroup = "G2";
            UserData currentUser = CommonUtility.Instance.GetCurrentUser();

            HttpContext context = HttpContext.Current;
            HttpContext.Current = null;

            SPSecurity.RunWithElevatedPrivileges(() =>
            {
                using (var site = new SPSite(webURL))
                using (var web = site.OpenWeb())
                {
                    var tempAllowUnsafeUpdates = web.AllowUnsafeUpdates;
                    web.AllowUnsafeUpdates = true;

                    var spUser = web.EnsureUser(currentUser.LoginName);
                    foreach (SPGroup g in spUser.Groups)
                    {
                        if (g.Name.StartsWith(DepartmentAdminGroup))
                        {
                            result = true;
                            break;
                        }
                    }

                    web.AllowUnsafeUpdates = tempAllowUnsafeUpdates;
                }
            });
            HttpContext.Current = context;
            return result;
        }
    }

    public class UserData
    {
        public string LoginName { get; set; }
        public string FullName { get; set; }
        public string EmailAddr { get; set; }

        public bool IsTeamLead { get; set; }
        public bool IsDepartmentAdmin { get; set; }
        public bool IsIAAdmin { get; set; }
        public bool IsSurveyAdmin { get; set; }
        public bool IsAdmin { get; set; }
    }
}
