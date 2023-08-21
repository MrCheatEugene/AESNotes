using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using System.Windows.Forms;

namespace AESNotes
{
    public partial class Form1 : Form
    {
        AesCryptographyService AES = new AesCryptographyService();
        MD5 md5 = MD5.Create();
        String AppData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        public string workDir { get { return AppData + "\\AESNotes"; } }
        public string workFile { get { return AppData + "\\AESNotes\\data.bin"; } }
        public string sigFile { get { return AppData + "\\AESNotes\\signature.md5"; } }

        public Form1()
        {
            InitializeComponent();
    }

    private void button1_Click(object sender, EventArgs e)
        {
            byte[] key = Encoding.UTF8.GetBytes(textBox1.Text);
            if(key.Length > 16)
            {
                MessageBox.Show("Длинна ключа должна быть меньше 16 байт.");
                return;
            }
            else
            {
                if (!Directory.Exists(workDir))
                {
                    Directory.CreateDirectory(workDir);
                }
                if (!File.Exists(workFile))
                {
                    byte[] empty = { };
                    File.WriteAllBytes(workFile, empty);
                }
                byte[] iv = key;
                Array.Reverse(iv);
                Form form = new Form2();
                try
                {
                    byte[] decoded = AES.Decrypt(File.ReadAllBytes(workFile), key, iv);
                    if (File.Exists(sigFile))
                    {
                        if (BitConverter.ToString(md5.ComputeHash(decoded)) != BitConverter.ToString(File.ReadAllBytes(sigFile))) // это единственный рабочий метод, простите
                        {
                            MessageBox.Show("Не удалось проверить целостность файла. Возможно, вы указали неверный ключ? Если вы уверены в подлинности ключа, можете удалить кэшированный хэш файла, и повторить попытку. ВНИМАНИЕ: ПРИ ТАКОМ МЕТОДЕ ЗАГРУЗКИ, ДАННЫЕ МОГУТ БЫТЬ БЕЗВОЗВРАТНО УТЕРЯНЫ!");
                            return;
                        }
                    }

                    form.Controls[0].Text = Encoding.UTF8.GetString(decoded);
                    form.Controls[1].Text = textBox1.Text;
                    form.ShowDialog();
                }catch(Exception)
                {
                    MessageBox.Show("Ошибка дешифровки. Возможно, ключ неверный, или имеет неверную длинну.");
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            File.Delete(sigFile);
            MessageBox.Show("Успех!");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            File.Delete(workFile);
            File.Delete(sigFile);
            MessageBox.Show("Успех!");
        }
    }
}
