using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace EncryptDecryptFile
{
    public partial class frmMain : Form
    {
        byte[] abc;
        byte[,] table;

        public frmMain()
        {
            InitializeComponent();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            rdoEnc.Checked = true;

            //Init abc, table
            abc = new byte[256];
            for(int i=0;i<256; i++)
            {
                abc[i] = Convert.ToByte(i);
            }
            table = new byte[256,256];
            for (int i = 0; i < 256; i++)
            {
                for(int j = 0; j < 256; j++)
                {
                    table[i, j] = abc[(i + j) % 256];
                }
            }
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog od = new OpenFileDialog();
            od.Multiselect = false;
            if (od.ShowDialog() == DialogResult.OK)
            {
                txtPath.Text = od.FileName;
            }
        }

        private void rdoEnc_CheckedChanged(object sender, EventArgs e)
        {
            if (rdoEnc.Checked)
            {
                rdoDec.Checked = false;
            }
        }

        private void rdoDec_CheckedChanged(object sender, EventArgs e)
        {
            if (rdoDec.Checked)
            {
                rdoEnc.Checked = false;
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            //Check input values
            if (!File.Exists(txtPath.Text))
            {
                MessageBox.Show("File does not exist.");
                return;
            }
            if (String.IsNullOrEmpty(txtPass.Text))
            {
                MessageBox.Show("Password empty. Please enter your password");
                return;
            }

            //Get file content and key for encrypt decrypt
            try
            {
                byte[] fileContent = File.ReadAllBytes(txtPath.Text);
                byte[] passWordTmp = Encoding.ASCII.GetBytes(txtPass.Text);
                byte[] keys = new byte[fileContent.Length];

                for(int i = 0; i < fileContent.Length; i++)
                {
                    keys[i] = passWordTmp[i % passWordTmp.Length];
                }

                //Encrypt
                byte[] result = new byte[fileContent.Length];
                if (rdoEnc.Checked)
                {
                    for(int i = 0; i < fileContent.Length; i++)
                    {
                        byte value = fileContent[i];
                        byte key = keys[i];
                        int valueIndex = -1, keyIndex = -1;
                        for(int j = 0; j < 256; j++)
                        {
                            if (abc[j] == value)
                            {
                                valueIndex = j;
                                break;
                            }
                        }
                        for(int j = 0; j < 256; j++)
                        {
                            if (abc[j] == key)
                            {
                                keyIndex = j;
                                break;
                            }
                        }
                        result[i] = table[keyIndex, valueIndex];
                    }
                }
                //Decrypt
                else
                {
                    for (int i = 0; i < fileContent.Length; i++)
                    {
                        byte value = fileContent[i];
                        byte key = keys[i];
                        int valueIndex = -1, keyIndex = -1;
                        for (int j = 0; j < 256; j++)
                        {
                            if (abc[j] == key)
                            {
                                keyIndex = j;
                                break;
                            }
                        }
                        for (int j = 0; j < 256; j++)
                        {
                            if (table[keyIndex,j] == value)
                            {
                                valueIndex = j;
                                break;
                            }
                        }
                        result[i] = abc[valueIndex];
                    }
                }
                //Save result to new file with the same extention
                String fileExt = Path.GetExtension(txtPath.Text);
                String fileName = Path.GetFileNameWithoutExtension(txtPath.Text);
                SaveFileDialog sd = new SaveFileDialog();
                sd.Filter = "Files (*" + fileExt + ") | *" + fileExt;
                if (rdoEnc.Checked)
                {
                    sd.FileName = fileName + "_Encrypt";
                }
                else
                {
                    sd.FileName = fileName.Replace("_Encrypt", "_Decrypt");
                }
                if (sd.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllBytes(sd.FileName, result);
                    MessageBox.Show("Process is success.");
                }
            }
            catch
            {
                MessageBox.Show("File is in use. Close other program is using this file and try again");
                return;
            }
        }
    }
}
