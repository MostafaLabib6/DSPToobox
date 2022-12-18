using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using DSPAlgorithms.DataStructures;

namespace DSPAlgorithms.Algorithms
{
    public class FastCorrelation : Algorithm
    {
        public Signal InputSignal1 { get; set; }
        public Signal InputSignal2 { get; set; }
        public List<float> OutputNonNormalizedCorrelation { get; set; }
        public List<float> OutputNormalizedCorrelation { get; set; }

        public override void Run()
        {
            if (InputSignal2 == null)
            {
                InputSignal2 = new Signal(new List<float>(InputSignal1.Samples), InputSignal1.Periodic);
            }
            // Calculate stuff for normalized correlation
            float signal1Sum = 0, signal2Sum = 0;
            for (int i = 0; i < InputSignal1.Samples.Count; i++)
            {
                signal1Sum += InputSignal1.Samples[i] * InputSignal1.Samples[i];
                signal2Sum += InputSignal2.Samples[i] * InputSignal2.Samples[i];
            }
            float res = (1 / (float)InputSignal1.Samples.Count) * (float)Math.Sqrt(signal1Sum * signal2Sum);
            // From time domain to frequency domain
            DiscreteFourierTransform dft = new DiscreteFourierTransform();
            dft.InputTimeDomainSignal = InputSignal1;
            dft.Run();
            InputSignal1 = dft.OutputFreqDomainSignal;
            dft = new DiscreteFourierTransform();
            dft.InputTimeDomainSignal = InputSignal2;
            dft.Run();
            InputSignal2 = dft.OutputFreqDomainSignal;
            // Conjugate of first signal
            for (int i = 0; i < InputSignal1.FrequenciesPhaseShifts.Count; i++)
            {
                InputSignal1.FrequenciesPhaseShifts[i] = -InputSignal1.FrequenciesPhaseShifts[i];
            }
            // Multiply InputSignal1 with InputSignal2 in frequency domain
            Signal resultFreqDomainSignal = new Signal(new List<float>(), false);
            resultFreqDomainSignal.FrequenciesAmplitudes = new List<float>();
            resultFreqDomainSignal.FrequenciesPhaseShifts = new List<float>();
            for (int i = 0; i < InputSignal1.FrequenciesAmplitudes.Count; i++)
            {
                // Convert InputSignal1 and InputSignal2 into complex numbers
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
            // Divide by N
            int N = resultTimeDomainSignal.Samples.Count;
            OutputNonNormalizedCorrelation = new List<float>();
            OutputNormalizedCorrelation = new List<float>();
            for (int i = 0; i < resultTimeDomainSignal.Samples.Count; i++)
            {
                resultTimeDomainSignal.Samples[i] /= N;
                OutputNonNormalizedCorrelation.Add(resultTimeDomainSignal.Samples[i]);
                OutputNormalizedCorrelation.Add(resultTimeDomainSignal.Samples[i] / res);
            }
        }
    }
}