namespace ImageProcessing.Entities.Filters
{
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
}
