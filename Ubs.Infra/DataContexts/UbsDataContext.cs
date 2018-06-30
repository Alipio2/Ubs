using Microsoft.Win32.SafeHandles;
using System;
using System.Data.SqlClient;
using System.Runtime.InteropServices;
using Ubs.Shared.Utils;

namespace Ubs.Infra.DataContexts
{
    public class UbsDataContext : IDisposable
    {
        private bool _disposed = false;
        private readonly SafeHandle _handle = new SafeFileHandle(IntPtr.Zero, true);
        public SqlConnection Connection { get; set; }

        public UbsDataContext()
        {
            Connection = new SqlConnection(Settings.ConnectionString);
            Connection.Open();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
                _handle.Dispose();

            _disposed = true;
        }
    }
}
