﻿using System;
using System.ComponentModel;

namespace Marv.Common.CorLas
{
    public class CorLas
    {
        /*********************************************************
         * profile = 1: detailed profile of depth versus length
         * profile = 2: depth and length of semi-ellipse only
         * profile = 3: depth and length of rectangle only
         * flow strength formula = 0: Flow Strength = YieldStrength + 10000
         * flow strength formula = 1: Flow Strength = Yieldstrength + FlowConstant * (UltimateStrength - YieldStrength)
         * yield strength must be between 1 and 147,500
         * flaws: if profile > 1 this can be null; otherwise it should be a list of flaw profile data
         * max length & depth: if profile = 1 then these can be zero; otherwise dimensions of semi-eliptical or rectangular flaw should be provided
         * 0 < maxlength
         * 0 < maxdepth < wt
         * od = Outside Diameter of pipe (must be between 2*wt and 9999.99)
         * wt = Wall Thickness of pipe (must be between flaw depth and od/2)
         * yFactor = Hoop Stress Y Factor
         *
         * Flow strength can not be > tensile strength
         * Flaw length can not be < 2 * flaw depth
         * Flaw length can not be > 3 * circumference
         * Ratio of yield strength to tensile strength can not be < 0.5 or > 1.0
         **/

        public static double CrackPredictedCriticalPressure(int profile,
                                                            int flowStrengthFormula,
                                                            double yieldStrength,
                                                            double ultimateStrength,
                                                            double flowConstant,
                                                            BindingList<Flaw> flaws,
                                                            double maxLength,
                                                            double maxDepth,
                                                            double od,
                                                            double wt,
                                                            double yFactor,
                                                            string jFract,
                                                            string location,
                                                            double mop,
                                                            double tMod,
                                                            double eMod,
                                                            double hExp,
                                                            double fTough,
                                                            int alloyGrp)
        {
            var ftmax = new EffectiveFlawData();

            double qfactor, qfactor1;
            double xx, xx1;
            double l2DivDt;
            double foliasm;
            double flowstr;

            if (flowStrengthFormula == 0)
            {
                flowstr = yieldStrength + 10000;
            }
            else
            {
                flowstr = yieldStrength + flowConstant * (ultimateStrength - yieldStrength);
            }

            ftmax.JValue = 0.0;

            //effs = new List<EffectiveFlawData>();

            if (profile == 1)
            {
                var delarea = new double[flaws.Count - 1];
                for (var i = 0; i < delarea.Length; i++)
                {
                    delarea[i] = 0.5 * (flaws[i + 1].Length - flaws[i].Length) * (flaws[i + 1].Depth + flaws[i].Depth);
                }

                for (var nstart = 0; nstart < flaws.Count - 1; nstart++)
                {
                    var areatot = 0.0;

                    for (var i = nstart; i < flaws.Count - 1; i++)
                    {
                        areatot += delarea[i];
                        var efd = new EffectiveFlawData();
                        efd.Area = areatot;
                        efd.Length = flaws[i + 1].Length - flaws[nstart].Length;
                        efd.CrackStart = flaws[nstart].Length;
                        l2DivDt = (efd.Length * efd.Length) / (od * wt);
                        foliasm = FoliasFact(l2DivDt);
                        efd.FlowStrStress = flowstr * (1.0 - efd.Area / (wt * efd.Length)) / (1.0 - efd.Area / (wt * efd.Length * foliasm));

                        var aspect = AspectRatio(efd.Length, efd.Area, profile);
                        var effDepth = EffectiveDepth(efd.Length, efd.Area, profile);

                        if (aspect >= 1.0)
                        {
                            xx = effDepth / efd.Length;
                            xx1 = (effDepth - 0.001) / efd.Length;
                        }
                        else
                        {
                            xx = 0.5;
                            xx1 = 0.5 * (effDepth - 0.001) / effDepth;
                        }
                        qfactor = Qefactor(xx, profile);
                        qfactor1 = Qefactor(xx1, profile);

                        //CritToughPres(prssop,prsscr,aspect,effdepth,rad,wallthk,flawloc,yfactor,outdia,iallygrp,yslb,emodf,shexpn,qfactor,qfactor1,jmin,safefct,safeprcrk,effval(j,5),effval(j,6),1,failcri,flowstr,tmodf,iprofiletype)
                        efd.JValue = CritToughPres(false,
                            profile,
                            jFract,
                            location,
                            tMod,
                            fTough,
                            hExp,
                            yieldStrength,
                            alloyGrp,
                            mop,
                            eMod,
                            aspect,
                            effDepth,
                            flowstr,
                            qfactor,
                            qfactor1,
                            od,
                            wt,
                            yFactor);

                        if (efd.JValue > ftmax.JValue)
                        {
                            ftmax = efd;
                        }
                    }
                }
            }

            if (profile >= 2)
            {
                ftmax.Length = maxLength; // alengthmax;
                ftmax.Area = EffectiveArea(maxLength, maxDepth, profile);
                ftmax.CrackStart = 0.0;
                l2DivDt = (ftmax.Length * ftmax.Length) / (od * wt);
                foliasm = FoliasFact(l2DivDt);
                ftmax.FlowStrStress = flowstr * (1.0 - ftmax.Area / (wt * ftmax.Length)) / (1.0 - ftmax.Area / (wt * ftmax.Length * foliasm));
            }

            var taspect = AspectRatio(ftmax.Length, ftmax.Area, profile);
            var teffDepth = EffectiveDepth(ftmax.Length, ftmax.Area, profile);
            if (taspect < 1.0)
            {
                taspect = 1.0;
            }
            xx = 0.5 / taspect;
            xx1 = (teffDepth - 0.001) / ftmax.Length;
            qfactor = Qefactor(xx, profile);
            qfactor1 = Qefactor(xx1, profile);

            return CritToughPres(true,
                profile,
                jFract,
                location,
                tMod,
                fTough,
                hExp,
                yieldStrength,
                alloyGrp,
                mop,
                eMod,
                taspect,
                teffDepth,
                flowstr,
                qfactor,
                qfactor1,
                od,
                wt,
                yFactor);
        }

        public static double FlawFailurePressure(int profile,
                                                 int flowStrengthFormula,
                                                 double yieldStrength,
                                                 double ultimateStrength,
                                                 double flowConstant,
                                                 BindingList<Flaw> flaws,
                                                 double maxLength,
                                                 double maxDepth,
                                                 double od,
                                                 double wt,
                                                 double yFactor)
        {
            double failpr;
            double flowstr;
            if (flowStrengthFormula == 0)
            {
                flowstr = yieldStrength + 10000;
            }
            else
            {
                flowstr = yieldStrength + flowConstant * (ultimateStrength - yieldStrength);
            }

            double l2DivDt;
            double foliasm;

            var fsmax = new EffectiveFlawData();
            fsmax.FlowStrStress = 999999.0;

            if (profile == 1)
            {
                double mD = 0;

                for (var i = 0; i < flaws.Count; i++)
                {
                    if (flaws[i].Depth > mD)
                    {
                        mD = flaws[i].Depth;
                    }
                }

                var delarea = new double[flaws.Count - 1];
                for (var i = 0; i < delarea.Length; i++)
                {
                    delarea[i] = 0.5 * (flaws[i + 1].Length - flaws[i].Length) * (flaws[i + 1].Depth + flaws[i].Depth);
                }

                for (var nstart = 0; nstart < flaws.Count - 1; nstart++)
                {
                    var areatot = 0.0;

                    for (var i = nstart; i < flaws.Count - 1; i++)
                    {
                        areatot += delarea[i];
                        var efd = new EffectiveFlawData();
                        efd.Area = areatot;
                        efd.Length = flaws[i + 1].Length - flaws[nstart].Length;
                        efd.CrackStart = flaws[nstart].Length;
                        l2DivDt = (efd.Length * efd.Length) / (od * wt);
                        foliasm = FoliasFact(l2DivDt);
                        efd.FlowStrStress = flowstr * (1.0 - efd.Area / (wt * efd.Length)) / (1.0 - efd.Area / (wt * efd.Length * foliasm));

                        if (efd.FlowStrStress < fsmax.FlowStrStress)
                        {
                            fsmax = efd;
                        }
                    }
                }
            }

            if (profile >= 2)
            {
                fsmax.Length = maxLength;
                fsmax.Area = EffectiveArea(maxLength, maxDepth, profile);
                fsmax.CrackStart = 0.0;
                l2DivDt = (fsmax.Length * fsmax.Length) / (od * wt);
                foliasm = FoliasFact(l2DivDt);
                fsmax.FlowStrStress = flowstr * (1.0 - fsmax.Area / (wt * fsmax.Length)) / (1.0 - fsmax.Area / (wt * fsmax.Length * foliasm));
            }

            failpr = fsmax.FlowStrStress / (od / (2.0 * wt) - yFactor);
            return failpr;
        }

        /*********************************************************
         * profile = 1: detailed profile of depth versus length
         * profile = 2: depth and length of semi-ellipse only
         * profile = 3: depth and length of rectangle only
         * flow strength formula = 0: Flow Strength = YieldStrength + 10000
         * flow strength formula = 1: Flow Strength = Yieldstrength + FlowConstant * (UltimateStrength - YieldStrength)
         * yield strength must be between 1 and 147,500
         * flaws: if profile > 1 this can be null; otherwise it should be a list of flaw profile data
         * max length & depth: if profile = 1 then these can be zero; otherwise dimensions of semi-eliptical or rectangular flaw should be provided
         * 0 < maxlength
         * 0 < maxdepth < wt
         * od = Outside Diameter of pipe (must be between 2*wt and 9999.99)
         * wt = Wall Thickness of pipe (must be between flaw depth and od/2)
         * yFactor = Hoop Stress Y Factor
         *
         * Flow strength can not be > tensile strength
         * Flaw length can not be < 2 * flaw depth
         * Flaw length can not be > 3 * circumference
         * Ratio of yield strength to tensile strength can not be < 0.5 or > 1.0
         *
         * jFract is either "JC" for J fracture toughness, or "TR" for tearing modulus
         * location is either "Inside" or "Outside"
         * MOP is maximum operating pressure
         * tMod is tearing modulus (125000.0*cvn/flowstr)
         * eMod is elastic modulus (usually 29500)
         * hExp is the strain hardness exponent (-0.00546 + 0.556 * YieldStrength/UltimateStrength - 0.547 * (YieldStrength/UltimateStrength) ^ 2)
         * fTough is the j fracture toughness (12 * cvn / 0.124)
         * alloyGrp = 1: API Steel alloys
         * alloyGrp > 1: ASTM Pipe or ASTM Steel alloys, or other
         **/

        private static double AspectRatio(double flnth, double flarea, int profile)
        {
            // flnth = flaw length
            // flarea = flaw profile area
            // profile = 1: detailed profile of depth versus length
            // profile = 2: depth and length of semi-ellipse only
            // profile = 3: depth and length of rectangle only

            if (profile <= 2)
            {
                return 0.3926991 * flnth * flnth / flarea;
            }
            return 0.5 * flnth * flnth / flarea;
        }

        private static double CritToughPres(bool retCrit,
                                            int profile,
                                            string jFractureToughnessCrit,
                                            string location,
                                            double tearingModulus,
                                            double fractureToughness,
                                            double strainHardeningExponent,
                                            double alloyYieldStrength,
                                            int alloyGroup,
                                            double mop,
                                            double elasticModulus,
                                            double aspect,
                                            double effDepth,
                                            double fslb,
                                            double qf,
                                            double qf1,
                                            double od,
                                            double wt,
                                            double yFactor)
        {
            var rad = 0.5 * od - wt * yFactor;
            var shexpn = 1.0 / strainHardeningExponent; // nexpnt(ialloy);
            var yslb = 0.001 * alloyYieldStrength; // ysaa(ialloy);

            double prmax;
            double prges, princ;

            //double prcr = 0;
            var critResults = 0.0;

            var f3N = (3.85 * Math.Sqrt(shexpn) * (1.0 - 1.0 / shexpn) + 3.14159 / shexpn) * (1.0 + 1.0 / shexpn);

            // compute folias factors
            var efflng = 2.0 * aspect * effDepth;
            var folias = efflng * efflng / (2.0 * rad * wt);
            var tm = FoliasFact(folias);
            var pm = FoliasSurface(effDepth, wt, tm, profile);
            var pm1 = FoliasSurface(effDepth - 0.001, wt, tm, profile);

            // compute maximum pressure at flow strength
            if (location.Equals("Inside") && profile <= 2)
            {
                prmax = (fslb / pm) / ((0.785398 * effDepth + 0.5 * od) / wt - yFactor);
            }
            else if (location.Equals("Inside") && profile == 3)
            {
                prmax = (fslb / pm) / ((effDepth + 0.5 * od) / wt - yFactor);
            }
            else
            {
                prmax = (fslb / pm) / (0.5 * od / wt - yFactor);
            }

            // ***********************************************************************
            // retCrit = false: compute J at operating pressure (prop)
            // retCrit = true:  compute critical pressure (prcr) at J toughness (jmin)
            // ***********************************************************************

            if (retCrit)
            {
                prges = prmax;
                princ = -0.5 * prmax;
            }
            else
            {
                prges = mop;
                princ = mop;
            }

            var i = 1;
            var done = false;
            while (!done && i <= 50)
            {
                var strs = 0.001 * prges * (0.5 * od / wt - yFactor);

                // compute local stress value
                var st = StressLocal(effDepth, prges, wt, strs, pm, location, profile);
                var st1 = StressLocal(effDepth - 0.001, prges, wt, strs, pm1, location, profile);
                st = Math.Min(st, 0.001 * fslb);
                st1 = Math.Min(st1, 0.001 * fslb);

                // compute plastic strain value
                double ep, ep1;
                if (alloyGroup - 1 == 0)
                {
                    ep = (0.005 - yslb / elasticModulus) * Math.Pow(st / yslb, shexpn);
                    ep1 = (0.005 - yslb / elasticModulus) * Math.Pow(st1 / yslb, shexpn);
                }
                else
                {
                    ep = 0.002 * Math.Pow(st / yslb, shexpn);
                    ep1 = 0.002 * Math.Pow(st1 / yslb, shexpn);
                }

                // compute back-face correction factor
                var fsfact = FsFactor(effDepth, wt, efflng);
                var fsfact1 = FsFactor(effDepth - 0.001, wt, efflng);

                // compute value of J integral
                var jval = 1000.0 * effDepth * qf * fsfact * (3.14159 * st * st / elasticModulus + f3N * st * ep);
                var valj1 = 1000.0 * (effDepth - 0.001) * qf1 * fsfact1 * (3.14159 * st1 * st1 / elasticModulus + f3N * st1 * ep1);
                var djda = (jval - valj1) / 0.001;
                var valtap = 1000.0 * elasticModulus * djda / (fslb * fslb);

                // ********************************************************************72
                // check jval against jmin and exit loop or make new guess and continue
                if (!retCrit)
                {
                    //double valjop, valtop;
                    var jvalResults = jval;

                    //results.TearingValue = valtap;
                    return jvalResults;
                }

                if (i == 1 && jFractureToughnessCrit.Equals("TR") && valtap < tearingModulus)
                {
                    critResults = prges;
                    done = true; //go to 100;
                }
                else if (i == 1 && jFractureToughnessCrit.Equals("JC") && jval < fractureToughness)
                {
                    critResults = prges;
                    done = true;
                }

                if ((jFractureToughnessCrit.Equals("TR")) && (Math.Abs(valtap - tearingModulus) < 0.5) && (jval < fractureToughness))
                {
                    jFractureToughnessCrit = "JC";
                    prges = prmax;
                    princ = -0.5 * prmax;
                    i = 1;
                    done = false;
                }
                else if ((jFractureToughnessCrit.Equals("TR")) && (Math.Abs(valtap - tearingModulus) < 0.5) && (jval >= fractureToughness))
                {
                    critResults = prges;
                    done = true;
                }
                else if ((jFractureToughnessCrit.Equals("JC")) && (Math.Abs(jval - fractureToughness) < 0.5))
                {
                    critResults = prges;
                    done = true;
                }
                else if (i == 50)
                {
                    //MessageBox.Show("Solution for critical pressure at this crack size did not converge!", "No Convergence", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else if ((princ > 0.0) && (fractureToughness < jval) && (jFractureToughnessCrit.Equals("JC")))
                {
                    princ = -0.5 * princ;
                }
                else if ((princ < 0.0) && (fractureToughness > jval) && (jFractureToughnessCrit.Equals("JC")))
                {
                    princ = -0.5 * princ;
                }
                else if ((princ > 0.0) && (tearingModulus < valtap) && (jFractureToughnessCrit.Equals("TR")))
                {
                    princ = -0.5 * princ;
                }
                else if ((princ < 0.0) && (tearingModulus > valtap) && (jFractureToughnessCrit.Equals("TR")))
                {
                    princ = -0.5 * princ;
                }

                prges = prges + princ;
                if (prges < 0.01)
                {
                    prges = 0.01;
                }
                else if (prges > prmax)
                {
                    prges = prmax;
                }

                i++;
            }
            return critResults;
        }

        private static double EffectiveArea(double flnth, double fldpth, double profile)
        {
            // flnth = flaw length
            // fldpth = flaw depth
            // profile = 1: detailed profile of depth versus length
            // profile = 2: depth and length of semi-ellipse only
            // profile = 3: depth and length of rectangle only

            if (profile <= 2)
            {
                return 0.785398 * flnth * fldpth;
            }
            return flnth * fldpth;
        }

        private static double EffectiveDepth(double flnth, double flarea, int profile)
        {
            // flnth = flaw length
            // flarea = flaw profile area
            // profile = 1: detailed profile of depth versus length
            // profile = 2: depth and length of semi-ellipse only
            // profile = 3: depth and length of rectangle only

            if (profile <= 2)
            {
                return 1.27324 * flarea / flnth;
            }
            return flarea / flnth;
        }

        private static double FoliasFact(double sqlovdt)
        {
            // sqlovdt = length squared over (diameter x wall thickness)

            if (sqlovdt <= 50.0)
            {
                return Math.Sqrt(1 + 0.6275 * sqlovdt - 0.003375 * sqlovdt * sqlovdt);
            }
            return 0.032 * sqlovdt + 3.3;
        }

        private static double FoliasSurface(double a, double wt, double tm, double profile)
        {
            // a = flaw depth
            // wt = wall thickness
            // tm = Folias factor
            // profile = 1: detailed profile of depth versus length
            // profile = 2: depth and length of semi-ellipse only
            // profile = 3: depth and length of rectangle only

            if (profile <= 2)
            {
                return (1.0 - 0.785398 * a / (wt * tm)) / (1.0 - 0.785398 * a / wt);
            }
            return (1.0 - a / (wt * tm)) / (1.0 - a / wt);
        }

        private static double FsFactor(double a, double thk, double clen)
        {
            // a = crack depth
            // thk = wall thickness
            // clen = effective crack length

            if (a / thk <= 0.01)
            {
                return 1.0;
            }
            if (a / thk <= 0.95)
            {
                return (0.63662 * thk / a) * Math.Tan(1.5708 * a / thk) * (1 - 2.0 * a / clen) + 2.0 * a / clen;
            }
            return (8.515 + (a / thk - 0.95) * 162.0 / thk) * (1 - 2.0 * a / clen) + 2.0 * a / clen;
        }

        private static double Qefactor(double dol, int profile)
        {
            // dol = crack depth over length
            // profile = 1: detailed profile of depth versus length
            // profile = 2: depth and length of semi-ellipse only
            // profile = 3: depth and length of rectangle only

            if (profile <= 2)
            {
                return 1.2581 - 0.20589 * dol - 11.493 * dol * dol + 29.586 * Math.Pow(dol, 3) - 23.584 * Math.Pow(dol, 4);
            }
            return 1.2581;
        }

        private static double StressLocal(double a, double pr, double wt, double sn, double pm, string flwloc, int profile)
        {
            // a = flaw depth
            // pr = operating pressure
            // wt = wall thickness
            // sn = nominal operating stress
            // pm = surface flaw Folias factor
            // flwloc = flaw location (inside or outside)
            // profile = 1: detailed profile of depth versus length
            // profile = 2: depth and length of semi-ellipse only
            // profile = 3: depth and length of rectangle only

            // character*7 flwloc
            if (flwloc.Equals("Inside") && profile <= 2)
            {
                return ((0.785398 * a * 0.001 * pr) / wt + sn) * pm;
            }
            if (flwloc.Equals("Inside") && profile == 3)
            {
                return ((a * 0.001 * pr) / wt + sn) * pm;
            }
            return sn * pm;
        }
    }
}