using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSPAlgorithms.DataStructures;

namespace DSPAlgorithms.Algorithms
{
    public class DirectCorrelation : Algorithm
    {
        public Signal InputSignal1 { get; set; }
        public Signal InputSignal2 { get; set; }
        public List<float> OutputNonNormalizedCorrelation { get; set; }
        public List<float> OutputNormalizedCorrelation { get; set; }

        public override void Run()
        {
            if(InputSignal2 == null)
            {
                InputSignal2 = new Signal(new List<float>(InputSignal1.Samples), InputSignal1.Periodic);
            }
            float signal1Sum = 0, signal2Sum = 0;
            for (int i = 0; i < InputSignal1.Samples.Count; i++)
            {
                signal1Sum += InputSignal1.Samples[i] * InputSignal1.Samples[i];
                signal2Sum += InputSignal2.Samples[i] * InputSignal2.Samples[i]; 
            }
            float result = (1 / (float)InputSignal1.Samples.Count) * (float)Math.Sqrt(signal1Sum * signal2Sum);
            OutputNonNormalizedCorrelation = new List<float>();
            OutputNormalizedCorrelation = new List<float>();
            for (int i = 0; i < InputSignal1.Samples.Count; i++)
            {
                List<float> shiftedSignal = AlgorithmsUtilities.shiftLeftSignal(InputSignal2.Samples, i, InputSignal2.Periodic);
                float sum = 0;
                for (int j = 0; j < InputSignal1.Samples.Count; j++)
                {
                    sum += InputSignal1.Samples[j] * shiftedSignal[j];
                }
                sum /= InputSignal1.Samples.Count;
                OutputNonNormalizedCorrelation.Add(sum);
                OutputNormalizedCorrelation.Add(sum / result);
            }
        }
    }
}