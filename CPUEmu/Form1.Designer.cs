namespace CPUEmu
{
    partial class Form1
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
            this.label1 = new System.Windows.Forms.Label();
            this.btnAbort = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.txtFlags = new System.Windows.Forms.TextBox();
            this.txtRegs = new System.Windows.Forms.TextBox();
            this.btnStep = new System.Windows.Forms.Button();
            this.txtDisassembly = new System.Windows.Forms.RichTextBox();
            this.btnPrintToggle = new System.Windows.Forms.Button();
            this.btnReExec = new System.Windows.Forms.Button();
            this.pnBreakPoints = new System.Windows.Forms.Panel();
            this.timDisassembly = new System.Windows.Forms.Timer(this.components);
            this.timTable = new System.Windows.Forms.Timer(this.components);
            this.timExecution = new System.Windows.Forms.Timer(this.components);
            this.menuStrip1.SuspendLayout();
            this.pnBreakPoints.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtlog
            // 
            this.txtlog.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.txtlog.BackColor = System.Drawing.Color.Black;
            this.txtlog.ForeColor = System.Drawing.Color.White;
            this.txtlog.Location = new System.Drawing.Point(12, 62);
            this.txtlog.Name = "txtlog";
            this.txtlog.ReadOnly = true;
            this.txtlog.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.txtlog.Size = new System.Drawing.Size(346, 376);
            this.txtlog.TabIndex = 0;
            this.txtlog.Text = "";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(800, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem});
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
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 46);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(48, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Console:";
            // 
            // btnAbort
            // 
            this.btnAbort.Enabled = false;
            this.btnAbort.Location = new System.Drawing.Point(364, 60);
            this.btnAbort.Name = "btnAbort";
            this.btnAbort.Size = new System.Drawing.Size(115, 23);
            this.btnAbort.TabIndex = 3;
            this.btnAbort.Text = "Abort Execution";
            this.btnAbort.UseVisualStyleBackColor = true;
            this.btnAbort.Click += new System.EventHandler(this.btnAbort_Click);
            // 
            // btnStop
            // 
            this.btnStop.Enabled = false;
            this.btnStop.Location = new System.Drawing.Point(364, 89);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(115, 23);
            this.btnStop.TabIndex = 4;
            this.btnStop.Text = "Stop Execution";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // txtFlags
            // 
            this.txtFlags.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFlags.Enabled = false;
            this.txtFlags.ForeColor = System.Drawing.Color.Black;
            this.txtFlags.Location = new System.Drawing.Point(679, 61);
            this.txtFlags.Multiline = true;
            this.txtFlags.Name = "txtFlags";
            this.txtFlags.ReadOnly = true;
            this.txtFlags.Size = new System.Drawing.Size(108, 120);
            this.txtFlags.TabIndex = 5;
            // 
            // txtRegs
            // 
            this.txtRegs.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtRegs.Enabled = false;
            this.txtRegs.ForeColor = System.Drawing.Color.Black;
            this.txtRegs.Location = new System.Drawing.Point(679, 187);
            this.txtRegs.Multiline = true;
            this.txtRegs.Name = "txtRegs";
            this.txtRegs.ReadOnly = true;
            this.txtRegs.Size = new System.Drawing.Size(108, 250);
            this.txtRegs.TabIndex = 6;
            // 
            // btnStep
            // 
            this.btnStep.Enabled = false;
            this.btnStep.Location = new System.Drawing.Point(364, 118);
            this.btnStep.Name = "btnStep";
            this.btnStep.Size = new System.Drawing.Size(115, 23);
            this.btnStep.TabIndex = 7;
            this.btnStep.Text = "Do Step";
            this.btnStep.UseVisualStyleBackColor = true;
            this.btnStep.Click += new System.EventHandler(this.btnStep_Click);
            // 
            // txtDisassembly
            // 
            this.txtDisassembly.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDisassembly.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtDisassembly.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtDisassembly.ForeColor = System.Drawing.Color.Black;
            this.txtDisassembly.Location = new System.Drawing.Point(23, 0);
            this.txtDisassembly.Name = "txtDisassembly";
            this.txtDisassembly.ReadOnly = true;
            this.txtDisassembly.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.txtDisassembly.Size = new System.Drawing.Size(286, 290);
            this.txtDisassembly.TabIndex = 8;
            this.txtDisassembly.Text = "";
            // 
            // btnPrintToggle
            // 
            this.btnPrintToggle.Location = new System.Drawing.Point(485, 89);
            this.btnPrintToggle.Name = "btnPrintToggle";
            this.btnPrintToggle.Size = new System.Drawing.Size(115, 23);
            this.btnPrintToggle.TabIndex = 10;
            this.btnPrintToggle.Text = "Disable Printing";
            this.btnPrintToggle.UseVisualStyleBackColor = true;
            this.btnPrintToggle.Click += new System.EventHandler(this.btnPrintToggle_Click);
            // 
            // btnReExec
            // 
            this.btnReExec.Enabled = false;
            this.btnReExec.Location = new System.Drawing.Point(485, 60);
            this.btnReExec.Name = "btnReExec";
            this.btnReExec.Size = new System.Drawing.Size(115, 23);
            this.btnReExec.TabIndex = 11;
            this.btnReExec.Text = "ReExecute";
            this.btnReExec.UseVisualStyleBackColor = true;
            this.btnReExec.Click += new System.EventHandler(this.btnReExec_Click);
            // 
            // pnBreakPoints
            // 
            this.pnBreakPoints.Controls.Add(this.txtDisassembly);
            this.pnBreakPoints.Location = new System.Drawing.Point(364, 147);
            this.pnBreakPoints.Name = "pnBreakPoints";
            this.pnBreakPoints.Size = new System.Drawing.Size(309, 290);
            this.pnBreakPoints.TabIndex = 12;
            this.pnBreakPoints.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pnBreakPoints_MouseUp);
            // 
            // timDisassembly
            // 
            this.timDisassembly.Interval = 40;
            this.timDisassembly.Tick += new System.EventHandler(this.timDisassembly_Tick);
            // 
            // timTable
            // 
            this.timTable.Interval = 16;
            this.timTable.Tick += new System.EventHandler(this.timTable_Tick);
            // 
            // timExecution
            // 
            this.timExecution.Tick += new System.EventHandler(this.timExecution_Tick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.pnBreakPoints);
            this.Controls.Add(this.btnReExec);
            this.Controls.Add(this.btnPrintToggle);
            this.Controls.Add(this.btnStep);
            this.Controls.Add(this.txtRegs);
            this.Controls.Add(this.txtFlags);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.btnAbort);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtlog);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "Form1";
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
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnAbort;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.TextBox txtFlags;
        private System.Windows.Forms.TextBox txtRegs;
        private System.Windows.Forms.Button btnStep;
        private System.Windows.Forms.RichTextBox txtDisassembly;
        private System.Windows.Forms.Button btnPrintToggle;
        private System.Windows.Forms.Button btnReExec;
        private System.Windows.Forms.Panel pnBreakPoints;
        private System.Windows.Forms.Timer timDisassembly;
        private System.Windows.Forms.Timer timTable;
        private System.Windows.Forms.Timer timExecution;
    }
}

