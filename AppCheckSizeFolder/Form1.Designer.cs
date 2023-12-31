namespace AppCheckSizeFolder
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            button1 = new Button();
            listBox1 = new ListBox();
            textBox1 = new TextBox();
            button2 = new Button();
            listBox2 = new ListBox();
            button3 = new Button();
            label1 = new Label();
            progressBar1 = new ProgressBar();
            label_size = new Label();
            SuspendLayout();
            // 
            // button1
            // 
            button1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button1.Font = new Font("Calibri", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            button1.Location = new Point(837, 62);
            button1.Name = "button1";
            button1.Size = new Size(97, 31);
            button1.TabIndex = 0;
            button1.Text = "Check";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // listBox1
            // 
            listBox1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            listBox1.BackColor = Color.Gray;
            listBox1.BorderStyle = BorderStyle.None;
            listBox1.Font = new Font("Cambria", 15.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            listBox1.ForeColor = SystemColors.ControlLightLight;
            listBox1.FormattingEnabled = true;
            listBox1.ItemHeight = 25;
            listBox1.Location = new Point(12, 99);
            listBox1.Name = "listBox1";
            listBox1.Size = new Size(718, 525);
            listBox1.TabIndex = 3;
            listBox1.SelectedIndexChanged += listBox1_SelectedIndexChanged;
            listBox1.MouseDoubleClick += listBox1_MouseDoubleClick;
            // 
            // textBox1
            // 
            textBox1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            textBox1.Font = new Font("Calibri", 14.25F);
            textBox1.Location = new Point(12, 12);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(842, 31);
            textBox1.TabIndex = 4;
            // 
            // button2
            // 
            button2.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button2.Font = new Font("Calibri", 14.25F);
            button2.Location = new Point(860, 13);
            button2.Name = "button2";
            button2.Size = new Size(74, 30);
            button2.TabIndex = 5;
            button2.Text = "Path";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // listBox2
            // 
            listBox2.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            listBox2.BackColor = Color.Gray;
            listBox2.BorderStyle = BorderStyle.None;
            listBox2.Font = new Font("Cambria", 15.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            listBox2.ForeColor = SystemColors.ControlLightLight;
            listBox2.FormattingEnabled = true;
            listBox2.ItemHeight = 25;
            listBox2.Location = new Point(730, 99);
            listBox2.Name = "listBox2";
            listBox2.Size = new Size(204, 525);
            listBox2.TabIndex = 6;
            // 
            // button3
            // 
            button3.Location = new Point(12, 49);
            button3.Name = "button3";
            button3.Size = new Size(113, 23);
            button3.TabIndex = 7;
            button3.Text = "Open this path";
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click;
            // 
            // label1
            // 
            label1.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            label1.AutoSize = true;
            label1.Font = new Font("Calibri", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label1.ForeColor = SystemColors.ButtonFace;
            label1.Location = new Point(12, 637);
            label1.Name = "label1";
            label1.Size = new Size(0, 19);
            label1.TabIndex = 8;
            // 
            // progressBar1
            // 
            progressBar1.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            progressBar1.Location = new Point(12, 659);
            progressBar1.Name = "progressBar1";
            progressBar1.Size = new Size(922, 23);
            progressBar1.Step = 1;
            progressBar1.TabIndex = 9;
            // 
            // label_size
            // 
            label_size.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            label_size.AutoSize = true;
            label_size.Font = new Font("Calibri", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label_size.ForeColor = SystemColors.ButtonFace;
            label_size.Location = new Point(745, 627);
            label_size.Name = "label_size";
            label_size.Size = new Size(17, 19);
            label_size.TabIndex = 8;
            label_size.Text = "..";
            label_size.TextAlign = ContentAlignment.MiddleRight;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.WindowFrame;
            ClientSize = new Size(946, 694);
            Controls.Add(progressBar1);
            Controls.Add(label_size);
            Controls.Add(label1);
            Controls.Add(button3);
            Controls.Add(listBox2);
            Controls.Add(button2);
            Controls.Add(textBox1);
            Controls.Add(listBox1);
            Controls.Add(button1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "Form1";
            Text = "Folder Size V.1.0.1";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button button1;
        private ListBox listBox1;
        private TextBox textBox1;
        private Button button2;
        private ListBox listBox2;
        private Button button3;
        private Label label1;
        private ProgressBar progressBar1;
        private Label label_size;
    }
}
