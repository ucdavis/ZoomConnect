using System;

namespace ZoomConnect.Web.Banner
{
    public class TestRepository : AbstractRepository
    {
        public TestRepository(BannerContext context) : base(context)
        {
        }

        public override bool TestConnection()
        {
            using (var command = Context.Connection.CreateCommand())
            {
                command.CommandText = "select * from dual";
                var scalar = command.ExecuteScalar();

                return "X" == (string)scalar;
            }
        }
    }
}
