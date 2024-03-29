﻿using Microsoft.Extensions.Logging;
using Oracle.ManagedDataAccess.Client;
using SecretJsonConfig;
using System;
using System.Data;
using ZoomConnect.Core.Config;

namespace ZoomConnect.Web.Banner
{
    /// <summary>
    /// Opens and closes connection to Banner for all queries in this scope
    /// </summary>
    public class BannerContext : IDisposable
    {
        private ZoomOptions _zoomOptions;
        private ILogger<BannerContext> _logger;
        private bool disposed = false;

        public IDbConnection Connection { get; set; }

        public BannerContext(SecretConfigManager<ZoomOptions> zoomOptions, ILogger<BannerContext> logger)
        {
            _zoomOptions = zoomOptions.GetValue().Result;
            _logger = logger;

            Connection = new OracleConnection(_zoomOptions.Banner.GetConnectionString());
            try
            {
                Connection.Open();
            }
            catch (Exception ex)
            {
                logger.LogError(ex.ToString());
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this.disposed)
            {
                // If disposing equals true, dispose all managed and unmanaged resources.
                if (disposing)
                {
                    // Dispose managed resources.
                    try
                    {
                        if (Connection.State != System.Data.ConnectionState.Closed)
                        {
                            Connection.Close();
                        }
                    }
                    finally
                    {
                        Connection.Dispose();
                    }
                }

                // Note disposing has been done.
                disposed = true;
            }
        }

        ~BannerContext()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
    }
}
