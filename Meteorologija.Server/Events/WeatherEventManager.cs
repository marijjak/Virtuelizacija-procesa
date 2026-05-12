using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meteorologija.Server.Events
{
    public delegate void TransferStartedHandler(string message);
    public delegate void SampleReceivedHandler(string message);
    public delegate void TransferCompletedHandler(string message);
    public delegate void WarningRaisedHandler(string message);

    public class WeatherEventManager
    {
        public event TransferStartedHandler OnTransferStarted;
        public event SampleReceivedHandler OnSampleReceived;
        public event TransferCompletedHandler OnTransferCompleted;
        public event WarningRaisedHandler OnWarningRaised;

        public void RaiseTransferStarted(string message)
        {
            OnTransferStarted?.Invoke(message);
        }

        public void RaiseSampleReceived(string message)
        {
            OnSampleReceived?.Invoke(message);
        }

        public void RaiseTransferCompleted(string message)
        {
            OnTransferCompleted?.Invoke(message);
        }

        public void RaiseWarning(string message)
        {
            OnWarningRaised?.Invoke(message);
        }
    }
}