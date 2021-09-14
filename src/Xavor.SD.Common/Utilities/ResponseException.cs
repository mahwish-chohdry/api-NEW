using System;
using System.Collections.Generic;
using System.Text;
using Xavor.SD.Common.ViewContracts;

namespace Xavor.SD.Common.Utilities
{
    [Serializable]
    public class ResponseException : Exception
    {
        private static readonly string DefaultMessage = "An exception has occured";
        public ResponseDTO _response { get; set; }

        public ResponseException() : base(DefaultMessage) { }

        public ResponseException(ResponseDTO response)
        : base(DefaultMessage)
        {
            _response = response;
        }

        protected ResponseException(
       System.Runtime.Serialization.SerializationInfo info,
       System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
