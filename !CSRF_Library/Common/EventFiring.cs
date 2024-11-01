using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _CSRF_Library.Common
{
    public class EventFiring : SPItemEventReceiver
    {
        public void DisableHandleEventFiring()
        {
            this.EventFiringEnabled = false;
        }
        public void EnableHandleEventFiring()
        {
            this.EventFiringEnabled = true;

        }
    }
}
