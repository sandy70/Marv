using NLog;

namespace Marv.Common
{
    public class NotificationIndeterminate : Notification
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public NotificationIndeterminate()
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