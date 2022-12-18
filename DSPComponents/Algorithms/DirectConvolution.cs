using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSPAlgorithms.DataStructures;

namespace DSPAlgorithms.Algorithms
{
    public class DirectConvolution : Algorithm
    {
        public Signal InputSignal1 { get; set; }
        public Signal InputSignal2 { get; set; }
        public Signal OutputConvolvedSignal { get; set; }

        /// <summary>
        /// Convolved InputSignal1 (considered as X) with InputSignal2 (considered as H)
        /// </summary>
        public override void Run()
        {
            Signal x = InputSignal1;
            Signal h = InputSignal2;
            OutputConvolvedSignal = new Signal(new List<float>(), new List<int>(), false);
            int minIndex;
            if (x.SamplesIndices[0] < 0 || h.SamplesIndices[0] < 0)
            {
                minIndex = x.SamplesIndices[0] + h.SamplesIndices[0];
            }
            else
            {
                minIndex = Math.Min(x.SamplesIndices[0], h.SamplesIndices[0]);
            }
            int numOfNonZeroSamples = x.Samples.Count + h.Samples.Count - 1;
            // Indices List
            int currentIndex = minIndex;
            for (int i = 0; i < numOfNonZeroSamples; i++)
            {
                OutputConvolvedSignal.SamplesIndices.Add(currentIndex);
                currentIndex++;
            }
            // Samples List (Convolution)
            for (int n = 0; n < numOfNonZeroSamples; n++)
            {
                float sum = 0;
                for (int k = 0; k <= n; k++)
                {
                    if (k >= x.Samples.Count || n - k >= h.Samples.Count) continue;
                    sum += x.Samples[k] * h.Samples[n - k];
                }
                OutputConvolvedSignal.Samples.Add(sum);
            }
            // Remove zeroes at the end
            int numOfConnectedZeroes = 0;
            for (int i = OutputConvolvedSignal.Samples.Count - 1; i >= 0; i--)
            {
                if (OutputConvolvedSignal.Samples[i] != 0) 
                {
                    break;
                }
                numOfConnectedZeroes++;
            }
            if (numOfConnectedZeroes > 0) {
                int startingIndex = OutputConvolvedSignal.Samples.Count - numOfConnectedZeroes;
                OutputConvolvedSignal.Samples.RemoveRange(startingIndex, numOfConnectedZeroes);
                OutputConvolvedSignal.SamplesIndices.RemoveRange(startingIndex, numOfConnectedZeroes);
            }
        }
    }
}
