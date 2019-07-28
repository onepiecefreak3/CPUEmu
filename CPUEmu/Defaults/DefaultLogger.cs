using System;
using System.Windows.Forms;
using CPUEmu.Interfaces;

namespace CPUEmu.Defaults
{
    class DefaultLogger : ILogger
    {
        private RichTextBox _textBox;

        public DefaultLogger(RichTextBox textBox)
        {
            _textBox = textBox;
        }

        public void Log(string message)
        {
            if (_textBox.InvokeRequired)
                _textBox.Invoke(new MethodInvoker(() => { Log(message); }));
            else
                _textBox.Text += message + Environment.NewLine;
        }

        public void Dispose()
        {
            _textBox = null;
        }
    }
}
