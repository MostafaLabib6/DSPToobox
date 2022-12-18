using DSPAlgorithms.DataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace DSPAlgorithms.Algorithms
{
    public class DCT: Algorithm
    {
        public Signal InputSignal { get; set; }
        public Signal OutputSignal { get; set; }

        public override void Run()
        {
            OutputSignal = new Signal(new List<float>(), false);
            int N = InputSignal.Samples.Count;
            for (int k = 0; k < N; k++)
            {
                double sum = 0;
                for (int n = 0; n < N; n++)
                {
                    double x_of_n = InputSignal.Samples[n];
                    double c = Math.Cos((Math.PI / (4 * N)) * ((2 * n) - 1) * ((2 * k) - 1));
                    //double c = Math.Cos(((2 * n) + 1) * k * Math.PI / 2 * N);
                    sum += x_of_n * c;
                }
                sum *= Math.Sqrt(2/(double)N);
                OutputSignal.Samples.Add((float)sum);
            }
        }
    }
}
