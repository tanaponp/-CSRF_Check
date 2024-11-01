using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _CSRF_Library.Common;
using Microsoft.SharePoint;

namespace _CSRF_Library.Utilities
{
    public partial class CommonUtility
    {
        public void SPExecRunWithElevatedPrivileges(string webURL, bool allowUnsafeUpdates, bool eventFiring, Action<SPSite, SPWeb> callback)
        {
            SPSecurity.RunWithElevatedPrivileges(() =>
            {
                SPExec(webURL, allowUnsafeUpdates, eventFiring, callback);
            });
        }

        public void SPExec(string webURL, bool allowUnsafeUpdates, bool eventFiring, Action<SPSite, SPWeb> callback)
        {
            using (var site = new SPSite(webURL))
            using (var web = site.OpenWeb())
            {
                if (allowUnsafeUpdates == true)
                {
                    site.AllowUnsafeUpdates = true;
                    web.AllowUnsafeUpdates = true;
                }

                EventFiring _eventFiring = null;
                if (eventFiring == true)
                {
                    _eventFiring = new EventFiring();
                    _eventFiring.DisableHandleEventFiring();
                }

                callback(site, web);

                if (eventFiring == true) _eventFiring.EnableHandleEventFiring();
                if (allowUnsafeUpdates == true)
                {
                    site.AllowUnsafeUpdates = false;
                    web.AllowUnsafeUpdates = false;
                }
            }
        }

        public SPUser SPCurrentUser
        {
            //get
            //{
            //    return SPContext.Current.Web.CurrentUser;
            //}
            get
            {
                SPUser currentUser = null;
                var WebURL = "";
                CommonUtility.Instance.SPExec(WebURL, true, false, (site, web) =>
                {
                    currentUser = web.CurrentUser;

                });

                return currentUser;
            }
        }

        public List<UserData> GetUserInSharePointGroup(string webURL, string groupName)
        {
            var result = new List<UserData>();
            var tmpUser = new List<UserData>();

            CommonUtility.Instance.SPExecRunWithElevatedPrivileges(webURL, true, false, (site, web) =>
            {
                var _users = web.Groups[groupName].Users;

                foreach (SPUser user in _users)
                {
                    var _userLogin = user.LoginName;
                    if (user.LoginName.IndexOf("|") > 0)
                    {
                        _userLogin = _userLogin.Split('|')[1];
                    }
                    var u = new UserData()
                    {
                        LoginName = _userLogin,
                        FullName = user.Name,
                        EmailAddr = user.Email
                    };

                    tmpUser.Add(u);
                }
            });

            foreach (var _u in tmpUser)
            {
                var user = CommonUtility.Instance.GetUserData(_u.LoginName);
                if (user == null) result.Add(_u);
                else result.Add(user);
            }

            return result;
        }
    }
}
