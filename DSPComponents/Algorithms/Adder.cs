using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSPAlgorithms.DataStructures;

namespace DSPAlgorithms.Algorithms
{
    public class Adder : Algorithm
    {
        public List<Signal> InputSignals { get; set; }
        public Signal OutputSignal { get; set; }

        public override void Run()
        {
            int maxSampleLen=-1;
           foreach(Signal i in InputSignals)
            {
                maxSampleLen = Math.Max(maxSampleLen,i.Samples.Count);
                
           }
            List<float> signalSamples = new List<float>();
            for(int i = 0; i < maxSampleLen; ++i)
            {
                float sum = 0;
               foreach(Signal sig in InputSignals)
                {
                    if (i < sig.Samples.Count)
                    {
                        sum += sig.Samples[i];
                    }
                }
                signalSamples.Add(sum);
            }
            Signal result = new Signal(signalSamples, false);
            OutputSignal = result;
        }
    }
}