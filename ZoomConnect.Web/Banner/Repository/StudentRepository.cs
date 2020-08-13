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
    public class StudentRepository : AbstractRepository<spriden_student>
    {
        public StudentRepository(BannerContext context, SecretConfigManager<ZoomOptions> options) : base(context, options) { }

        private const string _columns = "spriden_pidm as pidm, spriden_id as id, spriden_last_name as last_name, spriden_first_name as first_name";

        public override bool TestConnection()
        {
            if (Context.Connection == null || Context.Connection.State != ConnectionState.Open)
            {
                return false;
            }

            var sql = $"SELECT spriden_pidm FROM spriden WHERE rownum = 1";
            var rows = Context.Connection.Query<spriden_student>(sql).ToList();

            return rows.Count == 1;
        }

        public override List<spriden_student> GetAll()
        {
            var sql =
                "SELECT DISTINCT spriden_pidm as pidm, spriden_id as id, spriden_last_name as last_name, " +
                "       COALESCE(spbpers_pref_first_name, spriden_first_name) as first_name, " +
                "       goremal_email_address as email, " +
                "       sgbstdn_degc_code_1 as degc, " +
                "       nvl(sgkclas.f_class_code(sgbstdn_pidm, sgbstdn_levl_code, sgbstdn_term_code_eff), 'XX') as classlevel " +
                "  FROM spriden " +
                "    INNER JOIN sgbstdn ON spriden_pidm = sgbstdn_pidm " +
                "    INNER JOIN sfrstcr ON sgbstdn_pidm = sfrstcr_pidm AND sgbstdn_term_code_eff = sfrstcr_term_code " +
                "    INNER JOIN ssbsect ON sfrstcr_term_code = ssbsect_term_code AND sfrstcr_crn = ssbsect_crn " +
                "    INNER JOIN spbpers ON spriden_pidm = spbpers_pidm " +
                "    LEFT OUTER JOIN goremal ON spriden_pidm = goremal_pidm AND GOREMAL_EMAL_CODE = 'UCD' " +
                "  WHERE spriden_change_ind is null " +
                "    AND ssbsect_subj_code = :subj " +
                "    AND sgbstdn_term_code_eff = :term " +
                "    AND sgbstdn_stst_code = 'AS'";

            return Context
                .Connection
                .Query<spriden_student>(sql, new
                {
                    term = new DbString { Value = Options.CurrentTerm },
                    subj = new DbString { Value = Options.CurrentSubject }
                })
                .ToList();
        }
    }
}
