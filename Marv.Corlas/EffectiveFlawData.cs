using System.Collections.Generic;

namespace Marv.Corlas
{
    public class EffectiveFlawData
    {
        private double _area;
        private double _crackstart;

        //effval(i,2) = effective crack area, in.
        private double _flow;

        //effval(i,3) = flow-strength failure stress for effective crack, psi
        //effval(i,4) = start of effective crack, in.
        private double _jvalue;

        private double _length;     //effval(i,1) = effective crack length, in.

        //effval(i,5) = applied value of J, lb/in.
        private double _tearing;    //effval(i,6) = applied value of tearing

        public double Area
        {
            get { return _area; }
            set { _area = value; }
        }

        public double CrackStart
        {
            get { return _crackstart; }
            set { _crackstart = value; }
        }

        public double FlowStrStress
        {
            get { return _flow; }
            set { _flow = value; }
        }

        public double JValue
        {
            get { return _jvalue; }
            set { _jvalue = value; }
        }

        public double Length
        {
            get { return _length; }
            set { _length = value; }
        }

        public double Tearing
        {
            get { return _tearing; }
            set { _tearing = value; }
        }

        #region comparable stuff

        public static IComparer<EffectiveFlawData> SortByFlowStrength
        {
            get { return new EffectiveFlawCompareFlow(); }
        }

        public static IComparer<EffectiveFlawData> SortByJFractureToughness
        {
            get { return new EffectiveFlawCompareJfract(); }
        }

        #endregion comparable stuff
    }

    internal class EffectiveFlawCompareFlow : IComparer<EffectiveFlawData>
    {
        public EffectiveFlawCompareFlow()
        {
        }

        public int Compare(EffectiveFlawData e1, EffectiveFlawData e2)
        {
            if (e1.FlowStrStress > e2.FlowStrStress)
            {
                return 1;
            }
            else if (e1.FlowStrStress < e2.FlowStrStress)
            {
                return -1;
            }
            else if (e1.CrackStart > e2.CrackStart)
            {
                return 1;
            }
            else if (e1.CrackStart < e2.CrackStart)
            {
                return -1;
            }
            else
            {
                return 0;
            }
        }
    }

    internal class EffectiveFlawCompareJfract : IComparer<EffectiveFlawData>
    {
        public EffectiveFlawCompareJfract()
        {
        }

        public int Compare(EffectiveFlawData e1, EffectiveFlawData e2)
        {
            if (e1.JValue < e2.JValue)
            {
                return 1;
            }
            else if (e1.JValue > e2.JValue)
            {
                return -1;
            }
            else if (e1.CrackStart > e2.CrackStart)
            {
                return 1;
            }
            else if (e1.CrackStart < e2.CrackStart)
            {
                return -1;
            }
            else
            {
                return 0;
            }
        }
    }
}