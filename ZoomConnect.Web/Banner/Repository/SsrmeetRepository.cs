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
    public class SsrmeetRepository : AbstractRepository<ssrmeet>
    {
        public SsrmeetRepository(BannerContext context, SecretConfigManager<ZoomOptions> options) : base(context, options) { }

        private const string _columns =
            "ssrmeet_surrogate_id as surrogate_id, ssrmeet_term_code as term_code, ssrmeet_crn as crn, " +
            "ssrmeet_begin_time as begin_time, ssrmeet_end_time as end_time, " +
            "ssrmeet_bldg_code as bldg_code, ssrmeet_room_code as room_code, " +
            "ssrmeet_start_date as start_date, ssrmeet_end_date as end_date, " +
            "ssrmeet_catagory as catagory, ssrmeet_sun_day as sun_day, ssrmeet_mon_day as mon_day, ssrmeet_tue_day as tue_day, " +
            "ssrmeet_wed_day as wed_day, ssrmeet_thu_day as thu_day, ssrmeet_fri_day as fri_day, ssrmeet_sat_day as sat_day, " +
            "ssrmeet_schd_code as schd_code, ssrmeet_over_ride as over_ride, ssrmeet_meet_no as meet_no, ssrmeet_hrs_week as hrs_week";

        public override bool TestConnection()
        {
            if (Context.Connection == null || Context.Connection.State != ConnectionState.Open)
            {
                return false;
            }

            var sql = $"SELECT {_columns} FROM ssrmeet WHERE rownum = 1";
            var rows = Context.Connection.Query<ssrmeet>(sql).ToList();

            return rows.Count == 1;
        }

        public override List<ssrmeet> GetAll()
        {
            var sql =
                $"SELECT DISTINCT {_columns} " +
                "FROM ssrmeet " +
                "  INNER JOIN ssbsect ON ssrmeet_term_code = ssbsect_term_code AND ssrmeet_crn = ssbsect_crn " +
                "WHERE SSRMEET_TERM_CODE = :term " +
                "  AND ssbsect_subj_code = :subj " +
                "  AND ssrmeet_begin_time IS NOT NULL " +
                "  AND ssrmeet_end_time IS NOT NULL ";

            return Context
                .Connection
                .Query<ssrmeet>(sql, new
                {
                    term = new DbString { Value = Options.CurrentTerm },
                    subj = new DbString { Value = Options.CurrentSubject }
                })
                .ToList();
        }
    }
}
