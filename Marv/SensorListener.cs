using System;
using System.IO.Ports;
using System.Linq;
using System.Windows.Threading;
using Marv.Common.Graph;

namespace Marv
{
    public class SensorListener
    {
        private SerialPort serialPort = new SerialPort();
        private DispatcherTimer timer = new DispatcherTimer();
        private Vertex vertexViewModel;

        public SensorListener()
        {
            this.serialPort.BaudRate = 9600;
            this.serialPort.DataBits = 8;
            this.serialPort.Parity = Parity.None;
            this.serialPort.StopBits = StopBits.One;
            this.serialPort.ReadTimeout = 10;
            this.serialPort.WriteTimeout = 2000;

            this.timer.Interval = new TimeSpan(0, 0, 0, 0, 100);
            this.timer.Tick += timer_Tick;
        }

        public event EventHandler<Vertex> NewEvidenceAvailable;

        public void Start(Vertex aVertexViewModel)
        {
            this.vertexViewModel = aVertexViewModel;

            var portNames = SerialPort.GetPortNames();

            if (portNames.Count() > 0)
            {
                this.serialPort.PortName = portNames[0];
            }

            if (!this.serialPort.IsOpen) this.serialPort.Open();

            this.timer.Start();
        }

        public void Stop()
        {
            this.timer.Stop();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            try
            {
                var value = this.serialPort.ReadByte();

                var nStates = this.vertexViewModel.States.Count;

                var stateIndex = value * (nStates - 1) / 255;

                this.vertexViewModel.SelectState(stateIndex);

                var handler = this.NewEvidenceAvailable;

                if (handler != null)
                {
                    handler(this, this.vertexViewModel);
                }
            }
            catch (TimeoutException)
            {
                // do nothing for now
            }
        }
    }
}