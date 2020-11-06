using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using ZoomConnect.Web.Banner.Domain;
using ZoomConnect.Web.SetupRequirements;

namespace ZoomConnect.Web.ViewModels
{
    public class BannerOptionsViewModel
    {
        public BannerOptionsViewModel()
        {
            FailedBannerRequirements = new List<ISetupRequirement>();
            FailedZoomRequirements = new List<ISetupRequirement>();
            FailedCanvasRequirements = new List<ISetupRequirement>();
            FailedEmailRequirements = new List<ISetupRequirement>();
            FailedMediasiteRequirements = new List<ISetupRequirement>();
        }

        public string Instance { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public List<ISetupRequirement> FailedBannerRequirements { get; set; }

        public string CurrentTerm { get; set; }
        public string CurrentSubject { get; set; }

        public DateTime TermStart { get; set; }
        public DateTime TermEnd { get; set; }

        public string DownloadDirectory { get; set; }

        public string ExtraProfEmails { get; set; }

        public string ZoomApiKey { get; set; }
        public string ZoomApiSecret { get; set; }

        public bool ZoomRequireMeetingAuthentication { get; set; }
        public string ZoomAuthenticationOptionId { get; set; }
        public string ZoomAuthenticationDomains { get; set; }
        public string ZoomAlternateHosts { get; set; }
        public string ProfilePhotoDirectory { get; set; }
        public List<ISetupRequirement> FailedZoomRequirements { get; set; }

        public bool UseCanvas { get; set; }
        public string CanvasAccessToken { get; set; }
        public int CanvasAccountId { get; set; }
        public List<ISetupRequirement> FailedCanvasRequirements { get; set; }

        public string SmtpHost { get; set; }
        public string SmtpUsername { get; set; }
        public string SmtpPassword { get; set; }
        public string ParticipantReportCcList { get; set; }
        public List<ISetupRequirement> FailedEmailRequirements { get; set; }

        public bool UseMediasite { get; set; }
        public string MediasiteEndpoint { get; set; }
        public string MediasiteRootFolder { get; set; }
        public string MediasiteUsername { get; set; }
        public string MediasitePassword { get; set; }
        public string MediasiteApiKey { get; set; }

        public string MediasitePlayerId { get; set; }
        public string MediasiteTemplateId { get; set; }

        public string MediasiteUploadDirectory { get; set; }
        public string MediasiteReportToEmail { get; set; }
        public string MediasiteReportReplyToEmail { get; set; }
        public List<ISetupRequirement> FailedMediasiteRequirements { get; set; }

        // lists for dropdown population
        public List<stvsubj> BannerSubjects { get; set; }
        public List<stvterm> BannerTerms { get; set; }

    }
}
