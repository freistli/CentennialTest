namespace WinFormUWPContract
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.button1 = new System.Windows.Forms.Button();
            this.label = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.webView1 = new Microsoft.Toolkit.Forms.UI.Controls.WebView();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.webView1)).BeginInit();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(31, 158);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(132, 54);
            this.button1.TabIndex = 0;
            this.button1.Text = "Click Me";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label
            // 
            this.label.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label.AutoSize = true;
            this.label.Location = new System.Drawing.Point(37, 496);
            this.label.Name = "label";
            this.label.Size = new System.Drawing.Size(74, 32);
            this.label.TabIndex = 1;
            this.label.Text = "Path";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(25, 70);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(185, 32);
            this.label2.TabIndex = 3;
            this.label2.Text = "Geo Location";
            // 
            // webView1
            // 
            this.webView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.webView1.Location = new System.Drawing.Point(666, 32);
            this.webView1.MinimumSize = new System.Drawing.Size(20, 20);
            this.webView1.Name = "webView1";
            this.webView1.Size = new System.Drawing.Size(1597, 458);
            this.webView1.TabIndex = 4;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(31, 323);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(168, 54);
            this.button2.TabIndex = 5;
            this.button2.Text = "ToastTwo";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(31, 238);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(168, 54);
            this.button3.TabIndex = 6;
            this.button3.Text = "ToastOne";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(16F, 31F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(2376, 549);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.webView1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label);
            this.Controls.Add(this.button1);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.webView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label;
        private System.Windows.Forms.Label label2;
        private Microsoft.Toolkit.Forms.UI.Controls.WebView webView1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
    }
}

