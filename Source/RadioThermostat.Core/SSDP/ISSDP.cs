using System;
using System.Collections.Generic;
using System.Net;

namespace RadioThermostat.Core.SSDP
{
    public interface ISsdp : IDisposable
    {
        void Cleanup();
        void StartSearch();
        void StopSearch(SearchStoppedReason reason);

        event EventHandler<SearchStoppedEventArgs> SearchStopped;
        event EventHandler<EventArgs> SearchStarted;
        event EventHandler<DeviceFoundEventArgs> DeviceFound;
    }

    public enum SearchStoppedReason
    {
        Aborted,
        Complete,
        Error,
        TimedOut
    }

    public sealed class SearchStoppedEventArgs : EventArgs
    {
        public SearchStoppedReason Reason { get; private set; }

        public SearchStoppedEventArgs(SearchStoppedReason reason)
        {
            Reason = reason;
        }
    }

    public sealed class DeviceFoundEventArgs : EventArgs
    {
        public EndPoint RemoteEndpoint { get; private set; }
        public Dictionary<string, string> Results { get; private set; }

        public DeviceFoundEventArgs(EndPoint remoteEndpoint, Dictionary<string, string> results)
        {
            RemoteEndpoint = remoteEndpoint;
            Results = results;
        }
    }
}
