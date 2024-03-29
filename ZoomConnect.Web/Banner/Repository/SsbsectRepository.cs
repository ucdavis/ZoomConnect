﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using SecretJsonConfig;
using ZoomConnect.Web.Banner.Domain;
using ZoomConnect.Core.Config;

namespace ZoomConnect.Web.Banner.Repository
{
    public class SsbsectRepository : AbstractRepository<ssbsect>
    {
        public SsbsectRepository(BannerContext context, SecretConfigManager<ZoomOptions> options) : base(context, options) { }

        public override List<string> Tables => new List<string>() { "SSBSECT", "SCBCRSE" };

        public override bool TestConnection()
        {
            if (Context.Connection == null || Context.Connection.State != ConnectionState.Open)
            {
                return false;
            }

            var sql = $"select ssbsect_term_code, ssbsect_enrl from ssbsect where rownum = 1";
            var rows = Context.Connection.Query<stvterm>(sql).ToList();

            return rows.Count == 1;
        }

        public override List<ssbsect> GetAll()
        {
            var sql =
                $"SELECT ssbsect_term_code as term_code, ssbsect_crn as crn, ssbsect_subj_code as subj_code, " +
                "        ssbsect_crse_numb as crse_numb, ssbsect_seq_numb as seq_numb, " +
                "        COALESCE(ssbsect_crse_title, scbcrse_title) as crse_title, ssbsect_enrl as enrl " +
                "FROM ssbsect " +
                "  INNER JOIN scbcrse ON ssbsect_crse_numb = scbcrse_crse_numb " +
                "                    AND ssbsect_subj_code = scbcrse_subj_code " +
                "                    AND scbcrse_eff_term = ( " +
                "                        SELECT MAX(scbcrse_eff_term)  " +
                "                        FROM scbcrse " +
                "                        WHERE scbcrse_eff_term <= ssbsect_term_code " +
                "                          AND scbcrse_crse_numb = ssbsect_crse_numb " +
                "                          AND scbcrse_subj_code = ssbsect_subj_code " +
                "                    ) " +
                "WHERE ssbsect_term_code = :term " +
                "  AND ssbsect_subj_code = :subj ";

            return Context
                .Connection
                .Query<ssbsect>(sql, new
                {
                    term = new DbString { Value = Options.CurrentTerm },
                    subj = new DbString { Value = Options.CurrentSubject }
                })
                .ToList();
        }
    }
}
