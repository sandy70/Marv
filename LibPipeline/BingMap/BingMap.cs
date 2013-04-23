using Microsoft.Maps.MapControl.WPF;
using System.Windows.Input.Manipulations;
using System.Windows.Interactivity;

namespace LibPipeline
{
    public class BingMap : Map
    {
        public BingMap()
            : base()
        {
            this.SupportedManipulations = Manipulations2D.Scale | Manipulations2D.Translate;
            Interaction.GetBehaviors(this).Add(new BingMapUpdateBehavior());
        }

        public void Clear()
        {
            foreach (var child in this.Children)
            {
                IMapLayer mapLayer = child as IMapLayer;
                if (mapLayer != null)
                {
                    mapLayer.Clear();
                }
            }
        }

        public void GoToCanada()
        {
            this.GoToRect(new LocationRect(72, -142, 40, -47));
        }

        public void GoToKuwait()
        {
            this.GoToRect(new LocationRect(31, 46, 28, 49));
        }

        public void GoToRect(LocationRect locationRect)
        {
            this.SetView(locationRect);
            this.Heading = 0;
            this.Update();
        }

        public void GoToUsa()
        {
            this.GoToRect(new LocationRect(50, -125, 15, -65));
        }

        public void Update(bool force = false)
        {
            foreach (var child in this.Children)
            {
                IMapLayer mapLayer = child as IMapLayer;
                if (mapLayer != null)
                {
                    mapLayer.Update(this);
                }
            }
        }
    }
}