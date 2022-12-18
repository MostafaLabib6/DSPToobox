using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSPAlgorithms.DataStructures;

namespace DSPAlgorithms.Algorithms
{
    public class Shifter : Algorithm
    {
        public Signal InputSignal { get; set; }
        public int ShiftingValue { get; set; }
        public Signal OutputShiftedSignal { get; set; }

        public override void Run()
        {
            bool isPositive = false;
            if (ShiftingValue >= 0)
                isPositive = true;
            ShiftingValue = Math.Abs(ShiftingValue);
            // Advance
            // {1, 2, 3} shiftingValue = 1 --> {2,3,0}
            OutputShiftedSignal = new Signal(new List<float>(), false);
            OutputShiftedSignal.Samples = InputSignal.Samples;
            OutputShiftedSignal.SamplesIndices = new List<int>();
            OutputShiftedSignal.isFolded = InputSignal.isFolded;
            if (isPositive)
            {
                if (!InputSignal.isFolded)
                {
                    // Advance
                    for (int i = 0; i < InputSignal.SamplesIndices.Count; i++)
                    {
                        OutputShiftedSignal.SamplesIndices.Add(InputSignal.SamplesIndices[i] - ShiftingValue);
                    }
                }
                else
                {
                    // Delay
                    for (int i = 0; i < InputSignal.SamplesIndices.Count; i++)
                    {
                        OutputShiftedSignal.SamplesIndices.Add(InputSignal.SamplesIndices[i] + ShiftingValue);
                    }
                }
            }
            else
            {
                if (!InputSignal.isFolded)
                {
                    // Delay
                    for (int i = 0; i < InputSignal.SamplesIndices.Count; i++)
                    {
                        OutputShiftedSignal.SamplesIndices.Add(InputSignal.SamplesIndices[i] + ShiftingValue);
                    }
                }
                else
                {
                    // Advance
                    for (int i = 0; i < InputSignal.SamplesIndices.Count; i++)
                    {
                        OutputShiftedSignal.SamplesIndices.Add(InputSignal.SamplesIndices[i] - ShiftingValue);
                    }
                }
            }
        }
    }
}
