using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sotierprogramm_1._1
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();

        }

        public Point mouseLocation;
        private bool isDragging = false;
        private Point lastCursor;
        private Point lastForm;


        public string QuellBilder;
        public string ZielBilder;

        public int anzahlimQuell;
        public int anzahlimZiel;

        public int a = 0;
        public int b = 0;
        public int c = 0;

        private void Form1_Load(object sender, EventArgs e)
        {
            label6.Visible = false;
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button == MouseButtons.Left)
            {
                isDragging = true;
                lastCursor = Cursor.Position;
                lastForm = this.Location;
            }
        }


        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (isDragging)
            {
                Point delta = Point.Subtract(Cursor.Position, (Size)lastCursor);
                this.Location = Point.Add(lastForm, (Size)delta);
            }
        }

        private void panel1_MouseUp(object sender, MouseEventArgs e)
        {
            base.OnMouseUp(e);
            isDragging = false;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            pictureBox1.Padding = new Padding(0);
            pictureBox1.Margin = new Padding(0);
            pictureBox2.Padding = new Padding(0);
            pictureBox2.Margin = new Padding(0);

        }

        private void pictureBox1_MouseHover(object sender, EventArgs e)
        {
            pictureBox1.BackColor = Color.Red;
        }

        private void pictureBox1_MouseLeave(object sender, EventArgs e)
        {
            pictureBox1.BackColor = Color.Transparent;

        }

        private void pictureBox1_MouseEnter(object sender, EventArgs e)
        {

            pictureBox1.BackColor = Color.Red;
        }

        private void pictureBox2_MouseEnter(object sender, EventArgs e)
        {
            pictureBox2.BackColor = Color.LightBlue;


        }

        private void pictureBox2_MouseHover(object sender, EventArgs e)
        {
            pictureBox2.BackColor = Color.LightBlue;

        }

        private void pictureBox2_MouseLeave(object sender, EventArgs e)
        {
            pictureBox2.BackColor = Color.Transparent;

        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderDlg = new FolderBrowserDialog();
            folderDlg.ShowNewFolderButton = true;
            DialogResult result = folderDlg.ShowDialog();
            if (result == DialogResult.OK)
            {
                textBox1.Text = folderDlg.SelectedPath;
                Environment.SpecialFolder root = folderDlg.RootFolder;
            }
            if (Directory.Exists(textBox1.Text))
            {
                int totalFileCount = Directory.GetFiles(textBox1.Text, "*.*", SearchOption.AllDirectories).Length;


                label3.Text = "Anzahl im Ordner: " + totalFileCount.ToString();
            }
            else
            {
                MessageBox.Show("Der angegebene Ordner existiert nicht.");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderDlg = new FolderBrowserDialog();
            folderDlg.ShowNewFolderButton = true;
            DialogResult result = folderDlg.ShowDialog();
            if (result == DialogResult.OK)
            {
                textBox2.Text = folderDlg.SelectedPath;
                Environment.SpecialFolder root = folderDlg.RootFolder;
            }

            if (Directory.Exists(textBox2.Text))
            {
                int totalFileCount = Directory.GetFiles(textBox2.Text, "*.*", SearchOption.AllDirectories).Length;


                label4.Text = "Anzahl im Ordner: " + totalFileCount.ToString();
            }
            else
            {
                MessageBox.Show("Der angegebene Ordner existiert nicht.");
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (textBox1.Text + textBox2.Text != "")
            {
                btnStart.Enabled = true;
            }
            else
            {
                btnStart.Enabled = false;
            }
        }

        private void timer3_Tick(object sender, EventArgs e)
        {
            
        }

        private async void button3_Click(object sender, EventArgs e)
        {
            timer3.Start();
            label6.Visible = true;
            if (string.IsNullOrWhiteSpace(textBox1.Text) || string.IsNullOrWhiteSpace(textBox2.Text))
            {
                MessageBox.Show("Bitte gebe deine Quell- und Zielordner an!");
                return;
            }

            if (!Directory.Exists(textBox1.Text) || !Directory.Exists(textBox2.Text))
            {
                MessageBox.Show("Ein oder beide angegebene Ordner existieren nicht.");
                return;
            }

            QuellBilder = textBox1.Text;
            ZielBilder = textBox2.Text;

            label6.Text = "Loading...";
            label6.ForeColor = Color.Red;

            await ProcessFilesAsync(); 

            label6.Text = "Finish";
            label6.ForeColor = Color.Green;
        }

        private async Task ProcessFilesAsync()
        {
            try
            {
                DirectoryInfo sourceDirectory = new DirectoryInfo(QuellBilder);
                DirectoryInfo destinationDirectory = new DirectoryInfo(ZielBilder);

                UpdateFileCountLabels(sourceDirectory, destinationDirectory);

                var files = sourceDirectory.GetFiles("*", SearchOption.AllDirectories);

                foreach (var file in files)
                {
                    await Task.Run(() => ProcessFile(file, destinationDirectory));
                    UpdateProgress();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("FEHLER! " + ex.Message);
            }
        }

        private void UpdateFileCountLabels(DirectoryInfo sourceDir, DirectoryInfo destinationDir)
        {
            int sourceFileCount = Directory.GetFiles(sourceDir.FullName, "*.*", SearchOption.AllDirectories).Length;
            int destinationFileCount = Directory.GetFiles(destinationDir.FullName, "*.*", SearchOption.AllDirectories).Length;

            anzahlimQuell = sourceFileCount;
            anzahlimZiel = destinationFileCount;

            
            Invoke((MethodInvoker)delegate
            {
                label3.Text = $"Anzahl im Quellordner: {anzahlimQuell}";
                label4.Text = $"Anzahl im Zielordner: {anzahlimZiel}";
            });
        }

        private void ProcessFile(FileInfo file, DirectoryInfo destinationDir)
        {
            a++;
            DateTime fileDate = file.LastWriteTime;
            string year = fileDate.ToString("yyyy");
            string month = fileDate.ToString("MM");
            string day = fileDate.ToString("dd");

            string relativePath = GetRelativePath(file.FullName, QuellBilder);

            string newPath = Path.Combine(destinationDir.FullName, year, GetMonthName(month), day);

            Directory.CreateDirectory(newPath);

            if (file.Name == "Thumbs.db") return;

            try
            {
                string destinationFilePath = Path.Combine(newPath, file.Name);
                File.Copy(file.FullName, destinationFilePath, true);
                b++;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Kopieren der Datei {file.Name}: {ex.Message}");
            }
        }

        private string GetRelativePath(string filePath, string rootPath)
        {
            Uri fileUri = new Uri(filePath);
            Uri rootUri = new Uri(rootPath);
            Uri relativeUri = rootUri.MakeRelativeUri(fileUri);
            return Uri.UnescapeDataString(relativeUri.ToString());
        }

        private string GetMonthName(string month)
        {
            string monthName;
            switch (month)
            {
                case "01":
                    monthName = "01. - Januar";
                    break;
                case "02":
                    monthName = "02. - Februar";
                    break;
                case "03":
                    monthName = "03. - März";
                    break;
                case "04":
                    monthName = "04. - April";
                    break;
                case "05":
                    monthName = "05. - Mai";
                    break;
                case "06":
                    monthName = "06. - Juni";
                    break;
                case "07":
                    monthName = "07. - Juli";
                    break;
                case "08":
                    monthName = "08. - August";
                    break;
                case "09":
                    monthName = "09. - September";
                    break;
                case "10":
                    monthName = "10. - Oktober";
                    break;
                case "11":
                    monthName = "11. - November";
                    break;
                case "12":
                    monthName = "12. - Dezember";
                    break;
                default:
                    monthName = "99. - unbekannt";
                    break;
            }
            return monthName;
        }

        private void UpdateProgress()
        {
            Invoke((MethodInvoker)delegate
            {
                label5.Text = b.ToString() + " / " + anzahlimQuell;

                label4.Text = "Anzahl im Ordner: " + b.ToString();
            });
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }
    }
}
