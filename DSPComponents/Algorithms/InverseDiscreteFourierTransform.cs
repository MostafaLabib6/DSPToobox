using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using DSPAlgorithms.DataStructures;

namespace DSPAlgorithms.Algorithms
{
    public class InverseDiscreteFourierTransform : Algorithm
    {
        public Signal InputFreqDomainSignal { get; set; }
        public Signal OutputTimeDomainSignal { get; set; }

        public override void Run()
        {
            List<float> outSamples = new List<float>();
            OutputTimeDomainSignal = new Signal(new List<float>(), false);
            int N = InputFreqDomainSignal.FrequenciesAmplitudes.Count;
            for (int k = 0; k < N; ++k)
            {
                Complex sum = new Complex(0,0);
                for (int i = 0; i < N; i++)
                {
                    Complex xk = new Complex(InputFreqDomainSignal.FrequenciesAmplitudes[i]*Math.Cos(InputFreqDomainSignal.FrequenciesPhaseShifts[i]),
                        InputFreqDomainSignal.FrequenciesAmplitudes[i]*Math.Sin(InputFreqDomainSignal.FrequenciesPhaseShifts[i]));
                    Complex c = new Complex((Math.Cos((2 * Math.PI * k * i) / N)), (Math.Sin((2 * Math.PI * k * i) / N)));
                    sum += xk * c;
                }
                float sample= (float)(sum.Real / N);
                outSamples.Add(sample);
            }
            OutputTimeDomainSignal = new Signal(outSamples, false);
        }
    }
}
