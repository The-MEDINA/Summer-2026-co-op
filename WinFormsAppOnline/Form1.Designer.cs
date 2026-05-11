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
            HostLog = new GroupBox();
            DebugInfoBox = new TextBox();
            Host = new Button();
            ClientLog = new GroupBox();
            DebugInfoClient = new TextBox();
            Join = new Button();
            ServerStatus = new GroupBox();
            StatusBox = new TextBox();
            StopHosting = new Button();
            HostBackgroundWorker = new System.ComponentModel.BackgroundWorker();
            HostLog.SuspendLayout();
            ClientLog.SuspendLayout();
            ServerStatus.SuspendLayout();
            SuspendLayout();
            // 
            // IP
            // 
            IP.AutoSize = true;
            IP.Location = new Point(12, 9);
            IP.Name = "IP";
            IP.Size = new Size(0, 20);
            IP.TabIndex = 0;
            // 
            // HostLog
            // 
            HostLog.Controls.Add(DebugInfoBox);
            HostLog.Location = new Point(558, 12);
            HostLog.Name = "HostLog";
            HostLog.Size = new Size(508, 355);
            HostLog.TabIndex = 1;
            HostLog.TabStop = false;
            HostLog.Text = "Host Log";
            // 
            // DebugInfoBox
            // 
            DebugInfoBox.Location = new Point(6, 26);
            DebugInfoBox.Multiline = true;
            DebugInfoBox.Name = "DebugInfoBox";
            DebugInfoBox.ReadOnly = true;
            DebugInfoBox.ScrollBars = ScrollBars.Vertical;
            DebugInfoBox.Size = new Size(502, 323);
            DebugInfoBox.TabIndex = 2;
            // 
            // Host
            // 
            Host.Location = new Point(12, 441);
            Host.Name = "Host";
            Host.Size = new Size(540, 29);
            Host.TabIndex = 3;
            Host.Text = "Start hosting";
            Host.UseVisualStyleBackColor = true;
            Host.Click += Host_Click;
            // 
            // ClientLog
            // 
            ClientLog.Controls.Add(DebugInfoClient);
            ClientLog.Location = new Point(12, 12);
            ClientLog.Name = "ClientLog";
            ClientLog.Size = new Size(540, 355);
            ClientLog.TabIndex = 3;
            ClientLog.TabStop = false;
            ClientLog.Text = "Client Log";
            // 
            // DebugInfoClient
            // 
            DebugInfoClient.Location = new Point(6, 26);
            DebugInfoClient.Multiline = true;
            DebugInfoClient.Name = "DebugInfoClient";
            DebugInfoClient.ReadOnly = true;
            DebugInfoClient.ScrollBars = ScrollBars.Vertical;
            DebugInfoClient.Size = new Size(528, 323);
            DebugInfoClient.TabIndex = 2;
            // 
            // Join
            // 
            Join.Location = new Point(12, 476);
            Join.Name = "Join";
            Join.Size = new Size(1054, 29);
            Join.TabIndex = 4;
            Join.Text = "Join";
            Join.UseVisualStyleBackColor = true;
            // 
            // ServerStatus
            // 
            ServerStatus.Controls.Add(StatusBox);
            ServerStatus.Location = new Point(12, 372);
            ServerStatus.Name = "ServerStatus";
            ServerStatus.Size = new Size(1054, 63);
            ServerStatus.TabIndex = 5;
            ServerStatus.TabStop = false;
            ServerStatus.Text = "Server Status";
            // 
            // StatusBox
            // 
            StatusBox.Location = new Point(6, 26);
            StatusBox.Name = "StatusBox";
            StatusBox.ReadOnly = true;
            StatusBox.Size = new Size(1042, 27);
            StatusBox.TabIndex = 0;
            StatusBox.Text = "Disconnected";
            // 
            // StopHosting
            // 
            StopHosting.Location = new Point(558, 441);
            StopHosting.Name = "StopHosting";
            StopHosting.Size = new Size(508, 29);
            StopHosting.TabIndex = 6;
            StopHosting.Text = "Stop Hosting";
            StopHosting.UseVisualStyleBackColor = true;
            StopHosting.Click += StopHosting_Click;
            // 
            // HostBackgroundWorker
            // 
            HostBackgroundWorker.WorkerSupportsCancellation = true;
            HostBackgroundWorker.DoWork += HostBackgroundWorker_DoWork;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1078, 513);
            Controls.Add(StopHosting);
            Controls.Add(ServerStatus);
            Controls.Add(Join);
            Controls.Add(ClientLog);
            Controls.Add(Host);
            Controls.Add(HostLog);
            Controls.Add(IP);
            Name = "Form1";
            Text = "Form1";
            HostLog.ResumeLayout(false);
            HostLog.PerformLayout();
            ClientLog.ResumeLayout(false);
            ClientLog.PerformLayout();
            ServerStatus.ResumeLayout(false);
            ServerStatus.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label IP;
        private GroupBox HostLog;
        private Label DebugInfo;
        // private Button DebugInfo;
        private Button Host;
        private TextBox DebugInfoBox;
        private GroupBox ClientLog;
        private TextBox DebugInfoClient;
        private Button Join;
        private GroupBox ServerStatus;
        private TextBox StatusBox;
        private Button StopHosting;
        private System.ComponentModel.BackgroundWorker HostBackgroundWorker;
    }
}
