namespace WinFormsAppOnline
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
            IP = new Label();
            Log = new GroupBox();
            DebugInfoBox = new TextBox();
            Host = new Button();
            Log.SuspendLayout();
            SuspendLayout();
            // 
            // IP
            // 
            IP.AutoSize = true;
            IP.Location = new Point(12, 9);
            IP.Name = "IP";
            IP.Size = new Size(73, 20);
            IP.TabIndex = 0;
            IP.Text = "IPAddress";
            // 
            // Log
            // 
            Log.Controls.Add(DebugInfoBox);
            Log.Location = new Point(482, 12);
            Log.Name = "Log";
            Log.Size = new Size(541, 355);
            Log.TabIndex = 1;
            Log.TabStop = false;
            Log.Text = "Log";
            // 
            // DebugInfoBox
            // 
            DebugInfoBox.Location = new Point(6, 26);
            DebugInfoBox.Multiline = true;
            DebugInfoBox.Name = "DebugInfoBox";
            DebugInfoBox.ReadOnly = true;
            DebugInfoBox.ScrollBars = ScrollBars.Vertical;
            DebugInfoBox.Size = new Size(529, 323);
            DebugInfoBox.TabIndex = 2;
            // 
            // Host
            // 
            Host.Location = new Point(12, 303);
            Host.Name = "Host";
            Host.Size = new Size(464, 29);
            Host.TabIndex = 3;
            Host.Text = "Start hosting";
            Host.UseVisualStyleBackColor = true;
            Host.Click += Host_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1035, 417);
            Controls.Add(Host);
            Controls.Add(Log);
            Controls.Add(IP);
            Name = "Form1";
            Text = "Form1";
            Log.ResumeLayout(false);
            Log.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label IP;
        private GroupBox Log;
        private Label DebugInfo;
        // private Button DebugInfo;
        private Button Host;
        private TextBox DebugInfoBox;
    }
}
