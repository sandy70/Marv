﻿namespace Marv.Common
{
    public class SoftEvidence : IEvidence
    {
        public double[] Evidence { get; set; }

        public string SynergiString { get; set; }

        public void Set(Graph graph, string vertexKey)
        {
            graph.SetEvidence(vertexKey, this.Evidence);
        }
    }
}