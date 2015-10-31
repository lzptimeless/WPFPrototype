using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFPrototype.Commons.Downloads
{
    public interface ISegmentCalculator
    {
        CalculatedSegment[] GetSegments(int count, RemoteFileInfo fileInfo);
    }
}
