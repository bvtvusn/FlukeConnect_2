using System.Collections.Generic;

namespace FlukeConnect_Fluke289
{
    public class CommandResult
    {
        public enum ResponseStates
        {
            OK = 0,
            SyntaxError = 1,
            ExecutionError = 2
        }
        public ResponseStates ResponseState { get; set; }
        public string ResponseString { get; set; }
    }
}