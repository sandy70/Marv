using NLog;

namespace Marv.Common
{
    public class IndeterminateNotification : NotificationBase
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public IndeterminateNotification()
            : base()
        {
            this.IsIndeterminate = true;
        }

        public override void Open()
        {
            // do nothing
        }
    }
}