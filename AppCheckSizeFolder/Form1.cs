using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;


namespace AppCheckSizeFolder
{
    public partial class Form1 : Form
    {
        private static readonly EnumerationOptions ScanEnumerationOptions = new EnumerationOptions
        {
            IgnoreInaccessible = true,
            RecurseSubdirectories = false,
            AttributesToSkip = FileAttributes.ReparsePoint | FileAttributes.System
        };

        private static readonly TimeSpan CacheTtl = TimeSpan.FromSeconds(30);
        private static readonly ConcurrentDictionary<string, FolderCacheEntry> FolderSizeCache = new(StringComparer.OrdinalIgnoreCase);
        private static readonly ConcurrentDictionary<string, FileCacheEntry> FileSizeCache = new(StringComparer.OrdinalIgnoreCase);

        public List<string> pathsave = new List<string>();
        private int historyIndex = -1;

        public Form1()
        {
            InitializeComponent();
            string tx = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            textBox1.Text = tx;
            listBox1.Items.Add(tx);
            InitializeMyComputerTree();


            this.MouseDown += new MouseEventHandler(Form1_MouseDown);
            listBox1.MouseDown += new MouseEventHandler(Form1_MouseDown);

        }
        // Update 4

        int isback = 0;
        private async Task GetTotalFolderSize(string folderPath)
        {

            try
            {

                long totalSize = 0;

                // Check if the folder exists
                if (Directory.Exists(folderPath))
                {
                    var entries = Directory.EnumerateFileSystemEntries(folderPath).ToList();

                    if (isback != 1)
                    {
                        if (historyIndex >= 0 && historyIndex < pathsave.Count - 1)
                        {
                            pathsave.RemoveRange(historyIndex + 1, pathsave.Count - historyIndex - 1);
                        }

                        if (pathsave.Count == 0 || !string.Equals(pathsave[^1], folderPath, StringComparison.OrdinalIgnoreCase))
                        {
                            pathsave.Add(folderPath);
                        }

                        historyIndex = pathsave.Count - 1;
                    }


                    listBox1.Items.Clear();
                    listBox2.Items.Clear();
                    ConcurrentBag<(string Path, long Size)> folderItems = new ConcurrentBag<(string Path, long Size)>();
                    int count = entries.Count;
                    progressBar1.Visible = true;
                    progressBar1.Minimum = 0;
                    progressBar1.Maximum = count > 0 ? count : 1;
                    progressBar1.Value = 0;
                    int load = 0;
                    long totalSizeConcurrent = 0;
                    var parallelOptions = new ParallelOptions
                    {
                        MaxDegreeOfParallelism = Math.Max(2, Environment.ProcessorCount / 2)
                    };

                    await Parallel.ForEachAsync(entries, parallelOptions, async (entryPath, _) =>
                    {
                        long size = CalculateEntrySize(entryPath);
                        folderItems.Add((entryPath, size));
                        Interlocked.Add(ref totalSizeConcurrent, size);

                        int current = Interlocked.Increment(ref load);
                        await Task.Yield();
                        BeginInvoke(new Action(() =>
                        {
                            settext(entryPath);
                            progressBar1.Value = Math.Min(current, progressBar1.Maximum);
                        }));
                    });

                    totalSize = totalSizeConcurrent;
                    var sortedItems = folderItems.OrderBy(item => item.Size).ToList();
                    listBox1.Items.Clear();
                    listBox2.Items.Clear();
                    foreach (var item in sortedItems)
                    {
                        listBox1.Items.Add(item.Path);
                        listBox2.Items.Add(FormatBytes(item.Size));
                    }


                    string driveUsageText = GetDriveUsageText(folderPath);
                    label1.Text = string.IsNullOrWhiteSpace(driveUsageText)
                        ? "Complete."
                        : $"Complete. {driveUsageText}";
                    listBox2.Items.Add($"Total Size : {FormatBytes(totalSize)}");
                    label_size.Text = $"Total Size : {FormatBytes(totalSize)}";

                    progressBar1.Visible = false;

                }
                else
                {
                    label1.Text = "Not Find Folder.";
                }
                isback = 0;

            }
            catch (Exception ex)
            {
                isback = 0;
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        static long CalculateEntrySize(string path)
        {
            if (File.Exists(path))
            {
                return GetFileSize(path);
            }

            if (Directory.Exists(path))
            {
                return CalculateFolderSize(path);
            }

            return 0;
        }

        private static long CalculateFolderSize(string folderPath)
        {
            DateTime cacheTimeUtc = DateTime.UtcNow;
            DateTime? folderLastWriteUtc = SafeGetDirectoryLastWriteUtc(folderPath);
            if (TryGetCachedFolderSize(folderPath, folderLastWriteUtc, cacheTimeUtc, out long cachedSize))
            {
                return cachedSize;
            }

            long size = 0;
            var pending = new Stack<string>();
            pending.Push(folderPath);

            while (pending.Count > 0)
            {
                string currentPath = pending.Pop();

                try
                {
                    foreach (string file in Directory.EnumerateFiles(currentPath, "*", ScanEnumerationOptions))
                    {
                        try
                        {
                            size += GetFileSize(file);
                        }
                        catch (IOException)
                        {
                        }
                        catch (UnauthorizedAccessException)
                        {
                        }
                    }

                    foreach (string directory in Directory.EnumerateDirectories(currentPath, "*", ScanEnumerationOptions))
                    {
                        pending.Push(directory);
                    }
                }
                catch (IOException)
                {
                }
                catch (UnauthorizedAccessException)
                {
                }
            }

            SetCachedFolderSize(folderPath, size, folderLastWriteUtc, cacheTimeUtc);
            return size;
        }
        public void settext(string s)
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
        static long GetFileSize(string filePath)
        {
            try
            {
                // Check if the file exists
                if (File.Exists(filePath))
                {
                    FileInfo fileInfo = new FileInfo(filePath);
                    DateTime? lastWriteUtc = fileInfo.Exists ? fileInfo.LastWriteTimeUtc : null;
                    DateTime cacheTimeUtc = DateTime.UtcNow;
                    if (TryGetCachedFileSize(filePath, fileInfo.Length, lastWriteUtc, cacheTimeUtc, out long cachedSize))
                    {
                        return cachedSize;
                    }

                    SetCachedFileSize(filePath, fileInfo.Length, lastWriteUtc, cacheTimeUtc);
                    return fileInfo.Length;
                }
                else
                {
                    Console.WriteLine($"File not found: {filePath}");
                    return 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return 0;
            }
        }

        private static DateTime? SafeGetDirectoryLastWriteUtc(string folderPath)
        {
            try
            {
                return Directory.GetLastWriteTimeUtc(folderPath);
            }
            catch
            {
                return null;
            }
        }

        private static bool TryGetCachedFolderSize(string folderPath, DateTime? lastWriteUtc, DateTime nowUtc, out long size)
        {
            if (FolderSizeCache.TryGetValue(folderPath, out FolderCacheEntry entry))
            {
                if (entry.ExpiresAtUtc > nowUtc && entry.LastWriteTimeUtc == lastWriteUtc)
                {
                    size = entry.Size;
                    return true;
                }
            }

            size = 0;
            return false;
        }

        private static void SetCachedFolderSize(string folderPath, long size, DateTime? lastWriteUtc, DateTime nowUtc)
        {
            if (FolderSizeCache.Count > 100_000)
            {
                FolderSizeCache.Clear();
            }

            FolderSizeCache[folderPath] = new FolderCacheEntry(size, lastWriteUtc, nowUtc.Add(CacheTtl));
        }

        private static bool TryGetCachedFileSize(string filePath, long length, DateTime? lastWriteUtc, DateTime nowUtc, out long size)
        {
            if (FileSizeCache.TryGetValue(filePath, out FileCacheEntry entry))
            {
                if (entry.ExpiresAtUtc > nowUtc && entry.LastWriteTimeUtc == lastWriteUtc && entry.Length == length)
                {
                    size = entry.Size;
                    return true;
                }
            }

            size = 0;
            return false;
        }

        private static void SetCachedFileSize(string filePath, long size, DateTime? lastWriteUtc, DateTime nowUtc)
        {
            if (FileSizeCache.Count > 200_000)
            {
                FileSizeCache.Clear();
            }

            FileSizeCache[filePath] = new FileCacheEntry(size, size, lastWriteUtc, nowUtc.Add(CacheTtl));
        }

        private readonly record struct FolderCacheEntry(long Size, DateTime? LastWriteTimeUtc, DateTime ExpiresAtUtc);
        private readonly record struct FileCacheEntry(long Size, long Length, DateTime? LastWriteTimeUtc, DateTime ExpiresAtUtc);
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

        private static string GetDriveUsageText(string folderPath)
        {
            try
            {
                string? rootPath = Path.GetPathRoot(folderPath);
                if (string.IsNullOrWhiteSpace(rootPath))
                {
                    return string.Empty;
                }

                DriveInfo drive = new DriveInfo(rootPath);
                if (!drive.IsReady)
                {
                    return string.Empty;
                }

                long used = drive.TotalSize - drive.AvailableFreeSpace;
                return $"Drive {drive.Name} Used {FormatBytes(used)} / Free {FormatBytes(drive.AvailableFreeSpace)} / Total {FormatBytes(drive.TotalSize)}";
            }
            catch
            {
                return string.Empty;
            }
        }


        private void button1_Click(object sender, EventArgs e)
        {

            //label1.Text = "Loading...";
            //label_size.Text = "";
            //string folderPath = textBox1.Text;
            //if (folderPath != null || folderPath != "")
            //{
            //    await GetTotalFolderSize(folderPath);
            //}
            string folderPath = textBox1.Text;
            go(folderPath);
            //Debug.WriteLine($"Size of folder {folderPath}: {FormatBytes(folderSize)}");
        }

        private async void go(string folderPath)
        {

            label1.Text = "Loading...";
            label_size.Text = "";

            if (!string.IsNullOrWhiteSpace(folderPath))
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


            if ((Control.ModifierKeys & Keys.Control) == Keys.Control)
            {
                if (listBox1.SelectedItem != null)
                {
                    // Get the selected item
                    string? selectedItem = listBox1.SelectedItem?.ToString();
                    if (!string.IsNullOrWhiteSpace(selectedItem))
                    {
                        Process.Start("explorer.exe", $"\"{selectedItem}\"");
                    }

                }
            }
            else
            {
                if (listBox1.SelectedItem != null)
                {
                    // Get the selected item
                    string? selectedItem = listBox1.SelectedItem?.ToString();
                    if (!string.IsNullOrWhiteSpace(selectedItem))
                    {
                        textBox1.Text = selectedItem;
                        go(selectedItem);
                    }
                }
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
            if (selectedIndex2 > 0 && selectedIndex != -1)
            {
                listBox2.SetSelected(selectedIndex, true);

            }
        }

        private void listBox2_MouseClick(object sender, MouseEventArgs e)
        {
            // Get the selected item from the ListBox
            int selectedIndex2 = listBox2.SelectedIndex;
            int selectedIndex1 = listBox1.Items.Count;
            if (selectedIndex1 > 0 && selectedIndex2 < listBox1.Items.Count && selectedIndex2 != -1)
                listBox1.SetSelected(selectedIndex2, true);
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                // Handle the Enter key press
                string folderPath = textBox1.Text;
                go(folderPath);

                // Optionally, prevent the beep sound by setting Handled to true
                e.Handled = true;
            }
        }
        private void bt_back_Click(object sender, EventArgs e)
        {

            backward();
        }
        private void backward()
        {
            if (pathsave.Count == 0)
            {
                return;
            }

            if (historyIndex < 0)
            {
                historyIndex = pathsave.Count - 1;
            }

            if (historyIndex > 0)
            {
                historyIndex--;
                string target = pathsave[historyIndex];
                textBox1.Text = target;
                isback = 1;
                go(target);
            }
        }

        private void bt_forward_Click(object sender, EventArgs e)
        {
            forward();
        }

        private void forward()
        {
            if (pathsave.Count == 0)
            {
                return;
            }

            if (historyIndex < 0)
            {
                historyIndex = pathsave.Count - 1;
            }

            if (historyIndex < pathsave.Count - 1)
            {
                historyIndex++;
                string target = pathsave[historyIndex];
                textBox1.Text = target;
                isback = 1;
                go(target);
            }
        }

        private void Form1_MouseDown(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.XButton1)
            {
                // Handle the back button click
                backward();
            }
            else if (e.Button == MouseButtons.XButton2)
            {
                // Handle the forward button click
                forward();
            }
        }
        private void Form1_Click(object sender, EventArgs e)
        {
            Debug.WriteLine("Yes!!!!!");
        }

        private void InitializeMyComputerTree()
        {
            treeView1.Nodes.Clear();

            TreeNode root = new TreeNode("My Computer");
            treeView1.Nodes.Add(root);

            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                if (!drive.IsReady)
                {
                    continue;
                }

                TreeNode driveNode = new TreeNode(drive.Name)
                {
                    Tag = drive.RootDirectory.FullName
                };

                AddDummyNode(driveNode);
                root.Nodes.Add(driveNode);
            }

            root.Expand();
        }

        private static void AddDummyNode(TreeNode parentNode)
        {
            parentNode.Nodes.Clear();
            parentNode.Nodes.Add(new TreeNode("Loading..."));
        }

        private void treeView1_BeforeExpand(object? sender, TreeViewCancelEventArgs e)
        {
            if (e.Node is null || e.Node.Tag is not string path)
            {
                return;
            }

            if (e.Node.Nodes.Count == 1 && e.Node.Nodes[0].Text == "Loading...")
            {
                LoadChildDirectories(e.Node, path);
            }
        }

        private void LoadChildDirectories(TreeNode parentNode, string path)
        {
            parentNode.Nodes.Clear();

            try
            {
                foreach (string directory in Directory.EnumerateDirectories(path))
                {
                    DirectoryInfo dirInfo = new DirectoryInfo(directory);
                    TreeNode childNode = new TreeNode(dirInfo.Name)
                    {
                        Tag = directory
                    };

                    try
                    {
                        if (Directory.EnumerateDirectories(directory).Any())
                        {
                            AddDummyNode(childNode);
                        }
                    }
                    catch (UnauthorizedAccessException)
                    {
                    }
                    catch (IOException)
                    {
                    }

                    parentNode.Nodes.Add(childNode);
                }
            }
            catch (UnauthorizedAccessException)
            {
            }
            catch (IOException)
            {
            }
        }

        private void treeView1_NodeMouseDoubleClick(object? sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node is null || e.Node.Tag is not string path || string.IsNullOrWhiteSpace(path))
            {
                return;
            }

            textBox1.Text = path;
            go(path);
        }
    }
}
