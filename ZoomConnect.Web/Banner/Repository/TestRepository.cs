using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using ZoomConnect.Web.Banner.Domain;

namespace ZoomConnect.Web.Banner.Repository
{
    public class TestRepository : AbstractRepository<dual>
    {
        public TestRepository(BannerContext context) : base(context, null)
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

        public override List<dual> GetAll()
        {
            var sql = $"select * from dual";
            return Context.Connection.Query<dual>(sql).ToList();
        }
    }
}
