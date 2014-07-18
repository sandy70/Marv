using Telerik.Windows.Controls.GridView;

namespace Marv.Input
{
    public static class Extensions
    {
        public static CellModel ToModel(this GridViewCell cell)
        {
            return new CellModel(cell);
        }
    }
}