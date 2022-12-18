using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using DSPAlgorithms.DataStructures;

namespace DSPAlgorithms.Algorithms
{
    public class SinCos: Algorithm
    {
        public string type { get; set; }
        public float A { get; set; }
        public float PhaseShift { get; set; }
        public float AnalogFrequency { get; set; }
        public float SamplingFrequency { get; set; }
        public List<float> samples { get; set; }
        public override void Run()
        {
            /*The ability to generate sinusoidal or cosinusoidal signals, the user should enter he wants cosine or sine, the amplitude A, 
            the phase shift theta, the analog frequency, and the sampling frequency needed. 
            (Hint: the sampling frequency chosen should obey the sampling theorem).*/
            samples = new List<float>();
            for (int i = 0; i < SamplingFrequency; i++)
            {
                float sample = AlgorithmsUtilities.getSignalSample(type, i, A, PhaseShift, AnalogFrequency, SamplingFrequency);
                samples.Add(sample);
            }
        }
    }
}
