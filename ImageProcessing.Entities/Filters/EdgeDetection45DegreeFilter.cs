using System;
using System.Collections.Generic;
using System.Text;

namespace ImageProcessing.Entities.Filters
{
    public class EdgeDetection45DegreeFilter : ConvolutionFilterBase
    {
        public override double Factor { get; set; }
        public override double Bias { get; set; }
        public override double[,] FilterMatrix { get; set; }

        public EdgeDetection45DegreeFilter()
        {
            Factor = 1;
            FilterMatrix = new double[,]
            {
                { -1,  0,  0,  0,  0, },
                {  0, -2,  0,  0,  0, },
                {  0,  0,  6,  0,  0, },
                {  0,  0,  0, -2,  0, },
                {  0,  0,  0,  0, -1, }
            };
        }
    }
}
