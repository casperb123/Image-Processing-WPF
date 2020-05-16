﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ImageProcessing.Entities.Filters
{
    public class HighPass3x3Filter : ConvolutionFilterBase
    {
        public override string FilterName
        {
            get { return "HighPass3x3Filter"; }
        }

        private double factor = 1.0 / 16.0;
        public override double Factor
        {
            get { return factor; }
        }

        private double bias = 128.0;
        public override double Bias
        {
            get { return bias; }
        }

        private double[,] filterMatrix =
            new double[,] { { -1, -2, -1, },
                            { -2, 12, -2, },
                            { -1, -2, -1, }, };

        public override double[,] FilterMatrix
        {
            get { return filterMatrix; }
        }
    }
}
