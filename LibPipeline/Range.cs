using System;

namespace LibPipeline
{
    public class Range<T> where T : IComparable<T>
    {
        /// <summary>
        /// Maximum value of the range
        /// </summary>
        public T Max { get; set; }

        /// <summary>
        /// Minimum value of the range
        /// </summary>
        public T Min { get; set; }

        /// <summary>
        /// Determines if another range is inside the bounds of this range
        /// </summary>
        /// <param name="Range">The child range to test</param>
        /// <returns>True if range is inside, else false</returns>
        public Boolean ContainsRange(Range<T> Range)
        {
            return this.IsValid() && Range.IsValid() && this.ContainsValue(Range.Min) && this.ContainsValue(Range.Max);
        }

        /// <summary>
        /// Determines if the provided value is inside the range
        /// </summary>
        /// <param name="value">The value to test</param>
        /// <returns>True if the value is inside Range, else false</returns>
        public Boolean ContainsValue(T value)
        {
            return (Min.CompareTo(value) <= 0) && (value.CompareTo(Max) <= 0);
        }

        /// <summary>
        /// Determines if this Range is inside the bounds of another range
        /// </summary>
        /// <param name="Range">The parent range to test on</param>
        /// <returns>True if range is inclusive, else false</returns>
        public Boolean IsInsideRange(Range<T> Range)
        {
            return this.IsValid() && Range.IsValid() && Range.ContainsValue(this.Min) && Range.ContainsValue(this.Max);
        }

        /// <summary>
        /// Determines if the range is valid
        /// </summary>
        /// <returns>True if range is valid, else false</returns>
        public Boolean IsValid()
        {
            return Min.CompareTo(Max) <= 0;
        }

        /// <summary>
        /// Presents the Range in readable format
        /// </summary>
        /// <returns>String representation of the Range</returns>
        public override string ToString()
        {
            return String.Format("[{0} - {1}]", Min, Max);
        }
    }
}