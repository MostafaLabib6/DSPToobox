using System;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSPAlgorithms.DataStructures;

namespace DSPAlgorithms.Algorithms
{
    public class DiscreteFourierTransform : Algorithm
    {
        public Signal InputTimeDomainSignal { get; set; }
        public float InputSamplingFrequency { get; set; }
        public Signal OutputFreqDomainSignal { get; set; }

        public override void Run()
        {
            OutputFreqDomainSignal = new Signal(new List<float>(), false);
            OutputFreqDomainSignal.FrequenciesAmplitudes = new List<float>();
            OutputFreqDomainSignal.FrequenciesPhaseShifts = new List<float>();
            int N = InputTimeDomainSignal.Samples.Count;
            for (int k = 0; k < N; ++k) {
                Complex sum = new Complex();
                for (int i = 0; i < N; i++) {
                    double sample = InputTimeDomainSignal.Samples[i];
                    Complex c = new Complex((Math.Cos((-2*Math.PI*k*i)/N)),(Math.Sin((-2*Math.PI*k*i)/N)));
                    sum += sample * c;
                }
                double amplitude = sum.Magnitude;
                double phaseShift = sum.Phase;
                OutputFreqDomainSignal.FrequenciesAmplitudes.Add((float)amplitude);
                OutputFreqDomainSignal.FrequenciesPhaseShifts.Add((float)phaseShift);
            }
        }
    }
}
