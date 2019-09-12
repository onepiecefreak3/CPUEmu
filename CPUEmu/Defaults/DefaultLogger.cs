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

        public void Log(LogLevel logLevel, string message)
        {
            if (_textBox.InvokeRequired)
                _textBox.Invoke(new MethodInvoker(() => { Log(logLevel, message); }));
            else
            {
                _textBox.Text += $@"[{logLevel}] " + message + Environment.NewLine;
            }
        }

        public void Dispose()
        {
            _textBox = null;
        }
    }
}
