using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;


namespace AppCheckSizeFolder
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }


        private void GetTotalFolderSize(string folderPath)
        {
            try
            {

                long totalSize = 0;
                long Size = 0;
                // Check if the folder exists
                if (Directory.Exists(folderPath))
                {
                    string[] subfolderssss = Directory.GetFileSystemEntries(folderPath);

                    // Get all subfolders in the folder
                    string[] subfolders = Directory.GetDirectories(folderPath);
                    listBox1.Items.Clear();
                    listBox2.Items.Clear();
                    List<Tuple<string, double>> folderItems = new List<Tuple<string, double>>();
                    List<Tuple<string, double>> folderItems2 = new List<Tuple<string, double>>();
                    // Iterate through each subfolder and calculate its size
                    foreach (string subfolder in subfolderssss)
                    {
                        if (File.Exists(subfolder))
                        {
                            totalSize += GetFileSize(subfolder);
                            Size = GetFileSize(subfolder);
                            double dblSByte = Size;

                            folderItems.Add(Tuple.Create($"{subfolder}", dblSByte));
                            folderItems2.Add(Tuple.Create($"{FormatBytes(Size)}", dblSByte));
                            Debug.WriteLine($"File: {subfolder}, Size: {FormatBytes(GetFileSize(subfolder))}");
                        }
                        else if (Directory.Exists(subfolder))
                        {
                            totalSize += GetFolderSize(subfolder);
                            Size = GetFolderSize(subfolder);
                            double dblSByte = Size;

                            folderItems.Add(Tuple.Create($"{subfolder}", dblSByte));
                            folderItems2.Add(Tuple.Create($"{FormatBytes(Size)}", dblSByte));
                            Debug.WriteLine($"Folder: {subfolder}, Size: {FormatBytes(GetFolderSize(subfolder))}");
                        }




                        //listBox1.Items.Add($"{FormatBytes(totalSize)} | Size of folder {subfolder}");
                        //listBox1.Sorted = true;
                        //Debug.WriteLine($"Size of folder {folderPath}: {FormatBytes(totalSize)}");
                    }

                    folderItems = folderItems.OrderBy(item => item.Item2).ToList();
                    folderItems2 = folderItems2.OrderBy(item => item.Item2).ToList();
                    int i = 0;
                    foreach (var item in folderItems)
                    {
                        listBox1.Items.Add(item.Item1);
                        listBox2.Items.Add(folderItems2[i].Item1);
                        i++;
                    }
                    listBox2.Items.Add($"Total Size : {FormatBytes(totalSize)}");
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        static long GetFolderSize(string folderPath)
        {
            try
            {
                long size = 0;

                // Check if the folder exists
                if (Directory.Exists(folderPath))
                {
                    // Get all files in the folder and calculate their size
                    string[] files = Directory.GetFiles(folderPath, "*.*", SearchOption.AllDirectories);
                    foreach (string file in files)
                    {
                        FileInfo fileInfo = new FileInfo(file);
                        size += fileInfo.Length;
                    }
                }

                return size;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return -1; // Return -1 to indicate an error
            }
        }
        static long GetFileSize(string filePath)
        {
            try
            {
                // Check if the file exists
                if (File.Exists(filePath))
                {
                    // Get file size
                    FileInfo fileInfo = new FileInfo(filePath);
                    return fileInfo.Length;
                }
                else
                {
                    Console.WriteLine($"File not found: {filePath}");
                    return -1; // Return -1 to indicate an error
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return -1; // Return -1 to indicate an error
            }
        }
        static string FormatBytes(long bytes)
        {
            string[] suffixes = { "bytes", "KB", "MB", "GB", "TB" };
            int i = 0;
            double dblSByte = bytes;

            while (dblSByte >= 1024 && i < suffixes.Length - 1)
            {
                dblSByte /= 1024;
                i++;
            }

            return $"{dblSByte:0.##} {suffixes[i]}";
        }


        private void button1_Click(object sender, EventArgs e)
        {

            string folderPath = textBox1.Text;
            if (folderPath != null || folderPath != "")
            {
                GetTotalFolderSize(folderPath);
            }

            //Debug.WriteLine($"Size of folder {folderPath}: {FormatBytes(folderSize)}");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();

            // Set the initial directory (optional)
            //folderBrowserDialog.SelectedPath = @"C:\";

            // Set the title of the dialog
            folderBrowserDialog.Description = "Select a Folder";

            // Show the dialog and check if the user clicked OK
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                // Get the selected folder path
                string selectedFolderPath = folderBrowserDialog.SelectedPath;

                // Display the selected folder path in a MessageBox (you can do other operations here)
                //MessageBox.Show($"Selected Folder: {selectedFolderPath}");
                textBox1.Text = selectedFolderPath;
                GetTotalFolderSize(selectedFolderPath);

            }
        }



        private void listBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (listBox1.SelectedItem != null)
            {
                // Get the selected item
                string selectedItem = listBox1.SelectedItem.ToString();
                Process.Start("explorer.exe", $"\"{selectedItem}\"");

            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string path = textBox1.Text;
            if (path != null) {
                Process.Start("explorer.exe", $"\"{path}\"");
            }
           
        }
    }
}
