﻿using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using SecretJsonConfig;
using ZoomConnect.Web.Banner.Domain;
using ZoomConnect.Web.Models;

namespace ZoomConnect.Web.Banner
{
    public class StvsubjRepository : AbstractRepository<stvsubj>
    {
        public StvsubjRepository(BannerContext context, SecretConfigManager<ZoomOptions> options) : base(context, options) { }

        private const string _columns = "stvsubj_code as code, stvsubj_desc as description";

        public override bool TestConnection()
        {
            var sql = $"SELECT {_columns} FROM stvsubj WHERE rownum = 1";
            var rows = Context.Connection.Query<stvsubj>(sql).ToList();

            return rows.Count == 1;
        }

        public override List<stvsubj> GetAll()
        {
            var sql = $"SELECT {_columns} FROM stvsubj WHERE stvsubj_disp_web_ind = 'Y'";

            return Context
                .Connection
                .Query<stvsubj>(sql)
                .ToList();
        }
    }
}
