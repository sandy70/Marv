using System.Collections.Generic;

namespace Marv
{
    public class EffectiveFlawData
    {
        public double Area { get; set; }

        public double CrackStart { get; set; }

        public double FlowStrStress { get; set; }

        public double JValue { get; set; }

        public double Length { get; set; }

        public double Tearing { get; set; }

        public static IComparer<EffectiveFlawData> SortByFlowStrength
        {
            get
            {
                return new EffectiveFlawCompareFlow();
            }
        }

        public static IComparer<EffectiveFlawData> SortByJFractureToughness
        {
            get
            {
                return new EffectiveFlawCompareJfract();
            }
        }
    }

    internal class EffectiveFlawCompareFlow : IComparer<EffectiveFlawData>
    {
        public int Compare(EffectiveFlawData e1, EffectiveFlawData e2)
        {
            if (e1.FlowStrStress > e2.FlowStrStress)
            {
                return 1;
            }

            if (e1.FlowStrStress < e2.FlowStrStress)
            {
                return -1;
            }

            if (e1.CrackStart > e2.CrackStart)
            {
                return 1;
            }

            if (e1.CrackStart < e2.CrackStart)
            {
                return -1;
            }

            return 0;
        }
    }

    internal class EffectiveFlawCompareJfract : IComparer<EffectiveFlawData>
    {
        public int Compare(EffectiveFlawData e1, EffectiveFlawData e2)
        {
            if (e1.JValue < e2.JValue)
            {
                return 1;
            }

            if (e1.JValue > e2.JValue)
            {
                return -1;
            }

            if (e1.CrackStart > e2.CrackStart)
            {
                return 1;
            }

            if (e1.CrackStart < e2.CrackStart)
            {
                return -1;
            }

            return 0;
        }
    }
}