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
            string tx = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            textBox1.Text = tx;
            listBox1.Items.Add(tx);
        }
        // Update 4


        private async Task GetTotalFolderSize(string folderPath)
        {

            try
            {

                Stopwatch stopwatch = new Stopwatch();
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
                    double count = subfolderssss.Length;
                    double percen = 100 / count;
                    progressBar1.Visible = true;
                    progressBar1.Minimum = 0;
                    progressBar1.Maximum = (int)(count * percen);
                    double load = 0.00;
                    List<string> currentItems = listBox1.Items.Cast<string>().ToList();

                    foreach (string subfolder in subfolderssss)
                    {
                        stopwatch.Restart();
                        stopwatch.Start();
                        Debug.WriteLine($"STAT #1 : {stopwatch.ElapsedMilliseconds}, Size: {subfolder}");
                        await Task.Run(() => settext(subfolder));
                        if (File.Exists(subfolder))
                        {
                            await Task.Run(() => settext(subfolder));
                            totalSize += await GetFileSize(subfolder);
                            Size = await GetFileSize(subfolder);
                            double dblSByte = Size;
                            listBox2.Items.Add(FormatBytes(Size));
                            folderItems.Add(Tuple.Create($"{subfolder}", dblSByte));
                            folderItems2.Add(Tuple.Create($"{FormatBytes(Size)}", dblSByte));
                            Debug.WriteLine($"File: {subfolder}, Size: {FormatBytes(Size)}");
                            load = load + percen;
                            await Task.Run(() => settext(load.ToString()));
                            progressBar1.Value = (int)(load);

                        }
                        else if (Directory.Exists(subfolder))
                        {
                            await Task.Run(() => settext(subfolder));
                            totalSize += await GetFolderSize(subfolder);
                            Size = await GetFolderSize(subfolder);
                            double dblSByte = Size;
                            listBox2.Items.Add(FormatBytes(Size));
                            folderItems.Add(Tuple.Create($"{subfolder}", dblSByte));
                            folderItems2.Add(Tuple.Create($"{FormatBytes(Size)}", dblSByte));
                            Debug.WriteLine($"Folder: {subfolder}, Size: {FormatBytes(Size)}");
                            load = load + percen;

                            progressBar1.Value = (int)(load);
                        }
                        await Task.Run(() => settext(subfolder));
                        stopwatch.Stop();
                        Debug.WriteLine($"Stop #({load}%) : {stopwatch.ElapsedMilliseconds / 1000}, Size: {subfolder}");
                        //Application.DoEvents();
                        //listBox1.Items.Add($"{FormatBytes(totalSize)} | Size of folder {subfolder}");
                        //listBox1.Sorted = true;
                        //Debug.WriteLine($"Size of folder {folderPath}: {FormatBytes(totalSize)}");
                    }

                    folderItems = folderItems.OrderBy(item => item.Item2).ToList();
                    folderItems2 = folderItems2.OrderBy(item => item.Item2).ToList();
                    int i = 0;

                    listBox1.Items.Clear();
                    listBox2.Items.Clear();
                    foreach (var item in folderItems)
                    {
                        listBox1.Items.Add(item.Item1);
                        listBox2.Items.Add(folderItems2[i].Item1);
                        i++;
                    }


                    label1.Text = "Complete.";
                    listBox2.Items.Add($"Total Size : {FormatBytes(totalSize)}");
                    label_size.Text = $"Total Size : {FormatBytes(totalSize)}";

                    progressBar1.Visible = false;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        static async Task<long> GetFolderSize(string folderPath)
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
        public async void settext(string s)
        {
            // Assuming label1 is a control on the form
            if (label1.InvokeRequired)
            {
                // If this method is called from a non-UI thread, invoke it on the UI thread
                label1.Invoke(new MethodInvoker(() => label1.Text = "Loading...." + s));
            }
            else
            {
                // If this method is called from the UI thread, update the label directly
                label1.Text = "Loading...." + s;
            }
        }
        static async Task<long> GetFileSize(string filePath)
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


        private async void button1_Click(object sender, EventArgs e)
        {

            label1.Text = "Loading...";
            label_size.Text = "";
            string folderPath = textBox1.Text;
            if (folderPath != null || folderPath != "")
            {
                await GetTotalFolderSize(folderPath);
            }

            //Debug.WriteLine($"Size of folder {folderPath}: {FormatBytes(folderSize)}");
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            label1.Text = "Loading...";
            label_size.Text = "";
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
                await GetTotalFolderSize(selectedFolderPath);

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
            if (path != null)
            {
                Process.Start("explorer.exe", $"\"{path}\"");
            }
        }


        private void listBox1_MouseClick(object sender, MouseEventArgs e)
        {
            // Get the selected item from the ListBox
            int selectedIndex = listBox1.SelectedIndex;
            int selectedIndex2 = listBox2.Items.Count;
            if (selectedIndex2 > 0)
                listBox2.SetSelected(selectedIndex, true);
        }

        private void listBox2_MouseClick(object sender, MouseEventArgs e)
        {
            // Get the selected item from the ListBox
            int selectedIndex2 = listBox2.SelectedIndex;
            int selectedIndex1 = listBox1.Items.Count;
            if (selectedIndex1 > 0 && selectedIndex2 < listBox1.Items.Count)
                listBox1.SetSelected(selectedIndex2, true);
        }
    }
}
