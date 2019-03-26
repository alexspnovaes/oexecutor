using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Common;
using System.Configuration;

namespace OExecutor
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int size = -1;
            DialogResult result = openFileDialog1.ShowDialog(); // Show the dialog.
            if (result == DialogResult.OK) // Test result.
            {
                string file = openFileDialog1.FileName;

                DataTable logs = new DataTable();
                logs.Columns.Add("Nome");
                logs.Columns.Add("Descricao");

                try
                {
                   

                    var caminhoAplicacao = textBox1.Text;
                    var lines = File.ReadAllLines(file);

                    string sqlConnectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

                    using (SqlConnection conn = new SqlConnection(sqlConnectionString))
                    {
                        Server server = new Server(new ServerConnection(conn));
                        foreach (var line in lines)
                        {
                            var caminho = ($@"{caminhoAplicacao}\DATABASE\SCRIPTS{line.Replace("MSSQL,.", "")}").Trim();
                            if (File.Exists(caminho))
                            {
                                try
                                {
                                    var script = File.ReadAllText(caminho, Encoding.GetEncoding("iso-8859-1"));
                                    server.ConnectionContext.ExecuteNonQuery(script);
                                }
                                catch(Exception ex)
                                {
                                    logs.Rows.Add(line, "Erro ao executar script: " + ex.InnerException);
                                    break;
                                }
                                logs.Rows.Add(line, "Executado com sucesso!");
                            }
                            else
                            {
                                logs.Rows.Add(line, "Arquivo não encontrado");
                                break;
                            }

                        }
                    }
                }
                catch (IOException ex)
                {
                    throw new IOException("Erro ao processar o arquivo:" +ex.InnerException);
                }
                finally
                {
                    dgvLogs.DataSource = null;
                    dgvLogs.DataSource = logs;                    
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult result = folderBrowserDialog1.ShowDialog(); // Show the dialog.
            if (result == DialogResult.OK) // Test result.
            {
                textBox1.Text = folderBrowserDialog1.SelectedPath;
            }
        }
    }
}
