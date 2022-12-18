using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSPAlgorithms.DataStructures;

namespace DSPAlgorithms.Algorithms
{
    public class Folder : Algorithm
    {
        public Signal InputSignal { get; set; }
        public Signal OutputFoldedSignal { get; set; }

        public override void Run()
        {
            OutputFoldedSignal = new Signal(new List<float>(InputSignal.Samples), false);
            OutputFoldedSignal.SamplesIndices = new List<int>(InputSignal.SamplesIndices);
            OutputFoldedSignal.Samples.Reverse();
            OutputFoldedSignal.SamplesIndices.Reverse();
            OutputFoldedSignal.SamplesIndices = OutputFoldedSignal.SamplesIndices.Select(i => i * -1).ToList();
            OutputFoldedSignal.isFolded = !InputSignal.isFolded;
        }
    }
}
