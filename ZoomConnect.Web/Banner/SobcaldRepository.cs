using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using SecretJsonConfig;
using ZoomConnect.Web.Banner.Domain;
using ZoomConnect.Web.Models;

namespace ZoomConnect.Web.Banner
{
    public class SobcaldRepository : AbstractRepository<sobcald>
    {
        public SobcaldRepository(BannerContext context, SecretConfigManager<ZoomOptions> options) : base(context, options) { }

        private const string _columns = "sirasgn_term_code as term_code, sirasgn_crn as crn, sirasgn_pidm as pidm, sirasgn_primary_ind as primary_ind";

        public override bool TestConnection()
        {
            var sql = $"SELECT {_columns} FROM sirasgn WHERE rownum = 1";
            var rows = Context.Connection.Query<sobcald>(sql).ToList();

            return rows.Count == 1;
        }

        public override List<sobcald> GetAll()
        {
            var sql =
                $"SELECT sobcald_date, sobcald_dayt_code " +
                "FROM sobcald " +
                "  INNER JOIN stvterm ON stvterm_start_date <= sobcald_date AND stvterm_end_date >= sobcald_date " +
                "WHERE stvterm_code = :term";

            return Context
                .Connection
                .Query<sobcald>(sql, new
                {
                    term = new DbString { Value = Options.CurrentTerm },
                    subj = new DbString { Value = Options.CurrentSubject }
                })
                .ToList();
        }
    }
}
