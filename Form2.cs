using System;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Security.Cryptography;

namespace AESNotes
{
    public partial class Form2 : Form
    {
        MD5 md5 = MD5.Create();
        AesCryptographyService AES = new AesCryptographyService();
        String AppData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        public string workFile { get { return AppData + "\\AESNotes\\data.bin"; } }
        public string sigFile { get { return AppData + "\\AESNotes\\signature.md5"; } }

        public Form2()
        {
            InitializeComponent();
        }
        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            byte[] key = Encoding.UTF8.GetBytes(label1.Text);
            byte[] iv = key;
            Array.Reverse(iv);
            File.WriteAllBytes(workFile, AES.Encrypt(Encoding.UTF8.GetBytes(richTextBox1.Text), key, iv));
            byte[] decoded = AES.Decrypt(File.ReadAllBytes(workFile), key, iv);
            File.WriteAllBytes(sigFile, md5.ComputeHash(decoded));
        }
    }
}
