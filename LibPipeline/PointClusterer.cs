using System.Collections.Generic;
using System.Windows;

namespace LibPipeline
{
    public class PointClusterer
    {
        public PointClusterer()
        {
            this.Threshold = 100;
        }

        public double Threshold { get; set; }

        public List<PointCluster> Cluster(List<Point> points)
        {
            List<PointCluster> clusters = new List<PointCluster>();

            // if 0 points have been received, return 0
            if (points.Count == 0)
                return clusters;

            // Add the first point to clusters
            PointCluster firstCluster = new PointCluster { Center = new Point { X = points[0].X, Y = points[0].Y }, Count = 1 };

            // Now loop over the rest of the list
            int nPoints = points.Count;

            foreach (var point in points.GetRange(1, nPoints - 1))
            {
                // Whether to add a new point after iterating over all clusters
                bool AddNewPoint = true;

                // Loop over all clusters
                foreach (var cluster in clusters)
                {
                    // Get the pixel coordinates of the center of this cluster
                    Point cCenter = cluster.Center;

                    // Calculate the distance in pixels between the location and the cluster
                    double distance = (point.X - cCenter.X) * (point.X - cCenter.X) + (point.Y - cCenter.Y) * (point.Y - cCenter.Y);

                    if (distance < this.Threshold)
                    {
                        // Increase the radius of this cluster
                        cluster.Count++;

                        // And don't add a new cluster when we break
                        AddNewPoint = false;
                        break;
                    }
                }

                if (AddNewPoint)
                {
                    PointCluster cluster = new PointCluster { Center = new Point { X = point.X, Y = point.Y }, Count = 1 };
                    clusters.Add(cluster);
                }
            }

            return clusters;
        }
    }
}