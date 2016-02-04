using D3ManComm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D3ManMaster
{
    public class MasterWorker
    {
        public Action<string> OnAgentConnected
        {
            get;
            set;
        } = null;

        public string HandleMessage(string request )
        {
            string result = "";
            string[] info = request.Split(new char[] { Command.DELIMITER });
            switch (info[0])
            {
                case Command.CMD_Connect:
                    result = Command.MSG_OK;
                    if (OnAgentConnected != null)
                    {
                        OnAgentConnected(info[1]);
                    }
                    break;


            }

            return result;
        }
    }
}
