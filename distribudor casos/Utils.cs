using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace run_python_script_argv
{
    class Utils
    {
        public static string readFileDialog(FileDialog fileDialog)
        {
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                return fileDialog.FileName;
            }
            return "";
        }
    }
}
