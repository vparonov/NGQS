using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GenMailServiceLibrary
{
    // Summary:
    //     Describes the delivery notification options for e-mail.
    [Flags]
    public enum DeliveryNotificationOptions
    {
        // Summary:
        //     No notification.
        None = 0,
        //
        // Summary:
        //     Notify if the delivery is successful.
        OnSuccess = 1,
        //
        // Summary:
        //     Notify if the delivery is unsuccessful.
        OnFailure = 2,
        //
        // Summary:
        //     Notify if the delivery is delayed
        Delay = 4,
        //
        // Summary:
        //     Never notify.
        Never = 134217728,
    }
}
