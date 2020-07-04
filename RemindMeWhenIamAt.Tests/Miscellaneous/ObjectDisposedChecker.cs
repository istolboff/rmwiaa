using System;

namespace RemindMeWhenIamAt.Tests.Miscellaneous
{
    internal sealed class ObjectDisposedChecker
    {
        public ObjectDisposedChecker(string? objectName = null)
        {
            _objectName = objectName;
        }

        public void Check()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(_objectName);
            }
        }

        public bool Dispose()
        {
            if (_disposed)
            {
                return false;
            }

            _disposed = true;

            return true;
        }

        private readonly string? _objectName;
        private bool _disposed;
    }
}
