using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OfficeOpenXml;
using Microsoft.Office.Interop;
using Excel = Microsoft.Office.Interop.Excel;

namespace ConsolidadorTrazadoresHistorico
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void folderBrowserDialog1_HelpRequest(object sender, EventArgs e)
        {

        }

        private void buttonSearchFolder_Click(object sender, EventArgs e)
        {   
            // Get Path from folder 
            DialogResult result = folderBrowserDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                folderTextBox.Text = folderBrowserDialog.SelectedPath;
                button1.Enabled =true;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (folderTextBox.Text == null)
            {
                MessageBox.Show("Debe seleccionar una carpeta para comenzar el proceso");
                return;
            }

            // Get Directory
            DirectoryInfo d = new DirectoryInfo(folderTextBox.Text);

            // Allowed extensions
            string[] extensions = new[] { ".xls", ".xlsx" };

            // Get Files from directory
            FileInfo[] files = d.GetFiles().Where(f => extensions.Contains(f.Extension.ToLower())).ToArray();

            // Directory for raw files
            DirectoryInfo noProcesadorDir = Directory.CreateDirectory(Path.Combine(folderTextBox.Text, "no procesados"));
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.Commercial;

            // Creating a new excel output file
            var package = new ExcelPackage();

            // Sheet name and column names
            var ws = package.Workbook.Worksheets.Add("Compilado");

            // Headers for file 
            var headers = new string[] { "Nombre", "Fecha", "Puntaje", "Casos Asignados", "Casos Pendientes", "Casos Derivados", "Casos Cerrados", "Turno" };

            // Select de worksheet
            ws.Select();

            // Headers for table in "Compilado Worksheet"
            int i = 1;
            foreach (string header in headers)
            {
                ws.Cells[1, i].Value = header;
                i++;
            }
            int rowIndex = 2;

            // Read data and save in HISTORICO file
            foreach (FileInfo file in files)
            {
                try
                {
                    int countTracers = Get_Total_Tracers(file);

                    for (int posTracer = 0; posTracer < countTracers; posTracer++)
                    {
                        IDictionary<string, string> row = Get_information(file, posTracer);
                        int colIndex = 1;
                        foreach (string value in row.Values)
                        {
                            ws.Cells[rowIndex, colIndex].Value = value;
                            colIndex++;
                        }
                        rowIndex++;
                    }


                } catch (Exception error)
                {
                    Console.WriteLine(error.ToString());
                    // If any error occurs, add the file to unprocessed
                    File.Copy(file.FullName, Path.Combine(noProcesadorDir.FullName, file.Name));

                }
            }
            // finally save new Excel File
            DateTime today = DateTime.Today;
            string date = today.ToString("dd/MM/yyyy");
            string fileNameOut = "HISTORICO TRAZADORES - " + date + ".xlsx";
            string fileNameCSV = "HISTORICO TRAZADORES - " + date + ".csv";
            Console.WriteLine(folderTextBox.Text);
            string savePath = Path.Combine(folderTextBox.Text, fileNameOut);
            string savePathCSV = Path.Combine(folderTextBox.Text, fileNameCSV);

            package.SaveAs(new FileInfo(savePath));
            

            package.Dispose();
            folderTextBox.Text = "";

            // Convert excel in a csv
            Excel.Application xlapp = new Excel.Application
            {
                Visible = false
            };

            Excel.Workbook xlworkbook = xlapp.Workbooks.Open(savePath);
            xlworkbook = xlapp.ActiveWorkbook;
            Excel.Worksheet xlsheet = xlapp.ActiveSheet;

            xlsheet.SaveAs(savePathCSV, Excel.XlFileFormat.xlCSV);
            xlworkbook.Close();
            xlapp.Quit();


            string Text1 = "Se ha creado el archivo" + fileNameCSV + " en la carpeta selecionada, los archivos que no han podido procesarse estan en la carpeta \"no porcesados\"";
            MessageBox.Show(Text1);

        }

        /***
         * get total tracers for file
         * */
        private static int Get_Total_Tracers(FileInfo file)
        {
            using (var p = new ExcelPackage(file))
            {
                var ws = p.Workbook.Worksheets[0];

                int countTracers = 0;
                int auxNumber = 2;
                ExcelRange aux = ws.Cells[2, auxNumber];

                // Count tracers
                while (aux.Value != null)
                {
                    countTracers++;
                    auxNumber ++;
                    aux = ws.Cells[2, auxNumber];
                }
                
                return countTracers;
            }
        }

        /***
         * get information from tracer in a row
         */
        private static IDictionary<string,string> Get_information(FileInfo file, int auxPos)
        {
            IDictionary<string, string> finalRow = new Dictionary<string, string>();
            using (var p = new ExcelPackage(file))
            {
                var ws = p.Workbook.Worksheets[0];
                // colum for tracer
                int posTracer = 2 + auxPos;

                try
                {   // get data
                    string nombre = ws.Cells[2, posTracer].Value.ToString();
                    string asignados = ws.Cells[3, posTracer].Value.ToString();
                    string derivados = ws.Cells[4, posTracer].Value.ToString();
                    string cerrados = ws.Cells[5, posTracer].Value.ToString();
                    string pendientes = ws.Cells[6, posTracer].Value.ToString();
                    string puntaje = ws.Cells[7, posTracer].Value.ToString();

                    // Format date from Excel files 
                    string auxDate = ws.Cells[1, 3].Value.ToString(); // dd-mm-YYYY 00:00:00:00
                    DateTime dateAux = DateTime.Parse(auxDate);
                    string fecha = dateAux.ToString("dd/MM/yyyy"); // dd-mm-YYYY

                    string turno = ws.Cells[1, 5].Value.ToString();

                    // Add to row 
                    finalRow.Add("Nombre", nombre);
                    finalRow.Add("Fecha", fecha);
                    finalRow.Add("Puntaje", puntaje);
                    finalRow.Add("Asignados", asignados);
                    finalRow.Add("Pendientes", pendientes);
                    finalRow.Add("Derivados", derivados);
                    finalRow.Add("Cerrados", cerrados);
                    finalRow.Add("Turno", turno);

                    // return row
                    return finalRow;

                } catch (Exception e)
                {
                    return null;
                }
            }

        }
    }
}
