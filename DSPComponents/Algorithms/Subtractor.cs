using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSPAlgorithms.DataStructures;

namespace DSPAlgorithms.Algorithms
{
    public class Subtractor : Algorithm
    {
        public Signal InputSignal1 { get; set; }
        public Signal InputSignal2 { get; set; }
        public Signal OutputSignal { get; set; }

        /// <summary>
        /// To do: Subtract Signal2 from Signal1 
        /// i.e OutSig = Sig1 - Sig2 
        /// </summary>
        public override void Run()
        {
            OutputSignal = new Signal(new List<float>(), false);

            Adder adder = new Adder();
            MultiplySignalByConstant mulConstant = new MultiplySignalByConstant();
            List<Signal> inputSignals = new List<Signal>();

            mulConstant.InputSignal = InputSignal2;
            mulConstant.InputConstant = -1;
            mulConstant.Run();
            Signal inputSignal2Result= mulConstant.OutputMultipliedSignal;
            

            inputSignals.Add(InputSignal1);
            inputSignals.Add(inputSignal2Result);
            adder.InputSignals = inputSignals;
            adder.Run();

            OutputSignal = adder.OutputSignal;


        }
    }
}