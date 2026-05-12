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
            ClientBackgroundWorker = new System.ComponentModel.BackgroundWorker();
            ClientMessageGroup = new GroupBox();
            ClientSend = new Button();
            ClientMessagesBox = new TextBox();
            HostIpBox = new GroupBox();
            HostIp = new TextBox();
            HostLog.SuspendLayout();
            ClientLog.SuspendLayout();
            ServerStatus.SuspendLayout();
            ClientMessageGroup.SuspendLayout();
            HostIpBox.SuspendLayout();
            SuspendLayout();
            // 
            // IP
            // 
            IP.AutoSize = true;
            IP.Location = new Point(12, 69);
            IP.Name = "IP";
            IP.Size = new Size(0, 20);
            IP.TabIndex = 0;
            // 
            // HostLog
            // 
            HostLog.Controls.Add(DebugInfoBox);
            HostLog.Location = new Point(558, 72);
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
            Host.Location = new Point(12, 501);
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
            ClientLog.Location = new Point(12, 72);
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
            Join.Location = new Point(12, 536);
            Join.Name = "Join";
            Join.Size = new Size(1054, 29);
            Join.TabIndex = 4;
            Join.Text = "Join";
            Join.UseVisualStyleBackColor = true;
            Join.Click += Join_Click;
            // 
            // ServerStatus
            // 
            ServerStatus.Controls.Add(StatusBox);
            ServerStatus.Location = new Point(12, 432);
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
            StopHosting.Location = new Point(558, 501);
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
            // ClientBackgroundWorker
            // 
            ClientBackgroundWorker.WorkerSupportsCancellation = true;
            ClientBackgroundWorker.DoWork += ClientBackgroundWorker_DoWork;
            // 
            // ClientMessageGroup
            // 
            ClientMessageGroup.Controls.Add(ClientSend);
            ClientMessageGroup.Controls.Add(ClientMessagesBox);
            ClientMessageGroup.Location = new Point(18, 571);
            ClientMessageGroup.Name = "ClientMessageGroup";
            ClientMessageGroup.Size = new Size(1048, 63);
            ClientMessageGroup.TabIndex = 7;
            ClientMessageGroup.TabStop = false;
            ClientMessageGroup.Text = "Send Message";
            // 
            // ClientSend
            // 
            ClientSend.Location = new Point(948, 25);
            ClientSend.Name = "ClientSend";
            ClientSend.Size = new Size(94, 29);
            ClientSend.TabIndex = 1;
            ClientSend.Text = "Send";
            ClientSend.UseVisualStyleBackColor = true;
            ClientSend.Click += ClientSend_Click;
            // 
            // ClientMessagesBox
            // 
            ClientMessagesBox.Location = new Point(6, 26);
            ClientMessagesBox.MaxLength = 256;
            ClientMessagesBox.Name = "ClientMessagesBox";
            ClientMessagesBox.Size = new Size(936, 27);
            ClientMessagesBox.TabIndex = 0;
            // 
            // HostIpBox
            // 
            HostIpBox.Controls.Add(HostIp);
            HostIpBox.Location = new Point(18, 12);
            HostIpBox.Name = "HostIpBox";
            HostIpBox.Size = new Size(1048, 61);
            HostIpBox.TabIndex = 8;
            HostIpBox.TabStop = false;
            HostIpBox.Text = "Host IP or Hostname (for connecting to host)";
            // 
            // HostIp
            // 
            HostIp.Location = new Point(6, 21);
            HostIp.Name = "HostIp";
            HostIp.Size = new Size(1032, 27);
            HostIp.TabIndex = 0;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1078, 640);
            Controls.Add(HostIpBox);
            Controls.Add(ClientMessageGroup);
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
            ClientMessageGroup.ResumeLayout(false);
            ClientMessageGroup.PerformLayout();
            HostIpBox.ResumeLayout(false);
            HostIpBox.PerformLayout();
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
        private System.ComponentModel.BackgroundWorker ClientBackgroundWorker;
        private GroupBox ClientMessageGroup;
        private TextBox ClientMessagesBox;
        private Button ClientSend;
        private GroupBox HostIpBox;
        private TextBox HostIp;
    }
}
