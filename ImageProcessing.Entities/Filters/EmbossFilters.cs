using System;
using System.Collections.Generic;
using System.Text;

namespace ImageProcessing.Entities.Filters
{
    public class EmbossFilter : ConvolutionFilterBase
    {
        public override double Factor { get; set; }
        public override double Bias { get; set; }
        public override double[,] FilterMatrix { get; set; }

        public EmbossFilter()
        {
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

    public class Emboss45DegreeFilter : ConvolutionFilterBase
    {
        public override double Factor { get; set; }
        public override double Bias { get; set; }
        public override double[,] FilterMatrix { get; set; }

        public Emboss45DegreeFilter()
        {
            Factor = 1;
            Bias = 128;

            FilterMatrix = new double[,]
            {
                { -1, -1, 0, },
                { -1,  0, 1, },
                {  0,  1, 1, }
            };
        }
    }

    public class EmbossTopLeftFilter : ConvolutionFilterBase
    {
        public override double Factor { get; set; }
        public override double Bias { get; set; }
        public override double[,] FilterMatrix { get; set; }

        public EmbossTopLeftFilter()
        {
            Factor = 1;
            Bias = 128;

            FilterMatrix = new double[,]
            {
                { -1, 0, 0, },
                {  0, 0, 0, },
                {  0, 0, 1, }
            };
        }
    }

    public class EmbossIntenseFilter : ConvolutionFilterBase
    {
        public override double Factor { get; set; }
        public override double Bias { get; set; }
        public override double[,] FilterMatrix { get; set; }

        public EmbossIntenseFilter()
        {
            Factor = 1;
            Bias = 128;

            FilterMatrix = new double[,]
            {
                { -1, -1, -1, -1,  0, },
                { -1, -1, -1,  0,  1, },
                { -1, -1,  0,  1,  1, },
                { -1,  0,  1,  1,  1, },
                {  0,  1,  1,  1,  1, }
            };
        }
    }
}
