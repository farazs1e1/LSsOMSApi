using OMSServices.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMSServices.Data
{
    public class ServerResponse
    {
        public ENServerReplyType ReplyType { get; set; } = ENServerReplyType.SERVER_REPLY_NONE;
        public ENServerReplyOperations Operation { get; set; } = ENServerReplyOperations.OPERATION_NONE;
        public string Message { get; set; } = "";
        public bool ExtraParamsAvailable { get; set; } = false;
        public string ExtraParams { get; set; } = "";
    }
}
