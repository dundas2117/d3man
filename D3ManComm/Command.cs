using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D3ManComm
{
    public class Command
    {
        public const string CMD_Connect = "Connect";
        public const string CMD_SetLeader = "SetLeader";
        public const string CMD_SetFollower = "SetFollower";

        public const string MSG_OK = "ok";

        public const char DELIMITER = '|';
    }
}
