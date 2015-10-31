using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFPrototype.Commons.Downloads
{
    public class MinSizeSegmentCalculator : ISegmentCalculator
    {
        #region constructors
        public MinSizeSegmentCalculator()
        { }

        public MinSizeSegmentCalculator(long minSize)
        {
            this._minSize = minSize;
        }
        #endregion

        #region properties
        #region MinSize
        private long _minSize;
        /// <summary>
        /// Get or set <see cref="MinSize"/>
        /// </summary>
        public long MinSize
        {
            get { return _minSize; }
            set { _minSize = value; }
        }
        #endregion
        #endregion

        #region public methods

        public CalculatedSegment[] GetSegments(int count, RemoteFileInfo fileInfo)
        {
            if (count <= 0) throw new ArgumentException(string.Format("count:{0} can not smaller than 1.", count));
            if (fileInfo.Size <= 0) throw new InvalidOperationException(string.Format("fileInfo size:{0} not valid.", fileInfo.Size));
            if (this._minSize <= 0) throw new InvalidOperationException(string.Format("MinSizeSegmentCalculator.MinSize:{0} can not smaller than 1.", this._minSize));

            long segmentSize = fileInfo.Size / count;
            if (segmentSize < this._minSize)
            {
                segmentSize = this._minSize;
            }

            List<CalculatedSegment> segments = new List<CalculatedSegment>();
            long remainSize = fileInfo.Size;
            long currentSegmentSize = 0, offset = 0;
            do
            {
                currentSegmentSize = Math.Min(segmentSize, remainSize);
                segments.Add(new CalculatedSegment(offset, offset + currentSegmentSize));

                remainSize -= currentSegmentSize;
                offset += currentSegmentSize;
            } while (remainSize > 0);

            return segments.ToArray();
        }
        #endregion
    }
}
