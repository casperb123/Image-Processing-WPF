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
}
