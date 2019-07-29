namespace CPUEmu
{
    partial class MainForm
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.txtlog = new System.Windows.Forms.RichTextBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnAbort = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.txtFlags = new System.Windows.Forms.TextBox();
            this.txtRegs = new System.Windows.Forms.TextBox();
            this.pnBreakPoints = new System.Windows.Forms.Panel();
            this.timDisassembly = new System.Windows.Forms.Timer(this.components);
            this.timTable = new System.Windows.Forms.Timer(this.components);
            this.timExecution = new System.Windows.Forms.Timer(this.components);
            this.btnStartExecution = new System.Windows.Forms.Button();
            this.btnResume = new System.Windows.Forms.Button();
            this.txtDisassembly = new CPUEmu.BetterListBox();
            this.menuStrip1.SuspendLayout();
            this.pnBreakPoints.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtlog
            // 
            this.txtlog.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.txtlog.BackColor = System.Drawing.Color.Black;
            this.txtlog.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtlog.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(255)))), ((int)(((byte)(65)))));
            this.txtlog.Location = new System.Drawing.Point(347, 88);
            this.txtlog.Name = "txtlog";
            this.txtlog.ReadOnly = true;
            this.txtlog.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.txtlog.Size = new System.Drawing.Size(309, 342);
            this.txtlog.TabIndex = 0;
            this.txtlog.Text = "";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(668, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.closeToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.openToolStripMenuItem.Text = "&Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Enabled = false;
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.closeToolStripMenuItem.Text = "&Close";
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.CloseToolStripMenuItem_Click);
            // 
            // btnAbort
            // 
            this.btnAbort.Enabled = false;
            this.btnAbort.Location = new System.Drawing.Point(346, 59);
            this.btnAbort.Name = "btnAbort";
            this.btnAbort.Size = new System.Drawing.Size(151, 23);
            this.btnAbort.TabIndex = 3;
            this.btnAbort.Text = "Abort Execution";
            this.btnAbort.UseVisualStyleBackColor = true;
            this.btnAbort.Click += new System.EventHandler(this.btnAbort_Click);
            // 
            // btnStop
            // 
            this.btnStop.Enabled = false;
            this.btnStop.Location = new System.Drawing.Point(503, 30);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(153, 23);
            this.btnStop.TabIndex = 4;
            this.btnStop.Text = "Halt Execution";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // txtFlags
            // 
            this.txtFlags.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFlags.Enabled = false;
            this.txtFlags.ForeColor = System.Drawing.Color.Black;
            this.txtFlags.Location = new System.Drawing.Point(233, 27);
            this.txtFlags.Multiline = true;
            this.txtFlags.Name = "txtFlags";
            this.txtFlags.ReadOnly = true;
            this.txtFlags.Size = new System.Drawing.Size(108, 155);
            this.txtFlags.TabIndex = 5;
            // 
            // txtRegs
            // 
            this.txtRegs.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtRegs.Enabled = false;
            this.txtRegs.ForeColor = System.Drawing.Color.Black;
            this.txtRegs.Location = new System.Drawing.Point(233, 188);
            this.txtRegs.Multiline = true;
            this.txtRegs.Name = "txtRegs";
            this.txtRegs.ReadOnly = true;
            this.txtRegs.Size = new System.Drawing.Size(108, 242);
            this.txtRegs.TabIndex = 6;
            // 
            // pnBreakPoints
            // 
            this.pnBreakPoints.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.pnBreakPoints.Controls.Add(this.txtDisassembly);
            this.pnBreakPoints.Location = new System.Drawing.Point(12, 27);
            this.pnBreakPoints.Name = "pnBreakPoints";
            this.pnBreakPoints.Size = new System.Drawing.Size(215, 403);
            this.pnBreakPoints.TabIndex = 12;
            this.pnBreakPoints.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pnBreakPoints_MouseUp);
            // 
            // timDisassembly
            // 
            this.timDisassembly.Interval = 40;
            // 
            // timTable
            // 
            this.timTable.Interval = 16;
            // 
            // btnStartExecution
            // 
            this.btnStartExecution.Enabled = false;
            this.btnStartExecution.Location = new System.Drawing.Point(347, 30);
            this.btnStartExecution.Name = "btnStartExecution";
            this.btnStartExecution.Size = new System.Drawing.Size(150, 23);
            this.btnStartExecution.TabIndex = 13;
            this.btnStartExecution.Text = "Start Execution";
            this.btnStartExecution.UseVisualStyleBackColor = true;
            this.btnStartExecution.Click += new System.EventHandler(this.BtnStartExecution_Click);
            // 
            // btnResume
            // 
            this.btnResume.Enabled = false;
            this.btnResume.Location = new System.Drawing.Point(503, 59);
            this.btnResume.Name = "btnResume";
            this.btnResume.Size = new System.Drawing.Size(153, 23);
            this.btnResume.TabIndex = 14;
            this.btnResume.Text = "Resume Execution";
            this.btnResume.UseVisualStyleBackColor = true;
            this.btnResume.Click += new System.EventHandler(this.BtnResume_Click);
            // 
            // txtDisassembly
            // 
            this.txtDisassembly.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.txtDisassembly.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtDisassembly.FormattingEnabled = true;
            this.txtDisassembly.ItemHeight = 14;
            this.txtDisassembly.Location = new System.Drawing.Point(18, 3);
            this.txtDisassembly.Name = "txtDisassembly";
            this.txtDisassembly.ScrollAlwaysVisible = true;
            this.txtDisassembly.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.txtDisassembly.Size = new System.Drawing.Size(194, 396);
            this.txtDisassembly.TabIndex = 0;
            this.txtDisassembly.Scrolled += new System.EventHandler(this.TxtDisassembly_TopIndexChanged);
            this.txtDisassembly.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.TxtDisassembly_DrawItem);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(668, 442);
            this.Controls.Add(this.btnResume);
            this.Controls.Add(this.btnStartExecution);
            this.Controls.Add(this.pnBreakPoints);
            this.Controls.Add(this.txtRegs);
            this.Controls.Add(this.txtFlags);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.btnAbort);
            this.Controls.Add(this.txtlog);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.Text = "MainForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.pnBreakPoints.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox txtlog;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.Button btnAbort;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.TextBox txtFlags;
        private System.Windows.Forms.TextBox txtRegs;
        private System.Windows.Forms.Panel pnBreakPoints;
        private System.Windows.Forms.Timer timDisassembly;
        private System.Windows.Forms.Timer timTable;
        private System.Windows.Forms.Timer timExecution;
        private System.Windows.Forms.Button btnStartExecution;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private BetterListBox txtDisassembly;
        private System.Windows.Forms.Button btnResume;
    }
}

