using System;
using System.Collections.Generic;
using System.Linq;

namespace OpcCommunication.Devices
{
    public class CodeState<T>
    {
        private List<OpcConfigurationEntry<T>> configuration;

        public bool Permit
        {
            get
            {
                return !State.Values.Any(val => val.Item1 == true);
            }
        }
        public Dictionary<T, Tuple<bool, string>> State { get; set; }
        
        public CodeState(List<OpcConfigurationEntry<T>> _configuration)
        {
            configuration = _configuration;
            State = new Dictionary<T, Tuple<bool, string>>();
            foreach (var configurationEntry in configuration)
            {
                State.Add(configurationEntry.Type, new Tuple<bool, string>(false, ""));
            }
        }
          
        public void PopulateState(uint opcState)
        {
            State = new Dictionary<T, Tuple<bool, string>>();
            foreach(var configurationEntry in configuration)
            {
                State[configurationEntry.Type] = 
                    new Tuple<bool, string>((opcState & configurationEntry.Mask) != 0, configurationEntry.Msg);
            }
        }

        public List<string> GetErrorMessages()
        {
            var errorMessages = new List<string>();
            foreach(var item in State)
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
