using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _CSRF_Library.Data
{
    class Member
    {
        public Guid MemberGUID { get; set; }
        public string Company { get; set; }
        public string MemberType { get; set; }
        public string Element { get; set; }
        public string MemberRole { get; set; }
        public string Role { get; set; }
        public string MemberLoginName { get; set; }
        public string MemberNameTH { get; set; }
        public string MemberNameEN { get; set; }
        public string MemberNickName { get; set; }
        public string MemberPositionTH { get; set; }
        public string MemberPositionEN { get; set; }
        public string MemberDepartmentTH { get; set; }
        public string MemberDepartmentEN { get; set; }
        public string MemberEmail { get; set; }
        public string MemberTel { get; set; }
        public string MemberMobile { get; set; }
        public byte[] MemberPicture { get; set; }
        public byte[] MemberSignature { get; set; }
        public bool SyncWithHR { get; set; }
        public string CreatedByUserName { get; set; }
        public string CreatedByDisplayName { get; set; }
        public string ModifiedByUserName { get; set; }
        public string ModifiedByDisplayName { get; set; }
        public string AdditionalInfo { get; set; }
        public Guid? MemberProfileImageGUID { get; set; }
        public bool IsGreenBelt { get; set; }
        public string PermissionType { get; set; }

    }
    public class MemberProfileImageData
    {
        public Guid? MemberProfileImageGUID { get; set; }
        public string FileName { get; set; }
        public string MemberLoginName { get; set; }
        public byte[] MemberPicture { get; set; }
        public string CreatedByLoginName { get; set; }
        public string CompanyID { get; set; }
    }
}
