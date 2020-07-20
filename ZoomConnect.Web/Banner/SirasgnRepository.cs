using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using SecretJsonConfig;
using ZoomConnect.Web.Banner.Domain;
using ZoomConnect.Web.Models;

namespace ZoomConnect.Web.Banner
{
    public class SirasgnRepository : AbstractRepository<sirasgn>
    {
        public SirasgnRepository(BannerContext context, SecretConfigManager<ZoomOptions> options) : base(context, options) { }

        private const string _columns = "sirasgn_term_code as term_code, sirasgn_crn as crn, sirasgn_pidm as pidm, sirasgn_primary_ind as primary_ind";

        public override bool TestConnection()
        {
            var sql = $"SELECT {_columns} FROM sirasgn WHERE rownum = 1";
            var rows = Context.Connection.Query<sirasgn>(sql).ToList();

            return rows.Count == 1;
        }

        public override List<sirasgn> GetAll()
        {
            var sql =
                $"SELECT DISTINCT {_columns} " +
                "FROM sirasgn " +
                "INNER JOIN ssbsect ON sirasgn_term_code = ssbsect_term_code AND sirasgn_crn = ssbsect_crn " +
                "WHERE SIRASGN_TERM_CODE = :term " +
                "  AND ssbsect_subj_code = :subj";

            return Context
                .Connection
                .Query<sirasgn>(sql, new
                {
                    term = new DbString { Value = Options.CurrentTerm },
                    subj = new DbString { Value = Options.CurrentSubject }
                })
                .ToList();
        }
    }
}
