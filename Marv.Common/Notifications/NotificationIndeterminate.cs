using NLog;
using System;

namespace Marv.Common
{
    public class NotificationIndeterminate : NotificationBase
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public NotificationIndeterminate()
            : base()
        {
            this.IsIndeterminate = true;
        }

        public override void Start()
        {
            // do nothing
        }
    }
}