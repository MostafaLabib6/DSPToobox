using DSPAlgorithms.DataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSPAlgorithms.Algorithms
{
    public class Sampling : Algorithm
    {
        public int L { get; set; } //upsampling factor
        public int M { get; set; } //downsampling factor
        public Signal InputSignal { get; set; }
        public Signal OutputSignal { get; set; }


        private Signal ApplyLowPassFilter(Signal s)
        {

            FIR FIR_FILTER = new FIR();

            FIR_FILTER.InputTimeDomainSignal = s;
            FIR_FILTER.InputFilterType = DSPAlgorithms.DataStructures.FILTER_TYPES.LOW;
            FIR_FILTER.InputFS = 8000;
            FIR_FILTER.InputStopBandAttenuation = 50;
            FIR_FILTER.InputCutOffFrequency = 1500;
            FIR_FILTER.InputTransitionBand = 500;

            FIR_FILTER.Run();

            return FIR_FILTER.OutputYn;
        }

        private Signal ApplyUpSampling(Signal s, int samplingFactor)
        {
            Signal result = new Signal(new List<float>(), new List<int>(), s.Periodic);
            //int numOfSamples = ((samplingFactor - 1) * s.Samples.Count) + 1;
            int numOfSamples = samplingFactor * s.Samples.Count - (samplingFactor - 2);//count new number of samples
            //3*3-1=8
            //3*4-1=11
            for (int i = 0; i < numOfSamples; i++)
            {
                // 0,1,2 (L = 3) --> "0" ,1,2, "3",4,5, "6",7
                // 0,1,2,3 (L = 3) --> "0",1,2, "3",4,5, "6",7,8, "9",10
                if (i % samplingFactor == 0)
                {
                    result.Samples.Add(s.Samples[i / samplingFactor]);
                }
                else
                {
                    result.Samples.Add(0);
                }
                result.SamplesIndices.Add(i);
            }
            return result;
        }

        private Signal ApplyDownSampling(Signal s, int samplingFactor)
        {
            Signal result = new Signal(new List<float>(), new List<int>(), s.Periodic);
            //int numOfSamples = (s.Samples.Count / (samplingFactor - 1)) - 1;
            for (int i = 0; i < s.Samples.Count; i += samplingFactor)
            {
                // "0" ,1,2, "3",4,5, "6",7 (M == 3) --> 0,1,2
                result.Samples.Add(s.Samples[i]);
                result.SamplesIndices.Add(i / samplingFactor);
            }
            return result;
        }

        public override void Run()
        {
            if (M == 0 && L == 0)
            {
                Console.WriteLine("L and M can't be both zero.");
                return;
            }
            else if (M == 0 && L != 0)
            {
                Signal upsampledSignal = ApplyUpSampling(InputSignal, L);
                OutputSignal = ApplyLowPassFilter(upsampledSignal);
            }
            else if (L == 0 && M != 0)
            {
                Signal filteredSignal = ApplyLowPassFilter(InputSignal);
                OutputSignal = ApplyDownSampling(filteredSignal, M);
            }
            else
            {
                // L != 0 && M != 0
                Signal upsampledSignal = ApplyUpSampling(InputSignal, L);
                Signal filteredSignal = ApplyLowPassFilter(upsampledSignal);
                OutputSignal = ApplyDownSampling(filteredSignal, M);
            }
        }
    }

}