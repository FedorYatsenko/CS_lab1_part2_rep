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

namespace CS_lab1_part2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void buttonOpenFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = @"E:\GoogleDisk\Study\Temp\Комп'ютерні системи\lab1";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string filename = openFileDialog1.FileName;

                labelResult.Text = "";
                length = 0;
                ConvertFile(filename);
            }
        }

        private static readonly int CHUNK_SIZE = 30720;

        private void ConvertFile(string filename)
        {
            using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                using (BinaryReader br = new BinaryReader(fs))
                {
                    
                    byte[] chunk;
                    MyBuffer buffer = new MyBuffer(this);

                    chunk = br.ReadBytes(CHUNK_SIZE);
                    while (chunk.Length > 0)
                    {
                        buffer.Add(chunk);
                        chunk = br.ReadBytes(CHUNK_SIZE);
                    }
                }
            }
        }

        private int length;

        private void PrintBuff(string s)
        {
            while(s.Length > 76 - length)
            {
                labelResult.Text += s.Substring(0, 76);
                s = s.Remove(0, 76);

                labelResult.Text += "\n";

                length = 0;
            }

            length = s.Length;
            labelResult.Text += s;
        }

        class MyBuffer
        {
            string code = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";

            Form1 form1;

            public MyBuffer(Form1 form1)
            {
                this.form1 = form1;
            }

            public void Add(byte[] byteArray)
            {
                /*
                int singleCode = byteArray[0] * 65536 
                    + (byteArray.Length > 1?byteArray[1]:0) * 256 
                    + (byteArray.Length > 2 ? byteArray[2] : 0);

                string result = code[singleCode / 262144].ToString()
                    + code[singleCode / 4096 % 64]
                    + (byteArray.Length > 1 ? code[singleCode % 4096 / 64].ToString(): "=")
                    + (byteArray.Length > 2 ? code[singleCode % 64].ToString(): "=");
                    */

                string result = "";
                for (int i = 0; i < byteArray.Length; i += 3)
                {
                    result += code[byteArray[i] >> 2].ToString();
                    result += code[((byteArray[i] & 3) << 4) + (byteArray.Length > i + 1 ? byteArray[i + 1] >> 4 : 0)];
                    result += (byteArray.Length > i + 1 ? code[((byteArray[i + 1] & 15) << 2) + (byteArray.Length > i + 2 ? byteArray[i + 2] >> 6 : 0)].ToString() : "=");
                    result += (byteArray.Length > i + 2 ? code[byteArray[i + 2] & 63].ToString() : "=");
                }

                form1.PrintBuff(result);

            }
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();

            saveFileDialog1.Filter = "txt files (*.txt)|*.txt";

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string filename = saveFileDialog1.FileName;

                using (StreamWriter sw = new StreamWriter(filename, false, System.Text.Encoding.ASCII))
                {
                    foreach (char c in labelResult.Text)
                        if (c == '\n')
                            sw.WriteLine();
                        else
                            sw.Write(c);

                    MessageBox.Show("Success");
                }
            }
        }
    }
}
