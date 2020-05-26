using System;
using System.Collections.Generic;
using System.Text;

namespace ImageProcessing.Entities.Filters
{
    public class EmbossFilter : ConvolutionFilterBase
    {
        public override string FilterName { get; set; }
        public override double Factor { get; set; }
        public override double Bias { get; set; }
        public override double[,] FilterMatrix { get; set; }

        public EmbossFilter()
        {
            FilterName = "EmbossFilter";
            Factor = 1;
            Bias = 128;

            FilterMatrix = new double[,]
            {
                { 2, 0, 0 },
                { 0, -1, 0 },
                { 0, 0, -1 }
            };
        }
    }
}
