using DSPAlgorithms.DataStructures;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSPAlgorithms.Algorithms
{
    public class Derivatives: Algorithm
    {
        public Signal InputSignal { get; set; }
        public Signal FirstDerivative { get; set; }
        public Signal SecondDerivative { get; set; }

        public override void Run()
        {
            FirstDerivative = new Signal(new List<float>(), new List<int>(InputSignal.SamplesIndices), false);
            // 1st Derivative
            for (int i = 1; i < InputSignal.Samples.Count; i++)
            {
                FirstDerivative.Samples.Add(InputSignal.Samples[i] - InputSignal.Samples[i - 1]);
            }
            SecondDerivative = new Signal(new List<float>(), new List<int>(FirstDerivative.SamplesIndices), false);
            // 2nd Derivative
            for (int i = 1; i < FirstDerivative.Samples.Count; i++)
            {
                SecondDerivative.Samples.Add(FirstDerivative.Samples[i] - FirstDerivative.Samples[i - 1]);
            }
        }
    }
}
