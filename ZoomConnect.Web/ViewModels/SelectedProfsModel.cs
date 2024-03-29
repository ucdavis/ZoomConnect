﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace ZoomConnect.Web.ViewModels
{
    public class SelectedProfsModel
    {
        public SelectedProfsModel()
        {
            Profs = new List<ProfViewModel>();
        }

        public int SelectedProfs => Profs.Where(p => p.IsSelected).Count();
        public int RemainingLicenses { get; set; }
        public string AlertClass => SelectedProfs > RemainingLicenses
            ? "alert-danger"
            : "alert-info";
        public string AlertMessage => SelectedProfs > RemainingLicenses
            ? $"You selected {SelectedProfs} profs but have only {RemainingLicenses} remaining."
            : $"This action will use {SelectedProfs} of your remaining {RemainingLicenses} licenses.";

        public List<ProfViewModel> Profs { get; set; }

        public int ConnectedCount => Profs.Count(p => p.ZoomStatus == ZoomUserStatus.Connected);
        public int PendingCount => Profs.Count(p => p.ZoomStatus == ZoomUserStatus.Pending);
        public int BasicCount => Profs.Count(p => p.ZoomStatus == ZoomUserStatus.Basic);
        public int MissingCount => Profs.Count(p => p.ZoomStatus == ZoomUserStatus.Missing);
    }
}
