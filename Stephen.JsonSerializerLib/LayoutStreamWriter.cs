using System.IO;
using System.Text;

namespace Stephen.JsonSerializer
{
    /// <summary>
    /// Streamwriter class that includes formatting functionality
    /// </summary>
    public class LayoutStreamWriter : StringWriter
    {
        private int _indent;

        public LayoutStreamWriter(StringBuilder stringBuilder) : base(stringBuilder)
        {
        }

        /// <summary>
        /// Indents the output stream by another tab
        /// </summary>
        public void Indent()
        {
            _indent++;
        }

        /// <summary>
        /// Unindents the output stream by 1 tab
        /// </summary>
        public void UnIndent()
        {
            if (--_indent < 0)
                _indent = 0;
        }

        private bool _inCall;

        /// <summary>
        /// Writes the indent.
        /// </summary>
        private void WriteIndent()
        {
            if (!_inCall)
                Write(new string('\t', _indent));
        }

        /// <summary>
        /// Writes a line terminator to the text stream.
        /// </summary>
        public override void WriteLine()
        {
            WriteIndent();
            _inCall = true;
            base.WriteLine();
            _inCall = false;
        }

        /// <summary>
        /// Writes a character followed by a line terminator to the text stream.
        /// </summary>
        /// <param name="value"></param>
        public override void WriteLine(char value)
        {
            WriteIndent();
            _inCall = true;
            base.WriteLine(value);
            _inCall = false;
        }

        /// <summary>
        /// Writes an array of characters followed by a line terminator to the text stream.
        /// </summary>
        /// <param name="buffer"></param>
        public override void WriteLine(char[] buffer)
        {
            WriteIndent();
            _inCall = true;
            base.WriteLine(buffer);
            _inCall = false;
        }

        /// <summary>
        /// Writes a subarray of characters followed by a line terminator to the text stream
        /// </summary>
        /// <param name="buffer">The character array from which data is read.</param>
        /// <param name="index">The index into buffer at which to begin reading.</param>
        /// <param name="count">The maximum number of characters to write.</param>
        public override void WriteLine(char[] buffer, int index, int count)
        {
            WriteIndent();
            _inCall = true;
            base.WriteLine(buffer, index, count);
            _inCall = false;
        }

        /// <summary>
        ///     Writes the text representation of a Boolean followed by a line terminator
        ///     to the text stream.
        /// </summary>
        /// <param name="value">The Boolean to write.</param>
        public override void WriteLine(bool value)
        {
            WriteIndent();
            _inCall = true;
            base.WriteLine(value);
            _inCall = false;
        }

        /// <summary>
        ///     Writes the text representation of a 4-byte signed integer followed by a line
        ///     terminator to the text stream.
        /// </summary>
        /// <param name="value">The 4-byte signed integer to write.</param>
        public override void WriteLine(int value)
        {
            WriteIndent();
            _inCall = true;
            base.WriteLine(value);
            _inCall = false;
        }

        /// <summary>
        ///     Writes the text representation of a 4-byte unsigned integer followed by a
        ///     line terminator to the text stream.
        /// </summary>
        /// <param name="value">The 4-byte unsigned integer to write.</param>
        public override void WriteLine(uint value)
        {
            WriteIndent();
            _inCall = true;
            base.WriteLine(value);
            _inCall = false;
        }

        /// <summary>
        ///     Writes the text representation of an 8-byte signed integer followed by a
        ///     line terminator to the text stream.
        /// </summary>
        /// <param name="value">The 8-byte signed integer to write.</param>
        public override void WriteLine(long value)
        {
            WriteIndent();
            _inCall = true;
            base.WriteLine(value);
            _inCall = false;
        }

        /// <summary>
        ///     Writes the text representation of an 8-byte unsigned integer followed by
        ///     a line terminator to the text stream.
        /// </summary>
        /// <param name="value">The 8-byte unsigned integer to write.</param>
        public override void WriteLine(ulong value)
        {
            WriteIndent();
            _inCall = true;
            base.WriteLine(value);
            _inCall = false;
        }

        public override Encoding Encoding { get; }

        /// <summary>
        ///     Writes the text representation of a 4-byte floating-point value followed
        ///     by a line terminator to the text stream.
        /// </summary>
        /// <param name="value">The 4-byte floating-point value to write.</param>
        public override void WriteLine(float value)
        {
            WriteIndent();
            _inCall = true;
            base.WriteLine(value);
            _inCall = false;
        }

        /// <summary>
        ///     Writes the text representation of a 8-byte floating-point value followed
        ///     by a line terminator to the text stream.
        /// </summary>
        /// <param name="value">The 8-byte floating-point value to write.</param>
        public override void WriteLine(double value)
        {
            WriteIndent();
            _inCall = true;
            base.WriteLine(value);
            _inCall = false;
        }

        /// <summary>
        ///     Writes the text representation of a decimal value followed by a line terminator
        ///     to the text stream.
        /// </summary>
        /// <param name="value">The decimal value to write.</param>
        public override void WriteLine(decimal value)
        {
            WriteIndent();
            _inCall = true;
            base.WriteLine(value);
            _inCall = false;
        }

        /// <summary>
        /// Writes a string followed by a line terminator to the text stream.
        /// </summary>
        /// <param name="value">The string to write. If value is null, only the line termination characters are written.</param>
        public override void WriteLine(string value)
        {
            WriteIndent();
            _inCall = true;
            base.WriteLine(value);
            _inCall = false;
        }

        /// <summary>
        ///     Writes the text representation of an object by calling ToString on this object,
        ///     followed by a line terminator to the text stream.
        /// </summary>
        /// <param name="value">The object to write. If value is null, only the line termination characters are written.</param>
        public override void WriteLine(object value)
        {
            WriteIndent();
            _inCall = true;
            base.WriteLine(value);
            _inCall = false;
        }

        /// <summary>
        ///     Writes out a formatted string and a new line, using the same semantics as
        ///     System.String.Format(System.String,System.Object).
        /// </summary>
        /// <param name="format">The formatted string.</param>
        /// <param name="arg0">The object to write into the formatted string.</param>
        public override void WriteLine(string format, object arg0)
        {
            WriteIndent();
            _inCall = true;
            base.WriteLine(format, arg0);
            _inCall = false;
        }

        /// <summary>
        ///     Writes out a formatted string and a new line, using the same semantics as
        ///     System.String.Format(System.String,System.Object).
        /// </summary>
        /// <param name="format">The formatting string.</param>
        /// <param name="arg0">The object to write into the format string.</param>
        /// <param name="arg1">The object to write into the format string.</param>
        public override void WriteLine(string format, object arg0, object arg1)
        {
            WriteIndent();
            _inCall = true;
            base.WriteLine(format, arg0, arg1);
            _inCall = false;
        }

        /// <summary>
        ///     Writes out a formatted string and a new line, using the same semantics as
        ///     System.String.Format(System.String,System.Object).
        /// </summary>
        /// <param name="format">The formatting string.</param>
        /// <param name="arg0">The object to write into the format string.</param>
        /// <param name="arg1">The object to write into the format string.</param>
        /// <param name="arg2">The object to write into the format string.</param>
        public override void WriteLine(string format, object arg0, object arg1, object arg2)
        {
            WriteIndent();
            _inCall = true;
            base.WriteLine(format, arg0, arg1, arg2);
            _inCall = false;
        }

        /// <summary>
        ///   Writes out a formatted string and a new line, using the same semantics as
        ///   System.String.Format(System.String,System.Object).
        /// </summary>
        /// <param name="format">The formatting string.</param>
        /// <param name="arg">The object array to write into format string.</param>
        public override void WriteLine(string format, params object[] arg)
        {
            WriteIndent();
            _inCall = true;
            base.WriteLine(format, arg);
            _inCall = false;
        }
    }
}