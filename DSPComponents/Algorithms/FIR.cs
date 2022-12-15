using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSPAlgorithms.DataStructures;

namespace DSPAlgorithms.Algorithms
{
    public class FIR : Algorithm
    {
        public Signal InputTimeDomainSignal { get; set; }
        public FILTER_TYPES InputFilterType { get; set; }
        public float InputFS { get; set; }
        public float? InputCutOffFrequency { get; set; }
        public float? InputF1 { get; set; }
        public float? InputF2 { get; set; }
        public float InputStopBandAttenuation { get; set; }
        public float InputTransitionBand { get; set; }
        public Signal OutputHn { get; set; }
        public Signal OutputYn { get; set; }

        // map input attenuation into the first match one of the  attenuation list 
        public int find_first_greater_attenuation(float InputStopBandAttenuation)
        {
            List<int> StopBandAttenuationList = new List<int>() { 21, 44, 53, 74 };
            int firstMatch = StopBandAttenuationList.FirstOrDefault(x => x >= InputStopBandAttenuation);

            return firstMatch;
        }
        public int calc_N(float normalized_transation, int attenuaion)
        {
            int N = 0;
            switch (attenuaion)
            {
                case 21:
                    N = (int)(Math.Round(0.9 / normalized_transation, MidpointRounding.AwayFromZero));
                    break;
                case 44:
                    N = (int)Math.Round(3.1 / normalized_transation, MidpointRounding.AwayFromZero);
                    break;
                case 53:
                    N = (int)Math.Round(3.3 / normalized_transation, MidpointRounding.AwayFromZero);
                    break;

                case 74:
                    N = (int)Math.Round(5.5 / normalized_transation, MidpointRounding.AwayFromZero);
                    break;
            }
            if (N % 2 == 0)
                return N + 1;
            return N;
        }
        List<float> get_window_fn(int Attenuation, int N)
        {
            List<float> window = new List<float>();
            if (Attenuation == 44)
            {
                for (int n = 0; n <= N / 2; ++n)
                {
                    window.Add((float)(0.5 + 0.5 * Math.Cos((2 * Math.PI * n) / N)));
                }
                return window;
            }
            else if (Attenuation == 53)
            {
                for (int n = 0; n <= N / 2; ++n)
                {
                    window.Add((float)(0.54 + 0.46 * Math.Cos((2.0 * Math.PI * n) / N)));
                }
                return window;


            }
            else if (Attenuation == 74)
            {
                for (int n = 0; n <= N / 2; ++n)
                {
                    window.Add((float)(0.42 + 0.5 * Math.Cos((2.0 * Math.PI * n) / (N - 1.0)) + (0.08 * (Math.Cos((4.0 * Math.PI * n) / (N - 1.0))))));
                }
                return window;

            }

            for (int n = 0; n <= N / 2; ++n)
            {
                window.Add((float)1);
            }
            return window;
        }
        public List<float> get_low_pass(float fc, int N)
        {
            List<float> h0 = new List<float>();
            for (int n = 0; n <= N / 2; ++n)
            {
                if (n == 0)
                    h0.Add((float)(2.0 * fc));
                else
                    h0.Add((float)((2.0 * fc * Math.Sin(n * 2.0 * Math.PI * fc)) / (n * 2.0 * Math.PI * fc)));
            }
            return h0;

        }
        public List<float> get_high_pass(float fc, int N)
        {
            List<float> h0 = new List<float>();
            for (int n = 0; n <= N / 2; ++n)
            {
                if (n == 0)

                    h0.Add((float)(1 - (2 * fc)));
                else
                    h0.Add((float)((-2 * fc * Math.Sin(n * 2 * fc * Math.PI)) / (n * 2 * fc * Math.PI)));

            }
            return h0;
        }
        public List<float> get_band_pass(float fc1, float fc2, int N)
        {
            List<float> h0 = new List<float>();
            for (int n = 0; n <= N / 2; ++n)
            {
                if (n == 0)
                    h0.Add((float)(2.0 * (fc2 - fc1)));
                else
                    h0.Add((float)((((2.0 * fc2 * Math.Sin(n * 2.0 * Math.PI * fc2))
                        / (n * 2.0 * Math.PI * fc2))) - ((2 * fc1 * Math.Sin(2 * n * Math.PI * fc1)) 
                        / (n * 2 * Math.PI * fc1))));
            }
            return h0;
        }
        public List<float> get_band_reject(float fc1, float fc2, int N)
        {
            List<float> h0 = new List<float>();
            for (int n = 0; n <= N / 2; ++n)
            {
                if (n == 0)
                    h0.Add((float)(1.0 - (2.0 * (fc2 - fc1))));
                else
                    h0.Add((float)(((2.0 * fc1 * Math.Sin(n * 2.0 * fc1 * Math.PI))
                        /(n * 2.0 * fc1 * Math.PI)) - ((2.0 * fc2 * Math.Sin(n * 2.0 * fc2 * Math.PI)) 
                        /(n * 2.0 * fc2 * Math.PI))));
            }
            return h0;
        }



        public override void Run()
        {
            OutputHn = new Signal(new List<float>(), InputTimeDomainSignal.Periodic);
            OutputHn.SamplesIndices = new List<int>();
            OutputYn = new Signal(new List<float>(), InputTimeDomainSignal.Periodic);

            int Attenuation = find_first_greater_attenuation(InputStopBandAttenuation);
            float normalized_TransitionBand = InputTransitionBand / InputFS;
            int N = calc_N(normalized_TransitionBand, Attenuation);
            if (InputFilterType == FILTER_TYPES.LOW)
            {
                float CutOffFrequency = (float)(InputCutOffFrequency / InputFS + normalized_TransitionBand / 2.0);
                List<float> window = get_window_fn(Attenuation, N);
                List<float> h0 = get_low_pass(CutOffFrequency, N);
                for (int n = -N / 2; n <= N / 2; ++n)
                {
                    if (n < 0)
                    {
                        OutputHn.Samples.Add(h0[-1 * n] * window[-1 * n]);

                    }
                    else
                    {
                        OutputHn.Samples.Add(h0[n] * window[n]);
                    }

                    OutputHn.SamplesIndices.Add(n);

                }


            }
            else if (InputFilterType == FILTER_TYPES.HIGH)
            {
                float CutOffFrequency = (float)(InputCutOffFrequency / InputFS - normalized_TransitionBand / 2.0);
                List<float> window = get_window_fn(Attenuation, N);
                List<float> h0 = get_high_pass(CutOffFrequency, N);
                for (int n = -N / 2; n <= N / 2; ++n)
                {
                    if (n < 0)
                        OutputHn.Samples.Add((float)(h0[-1 * n] * window[-1 * n]));
                    else
                        OutputHn.Samples.Add((float)(h0[n] * window[n]));


                    OutputHn.SamplesIndices.Add(n);

                }

            }
            else if (InputFilterType == FILTER_TYPES.BAND_PASS)
            {
                float CutOffFrequency1 = (float)(InputF1 / InputFS - normalized_TransitionBand / 2.0);
                float CutOffFrequency2 = (float)(InputF2 / InputFS + normalized_TransitionBand / 2.0);
                List<float> window = get_window_fn(Attenuation, N);
                List<float> h0 = get_band_pass(CutOffFrequency1, CutOffFrequency2, N);
                for (int n = -N / 2; n <= N / 2; ++n)
                {
                    if (n < 0)
                    {
                        OutputHn.Samples.Add(h0[-1 * n] * window[-1 * n]);

                    }
                    else
                    {
                        OutputHn.Samples.Add(h0[n] * window[n]);
                    }
                    OutputHn.SamplesIndices.Add(n);

                }


            }
            else if (InputFilterType == FILTER_TYPES.BAND_STOP)
            {
                float CutOffFrequency1 = (float)(InputF2 / InputFS + normalized_TransitionBand / 2.0);
                float CutOffFrequency2 = (float)(InputF1 / InputFS - normalized_TransitionBand / 2.0);
                List<float> window = get_window_fn(Attenuation, N);
                List<float> h0 = get_band_reject(CutOffFrequency1, CutOffFrequency2, N);
                for (int n = -N / 2; n <= N / 2; ++n)
                {
                    if (n < 0)
                        OutputHn.Samples.Add(h0[-1 * n] * window[-1 * n]);
                    else
                        OutputHn.Samples.Add(h0[n] * window[n]);


                    OutputHn.SamplesIndices.Add(n);

                }


            }


            DirectConvolution conv = new DirectConvolution();
            conv.InputSignal1 = OutputHn;
            conv.InputSignal2 = InputTimeDomainSignal;
            conv.Run();
            OutputYn = conv.OutputConvolvedSignal;
        }
    }
}
