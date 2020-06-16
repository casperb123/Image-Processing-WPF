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

    public class EdgeDetectionTopLeftFilter : ConvolutionFilterBase
    {
        public override double Factor { get; set; }
        public override double Bias { get; set; }
        public override double[,] FilterMatrix { get; set; }

        public EdgeDetectionTopLeftFilter()
        {
            Factor = 1;
            FilterMatrix = new double[,]
            {
                { -5,  0,  0, },
                {  0,  0,  0, },
                {  0,  0,  5, }
            };
        }
    }

    public class EdgeDetectionVerticalFilter : ConvolutionFilterBase
    {
        public override double Factor { get; set; }
        public override double Bias { get; set; }
        public override double[,] FilterMatrix { get; set; }

        public EdgeDetectionVerticalFilter()
        {
            Factor = 1;
            FilterMatrix = new double[,]
            {
                {  0,  0, -1,  0,  0, },
                {  0,  0, -1,  0,  0, },
                {  0,  0,  4,  0,  0, },
                {  0,  0, -1,  0,  0, },
                {  0,  0, -1,  0,  0, }
            };
        }
    }
}
