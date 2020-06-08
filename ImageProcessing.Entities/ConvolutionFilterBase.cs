namespace ImageProcessing.Entities
{
    public abstract class ConvolutionFilterBase
    {
        public abstract double Factor { get; set; }
        public abstract double Bias { get; set; }
        public abstract double[,] FilterMatrix { get; set; }
    }
}
