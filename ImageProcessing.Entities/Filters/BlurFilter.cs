namespace ImageProcessing.Entities.Filters
{
    public class BlurFilter : ConvolutionFilterBase
    {
        public override string FilterName { get; set; }
        public override double Factor { get; set; }
        public override double Bias { get; set; }
        public override double[,] FilterMatrix { get; set; }

        public BlurFilter(int amount)
        {
            FilterName = $"Blur{amount}x{amount}Filter";

            FilterMatrix = new double[amount, amount];

            bool increase = true;
            int amounts = 1;
            int totalAmount = 0;
            for (int i = 0; i < amount; i++)
            {
                int index = amount / 2;
                FilterMatrix[i, index] = 1;
                totalAmount++;

                for (int j = 1; j < amounts; j++)
                {
                    FilterMatrix[i, index + j] = 1;
                    totalAmount++;
                }

                for (int j = 1; j < amounts; j++)
                {
                    FilterMatrix[i, index - j] = 1;
                    totalAmount++;
                }

                if (amounts == (amount / 2) + 1)
                    increase = false;

                if (increase)
                    amounts++;
                else
                    amounts--;
            }

            Factor = 1 / (1d * totalAmount);
        }
    }
}
