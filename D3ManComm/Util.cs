using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D3ManComm
{
    public class Util
    {
        public static void Log(string msg)
        {
            using (StreamWriter sw = new StreamWriter("D3Man_" + DateTime.Today.ToString("yyyy_MM_dd") + ".log", true))
            {
                sw.WriteLine(DateTime.Now.ToString("yyyy_MM_dd HH:mm:ss") + " " + msg);
                sw.Flush();
                sw.Close();
            }

        }
    }
}
