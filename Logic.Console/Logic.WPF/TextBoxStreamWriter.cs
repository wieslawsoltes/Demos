
namespace Logic.WPF
{
    #region References

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Controls;

    #endregion

    #region TextBoxStreamWriter

    public class TextBoxStreamWriter : System.IO.TextWriter
    {
        TextBox output = null;

        public TextBoxStreamWriter(TextBox output)
        {
            this.output = output;
        }

        public override void Write(char value)
        {
            base.Write(value);

            output.Dispatcher.BeginInvoke(
                new Action(() =>
                {
                    output.AppendText(value.ToString());

                    //_output.Focus();
                    output.CaretIndex = output.Text.Length;
                    output.ScrollToEnd();
                })
            );
        }

        public override Encoding Encoding
        {
            get { return System.Text.Encoding.UTF8; }
        }
    }

    #endregion
}
