using System;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using CPUEmu.MemoryManipulation;

namespace CPUEmu.Forms
{
    public partial class MemoryManipulationForm : Form
    {
        public IMemoryManipulation CurrentMemoryManipulation { get; private set; }

        public MemoryManipulationForm()
        {
            InitializeComponent();
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            FinishMemoryManipulation();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            if (Visible)
            {
                DialogResult = DialogResult.None;
                CurrentMemoryManipulation = null;
            }

            base.OnVisibleChanged(e);
        }

        private void CmbType_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnChooseFile.Visible = (string)cmbType.SelectedItem == "File";
        }

        private void BtnChooseFile_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                if (File.Exists(ofd.FileName))
                {
                    txtManipulationValue.Text = ofd.FileName;
                }
            }
        }

        private void TxtOffset_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
                FinishMemoryManipulation();
        }

        private void TxtManipulationValue_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
                FinishMemoryManipulation();
        }

        private void FinishMemoryManipulation()
        {
            if (string.IsNullOrEmpty(txtManipulationValue.Text))
            {
                MessageBox.Show("You have to set the value first.", "Value not set", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!int.TryParse(txtOffset.Text, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var offset))
            {
                MessageBox.Show("The offset is invalid.", "Invalid offset.", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            switch (cmbType.Text)
            {
                case "Value":
                    if (!byte.TryParse(txtManipulationValue.Text, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var value))
                    {
                        MessageBox.Show("The value doesn't fit in a byte.", "Invalid value", MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                        return;
                    }

                    CurrentMemoryManipulation = new ValueMemoryManipulation(offset, value);
                    break;
                case "File":
                    if (!File.Exists(txtManipulationValue.Text))
                    {
                        MessageBox.Show("The file does not exist.", "Invalid file", MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                        return;
                    }

                    CurrentMemoryManipulation = new FileMemoryManipulation(offset, txtManipulationValue.Text);
                    break;
            }

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
