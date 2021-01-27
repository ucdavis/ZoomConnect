using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using SecretJsonConfig;
using ZoomConnect.Core.Config;
using ZoomConnect.Web.Banner.Domain;

namespace ZoomConnect.Web.Banner.Repository
{
    public class SpridenRepository : AbstractRepository<spriden>
    {
        public SpridenRepository(BannerContext context, SecretConfigManager<ZoomOptions> options) : base(context, options) { }

        private const string _columns = "spriden_pidm as pidm, spriden_id as id, spriden_last_name as last_name, spriden_first_name as first_name";

        public override bool TestConnection()
        {
            if (Context.Connection == null || Context.Connection.State != ConnectionState.Open)
            {
                return false;
            }

            var sql = $"SELECT {_columns} FROM spriden WHERE rownum = 1";
            var rows = Context.Connection.Query<spriden>(sql).ToList();

            return rows.Count == 1;
        }

        public override List<spriden> GetAll()
        {
            var sql =
                $"SELECT DISTINCT {_columns} " +
                "FROM spriden " +
                "  INNER JOIN sirasgn ON spriden_pidm = sirasgn_pidm " +
                "  INNER JOIN ssbsect ON sirasgn_term_code = ssbsect_term_code " +
                "                    AND sirasgn_crn = ssbsect_crn " +
                "WHERE spriden_change_ind is null " +
                "  AND sirasgn_term_code = :term " +
                "  AND ssbsect_subj_code = :subj";

            return Context
                .Connection
                .Query<spriden>(sql, new
                {
                    term = new DbString { Value = Options.CurrentTerm },
                    subj = new DbString { Value = Options.CurrentSubject }
                })
                .ToList();
        }

        public spriden GetOneStudent(decimal pidm)
        {
            var sql =
                $"SELECT DISTINCT spriden_pidm as pidm, spriden_id as id, spriden_last_name as last_name, " +
                "        COALESCE(spbpers_pref_first_name, spriden_first_name) as first_name " +
                "FROM spriden " +
                "  INNER JOIN spbpers ON spriden_pidm = spbpers_pidm " +
                "WHERE spriden_change_ind is null " +
                "  AND spriden_pidm = :pidm ";

            return Context
                .Connection
                .Query<spriden>(sql, new { pidm })
                .FirstOrDefault();
        }
    }
}
