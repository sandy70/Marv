using System;
using System.Collections;
using System.Collections.Generic;

namespace LibPipeline
{
    public enum SegmentPosition { BeforeStart, Start, Middle, End, AfterEnd }

    public class MultiLocationSegmentEnumerator : IEnumerator<Location>
    {
        private Location current;
        private MultiLocationSegment segment;
        private SegmentPosition segmentPosition = SegmentPosition.BeforeStart;

        public MultiLocationSegmentEnumerator(MultiLocationSegment aSegment)
        {
            this.segment = aSegment;
        }

        public Location Current
        {
            get
            {
                return this.current;
            }
        }

        object IEnumerator.Current
        {
            get
            {
                return this.current;
            }
        }

        public void Dispose()
        {
           // do nothing
        }

        public bool MoveNext()
        {
            if (this.segmentPosition == SegmentPosition.BeforeStart)
            {
                if (this.segment.Start != null)
                {
                    this.current = this.segment.Start;
                    this.segmentPosition = SegmentPosition.Start;
                    return true;
                }
                else if (this.segment.Middle != null)
                {
                    this.current = this.segment.Middle;
                    this.segmentPosition = SegmentPosition.Middle;
                    return true;
                }
                else if (this.segment.End != null)
                {
                    this.current = this.segment.End;
                    this.segmentPosition = SegmentPosition.End;
                    return true;
                }
                else
                {
                    this.segmentPosition = SegmentPosition.AfterEnd;
                    return false;
                }
            }
            else if (this.segmentPosition == SegmentPosition.Start)
            {
                if (this.segment.Middle != null)
                {
                    this.current = this.segment.Middle;
                    this.segmentPosition = SegmentPosition.Middle;
                    return true;
                }
                else if (this.segment.End != null)
                {
                    this.current = this.segment.End;
                    this.segmentPosition = SegmentPosition.End;
                    return true;
                }
                else
                {
                    this.segmentPosition = SegmentPosition.AfterEnd;
                    return false;
                }
            }
            else if (this.segmentPosition == SegmentPosition.Middle)
            {
                if (this.segment.End != null)
                {
                    this.current = this.segment.End;
                    this.segmentPosition = SegmentPosition.End;
                    return true;
                }
                else
                {
                    this.segmentPosition = SegmentPosition.AfterEnd;
                    return false;
                }
            }
            else if (this.segmentPosition == SegmentPosition.End)
            {
                this.segmentPosition = SegmentPosition.AfterEnd;
                return false;
            }
            else
            {
                return false;
            }
        }

        public void Reset()
        {
            throw new NotSupportedException();
        }
    }
}