using _CSRF_Library.Data;
using _CSRF_Library.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _CSRF_Library.Manager
{
    public class MemberRegistrationManager
    {
        public string CreateMemberProfileImage(MemberProfileImageData data)
        {
            string result = string.Empty;
            try
            {
                CommonUtility.Instance.SqlExec((conn, comm) =>
                {
                    comm.CommandText = "usp_M_A1";
                    comm.Parameters.Clear();
                    comm.Parameters.AddWithValueWithCheckDbNull("@MemberLoginName", data.MemberLoginName);
                    comm.Parameters.AddWithValueWithCheckDbNull("@MemberPicture", data.MemberPicture);
                    comm.Parameters.AddWithValueWithCheckDbNull("@CreatedByLoginName", data.CreatedByLoginName);
                    comm.Parameters.AddWithValueWithCheckDbNull("@CompanyID", data.CompanyID);
                    comm.Parameters.AddWithValueWithCheckDbNull("@ProfileFrom", "Control");

                    result = comm.ExecuteScalar().ForceToString();
                });
            }
            catch (Exception ex)
            {
                throw new Exception("CreateMemberProfileImage Error: " + ex.Message);
            }
            return result;
        }
        public MemoryStream GetProfileImageFromUpload(string MemberProfileImageGUID)
        {
            MemoryStream s = new MemoryStream();
            try
            {
                var imgByte = GetImageByteFromUpload(MemberProfileImageGUID);
                if (imgByte.Length > 0)
                {
                    s = new MemoryStream(imgByte);
                    s.Position = 0;
                }
                else
                {
                    throw new Exception("Image byte array is null or empty.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("GetProfileImageFromUpload Error: " + ex.Message);
            }
            return s;
        }
        private byte[] GetImageByteFromUpload(string MemberProfileImageGUID)
        {
            var result = new byte[0];
            try
            {
                CommonUtility.Instance.SqlExec((conn, comm) =>
                {
                    comm.CommandText = "usp_M_A2";
                    comm.Parameters.AddWithValueWithCheckDbNull("@MemberProfileImageGUID", MemberProfileImageGUID);

                    using (var reader = comm.ExecuteReader())
                    {
                        if (reader.Read() && reader.HasRows)
                        {
                            result = reader["MemberPicture"].ForceToByte();
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                throw new Exception("GetImageByteFromUpload Error: " + ex.Message);
            }
            return result;
        }
    }
}
