using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace HazelDash {

    public partial class Dashboard : Form
    {
        public Dashboard()
        {
            InitializeComponent();
        }

        private void Dashboard_Load(object sender, EventArgs e)
        {
            new Task(updateTodoLoop).Start();
            DashboardServer server = new DashboardServer();
            server.start();
        }

        private void updateTodoLoop()
        {
            while (true)
            {
                Thread.Sleep(1000);
                updateTodo();
            }
        }

        private void updateTodo()
        {
            todoListBox.Invoke(new Action( todoListBox.Items.Clear));
            for (uint i = 0; i < TODOs.size(); i++)
            {
                Action action = () => todoListBox.Items.Add( TODOs.get(i));
                todoListBox.Invoke(action);
            }
        }
    }
}
