using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using DSPAlgorithms.DataStructures;

namespace DSPAlgorithms.Algorithms
{
    public class FastConvolution : Algorithm
    {
        public Signal InputSignal1 { get; set; }
        public Signal InputSignal2 { get; set; }
        public Signal OutputConvolvedSignal { get; set; }

        /// <summary>
        /// Convolved InputSignal1 (considered as X) with InputSignal2 (considered as H)
        /// </summary>
        public override void Run()
        {
            // Padding 0's to the end of the both signals
            int numOfNonZeroSamples = InputSignal1.Samples.Count + InputSignal2.Samples.Count - 1;
            int signal1Padding = numOfNonZeroSamples - InputSignal1.Samples.Count;
            int signal2Padding = numOfNonZeroSamples - InputSignal2.Samples.Count;
            for (int i = 0; i < signal1Padding; i++)
            {
                InputSignal1.Samples.Add(0);
            }
            for (int i = 0; i < signal2Padding; i++)
            {
                InputSignal2.Samples.Add(0);
            }
            // From time domain to frequency domain
            DiscreteFourierTransform dft = new DiscreteFourierTransform();
            dft.InputTimeDomainSignal = InputSignal1;
            dft.Run();
            InputSignal1 = dft.OutputFreqDomainSignal;
            dft = new DiscreteFourierTransform();
            dft.InputTimeDomainSignal = InputSignal2;
            dft.Run();
            InputSignal2 = dft.OutputFreqDomainSignal;
            // Multiply InputSignal1 with InputSignal2 in frequency domain
            Signal resultFreqDomainSignal = new Signal(new List<float>(), false);
            resultFreqDomainSignal.FrequenciesAmplitudes = new List<float>();
            resultFreqDomainSignal.FrequenciesPhaseShifts = new List<float>();
            for (int i = 0; i < InputSignal1.FrequenciesAmplitudes.Count; i++)
            {
                Complex c1 = new Complex(InputSignal1.FrequenciesAmplitudes[i] * Math.Cos(InputSignal1.FrequenciesPhaseShifts[i]),
                        InputSignal1.FrequenciesAmplitudes[i] * Math.Sin(InputSignal1.FrequenciesPhaseShifts[i]));
                Complex c2 = new Complex(InputSignal2.FrequenciesAmplitudes[i] * Math.Cos(InputSignal2.FrequenciesPhaseShifts[i]),
                      InputSignal2.FrequenciesAmplitudes[i] * Math.Sin(InputSignal2.FrequenciesPhaseShifts[i]));
                Complex result = c1 * c2;
                resultFreqDomainSignal.FrequenciesAmplitudes.Add((float)result.Magnitude);
                resultFreqDomainSignal.FrequenciesPhaseShifts.Add((float)result.Phase);
            }
            // Apply inverse discrete fourier transform
            InverseDiscreteFourierTransform idft = new InverseDiscreteFourierTransform();
            idft.InputFreqDomainSignal = resultFreqDomainSignal;
            idft.Run();
            Signal resultTimeDomainSignal = idft.OutputTimeDomainSignal;
            OutputConvolvedSignal = resultTimeDomainSignal;
        }
    }
}
