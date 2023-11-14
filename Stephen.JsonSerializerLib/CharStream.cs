using System;
using System.IO;

namespace Stephen.JsonSerializer;

public class CharStream
{
    private readonly char[] _buffer;
    private long _offset;

    public CharStream(string value)
    {
        _buffer = value.ToCharArray();
    }
    
    public bool EndOfStream => _offset == _buffer.Length;

    public char ReadChar()
    {
        if (_offset > _buffer.Length - 1)
            throw new OverflowException("Read past end");
        return _buffer[_offset++];
    }

    public void Seek(long offset, SeekOrigin origin)
    {
        switch (origin)
        {
            case SeekOrigin.Begin:
                _offset = offset;
                break;
            case SeekOrigin.Current:
                _offset += offset;
                break;
            case SeekOrigin.End:
                _offset = _buffer.Length - 1 + offset;
                break;
        }

        if (_offset > _buffer.Length - 1)
            throw new OverflowException("Seek past end");
        if (_offset < 0)
            throw new OverflowException("Seek past beginning");
    }
}