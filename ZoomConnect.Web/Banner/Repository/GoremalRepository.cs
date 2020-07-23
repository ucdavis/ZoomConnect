using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using SecretJsonConfig;
using ZoomConnect.Web.Banner.Domain;
using ZoomConnect.Web.Models;

namespace ZoomConnect.Web.Banner.Repository
{
    public class GoremalRepository : AbstractRepository<goremal>
    {
        public GoremalRepository(BannerContext context, SecretConfigManager<ZoomOptions> options) : base(context, options) { }

        private string _columns =
            "goremal_pidm as pidm, goremal_emal_code as emal_code, goremal_email_address as email_address, " +
            "goremal_status_ind as status_ind, goremal_preferred_ind as preferred_ind";

        public override bool TestConnection()
        {
            if (Context.Connection == null || Context.Connection.State != ConnectionState.Open)
            {
                return false;
            }

            var sql = $"select {_columns} from goremal where rownum = 1";
            var rows = Context.Connection.Query<goremal>(sql).ToList();

            return rows.Count == 1;
        }

        public override List<goremal> GetAll()
        {
            var sql =
                $"SELECT DISTINCT {_columns} " +
                "FROM goremal " +
                "  INNER JOIN sirasgn ON goremal_pidm = sirasgn_pidm " +
                "  INNER JOIN ssbsect ON sirasgn_term_code = ssbsect_term_code AND sirasgn_crn = ssbsect_crn " +
                "WHERE sirasgn_term_code = :term " +
                "  AND ssbsect_subj_code = :subj " +
                "  AND goremal_status_ind = 'A'";

            return Context
                .Connection
                .Query<goremal>(sql, new
                {
                    term = new DbString { Value = Options.CurrentTerm },
                    subj = new DbString { Value = Options.CurrentSubject }
                })
                .ToList();
        }
    }
}
