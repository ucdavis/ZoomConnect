using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using SecretJsonConfig;
using ZoomConnect.Web.Banner.Domain;
using ZoomConnect.Web.Models;

namespace ZoomConnect.Web.Banner.Repository
{
    public class StvtermRepository : AbstractRepository<stvterm>
    {
        public StvtermRepository(BannerContext context, SecretConfigManager<ZoomOptions> options) : base(context, options) { }

        private const string _columns = "stvterm_code as code, stvterm_desc as description, stvterm_start_date as start_date, stvterm_end_date as end_date";

        public override bool TestConnection()
        {
            var sql = $"select {_columns} from stvterm where rownum = 1";
            var rows = Context.Connection.Query<stvterm>(sql).ToList();

            return rows.Count == 1;
        }

        public override List<stvterm> GetAll()
        {
            var sql = $"select {_columns} from stvterm order by 1";
            return Context.Connection.Query<stvterm>(sql).ToList();
        }
    }
}
