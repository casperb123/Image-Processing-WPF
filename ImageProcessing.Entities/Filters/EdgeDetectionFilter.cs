using System;
using System.Collections.Generic;
using System.Text;

namespace ImageProcessing.Entities.Filters
{
    public class EdgeDetectionFilter : ConvolutionFilterBase
    {
        public override double Factor { get; set; }
        public override double Bias { get; set; }
        public override double[,] FilterMatrix { get; set; }

        public EdgeDetectionFilter()
        {
            Factor = 1;
            FilterMatrix = new double[,]
            {
                { -1, -1, -1, },
                { -1,  8, -1, },
                { -1, -1, -1, }
            };
        }
    }
}
