using D3ManComm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D3ManAgent
{
    public class AgentWorker
    {
        CommunicationManager _commMan;
        public Action OnSetLeader
        {
            get;
            set;
        } = null;

        public AgentWorker(CommunicationManager commMan)
        {
            _commMan = commMan;
        }

        public string HandleMessage(string request)
        {
            string result = "";
            Util.Log("Message received: " + request);
            string[] info = request.Split(new char[] { Command.DELIMITER });
            switch (info[0])
            {
                case Command.CMD_SetLeader:
                    result = Command.MSG_OK;
                    if (OnSetLeader != null)
                    {
                        OnSetLeader();
                    }
                    break;


            }

            Util.Log("Replied Message: " + result);
            return result;
        }
    }
}
