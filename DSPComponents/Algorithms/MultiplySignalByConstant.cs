using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSPAlgorithms.DataStructures;

namespace DSPAlgorithms.Algorithms
{
    public class MultiplySignalByConstant : Algorithm
    {
        public Signal InputSignal { get; set; }
        public float InputConstant { get; set; }
        public Signal OutputMultipliedSignal { get; set; }

        public override void Run()
        {
            List<float> signalSamples = new List<float>();
            OutputMultipliedSignal = new Signal(signalSamples,false);
            for(int i = 0; i < InputSignal.Samples.Count; ++i)
            {
                OutputMultipliedSignal.Samples.Add(InputConstant * InputSignal.Samples[i]);
            }
        }
    }
}
