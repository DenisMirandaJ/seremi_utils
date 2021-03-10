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

namespace planillasContactsoExtractor
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private IDictionary<string, string> extractDataFromContactosFile(string filename)
        {
            Console.WriteLine(filename);
            // Dictionary for each data read
            IDictionary<string, string> row = new Dictionary<string, string>();
            var fi = new FileInfo(filename);
            bool masFilas = false;
            using (var p = new ExcelPackage(fi))
            {
                var ws = p.Workbook.Worksheets[0];
                if (ws.Cells[12, 2].Value == null || !ws.Cells[12, 2].Value.ToString().Contains("Nombre Empresa"))
                    return null;
                if (ws.Cells[12, 7].Value == null || !ws.Cells[12, 7].Value.ToString().Contains("Nombre caso positivo"))
                    return null;
                if (ws.Cells[23, 2].Value != null && ws.Cells[23, 2].Value.ToString().Contains("Nombre del trabajador"))
                    masFilas=true;
               
                row.Add("Fecha", "");
                row.Add("INGRESO", "");
                if (ws.Cells[14, 3].Value != null && masFilas==false) 
                { 
                    row.Add("OAL/HONO", ws.Cells[14, 3].Value.ToString().ToUpper()); 
                }
                else if (ws.Cells[15, 3].Value != null && masFilas == true)
                {
                    row.Add("OAL/HONO", ws.Cells[15, 3].Value.ToString().ToUpper());
                }
                else 
                { 
                    row.Add("OAL/HONO", ""); 
                }
                row.Add("NOMBRE COMPLETO CASO INDICE", ws.Cells[12,8].Value.ToString().ToUpper());
                row.Add("RUT",ws.Cells[13,8].Value.ToString().ToUpper());
                row.Add("FOLIO", ws.Cells[15,8].Value.ToString().ToUpper());
                row.Add("SIN/ASIN","");
                row.Add("INICIO C", "");
                row.Add("TERMINO C", "");
                row.Add("EMPRESA", ws.Cells[12,3].Value.ToString().ToUpper());
                row.Add("ESTADO", "");

                //Numero contactos
                int contactosCount = 0;

                int firstContactoRowIndex = 23;

                if (masFilas == true)
                {
                    firstContactoRowIndex = 24;
                }
                
                ExcelRange aux = ws.Cells[firstContactoRowIndex, 2];
                while (aux.Value != null)
                {
                    contactosCount++;
                    firstContactoRowIndex++;
                    aux = ws.Cells[firstContactoRowIndex, 2];
                }

                row.Add("N° CELS", contactosCount.ToString());
                
                // Agrega Contactos Estrechos
                if (contactosCount != 0)
                {
                    int countAux = 23;
                    if(masFilas == true)
                    {
                        countAux = 24;
                    }
                        
                    for (int i=0; i < contactosCount; i++)
                    {
                        try
                        {
                            string nombre = "";
                            string apellidoP = "";
                            string apellidoC = "";
                            if (ws.Cells[countAux, 2].Value  != null )
                            {
                                nombre = ws.Cells[countAux, 2].Value.ToString();
                            }
                            if(ws.Cells[countAux, 3].Value != null)
                            {
                                apellidoP = ws.Cells[countAux, 3].Value.ToString();
                            }
                            if(ws.Cells[countAux, 4].Value != null)
                            {
                                apellidoC = ws.Cells[countAux, 4].Value.ToString();
                            }
                            string nombreCompleto = nombre + " " +apellidoP + " " + apellidoC;

                            row.Add("C " + countAux.ToString(), nombreCompleto.ToUpper());
                            row.Add("RUT CEL" + countAux.ToString(), ws.Cells[countAux, 5].Value.ToString().ToUpper());
                            row.Add("INICIO C CEL" + countAux.ToString(), "");
                            row.Add("TERMINO C CEL" + countAux.ToString(), "");
                            if (ws.Cells[countAux, 9].Value != null)
                            {
                                row.Add("NUMERO" + countAux.ToString(), ws.Cells[countAux, 9].Value.ToString());
                            }
                            else
                            {
                                row.Add("NUMERO" + countAux.ToString(), "");
                            }
                            row.Add("DIRECCION" + countAux.ToString().ToUpper(), "");
                            row.Add("1" + countAux.ToString().ToUpper(), "");
                            row.Add("2" + countAux.ToString().ToUpper(), "");
                            row.Add("3" + countAux.ToString().ToUpper(), "");
                            countAux++;
                        } catch (Exception error)
                        {
                            return null;
                        }
                  

                    }
                }
            }
            return row;
        }

        private void searchButton_Click(object sender, EventArgs e)
        {
            DialogResult result = folderBrowserDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                folderTextBox.Text = folderBrowserDialog.SelectedPath;
            }
        }

        private void submitButton_Click(object sender, EventArgs e)
        {   
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
            var p = new ExcelPackage();

            // Sheet name and column names
            var ws = p.Workbook.Worksheets.Add("Hoja 1");
            
            // Index for headers positions
            int i = 1; // Position Excel.Cell
            int k = 1; // Count from ContactosEstrechos
            

            // Headers for file 
            var headers = new string[] { "FECHA","INGRESO", "OAL/HONO", "NOMBRE COMPLETO CASO INDICE",
                                        "RUT", "FOLIO", "SIN/ASIN","INICIO C", "TERMINO C", "EMPRESA",
                                        "ESTADO", "N° CELS"};

            var headersCEL = new string[] { "RUT", "INICIO C", "TERMINO C","NUMERO","DIRECCION"};

            // Select de worksheet
            ws.Select();
            
            // Basic Data
            foreach (string header in headers)
            {
                ws.Cells[1, i].Value = header;     
                i++;
            }

            // Headers Contacto Estrecho Laboral
            for (int x =0; x <30; x++)
            {
                ws.Cells[1, i].Value = "C " + k.ToString();
                k++;

                i++;

                // loops for add Contactos Estrechos Laborales Headers
                foreach (string hc in headersCEL)
                {
                    ws.Cells[1, i].Value = hc;
                    i++;
                }

                for (int j = 1; j<4;j++)
                {
                    ws.Cells[1, i].Value = j;
                    i++;
                }
            }

            int rowIndex = 2;
            foreach (FileInfo file in files)
            {   try
                {   // File data extraction
                    IDictionary<string, string> row = extractDataFromContactosFile(file.FullName);

                    // If it fails, it is added to the unprocessed folder and continues
                    if (row == null)
                    {   
                        File.Copy(file.FullName, Path.Combine(noProcesadorDir.FullName, file.Name));
                        continue;
                    }
                    // If it doesn't fail, add the column to the output excel
                    int colIndex = 1;
                    foreach (string value in row.Values)
                    {
                        ws.Cells[rowIndex, colIndex].Value = value;
                        colIndex++;
                    }

                    rowIndex++;
                } catch (Exception error)
                {
                    Console.WriteLine(error.ToString());
                    // If any error occurs, add the file to unprocessed
                    File.Copy(file.FullName, Path.Combine(noProcesadorDir.FullName, file.Name));
                }

            }
            // finally save new Excel File
            string savePath = Path.Combine(folderTextBox.Text, "consolidados.xlsx");
            p.SaveAs(new FileInfo(savePath));
            const string Text1 = "Se ha creado el archivo consolidado.xslx en la carpeta selecionada, los archivos que no han podido procesarse estan en la carpeta \"no porcesados\"";
            MessageBox.Show(Text1);
            folderTextBox.Text = "";
        }

        private void folderTextBox_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
