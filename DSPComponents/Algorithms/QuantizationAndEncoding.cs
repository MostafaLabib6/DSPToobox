using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSPAlgorithms.DataStructures;

namespace DSPAlgorithms.Algorithms
{
    public class QuantizationAndEncoding : Algorithm
    {
        // You will have only one of (InputLevel or InputNumBits), the other property will take a negative value
        // If InputNumBits is given, you need to calculate and set InputLevel value and vice versa
        public int InputLevel { get; set; }
        public int InputNumBits { get; set; }
        public Signal InputSignal { get; set; }
        public Signal OutputQuantizedSignal { get; set; }
        public List<int> OutputIntervalIndices { get; set; }
        public List<string> OutputEncodedSignal { get; set; }
        public List<float> OutputSamplesError { get; set; }

        public override void Run()
        {
            /*
             * The ability to quantize an input signal (its samples), 
             * the application should ask the user for the needed levels or number of bits available 
             * (in case of number of bits the application should compute from it the appropriate number of levels). 
             * Thereafter, the application should display the quantized signal and quantization error besides the encoded signal.
             */
            // Calculate number of levels
            int numOfLvls = 0;
            if(InputLevel == 0)
            {
                numOfLvls = (int)Math.Pow(2, InputNumBits);
                InputLevel = numOfLvls;
            }
            else
            {
                numOfLvls = InputLevel;
                InputNumBits = (int)Math.Log(InputLevel, 2);
            }
            //Calculate midpoints
            float minSample = InputSignal.Samples.Min();
            float maxSample = InputSignal.Samples.Max();
            float levelRange = (maxSample - minSample) / numOfLvls;
            float start = minSample;
            float end = -1;
            List<float> midpoints = new List<float>();
            for (int i = 0; i < numOfLvls; i++)
            {
                end = start + levelRange;
                float midpoint = (start + end) / 2;
                midpoints.Add(midpoint);
                start = end;
            }
            // Calculate quantized signal
            OutputQuantizedSignal = new Signal(new List<float>(), false);
            for (int i = 0; i < InputSignal.Samples.Count; i++)
            {
                float sample = InputSignal.Samples[i];
                float closest = midpoints.Aggregate((x, y) => Math.Abs(x - sample) < Math.Abs(y - sample) ? x : y);
                OutputQuantizedSignal.Samples.Add(closest);
            }
            // Interval indices
            OutputIntervalIndices = new List<int>();
            float smallestMidpoint = midpoints[0];
            for (int i = 0; i < InputSignal.Samples.Count; i++)
            {
                float quantizedSample = OutputQuantizedSignal.Samples[i];
                // Added 1 ("indices" starts from 1) and fixed the issue (floating point arithmetic)
                // Error example: 0.45 - 0.25 = 0.1999 --> 0.1999 / 0.1 = 1.999 --> (int)1.999 = 1
                int index = (int)Math.Round((quantizedSample - smallestMidpoint) / levelRange, MidpointRounding.AwayFromZero) + 1;
                OutputIntervalIndices.Add(index);
            }
            // Encoded signal
            OutputEncodedSignal = new List<string>();
            for (int i = 0; i < InputSignal.Samples.Count; i++)
            {
                // Subtracted 1 (Encoded is the values in OutputIntervalIndices but subtracted by 1)
                string unpaddedStr = Convert.ToString(OutputIntervalIndices[i] - 1, 2);
                // 11 --> 0011
                string paddedStr = new string('0', InputNumBits - unpaddedStr.Length) + unpaddedStr;
                OutputEncodedSignal.Add(paddedStr);
            }
            // Samples Error
            OutputSamplesError = new List<float>();
            for (int i = 0; i < InputSignal.Samples.Count; i++)
            {
                // Removed Abs and swapped what was being subracted
                float error = OutputQuantizedSignal.Samples[i] - InputSignal.Samples[i]; 
                OutputSamplesError.Add(error);
            }
        }
    }
}
