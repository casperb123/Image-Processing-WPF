namespace ImageProcessing.Entities.Filters
{
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
