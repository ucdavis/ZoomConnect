using System;

namespace ZoomConnect.Web.Banner
{
    public class BannerRepository : IRepository
    {
        private BannerConnection _connection;

        public BannerRepository(BannerConnection connection)
        {
            _connection = connection;
        }

        public string TestConnection()
        {
            using (var command = _connection.Connection.CreateCommand())
            {
                command.CommandText = "select * from dual";
                var scalar = command.ExecuteScalar();

                return (string)scalar;
            }
        }
    }
}
