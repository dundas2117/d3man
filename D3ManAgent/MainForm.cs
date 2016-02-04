using D3ManComm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace D3ManAgent
{
    public partial class MainForm : Form
    {
        CommunicationManager _communicationManager;
        CancellationTokenSource _cts;
        AgentWorker _agentWorker;
        public MainForm()
        {
            InitializeComponent();
            _cts = new CancellationTokenSource();
            _communicationManager = new CommunicationManager(_cts,false);
            _agentWorker = new AgentWorker(_communicationManager);
            _communicationManager.ProcessIncomingMessage = _agentWorker.HandleMessage;
            _communicationManager.OnListenerStarted = () => { listenerStatusLabel.Text = "Listener started"; };
        }

        private async void btnConnect_Click(object sender, EventArgs e)
        {
            string serverIP = tbxUrl.Text;
            bool result = false;
            try
            {
                result = await _communicationManager.ConnectToServer(serverIP);

                if (result)
                {
                    statusLabel.Text = "Connected";
                }
                else
                {
                    statusLabel.Text = "";
                }
            }
            catch ( Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private async void MainForm_Load(object sender, EventArgs e)
        {
            await _communicationManager.Start(_cts.Token);
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _communicationManager.Stop();
        }
    }
}
