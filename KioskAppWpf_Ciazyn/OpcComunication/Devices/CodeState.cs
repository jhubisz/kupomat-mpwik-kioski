using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace OpcCommunication.Devices
{
    public class CodeState<T>
    {
        private List<OpcConfigurationEntry<T>> configuration;
        private volatile Dictionary<T, Tuple<bool, string>> _state;

        public bool Permit
        {
            get
            {
                return !_state.Values.Any(val => val.Item1 == true);
            }
        }
        public Dictionary<T, Tuple<bool, string>> State
        {
            get => _state;
            set => Interlocked.Exchange(ref _state, value);
        }

        public CodeState(List<OpcConfigurationEntry<T>> _configuration)
        {
            configuration = _configuration;
            var initialState = new Dictionary<T, Tuple<bool, string>>();
            foreach (var configurationEntry in configuration)
            {
                initialState.Add(configurationEntry.Type, new Tuple<bool, string>(false, ""));
            }
            State = initialState;
        }

        public void PopulateState(uint opcState)
        {
            var newState = new Dictionary<T, Tuple<bool, string>>();
            foreach (var configurationEntry in configuration)
            {
                newState[configurationEntry.Type] =
                    new Tuple<bool, string>((opcState & configurationEntry.Mask) != 0, configurationEntry.Msg);
            }
            State = newState;
        }

        public void PopulateStateInt(uint opcState)
        {
            var newState = new Dictionary<T, Tuple<bool, string>>();
            foreach (var configurationEntry in configuration)
            {
                newState[configurationEntry.Type] =
                    new Tuple<bool, string>(opcState == configurationEntry.Mask, configurationEntry.Msg);
            }
            State = newState;
        }

        public List<string> GetErrorMessages()
        {
            var snapshot = _state;
            var errorMessages = new List<string>();
            foreach (var item in snapshot)
            {
                if (item.Value.Item1)
                {
                    errorMessages.Add(item.Value.Item2);
                }
            }
            return errorMessages;
        }
    }
}
