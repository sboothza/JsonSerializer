using System;
using System.IO;

namespace Stephen.JsonSerializer
{
    public class LayoutBlock : IDisposable
    {
        private readonly LayoutStreamWriter _writer;

        internal LayoutBlock(LayoutStreamWriter writer)
        {
            _writer = writer;
            _writer.Indent();
        }

        internal LayoutBlock()
        {
        }

        public virtual void Dispose()
        {
            _writer?.UnIndent();
        }
    }

    public class EmptyLayoutBlock : LayoutBlock
    {
        internal EmptyLayoutBlock(LayoutStreamWriter writer)
        {
        }

        internal EmptyLayoutBlock()
        {
        }

        public override void Dispose()
        {
        }
    }

    /// <summary>
    /// Streamwriter class that includes formatting functionality
    /// </summary>
    public class LayoutStreamWriter : IDisposable
    {
        private int _indent;
        private bool _lineStarted;
        private readonly StringWriter _writer = new StringWriter();

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

        public LayoutBlock StartBlock(bool pretty)
        {
            return pretty ? new LayoutBlock(this) : new EmptyLayoutBlock();
        }

        /// <summary>
        /// Writes the indent.
        /// </summary>
        private void WriteIndent()
        {
            if (!_lineStarted)
                _writer.Write(new string('\t', _indent));
        }

        /// <summary>
        /// Writes a line terminator to the text stream.
        /// </summary>
        public void WriteLine(bool startLine = true)
        {
            if (startLine && !_lineStarted)
                WriteIndent();
            _writer.WriteLine();
            _lineStarted = false;
        }

        /// <summary>
        /// Writes a string followed by a line terminator to the text stream.
        /// </summary>
        /// <param name="value">The string to write. If value is null, only the line termination characters are written.</param>
        /// <param name="startLine">If the line starts here or is a continuation</param>
        public void WriteLine(string value, bool startLine = true)
        {
            if (startLine && !_lineStarted)
                WriteIndent();
            _writer.WriteLine(value);
            _lineStarted = false;
        }

        /// <summary>
        ///  Writes a string  to the text stream.
        /// </summary>
        /// <param name="value">The string to write</param>
        /// <param name="startLine">If the line starts here or is a continuation</param>
        public void Write(string value, bool startLine = true)
        {
            if (startLine && !_lineStarted)
                WriteIndent();
            _writer.Write(value);
            _lineStarted = true;
        }

        public void Dispose()
        {
            _writer.Dispose();
        }

        public override string ToString()
        {
            return _writer.ToString();
        }
    }
}