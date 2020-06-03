using System;
using System.Collections.Generic;
using System.Text;

namespace ImageProcessing.Entities.Filters
{
    public class EdgeDetectionHorizontalFilter : ConvolutionFilterBase
    {
        public override double Factor { get; set; }
        public override double Bias { get; set; }
        public override double[,] FilterMatrix { get; set; }

        public EdgeDetectionHorizontalFilter()
        {
            Factor = 1;
            FilterMatrix = new double[,]
            {
                {  0,  0,  0,  0,  0, },
                {  0,  0,  0,  0,  0, },
                { -1, -1,  2,  0,  0, },
                {  0,  0,  0,  0,  0, },
                {  0,  0,  0,  0,  0, }
            };
        }
    }
}
