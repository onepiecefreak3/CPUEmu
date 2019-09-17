using System.Drawing;
using Be.Windows.Forms;

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
            this.txtlog = new System.Windows.Forms.RichTextBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnAbort = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnStartExecution = new System.Windows.Forms.Button();
            this.btnResume = new System.Windows.Forms.Button();
            this.hexBox = new Be.Windows.Forms.HexBox();
            this.bytePositionTxt = new System.Windows.Forms.TextBox();
            this.btnStep = new System.Windows.Forms.Button();
            this.manipulationTxt = new System.Windows.Forms.TextBox();
            this.btnAddManipulation = new System.Windows.Forms.Button();
            this.txtDisassembly = new CPUEmu.CodingListBox();
            this.txtFlags = new System.Windows.Forms.ListBox();
            this.txtRegisters = new System.Windows.Forms.ListBox();
            this.assemblyContainer = new System.Windows.Forms.SplitContainer();
            this.registerFlagsContainer = new System.Windows.Forms.SplitContainer();
            this.txtEditFlagRegister = new System.Windows.Forms.TextBox();
            this.executionContainer = new System.Windows.Forms.SplitContainer();
            this.subBtnContainer = new System.Windows.Forms.SplitContainer();
            this.mainContainer = new System.Windows.Forms.SplitContainer();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.assemblyContainer)).BeginInit();
            this.assemblyContainer.Panel1.SuspendLayout();
            this.assemblyContainer.Panel2.SuspendLayout();
            this.assemblyContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.registerFlagsContainer)).BeginInit();
            this.registerFlagsContainer.Panel1.SuspendLayout();
            this.registerFlagsContainer.Panel2.SuspendLayout();
            this.registerFlagsContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.executionContainer)).BeginInit();
            this.executionContainer.Panel1.SuspendLayout();
            this.executionContainer.Panel2.SuspendLayout();
            this.executionContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.subBtnContainer)).BeginInit();
            this.subBtnContainer.Panel1.SuspendLayout();
            this.subBtnContainer.Panel2.SuspendLayout();
            this.subBtnContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mainContainer)).BeginInit();
            this.mainContainer.Panel1.SuspendLayout();
            this.mainContainer.Panel2.SuspendLayout();
            this.mainContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtlog
            // 
            this.txtlog.BackColor = System.Drawing.Color.Black;
            this.txtlog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtlog.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtlog.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(255)))), ((int)(((byte)(65)))));
            this.txtlog.Location = new System.Drawing.Point(0, 0);
            this.txtlog.Name = "txtlog";
            this.txtlog.ReadOnly = true;
            this.txtlog.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.txtlog.Size = new System.Drawing.Size(300, 297);
            this.txtlog.TabIndex = 0;
            this.txtlog.Text = "";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(888, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "mainMenuStrip";
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
            this.btnAbort.AutoSize = true;
            this.btnAbort.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.btnAbort.Enabled = false;
            this.btnAbort.Location = new System.Drawing.Point(0, 34);
            this.btnAbort.Name = "btnAbort";
            this.btnAbort.Size = new System.Drawing.Size(144, 23);
            this.btnAbort.TabIndex = 3;
            this.btnAbort.Text = "Abort Execution";
            this.btnAbort.UseVisualStyleBackColor = true;
            this.btnAbort.Click += new System.EventHandler(this.btnAbort_Click);
            // 
            // btnStop
            // 
            this.btnStop.AutoSize = true;
            this.btnStop.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnStop.Enabled = false;
            this.btnStop.Location = new System.Drawing.Point(0, 0);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(152, 23);
            this.btnStop.TabIndex = 4;
            this.btnStop.Text = "Halt Execution";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnStartExecution
            // 
            this.btnStartExecution.AutoSize = true;
            this.btnStartExecution.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnStartExecution.Enabled = false;
            this.btnStartExecution.Location = new System.Drawing.Point(0, 0);
            this.btnStartExecution.Name = "btnStartExecution";
            this.btnStartExecution.Size = new System.Drawing.Size(144, 23);
            this.btnStartExecution.TabIndex = 13;
            this.btnStartExecution.Text = "Start Execution";
            this.btnStartExecution.UseVisualStyleBackColor = true;
            this.btnStartExecution.Click += new System.EventHandler(this.BtnStartExecution_Click);
            // 
            // btnResume
            // 
            this.btnResume.AutoSize = true;
            this.btnResume.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.btnResume.Enabled = false;
            this.btnResume.Location = new System.Drawing.Point(0, 34);
            this.btnResume.Name = "btnResume";
            this.btnResume.Size = new System.Drawing.Size(152, 23);
            this.btnResume.TabIndex = 14;
            this.btnResume.Text = "Resume Execution";
            this.btnResume.UseVisualStyleBackColor = true;
            this.btnResume.Click += new System.EventHandler(this.BtnResume_Click);
            // 
            // hexBox
            // 
            this.hexBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.hexBox.ColumnInfoVisible = true;
            this.hexBox.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.hexBox.LineInfoVisible = true;
            this.hexBox.Location = new System.Drawing.Point(12, 429);
            this.hexBox.Name = "hexBox";
            this.hexBox.ReadOnly = true;
            this.hexBox.ShadowSelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(60)))), ((int)(((byte)(188)))), ((int)(((byte)(255)))));
            this.hexBox.Size = new System.Drawing.Size(639, 193);
            this.hexBox.StringViewVisible = true;
            this.hexBox.TabIndex = 15;
            this.hexBox.VScrollBarVisible = true;
            this.hexBox.ByteProviderChanged += new System.EventHandler(this.HexBox_ByteProviderChanged);
            this.hexBox.CurrentLineChanged += new System.EventHandler(this.HexBox_CurrentLineChanged);
            this.hexBox.CurrentPositionInLineChanged += new System.EventHandler(this.HexBox_CurrentPositionInLineChanged);
            // 
            // bytePositionTxt
            // 
            this.bytePositionTxt.Enabled = false;
            this.bytePositionTxt.Location = new System.Drawing.Point(12, 429);
            this.bytePositionTxt.Name = "bytePositionTxt";
            this.bytePositionTxt.Size = new System.Drawing.Size(63, 20);
            this.bytePositionTxt.TabIndex = 16;
            this.bytePositionTxt.Visible = false;
            this.bytePositionTxt.KeyUp += new System.Windows.Forms.KeyEventHandler(this.BytePositionTxt_KeyUp);
            // 
            // btnStep
            // 
            this.btnStep.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnStep.Enabled = false;
            this.btnStep.Location = new System.Drawing.Point(0, 63);
            this.btnStep.Name = "btnStep";
            this.btnStep.Size = new System.Drawing.Size(301, 23);
            this.btnStep.TabIndex = 17;
            this.btnStep.Text = "Step Execution";
            this.btnStep.UseVisualStyleBackColor = true;
            this.btnStep.Click += new System.EventHandler(this.BtnStep_Click);
            // 
            // manipulationTxt
            // 
            this.manipulationTxt.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.manipulationTxt.Location = new System.Drawing.Point(657, 458);
            this.manipulationTxt.Multiline = true;
            this.manipulationTxt.Name = "manipulationTxt";
            this.manipulationTxt.ReadOnly = true;
            this.manipulationTxt.Size = new System.Drawing.Size(219, 164);
            this.manipulationTxt.TabIndex = 18;
            // 
            // btnAddManipulation
            // 
            this.btnAddManipulation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAddManipulation.Enabled = false;
            this.btnAddManipulation.Location = new System.Drawing.Point(657, 429);
            this.btnAddManipulation.Name = "btnAddManipulation";
            this.btnAddManipulation.Size = new System.Drawing.Size(218, 23);
            this.btnAddManipulation.TabIndex = 19;
            this.btnAddManipulation.Text = "Add memory manipulation";
            this.btnAddManipulation.UseVisualStyleBackColor = true;
            this.btnAddManipulation.Click += new System.EventHandler(this.BtnAddManipulation_Click);
            // 
            // txtDisassembly
            // 
            this.txtDisassembly.BreakpointAreaWidth = 17;
            this.txtDisassembly.BreakpointColor = System.Drawing.Color.Red;
            this.txtDisassembly.BreakpointRadius = 8;
            this.txtDisassembly.CurrentInstructionColor = System.Drawing.Color.Yellow;
            this.txtDisassembly.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtDisassembly.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.txtDisassembly.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtDisassembly.FormattingEnabled = true;
            this.txtDisassembly.ItemHeight = 14;
            this.txtDisassembly.Location = new System.Drawing.Point(0, 0);
            this.txtDisassembly.Name = "txtDisassembly";
            this.txtDisassembly.ScrollAlwaysVisible = true;
            this.txtDisassembly.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.txtDisassembly.Size = new System.Drawing.Size(410, 390);
            this.txtDisassembly.TabIndex = 0;
            this.txtDisassembly.BreakpointAdded += new System.EventHandler<CPUEmu.IndexEventArgs>(this.TxtDisassembly_BreakpointAdded);
            this.txtDisassembly.BreakpointRemoved += new System.EventHandler<CPUEmu.IndexEventArgs>(this.TxtDisassembly_BreakpointRemoved);
            this.txtDisassembly.BreakpointEnabled += new System.EventHandler<CPUEmu.IndexEventArgs>(this.TxtDisassembly_BreakpointEnabled);
            this.txtDisassembly.BreakpointDisabled += new System.EventHandler<CPUEmu.IndexEventArgs>(this.TxtDisassembly_BreakpointDisabled);
            // 
            // txtFlags
            // 
            this.txtFlags.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtFlags.FormattingEnabled = true;
            this.txtFlags.Location = new System.Drawing.Point(0, 0);
            this.txtFlags.Name = "txtFlags";
            this.txtFlags.Size = new System.Drawing.Size(145, 117);
            this.txtFlags.TabIndex = 20;
            this.txtFlags.DoubleClick += new System.EventHandler(this.TxtFlags_DoubleClick);
            this.txtFlags.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TxtFlags_KeyPress);
            // 
            // txtRegisters
            // 
            this.txtRegisters.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtRegisters.FormattingEnabled = true;
            this.txtRegisters.Location = new System.Drawing.Point(0, 0);
            this.txtRegisters.Name = "txtRegisters";
            this.txtRegisters.Size = new System.Drawing.Size(145, 265);
            this.txtRegisters.TabIndex = 21;
            this.txtRegisters.DoubleClick += new System.EventHandler(this.TxtRegisters_DoubleClick);
            this.txtRegisters.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TxtRegisters_KeyPress);
            // 
            // assemblyContainer
            // 
            this.assemblyContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.assemblyContainer.Location = new System.Drawing.Point(0, 0);
            this.assemblyContainer.Name = "assemblyContainer";
            // 
            // assemblyContainer.Panel1
            // 
            this.assemblyContainer.Panel1.Controls.Add(this.txtDisassembly);
            // 
            // assemblyContainer.Panel2
            // 
            this.assemblyContainer.Panel2.Controls.Add(this.registerFlagsContainer);
            this.assemblyContainer.Size = new System.Drawing.Size(559, 390);
            this.assemblyContainer.SplitterDistance = 410;
            this.assemblyContainer.TabIndex = 22;
            // 
            // registerFlagsContainer
            // 
            this.registerFlagsContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.registerFlagsContainer.IsSplitterFixed = true;
            this.registerFlagsContainer.Location = new System.Drawing.Point(0, 0);
            this.registerFlagsContainer.Name = "registerFlagsContainer";
            this.registerFlagsContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // registerFlagsContainer.Panel1
            // 
            this.registerFlagsContainer.Panel1.Controls.Add(this.txtFlags);
            // 
            // registerFlagsContainer.Panel2
            // 
            this.registerFlagsContainer.Panel2.Controls.Add(this.txtRegisters);
            this.registerFlagsContainer.Size = new System.Drawing.Size(145, 390);
            this.registerFlagsContainer.SplitterDistance = 117;
            this.registerFlagsContainer.SplitterIncrement = 13;
            this.registerFlagsContainer.SplitterWidth = 8;
            this.registerFlagsContainer.TabIndex = 22;
            // 
            // txtEditFlagRegister
            // 
            this.txtEditFlagRegister.Location = new System.Drawing.Point(3, 3);
            this.txtEditFlagRegister.Name = "txtEditFlagRegister";
            this.txtEditFlagRegister.Size = new System.Drawing.Size(100, 20);
            this.txtEditFlagRegister.TabIndex = 21;
            this.txtEditFlagRegister.Visible = false;
            this.txtEditFlagRegister.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TxtEditFlagRegister_KeyPress);
            this.txtEditFlagRegister.Leave += new System.EventHandler(this.TxtEditFlagRegister_Leave);
            // 
            // executionContainer
            // 
            this.executionContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.executionContainer.IsSplitterFixed = true;
            this.executionContainer.Location = new System.Drawing.Point(0, 0);
            this.executionContainer.Name = "executionContainer";
            this.executionContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // executionContainer.Panel1
            // 
            this.executionContainer.Panel1.Controls.Add(this.subBtnContainer);
            this.executionContainer.Panel1.Controls.Add(this.btnStep);
            // 
            // executionContainer.Panel2
            // 
            this.executionContainer.Panel2.Controls.Add(this.txtlog);
            this.executionContainer.Size = new System.Drawing.Size(300, 390);
            this.executionContainer.SplitterDistance = 89;
            this.executionContainer.TabIndex = 23;
            // 
            // subBtnContainer
            // 
            this.subBtnContainer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.subBtnContainer.Location = new System.Drawing.Point(0, 0);
            this.subBtnContainer.Name = "subBtnContainer";
            // 
            // subBtnContainer.Panel1
            // 
            this.subBtnContainer.Panel1.Controls.Add(this.btnAbort);
            this.subBtnContainer.Panel1.Controls.Add(this.btnStartExecution);
            // 
            // subBtnContainer.Panel2
            // 
            this.subBtnContainer.Panel2.Controls.Add(this.btnStop);
            this.subBtnContainer.Panel2.Controls.Add(this.btnResume);
            this.subBtnContainer.Size = new System.Drawing.Size(300, 57);
            this.subBtnContainer.SplitterDistance = 144;
            this.subBtnContainer.TabIndex = 18;
            // 
            // mainContainer
            // 
            this.mainContainer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mainContainer.Location = new System.Drawing.Point(12, 30);
            this.mainContainer.Name = "mainContainer";
            // 
            // mainContainer.Panel1
            // 
            this.mainContainer.Panel1.Controls.Add(this.assemblyContainer);
            // 
            // mainContainer.Panel2
            // 
            this.mainContainer.Panel2.Controls.Add(this.executionContainer);
            this.mainContainer.Size = new System.Drawing.Size(863, 390);
            this.mainContainer.SplitterDistance = 559;
            this.mainContainer.TabIndex = 24;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(888, 634);
            this.Controls.Add(this.bytePositionTxt);
            this.Controls.Add(this.txtEditFlagRegister);
            this.Controls.Add(this.mainContainer);
            this.Controls.Add(this.btnAddManipulation);
            this.Controls.Add(this.manipulationTxt);
            this.Controls.Add(this.hexBox);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.Text = "MainForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.assemblyContainer.Panel1.ResumeLayout(false);
            this.assemblyContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.assemblyContainer)).EndInit();
            this.assemblyContainer.ResumeLayout(false);
            this.registerFlagsContainer.Panel1.ResumeLayout(false);
            this.registerFlagsContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.registerFlagsContainer)).EndInit();
            this.registerFlagsContainer.ResumeLayout(false);
            this.executionContainer.Panel1.ResumeLayout(false);
            this.executionContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.executionContainer)).EndInit();
            this.executionContainer.ResumeLayout(false);
            this.subBtnContainer.Panel1.ResumeLayout(false);
            this.subBtnContainer.Panel1.PerformLayout();
            this.subBtnContainer.Panel2.ResumeLayout(false);
            this.subBtnContainer.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.subBtnContainer)).EndInit();
            this.subBtnContainer.ResumeLayout(false);
            this.mainContainer.Panel1.ResumeLayout(false);
            this.mainContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.mainContainer)).EndInit();
            this.mainContainer.ResumeLayout(false);
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
        private System.Windows.Forms.Button btnStartExecution;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private CodingListBox txtDisassembly;
        private System.Windows.Forms.Button btnResume;
        private HexBox hexBox;
        private System.Windows.Forms.TextBox bytePositionTxt;
        private System.Windows.Forms.Button btnStep;
        private System.Windows.Forms.TextBox manipulationTxt;
        private System.Windows.Forms.Button btnAddManipulation;
        private System.Windows.Forms.ListBox txtFlags;
        private System.Windows.Forms.ListBox txtRegisters;
        private System.Windows.Forms.SplitContainer assemblyContainer;
        private System.Windows.Forms.SplitContainer executionContainer;
        private System.Windows.Forms.SplitContainer subBtnContainer;
        private System.Windows.Forms.SplitContainer mainContainer;
        private System.Windows.Forms.SplitContainer registerFlagsContainer;
        private System.Windows.Forms.TextBox txtEditFlagRegister;
    }
}

