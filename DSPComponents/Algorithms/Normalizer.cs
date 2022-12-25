using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSPAlgorithms.DataStructures;

namespace DSPAlgorithms.Algorithms
{
    public class Normalizer : Algorithm
    {
        public Signal InputSignal { get; set; }
        public float InputMinRange { get; set; }
        public float InputMaxRange { get; set; }
        public Signal OutputNormalizedSignal { get; set; }

        public override void Run()
        {
            OutputNormalizedSignal = new Signal(new List<float>(), new List<int>(), false);
            float oldMin = InputSignal.Samples.Min();
            float oldMax = InputSignal.Samples.Max();

            for (int i = 0; i < InputSignal.Samples.Count; ++i)
            {
                OutputNormalizedSignal.Samples.Add(((InputSignal.Samples[i] - oldMin) /
                    (oldMax - oldMin)) * (InputMaxRange - InputMinRange) + InputMinRange);
                OutputNormalizedSignal.SamplesIndices.Add(InputSignal.SamplesIndices[i]);
            }

        }
    }
}
