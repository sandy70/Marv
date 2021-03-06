﻿using System;

namespace Marv.Common.Interpolators
{
    /// <summary>
    ///     Cubic spline interpolation.
    ///     Call Fit (or use the corrector constructor) to compute spline coefficients, then Eval to evaluate the spline at
    ///     other X coordinates.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         This is implemented based on the wikipedia article:
    ///         http://en.wikipedia.org/wiki/Spline_interpolation
    ///         I'm not sure I have the right to include a copy of the article so the equation numbers referenced in
    ///         comments will end up being wrong at some point.
    ///     </para>
    ///     <para>
    ///         This is not optimized, and is not MT safe.
    ///         This can extrapolate off the ends of the splines.
    ///         You must provide points in X sort order.
    ///     </para>
    /// </remarks>
    public class CubicSplineInterpolator : IInterpolator
    {
        #region Fields

        // N-1 spline coefficients for N points
        private float[] a;
        private float[] b;

        // Save the original x and y for Eval
        private float[] xOrig;
        private float[] yOrig;

        #endregion

        #region Ctor

        /// <summary>
        ///     Default ctor.
        /// </summary>
        public CubicSplineInterpolator() {}

        /// <summary>
        ///     Construct and call Fit.
        /// </summary>
        /// <param name="x">Input. X coordinates to fit.</param>
        /// <param name="y">Input. Y coordinates to fit.</param>
        /// <param name="startSlope">Optional slope constraint for the first point. Single.NaN means no constraint.</param>
        /// <param name="endSlope">Optional slope constraint for the final point. Single.NaN means no constraint.</param>
        /// <param name="debug">Turn on console output. Default is false.</param>
        public CubicSplineInterpolator(float[] x, float[] y, float startSlope = float.NaN, float endSlope = float.NaN, bool debug = false)
        {
            this.Fit(x, y, startSlope, endSlope, debug);
        }

        #endregion

        #region Private Methods

        private int lastIndex;

        /// <summary>
        ///     Throws if Fit has not been called.
        /// </summary>
        private void CheckAlreadyFitted()
        {
            if (this.a == null)
            {
                throw new Exception("Fit must be called before you can evaluate.");
            }
        }

        /// <summary>
        ///     Evaluate the specified x value using the specified spline.
        /// </summary>
        /// <param name="x">The x value.</param>
        /// <param name="j">Which spline to use.</param>
        /// <param name="debug">Turn on console output. Default is false.</param>
        /// <returns>The y value.</returns>
        private float EvalSpline(float x, int j, bool debug = false)
        {
            var dx = this.xOrig[j + 1] - this.xOrig[j];
            var t = (x - this.xOrig[j]) / dx;
            var y = (1 - t) * this.yOrig[j] + t * this.yOrig[j + 1] + t * (1 - t) * (this.a[j] * (1 - t) + this.b[j] * t); // equation 9
            if (debug)
            {
                Console.WriteLine("xs = {0}, j = {1}, t = {2}", x, j, t);
            }
            return y;
        }

        /// <summary>
        ///     Find where in xOrig the specified x falls, by simultaneous traverse.
        ///     This allows xs to be less than x[0] and/or greater than x[n-1]. So allows extrapolation.
        ///     This keeps state, so requires that x be sorted and xs called in ascending order, and is not multi-thread safe.
        /// </summary>
        private int GetNextXIndex(float x)
        {
            if (x < this.xOrig[this.lastIndex])
            {
                throw new ArgumentException("The X values to evaluate must be sorted.");
            }

            while ((this.lastIndex < this.xOrig.Length - 2) && (x > this.xOrig[this.lastIndex + 1]))
            {
                this.lastIndex++;
            }

            return this.lastIndex;
        }

        #endregion

        #region Fit*

        /// <summary>
        ///     Compute spline coefficients for the specified x,y points.
        ///     This does the "natural spline" style for ends.
        ///     This can extrapolate off the ends of the splines.
        ///     You must provide points in X sort order.
        /// </summary>
        /// <param name="x">Input. X coordinates to fit.</param>
        /// <param name="y">Input. Y coordinates to fit.</param>
        /// <param name="startSlope">Optional slope constraint for the first point. Single.NaN means no constraint.</param>
        /// <param name="endSlope">Optional slope constraint for the final point. Single.NaN means no constraint.</param>
        /// <param name="debug">Turn on console output. Default is false.</param>
        public void Fit(float[] x, float[] y, float startSlope = float.NaN, float endSlope = float.NaN, bool debug = false)
        {
            if (Single.IsInfinity(startSlope) || Single.IsInfinity(endSlope))
            {
                throw new Exception("startSlope and endSlope cannot be infinity.");
            }

            // Save x and y for eval
            this.xOrig = x;
            this.yOrig = y;

            var n = x.Length;
            var r = new float[n]; // the right hand side numbers: wikipedia page overloads b

            var m = new TriDiagonalMatrixF(n);
            float dx1, dx2, dy1, dy2;

            // First row is different (equation 16 from the article)
            if (float.IsNaN(startSlope))
            {
                dx1 = x[1] - x[0];
                m.C[0] = 1.0f / dx1;
                m.B[0] = 2.0f * m.C[0];
                r[0] = 3 * (y[1] - y[0]) / (dx1 * dx1);
            }
            else
            {
                m.B[0] = 1;
                r[0] = startSlope;
            }

            // Body rows (equation 15 from the article)
            for (var i = 1; i < n - 1; i++)
            {
                dx1 = x[i] - x[i - 1];
                dx2 = x[i + 1] - x[i];

                m.A[i] = 1.0f / dx1;
                m.C[i] = 1.0f / dx2;
                m.B[i] = 2.0f * (m.A[i] + m.C[i]);

                dy1 = y[i] - y[i - 1];
                dy2 = y[i + 1] - y[i];
                r[i] = 3 * (dy1 / (dx1 * dx1) + dy2 / (dx2 * dx2));
            }

            // Last row also different (equation 17 from the article)
            if (float.IsNaN(endSlope))
            {
                dx1 = x[n - 1] - x[n - 2];
                dy1 = y[n - 1] - y[n - 2];
                m.A[n - 1] = 1.0f / dx1;
                m.B[n - 1] = 2.0f * m.A[n - 1];
                r[n - 1] = 3 * (dy1 / (dx1 * dx1));
            }
            else
            {
                m.B[n - 1] = 1;
                r[n - 1] = endSlope;
            }

            if (debug)
            {
                Console.WriteLine("Tri-diagonal matrix:\n{0}", m.ToDisplayString(":0.0000", "  "));
            }
            if (debug)
            {
                Console.WriteLine("r: {0}", r.String());
            }

            // k is the solution to the matrix
            var k = m.Solve(r);
            if (debug)
            {
                Console.WriteLine("k = {0}", k.String());
            }

            // a and b are each spline's coefficients
            this.a = new float[n - 1];
            this.b = new float[n - 1];

            for (var i = 1; i < n; i++)
            {
                dx1 = x[i] - x[i - 1];
                dy1 = y[i] - y[i - 1];
                this.a[i - 1] = k[i - 1] * dx1 - dy1; // equation 10 from the article
                this.b[i - 1] = -k[i] * dx1 + dy1; // equation 11 from the article
            }

            if (debug)
            {
                Console.WriteLine("a: {0}", this.a.String());
            }
            if (debug)
            {
                Console.WriteLine("b: {0}", this.b.String());
            }
        }

        /// <summary>
        ///     Fit x,y and then eval at points xs and return the corresponding y's.
        ///     This does the "natural spline" style for ends.
        ///     This can extrapolate off the ends of the splines.
        ///     You must provide points in X sort order.
        /// </summary>
        /// <param name="x">Input. X coordinates to fit.</param>
        /// <param name="y">Input. Y coordinates to fit.</param>
        /// <param name="xs">Input. X coordinates to evaluate the fitted curve at.</param>
        /// <param name="startSlope">Optional slope constraint for the first point. Single.NaN means no constraint.</param>
        /// <param name="endSlope">Optional slope constraint for the final point. Single.NaN means no constraint.</param>
        /// <param name="debug">Turn on console output. Default is false.</param>
        /// <returns>The computed y values for each xs.</returns>
        public float[] FitAndEval(float[] x, float[] y, float[] xs, float startSlope = float.NaN, float endSlope = float.NaN, bool debug = false)
        {
            this.Fit(x, y, startSlope, endSlope, debug);
            return this.Eval(xs, debug);
        }

        #endregion

        #region Eval*

        public double Eval(double x)
        {
            return this.Eval(new[]
            {
                (float) x
            })[0];
        }

        /// <summary>
        ///     Evaluate the spline at the specified x coordinates.
        ///     This can extrapolate off the ends of the splines.
        ///     You must provide X's in ascending order.
        ///     The spline must already be computed before calling this, meaning you must have already called Fit() or
        ///     FitAndEval().
        /// </summary>
        /// <param name="x">Input. X coordinates to evaluate the fitted curve at.</param>
        /// <param name="debug">Turn on console output. Default is false.</param>
        /// <returns>The computed y values for each x.</returns>
        public float[] Eval(float[] x, bool debug = false)
        {
            this.CheckAlreadyFitted();

            var n = x.Length;
            var y = new float[n];
            this.lastIndex = 0; // Reset simultaneous traversal in case there are multiple calls

            for (var i = 0; i < n; i++)
            {
                // Find which spline can be used to compute this x (by simultaneous traverse)
                var j = this.GetNextXIndex(x[i]);

                // Evaluate using j'th spline
                y[i] = this.EvalSpline(x[i], j, debug);
            }

            return y;
        }

        /// <summary>
        ///     Evaluate (compute) the slope of the spline at the specified x coordinates.
        ///     This can extrapolate off the ends of the splines.
        ///     You must provide X's in ascending order.
        ///     The spline must already be computed before calling this, meaning you must have already called Fit() or
        ///     FitAndEval().
        /// </summary>
        /// <param name="x">Input. X coordinates to evaluate the fitted curve at.</param>
        /// <param name="debug">Turn on console output. Default is false.</param>
        /// <returns>The computed y values for each x.</returns>
        public float[] EvalSlope(float[] x, bool debug = false)
        {
            this.CheckAlreadyFitted();

            var n = x.Length;
            var qPrime = new float[n];
            this.lastIndex = 0; // Reset simultaneous traversal in case there are multiple calls

            for (var i = 0; i < n; i++)
            {
                // Find which spline can be used to compute this x (by simultaneous traverse)
                var j = this.GetNextXIndex(x[i]);

                // Evaluate using j'th spline
                var dx = this.xOrig[j + 1] - this.xOrig[j];
                var dy = this.yOrig[j + 1] - this.yOrig[j];
                var t = (x[i] - this.xOrig[j]) / dx;

                // From equation 5 we could also compute q' (qp) which is the slope at this x
                qPrime[i] = dy / dx
                            + (1 - 2 * t) * (this.a[j] * (1 - t) + this.b[j] * t) / dx
                            + t * (1 - t) * (this.b[j] - this.a[j]) / dx;

                if (debug)
                {
                    Console.WriteLine("[{0}]: xs = {1}, j = {2}, t = {3}", i, x[i], j, t);
                }
            }

            return qPrime;
        }

        #endregion

        #region Static Methods

        /// <summary>
        ///     Static all-in-one method to fit the splines and evaluate at X coordinates.
        /// </summary>
        /// <param name="x">Input. X coordinates to fit.</param>
        /// <param name="y">Input. Y coordinates to fit.</param>
        /// <param name="xs">Input. X coordinates to evaluate the fitted curve at.</param>
        /// <param name="startSlope">Optional slope constraint for the first point. Single.NaN means no constraint.</param>
        /// <param name="endSlope">Optional slope constraint for the final point. Single.NaN means no constraint.</param>
        /// <param name="debug">Turn on console output. Default is false.</param>
        /// <returns>The computed y values for each xs.</returns>
        public static float[] Compute(float[] x, float[] y, float[] xs, float startSlope = float.NaN, float endSlope = float.NaN, bool debug = false)
        {
            var spline = new CubicSplineInterpolator();
            return spline.FitAndEval(x, y, xs, startSlope, endSlope, debug);
        }

        /// <summary>
        ///     Fit the input x,y points using a 'geometric' strategy so that y does not have to be a single-valued
        ///     function of x.
        /// </summary>
        /// <param name="x">Input x coordinates.</param>
        /// <param name="y">Input y coordinates, do not need to be a single-valued function of x.</param>
        /// <param name="nOutputPoints">How many output points to create.</param>
        /// <param name="xs">Output (interpolated) x values.</param>
        /// <param name="ys">Output (interpolated) y values.</param>
        public static void FitGeometric(float[] x, float[] y, int nOutputPoints, out float[] xs, out float[] ys)
        {
            // Compute distances
            var n = x.Length;
            var dists = new float[n]; // cumulative distance
            dists[0] = 0;
            float totalDist = 0;

            for (var i = 1; i < n; i++)
            {
                var dx = x[i] - x[i - 1];
                var dy = y[i] - y[i - 1];
                var dist = (float) Math.Sqrt(dx * dx + dy * dy);
                totalDist += dist;
                dists[i] = totalDist;
            }

            // Create 'times' to interpolate to
            var dt = totalDist / (nOutputPoints - 1);
            var times = new float[nOutputPoints];
            times[0] = 0;

            for (var i = 1; i < nOutputPoints; i++)
            {
                times[i] = times[i - 1] + dt;
            }

            // Spline fit both x and y to times
            var xSpline = new CubicSplineInterpolator();
            xs = xSpline.FitAndEval(dists, x, times);

            var ySpline = new CubicSplineInterpolator();
            ys = ySpline.FitAndEval(dists, y, times);
        }

        #endregion
    }
}