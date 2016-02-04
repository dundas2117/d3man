using D3ManComm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace D3ManMaster
{
    public partial class MainForm : Form
    {
        CommunicationManager _communicationManager;
        MasterWorker _masterWorker;
        CancellationTokenSource _cts;

        public MainForm()
        {
            InitializeComponent();
            _cts = new CancellationTokenSource();
            _communicationManager = new CommunicationManager(_cts, true);
            _masterWorker = new MasterWorker();
            _masterWorker.OnAgentConnected = OnAgentConnected;
            _communicationManager.ProcessIncomingMessage = _masterWorker.HandleMessage;
            _communicationManager.OnListenerStarted = () => { statusLabel.Text = "Listener started"; };

        }

      
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {

            _communicationManager.Stop();
        }

        public void OnAgentConnected(string agentName)
        {
            dgvAgents.Rows.Add(agentName, "Connected", "");
        }

        private async void btnSetLeader_Click(object sender, EventArgs e)
        {
            if ( dgvAgents.SelectedRows.Count == 0)
            {
                MessageBox.Show("Select an agent first.");
                    return;
            }
            try
            {
                bool result = await _communicationManager.SetLeader(dgvAgents.SelectedRows[0].Cells[0].Value.ToString());
                if (result)
                {
                    dgvAgents.SelectedRows[0].Cells[2].Value = "Y";
                }
            }
            catch   ( Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


        }

        private async void MainForm_Load(object sender, EventArgs e)
        {
            await _communicationManager.Start(_cts.Token);

        }
    }
}
