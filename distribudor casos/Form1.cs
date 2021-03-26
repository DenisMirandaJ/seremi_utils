using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace run_python_script_argv
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void openCorteButton_Click(object sender, EventArgs e)
        {

            corteTextBox.Text = Utils.readFileDialog(corteFileDialog);
        }

        private void OpenTrazadoresButton_Click(object sender, EventArgs e)
        {
            trazadoresTextBox.Text = Utils.readFileDialog(trazadoresFileDialog);
        }

        private async void submitButton_Click(object sender, EventArgs e)
        {
            submitButton.Enabled = false;
            await DistribuidorCasos.DistribuirConPriorizacion(corteTextBox.Text, trazadoresTextBox.Text, casosPorTrazadorNumericSelect.Value.ToString());
            submitButton.Enabled = true;
            MessageBox.Show("Casos priorizados y distribuidos");
        }
    }
}
