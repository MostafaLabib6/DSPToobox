using DSPAlgorithms.DataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace DSPAlgorithms.Algorithms
{
    public class PracticalTask2 : Algorithm
    {
        public String SignalPath { get; set; }
        public float Fs { get; set; }
        public float miniF { get; set; }
        public float maxF { get; set; }
        public float newFs { get; set; }
        public int L { get; set; } //upsampling factor
        public int M { get; set; } //downsampling factor
        public Signal OutputFreqDomainSignal { get; set; }

        public override void Run()
        {
            Signal InputSignal = LoadSignal(SignalPath);
            // 1) Display input signal

            // 2) Filter using FIR filter
            FIR FilterObj = new FIR();
            FilterObj.InputFilterType = FILTER_TYPES.BAND_PASS;
            FilterObj.InputTimeDomainSignal = InputSignal;
            FilterObj.InputF1 = miniF;
            FilterObj.InputF2 = maxF;
            FilterObj.InputFS = Fs;
            FilterObj.InputStopBandAttenuation = 50;
            FilterObj.InputTransitionBand = 500;

            FilterObj.Run();

            Signal filteredSignal = FilterObj.OutputYn;
            // 3) Resample the signal to newFs only if newFs doesn’t destroy the signal
            // , else show a message to the user “newFs is not valid” and continue executing the next instructions
            // ,“sample using L &M” as explained in Practical task1. (if sampling is done, save resulted signal in file)
            Signal sampledSignal = filteredSignal;
            float samplingFrequency = Fs;
            if (newFs < 2 * maxF)
            {
                Console.WriteLine("Invalid sampling frequency.");
            }
            else
            {
                Sampling SamplingObj = new Sampling();
                SamplingObj.InputSignal = filteredSignal;
                SamplingObj.L = L;
                SamplingObj.M = M;

                SamplingObj.Run();

                sampledSignal = SamplingObj.OutputSignal;
                samplingFrequency = newFs;
            }

            // 4) Remove the DC component. (save resulted signal in file)
            DC_Component DC_Component_Obj = new DC_Component();
            DC_Component_Obj.InputSignal = sampledSignal;

            DC_Component_Obj.Run();

            Signal removedDCComponentSignal = DC_Component_Obj.OutputSignal;
            // 5) Display the resulted signal from 4.

            // 6) Normalize the signal to be from -1 to 1. (save resulted signal in file)
            Normalizer normalizer = new Normalizer();
            normalizer.InputSignal = removedDCComponentSignal;
            normalizer.InputMinRange = -1;
            normalizer.InputMaxRange = 1;

            normalizer.Run();

            Signal normalizedSignal = normalizer.OutputNormalizedSignal;
            // 7) Display the resulted signal from 6.

            // 8) Compute DFT. (save resulted signal in file)
            DiscreteFourierTransform DFT = new DiscreteFourierTransform();
            DFT.InputTimeDomainSignal = normalizedSignal;
            DFT.InputSamplingFrequency = samplingFrequency;

            DFT.Run();

            OutputFreqDomainSignal = DFT.OutputFreqDomainSignal; 
            // 9) Display the resulted components from 8.
            
        }

        public Signal LoadSignal(string filePath)
        {
            Stream stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            var sr = new StreamReader(stream);

            var sigType = byte.Parse(sr.ReadLine());
            var isPeriodic = byte.Parse(sr.ReadLine());
            long N1 = long.Parse(sr.ReadLine());

            List<float> SigSamples = new List<float>(unchecked((int)N1));
            List<int> SigIndices = new List<int>(unchecked((int)N1));
            List<float> SigFreq = new List<float>(unchecked((int)N1));
            List<float> SigFreqAmp = new List<float>(unchecked((int)N1));
            List<float> SigPhaseShift = new List<float>(unchecked((int)N1));

            if (sigType == 1)
            {
                SigSamples = null;
                SigIndices = null;
            }

            for (int i = 0; i < N1; i++)
            {
                if (sigType == 0 || sigType == 2)
                {
                    var timeIndex_SampleAmplitude = sr.ReadLine().Split();
                    SigIndices.Add(int.Parse(timeIndex_SampleAmplitude[0]));
                    SigSamples.Add(float.Parse(timeIndex_SampleAmplitude[1]));
                }
                else
                {
                    var Freq_Amp_PhaseShift = sr.ReadLine().Split();
                    SigFreq.Add(float.Parse(Freq_Amp_PhaseShift[0]));
                    SigFreqAmp.Add(float.Parse(Freq_Amp_PhaseShift[1]));
                    SigPhaseShift.Add(float.Parse(Freq_Amp_PhaseShift[2]));
                }
            }

            if (!sr.EndOfStream)
            {
                long N2 = long.Parse(sr.ReadLine());

                for (int i = 0; i < N2; i++)
                {
                    var Freq_Amp_PhaseShift = sr.ReadLine().Split();
                    SigFreq.Add(float.Parse(Freq_Amp_PhaseShift[0]));
                    SigFreqAmp.Add(float.Parse(Freq_Amp_PhaseShift[1]));
                    SigPhaseShift.Add(float.Parse(Freq_Amp_PhaseShift[2]));
                }
            }

            stream.Close();
            return new Signal(SigSamples, SigIndices, isPeriodic == 1, SigFreq, SigFreqAmp, SigPhaseShift);
        }
    }
}
