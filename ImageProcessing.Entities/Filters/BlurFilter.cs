using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

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

            //if (filterName == "3x3")
            //{
            //    //FilterMatrix = new double[,]
            //    //{
            //    //    { 0, amount, 0 },
            //    //    { amount, amount, amount },
            //    //    { 0, amount, 0 }
            //    //};

            //    FilterMatrix = new double[amount, amount];

            //    bool increase = true;
            //    int amounts = 1;
            //    for (int i = 0; i < amount; i++)
            //    {
            //        for (int j = 0; j < amounts; j++)
            //        {
            //            double divided = amount / 2;
            //            int index = (int)Math.Round(divided);
            //            int lol = 1;
            //        }

            //        if (amounts == 3)
            //            increase = false;

            //        if (increase)
            //            amounts += 2;
            //        else
            //            amounts -= 2;
            //    }

            //    Factor = 1 / (amount * 5);
            //}
            //else if (filterName == "5x5")
            //{
            //    FilterMatrix = new double[,]
            //    {
            //        { 0, 0, amount, 0, 0 },
            //        { 0, amount, amount, amount, 0 },
            //        { amount, amount, amount, amount, amount },
            //        { 0, amount, amount, amount, 0 },
            //        { 0, 0, amount, 0, 0 }
            //    };

            //    Factor = 1 / (amount * 13);
            //}
            //else if (filterName == "7x7")
            //{
            //    FilterMatrix = new double[,]
            //    {
            //        { 0, 0, 0, amount, 0, 0, 0 },
            //        { 0, 0, amount, amount, amount, 0, 0 },
            //        { 0, amount, amount, amount, amount, amount, 0 },
            //        { amount, amount, amount, amount, amount, amount, amount },
            //        { 0, amount, amount, amount, amount, amount, 0 },
            //        { 0, 0, amount, amount, amount, 0, 0 },
            //        { 0, 0, 0, amount, 0, 0, 0 }
            //    };

            //    Factor = 1 / (amount * 25);
            //}
            //else if (filterName == "9x9")
            //{
            //    FilterMatrix = new double[,]
            //    {
            //        { 0, 0, 0, 0, amount, 0, 0, 0, 0 },
            //        { 0, 0, 0, amount, amount, amount, 0, 0, 0 },
            //        { 0, 0, amount, amount, amount, amount, amount, 0, 0 },
            //        { 0, amount, amount, amount, amount, amount, amount, amount, 0 },
            //        { amount, amount, amount, amount, amount, amount, amount, amount, amount },
            //        { 0, amount, amount, amount, amount, amount, amount, amount, 0 },
            //        { 0, 0, amount, amount, amount, amount, amount, 0, 0 },
            //        { 0, 0, 0, amount, amount, amount, 0, 0, 0 },
            //        { 0, 0, 0, 0, amount, 0, 0, 0, 0 }
            //    };

            //    Factor = 1 / (amount * 41);
            //}
        }
    }
}
