using System;
using System.IO;

namespace Meteorologija.Server
{
    public class FileWriterWrapper : IDisposable
    {
        private StreamWriter _writer;
        private bool _disposed = false;
        private string _path;

        public FileWriterWrapper(string path, bool append)
        {
            _path = path;
            _writer = new StreamWriter(path, append);
        }

        public void WriteLine(string line)
        {
            if (_disposed)
                throw new ObjectDisposedException("FileWriterWrapper");
            _writer.WriteLine(line);
        }

        public void Flush()
        {
            if (_disposed)
                throw new ObjectDisposedException("FileWriterWrapper");
            _writer.Flush();
        }

        ~FileWriterWrapper()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {

                    if (_writer != null)
                    {
                        _writer.Flush();
                        _writer.Close();
                        _writer = null;
                    }
                }
                _disposed = true;
                Console.WriteLine("[DISPOSE] FileWriterWrapper oslobodjen: " + _path);
            }
        }
    }
}