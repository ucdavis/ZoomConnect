using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using ZoomConnect.Web.Banner.Domain;

namespace ZoomConnect.Web.Banner
{
    public class StvtermRepository : AbstractRepository
    {
        public StvtermRepository(BannerContext context) : base(context) { }

        private const string _columns = "stvterm_code, stvterm_desc, stvterm_start_date, stvterm_end_date";

        public override bool TestConnection()
        {
            var sql = $"select {_columns} from stvterm where rownum = 1";
            var rows = Context.Connection.Query<stvterm>(sql).ToList();

            return rows.Count == 1;
        }

        public List<stvterm> ReadAll()
        {
            var sql = $"select {_columns} from stvterm order by 1";
            return Context.Connection.Query<stvterm>(sql).ToList();
        }
    }
}
