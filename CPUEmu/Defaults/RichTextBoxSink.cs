using System;
using System.Windows.Forms;
using Serilog.Core;
using Serilog.Events;

namespace CPUEmu.Defaults
{
    public class RichTextBoxSink : ILogEventSink
    {
        private RichTextBox _textBox;

        public RichTextBoxSink(RichTextBox textBox)
        {
            _textBox = textBox;
        }

        ~RichTextBoxSink()
        {
            _textBox = null;
        }

        public void Emit(LogEvent logEvent)
        {
            if (_textBox.InvokeRequired)
                _textBox.Invoke(new MethodInvoker(() => { Emit(logEvent); }));
            else
            {
                _textBox.Text += $@"[{logEvent.Level}] " + logEvent.RenderMessage() + Environment.NewLine;
            }
        }
    }
}
