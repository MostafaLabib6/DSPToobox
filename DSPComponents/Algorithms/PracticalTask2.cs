using DSPAlgorithms.DataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot.WindowsForms;

namespace DSPAlgorithms.Algorithms
{
    public class PracticalTask2 : Algorithm
    {
        public String SignalPath { get; set; }
        public float Fs { get; set; }
        public float miniF { get; set; }
        public float maxF { get; set; }
        public float newFs { get; set; }
        public int L { get; set; } //upsampling factor
        public int M { get; set; } //downsampling factor
        public Signal OutputFreqDomainSignal { get; set; }

        /*
         * The file shall has an extension of .ds standing for digital signal
            The file will contain the samples in time domain or frequency domain or both,
            Follows is a description for how to build such a file:
            -----------------------------------------------------------------------------
            [SignalType] // Time-->0/Freq-->1/Time&Freq-->2
            [IsPeriodic] // takes 0 or 1
            [N1] // number of samples to follow or number of frequencies to follow 
            [Index SampleAmp] or [Freq Amp PhaseShift]
            // N1 rows follow with Sample Index followed by space followed by Sample Amplitude in case Time domain was specified in the first row in the file, 
            or N1 rows follow with frequency followed by space followed by amplitude followed by Phase shift
         */
        private void SaveSignalToFile(Signal signal, string filePath, bool inFrequencyDomain = false)
        {
            // File Headers
            int signalType = inFrequencyDomain ? 1 : 0;
            int isPeriodic = signal.Periodic ? 1 : 0;
            int N1 = -1;
            if (inFrequencyDomain)
            {
                N1 = signal.FrequenciesAmplitudes.Count;
            }
            else
            {
                N1 = signal.Samples.Count;
            }            
            StreamWriter writer = new StreamWriter(filePath);
            writer.WriteLine(signalType.ToString());
            writer.WriteLine(isPeriodic.ToString());
            writer.Write(N1.ToString());
            // Signal Values
            if (inFrequencyDomain)
            {
                List<float> frequencies = signal.Frequencies;
                List<float> amplitudes = signal.FrequenciesAmplitudes;
                List<float> phaseShifts = signal.FrequenciesPhaseShifts;
                // [Freq Amp PhaseShift]
                for (int i = 0; i < N1; i++)
                {
                    writer.Write('\n' + frequencies[i].ToString() + ' ' + amplitudes[i].ToString() + ' ' + phaseShifts[i].ToString());
                }
            }
            else
            {
                List<float> samples = signal.Samples;
                List<int> indices = signal.SamplesIndices;
                // [Index SampleAmp]
                for (int i = 0; i < N1; i++)
                {
                    writer.Write('\n' + indices[i].ToString() + ' ' + samples[i].ToString());
                }
            }
            writer.Close();
        }

        public override void Run()
        {
            Signal InputSignal = LoadSignal(SignalPath);
            const string signalsFolderPath = @"PracticalTask2 Signals\";
            SaveSignalToFile(InputSignal, signalsFolderPath + "input_signal.ds");
            // 1) Display input signal

            // 2) Filter using FIR filter
            FIR FilterObj = new FIR();
            FilterObj.InputFilterType = FILTER_TYPES.BAND_PASS;
            FilterObj.InputTimeDomainSignal = InputSignal;
            FilterObj.InputF1 = miniF;
            FilterObj.InputF2 = maxF;
            FilterObj.InputFS = Fs;
            FilterObj.InputStopBandAttenuation = 50;
            FilterObj.InputTransitionBand = 500;

            FilterObj.Run();

            Signal filteredSignal = FilterObj.OutputYn;
            // 3) Resample the signal to newFs only if newFs doesn’t destroy the signal
            // , else show a message to the user “newFs is not valid” and continue executing the next instructions
            // ,“sample using L &M” as explained in Practical task1. 
            // (if sampling is done, save resulted signal in file)  **TDM** 
            Signal sampledSignal = filteredSignal;
            float samplingFrequency = Fs;
            if (newFs < 2 * maxF)
            {
                Console.WriteLine("Invalid sampling frequency.");
            }
            else
            {
                Sampling SamplingObj = new Sampling();
                SamplingObj.InputSignal = filteredSignal;
                SamplingObj.L = L;
                SamplingObj.M = M;

                SamplingObj.Run();

                sampledSignal = SamplingObj.OutputSignal;
                samplingFrequency = newFs;
                SaveSignalToFile(sampledSignal, signalsFolderPath + "sampled_signal.ds");
            }

            // 4) Remove the DC component. 
            // (save resulted signal in file) **TDM**
            DC_Component DC_Component_Obj = new DC_Component();
            DC_Component_Obj.InputSignal = sampledSignal;

            DC_Component_Obj.Run();

            Signal removedDCComponentSignal = DC_Component_Obj.OutputSignal;
            SaveSignalToFile(removedDCComponentSignal, signalsFolderPath + "removed_dc_component_signal.ds");
            // 5) Display the resulted signal from 4.

            // 6) Normalize the signal to be from -1 to 1.
            // (save resulted signal in file) **TDM**
            Normalizer normalizer = new Normalizer();
            normalizer.InputSignal = removedDCComponentSignal;
            normalizer.InputMinRange = -1;
            normalizer.InputMaxRange = 1;

            normalizer.Run();

            Signal normalizedSignal = normalizer.OutputNormalizedSignal;
            SaveSignalToFile(normalizedSignal, signalsFolderPath + "normalized_signal.ds");
            // 7) Display the resulted signal from 6.

            // 8) Compute DFT.
            // (save resulted signal in file) **FDM**
            DiscreteFourierTransform DFT = new DiscreteFourierTransform();
            DFT.InputTimeDomainSignal = normalizedSignal;
            DFT.InputSamplingFrequency = samplingFrequency;

            DFT.Run();

            OutputFreqDomainSignal = DFT.OutputFreqDomainSignal;
            SaveSignalToFile(OutputFreqDomainSignal, signalsFolderPath + "DFT_signal.ds", true);
            // 9) Display the resulted components from 8.

        }

        public Signal LoadSignal(string filePath)
        {
            Stream stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            var sr = new StreamReader(stream);

            var sigType = byte.Parse(sr.ReadLine());
            var isPeriodic = byte.Parse(sr.ReadLine());
            long N1 = long.Parse(sr.ReadLine());

            List<float> SigSamples = new List<float>(unchecked((int)N1));
            List<int> SigIndices = new List<int>(unchecked((int)N1));
            List<float> SigFreq = new List<float>(unchecked((int)N1));
            List<float> SigFreqAmp = new List<float>(unchecked((int)N1));
            List<float> SigPhaseShift = new List<float>(unchecked((int)N1));

            if (sigType == 1)
            {
                SigSamples = null;
                SigIndices = null;
            }

            for (int i = 0; i < N1; i++)
            {
                if (sigType == 0 || sigType == 2)
                {
                    var timeIndex_SampleAmplitude = sr.ReadLine().Split();
                    SigIndices.Add(int.Parse(timeIndex_SampleAmplitude[0]));
                    SigSamples.Add(float.Parse(timeIndex_SampleAmplitude[1]));
                }
                else
                {
                    var Freq_Amp_PhaseShift = sr.ReadLine().Split();
                    SigFreq.Add(float.Parse(Freq_Amp_PhaseShift[0]));
                    SigFreqAmp.Add(float.Parse(Freq_Amp_PhaseShift[1]));
                    SigPhaseShift.Add(float.Parse(Freq_Amp_PhaseShift[2]));
                }
            }

            if (!sr.EndOfStream)
            {
                long N2 = long.Parse(sr.ReadLine());

                for (int i = 0; i < N2; i++)
                {
                    var Freq_Amp_PhaseShift = sr.ReadLine().Split();
                    SigFreq.Add(float.Parse(Freq_Amp_PhaseShift[0]));
                    SigFreqAmp.Add(float.Parse(Freq_Amp_PhaseShift[1]));
                    SigPhaseShift.Add(float.Parse(Freq_Amp_PhaseShift[2]));
                }
            }

            stream.Close();
            return new Signal(SigSamples, SigIndices, isPeriodic == 1, SigFreq, SigFreqAmp, SigPhaseShift);
        }

        public static void PlotGraph(Signal Sig)
        {
            PlotView plotSamples = null;
            PlotView plotFreqAmp = null;
            PlotView plotFreqPhase = null;
            short time_freq_timefreq = -1;

            if (Sig.Samples != null && Sig.Samples.Count > 0)
            {
                time_freq_timefreq = 0;
                plotSamples = GetPlot(Sig.SamplesIndices, Sig.Samples);
            }
            if (Sig.Frequencies != null && Sig.Frequencies.Count > 0)
            {
                if (time_freq_timefreq == 0)
                    time_freq_timefreq = 2;
                else
                    time_freq_timefreq = 1;

                plotFreqAmp = GetPlot(Sig.Frequencies, Sig.FrequenciesAmplitudes);
                plotFreqPhase = GetPlot(Sig.Frequencies, Sig.FrequenciesPhaseShifts);
            }

            TableLayoutPanel tlp = new TableLayoutPanel();
            tlp.Dock = DockStyle.Fill;

            var fGraph = new Form();

            switch (time_freq_timefreq)
            {
                case 0:
                    var p1 = new Panel();
                    p1.Dock = DockStyle.Fill;
                    p1.Controls.Add(plotSamples);
                    tlp.Controls.Add(p1, 0, 0);
                    break;

                case 1:
                    p1 = new Panel();
                    p1.Dock = DockStyle.Fill;
                    p1.Controls.Add(plotFreqAmp);
                    tlp.Controls.Add(p1, 0, 0);

                    p1 = new Panel();
                    p1.Dock = DockStyle.Fill;
                    p1.Controls.Add(plotFreqPhase);
                    tlp.Controls.Add(p1, 1, 0);

                    tlp.ColumnStyles.Add(new ColumnStyle() { SizeType = SizeType.Percent, Width = 0.5f });
                    tlp.ColumnStyles.Add(new ColumnStyle() { SizeType = SizeType.Percent, Width = 0.5f });
                    break;

                case 2:
                    p1 = new Panel();
                    p1.Dock = DockStyle.Fill;
                    p1.Controls.Add(plotSamples);
                    tlp.Controls.Add(p1, 0, 0);
                    p1 = new Panel();
                    p1.Dock = DockStyle.Fill;
                    p1.Controls.Add(plotFreqAmp);
                    tlp.Controls.Add(p1, 1, 0);
                    p1 = new Panel();
                    p1.Dock = DockStyle.Fill;
                    p1.Controls.Add(plotFreqPhase);
                    tlp.Controls.Add(p1, 2, 0);

                    tlp.ColumnStyles.Add(new ColumnStyle() { SizeType = SizeType.Percent, Width = 0.34f });
                    tlp.ColumnStyles.Add(new ColumnStyle() { SizeType = SizeType.Percent, Width = 0.33f });
                    tlp.ColumnStyles.Add(new ColumnStyle() { SizeType = SizeType.Percent, Width = 0.33f });
                    break;
            }


            //tlp.Controls.Add(new Label() { Text = "Type:", Anchor = AnchorStyles.Left, AutoSize = true }, 0, 0);
            //tlp.Controls.Add(new ComboBox() { Dock = DockStyle.Fill }, 0, 1);
            fGraph.Controls.Add(tlp);
            fGraph.BackColor = System.Drawing.Color.White;
            fGraph.Width = 1000;
            fGraph.Height = 500;
            fGraph.Show();
        }
        private static PlotView GetPlot(List<int> list1, List<float> list2)
        {
            var newListFloat = new List<float>(list1.Count);

            foreach (var l in list1)
            {
                newListFloat.Add(l);
            }

            return GetPlot(newListFloat, list2);
        }

        private static PlotView GetPlot(List<float> xAxisSamples, List<float> yAxisSamples)
        {
            var plot = new PlotView();
            LinearAxis XAxis = new LinearAxis() { Position = AxisPosition.Bottom };//, Minimum = 0, Maximum = 10 };
            LinearAxis YAxis = new LinearAxis();

            PlotModel pm = new PlotModel();
            pm.Axes.Add(XAxis);
            pm.Axes.Add(YAxis);

            FunctionSeries fs = new FunctionSeries();
            for (int i = 0; i < xAxisSamples.Count; i++)
            {
                double x = i;
                fs.Points.Add(new DataPoint(xAxisSamples[i], yAxisSamples[i]));
            }

            pm.Series.Add(fs);
            plot.Model = pm;
            //plot.Anchor = AnchorStyles.Left|AnchorStyles.Right;
            //plot.Size = new System.Drawing.Size(100, 100);
            plot.Dock = DockStyle.Fill;
            return plot;
        }
    }
}
