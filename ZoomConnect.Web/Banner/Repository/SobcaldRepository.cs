using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using SecretJsonConfig;
using ZoomConnect.Web.Banner.Domain;
using ZoomConnect.Core.Config;

namespace ZoomConnect.Web.Banner.Repository
{
    public class SobcaldRepository : AbstractRepository<sobcald>
    {
        public SobcaldRepository(BannerContext context, SecretConfigManager<ZoomOptions> options) : base(context, options) { }

        private const string _columns = "sirasgn_term_code as term_code, sirasgn_crn as crn, sirasgn_pidm as pidm, sirasgn_primary_ind as primary_ind";

        public override bool TestConnection()
        {
            if (Context.Connection == null || Context.Connection.State != ConnectionState.Open)
            {
                return false;
            }

            var sql = $"SELECT {_columns} FROM sirasgn WHERE rownum = 1";
            var rows = Context.Connection.Query<sobcald>(sql).ToList();

            return rows.Count == 1;
        }

        public override List<sobcald> GetAll()
        {
            var sql =
                $"SELECT sobcald_date as \"date\", sobcald_dayt_code as dayt_code " +
                "FROM sobcald " +
                "  INNER JOIN stvterm ON stvterm_start_date <= sobcald_date AND stvterm_end_date >= sobcald_date " +
                "WHERE stvterm_code = :term " +
                "  AND sobcald_date >= :termStart " +
                "  AND sobcald_date <= :termEnd";

            return Context
                .Connection
                .Query<sobcald>(sql, new
                {
                    term = new DbString { Value = Options.CurrentTerm },
                    termStart = Options.TermStart.Date,
                    termEnd = Options.TermEnd.Date
                })
                .ToList();
        }
    }
}
