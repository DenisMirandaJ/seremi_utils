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
            IDictionary<string, string> row = new Dictionary<string, string>();
            var fi = new FileInfo(filename);
            using (var p = new ExcelPackage(fi))
            {
                var ws = p.Workbook.Worksheets[0];
                if (!ws.Cells[12, 2].Value.ToString().Contains("Nombre Empresa"))
                    return null;
                if (!ws.Cells[12, 7].Value.ToString().Contains("Nombre caso positivo"))
                    return null;
                if (!ws.Cells[22, 2].Value.ToString().Contains("Nombre del trabajador"))
                    return null;

                //Datos empresa
                row.Add("Nombre Empresa", ws.Cells[12, 3].Value.ToString());
                row.Add("Rut Empresa", ws.Cells[13, 3].Value.ToString());
                row.Add("Nombre OAL", ws.Cells[14, 3].Value.ToString());
                row.Add("Región", ws.Cells[15, 3].Value.ToString());
                row.Add("Comuna", ws.Cells[16, 3].Value.ToString());
                row.Add("Persona responsable", ws.Cells[17, 3].Value.ToString());
                row.Add("Telefono responsable", ws.Cells[18, 3].Value.ToString());
                //DAtos caso
                row.Add("Nombre caso positivo", ws.Cells[12, 8].Value.ToString());
                row.Add("Rut caso positivo", ws.Cells[13, 8].Value.ToString());
                row.Add("Teléfono caso positivo", ws.Cells[14, 8].Value.ToString());
                row.Add("Folio Epivigila", ws.Cells[15, 8].Value.ToString());
                //Numero contactos
                int contactosCount = 0;
                int firstContactoRowIndex = 23;
                ExcelRange aux = ws.Cells[firstContactoRowIndex, 2];
                while (aux.Value != null)
                {
                    contactosCount++;
                    firstContactoRowIndex++;
                    aux = ws.Cells[firstContactoRowIndex, 2];
                }

                row.Add("Numero contactos", contactosCount.ToString());
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
            DirectoryInfo d = new DirectoryInfo(folderTextBox.Text);
            string[] extensions = new[] { ".xls", ".xlsx" };
            FileInfo[] files = d.GetFiles().Where(f => extensions.Contains(f.Extension.ToLower())).ToArray();
            DirectoryInfo noProcesadorDir = Directory.CreateDirectory(Path.Combine(folderTextBox.Text, "no procesados"));
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.Commercial;
            var p = new ExcelPackage();
            var ws = p.Workbook.Worksheets.Add("Hoja 1");
            var headers = new string[] { "Nombre Empresa", "Rut Empresa", "Nombre OAL", "Región", "Comuna", "Persona responsable", "Telefono responsable", "Nombre caso positivo", "Rut caso positivo", "Teléfono caso positivo", "Folio Epivigila", "Numero contactos" };
            int i = 1;
            foreach (string header in headers)
            {
                ws.Cells[1, i].Value = header;
                i++;
            }
            int rowIndex = 2;
            foreach (FileInfo file in files)
            {   try
                {
                    IDictionary<string, string> row = extractDataFromContactosFile(file.FullName);
                    if (row == null)
                    {
                        File.Copy(file.FullName, Path.Combine(noProcesadorDir.FullName, file.Name));
                        continue;
                    }
                    int colIndex = 1;
                    foreach (string value in row.Values)
                    {
                        ws.Cells[rowIndex, colIndex].Value = value;
                        colIndex++;
                    }
                    rowIndex++;
                } catch (Exception error)
                {
                    File.Copy(file.FullName, Path.Combine(noProcesadorDir.FullName, file.Name));
                }

            }
            string savePath = Path.Combine(folderTextBox.Text, "consolidad.xlsx");
            p.SaveAs(new FileInfo(savePath));
            const string Text1 = "Se ha creado el archivo consolidado.xslx en la carpeta selecionada, los archivos que no han podido procesarse estan en la carpeta \"no porcesados\"";
            MessageBox.Show(Text1);
            folderTextBox.Text = "";
        }
    }
}
