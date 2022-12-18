using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSPAlgorithms.DataStructures;

namespace DSPAlgorithms.Algorithms
{
    public class MovingAverage : Algorithm
    {
        public Signal InputSignal { get; set; }
        public int InputWindowSize { get; set; }
        public Signal OutputAverageSignal { get; set; }
 
        public override void Run()
        {
            OutputAverageSignal = new Signal(new List<float>(), new List<int>(InputSignal.SamplesIndices), false);
            for (int i = InputWindowSize / 2; i <= InputSignal.Samples.Count - 1 - InputWindowSize / 2; i++)
            {
                float average = 0;
                for (int j = - InputWindowSize / 2; j <= InputWindowSize / 2; j++)
                {
                    average += InputSignal.Samples[i + j];
                }
                average /= InputWindowSize;
                OutputAverageSignal.Samples.Add(average);
            }
        }
    }
}
