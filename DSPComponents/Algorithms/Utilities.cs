using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DSPAlgorithms.Algorithms
{
    public class AlgorithmsUtilities
    {
        // Implement here any functionalities you need to share between Algorithm classes
        public static float getSignalSample(string type, int sampleIndex, float amplititude, float phaseShift, float analogFreq, float samlingFreq)
        {
            return (float)Math.Round(type == "sin" ? amplititude * Math.Sin(2 * Math.PI * (analogFreq / samlingFreq) * sampleIndex + phaseShift)
                : amplititude * Math.Cos(2 * Math.PI * (analogFreq / samlingFreq) * sampleIndex + phaseShift), 7);
        }

        public static List<float> shiftLeftSignal(List<float> signal, int shiftValue, bool isPeriodic)
        {
            List<float> result = new List<float>();
            // Special Cases
            if (shiftValue == 0)
            { 
                return new List<float>(signal); 
            }
            // Shifting Left
            for (int i = shiftValue; i < signal.Count; i++)
            {
                result.Add(signal[i]);
            }
            if (isPeriodic)
            {
                // Wrap around
                for (int i = 0; i < shiftValue; i++)
                {
                    result.Add(signal[i]);
                }
            }
            else
            {
                // Add zeroes
                for (int i = 0; i < shiftValue; i++)
                {
                    result.Add(0);
                }
            }
            return result;
        }
    }
}
