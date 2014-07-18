namespace Marv.Common
{
    public class NotificationIndeterminate : Notification
    {
        public NotificationIndeterminate()
        {
            this.IsIndeterminate = true;
        }

        public override void Open()
        {
            // do nothing
        }
    }
}