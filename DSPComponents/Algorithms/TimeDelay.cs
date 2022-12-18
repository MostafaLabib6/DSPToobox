using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSPAlgorithms.DataStructures;

namespace DSPAlgorithms.Algorithms
{
    public class TimeDelay:Algorithm
    {
        public Signal InputSignal1 { get; set; }
        public Signal InputSignal2 { get; set; }
        public float InputSamplingPeriod { get; set; }
        public float OutputTimeDelay { get; set; }

        public override void Run()
        {
            DirectCorrelation directCorrelation = new DirectCorrelation();
            directCorrelation.InputSignal1 = InputSignal1;
            directCorrelation.InputSignal2 = InputSignal2;
            directCorrelation.Run();
            List<float> nonNormalizedCorrelation = directCorrelation.OutputNonNormalizedCorrelation;
            int maxCorrelationIndex = -1;
            float maxCorrelation = float.MinValue;
            for (int i = 0; i < InputSignal1.Samples.Count; i++)
            {
                if(nonNormalizedCorrelation[i] > maxCorrelation)
                {
                    maxCorrelation = nonNormalizedCorrelation[i];
                    maxCorrelationIndex = i;
                }
            }
            OutputTimeDelay = maxCorrelationIndex * InputSamplingPeriod;
        }
    }
}
