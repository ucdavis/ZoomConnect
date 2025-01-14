﻿using System;
using System.Collections.Generic;
using System.Linq;
using CanvasClient;
using CanvasClient.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SecretJsonConfig;
using ZoomConnect.Core.Config;
using ZoomConnect.Web.Banner.Cache;
using ZoomConnect.Web.Banner.Domain;
using ZoomConnect.Web.Services;
using ZoomConnect.Web.Services.Canvas;
using ZoomConnect.Web.SetupRequirements;
using ZoomConnect.Web.ViewModels;

namespace ZoomConnect.Web.Controllers
{
    [Authorize("ConfiguredAdmins")]
    public class SetupController : Controller
    {
        private SecretConfigManager<ZoomOptions> _secretOptions;
        private CachedRepository<stvterm> _termRepository;
        private CachedRepository<stvsubj> _subjectRepository;
        private RequirementManager _requirementManager;
        private ILogger<HomeController> _logger;
        private const string _passwordPlaceholder = "*********";

        public SetupController(SecretConfigManager<ZoomOptions> secretOptions, CachedRepository<stvterm> termRepository,
            CachedRepository<stvsubj> subjectRepository,
            RequirementManager requirementManager, ILogger<HomeController> logger)
        {
            _secretOptions = secretOptions;
            _termRepository = termRepository;
            _subjectRepository = subjectRepository;
            _requirementManager = requirementManager;
            _logger = logger;
        }

        public IActionResult Index()
        {
            var options = _secretOptions.GetValue().Result;
            var viewModel = new BannerOptionsViewModel
            {
                Instance = options.Banner?.Instance,
                Username = options.Banner?.Username.Value,
                Password = String.IsNullOrEmpty(options.Banner?.Password.Value) ? "" : _passwordPlaceholder,

                CurrentTerm = options.CurrentTerm,
                CurrentSubject = options.CurrentSubject,

                DownloadDirectory = options.DownloadDirectory,
                ProfilePhotoDirectory = options.ProfilePhotoDirectory,
                ExtraProfEmails = options.ExtraProfEmails,

                TermStart = options.TermStart,
                TermEnd = options.TermEnd,

                ZoomAccountId = String.IsNullOrEmpty(options.ZoomApi?.AccountId.Value) ? "" : _passwordPlaceholder,
                ZoomClientId = String.IsNullOrEmpty(options.ZoomApi?.ClientId.Value) ? "" : _passwordPlaceholder,
                ZoomClientSecret = String.IsNullOrEmpty(options.ZoomApi?.ClientSecret.Value) ? "" : _passwordPlaceholder,

                ZoomRequireMeetingAuthentication = options.ZoomApi?.RequireMeetingAuthentication ?? false,
                ZoomAuthenticationOptionId = options.ZoomApi?.AuthenticationOptionId,
                ZoomAuthenticationDomains = options.ZoomApi?.AuthenticationDomains,
                ZoomAlternateHosts = options.ZoomApi?.AlternateHosts,

                UseCanvas = options.CanvasApi.UseCanvas,
                CanvasAccessToken = String.IsNullOrEmpty(options.CanvasApi.ApiAccessToken) ? "" : _passwordPlaceholder,
                CanvasAccountId = options.CanvasApi.SelectedAccount,

                SmtpHost = options.EmailOptions?.SmtpHost,
                SmtpUsername = options.EmailOptions?.Username,
                SmtpPassword = String.IsNullOrEmpty(options.EmailOptions?.Password.Value) ? "" : _passwordPlaceholder,
                ParticipantReportCcList = options.EmailOptions?.ParticipantReportCcList,

                // mediasite settings
                UseMediasite = options.MediasiteOptions?.UseMediasite ?? false,
                MediasiteEndpoint = options.MediasiteOptions?.Endpoint,
                MediasiteRootFolder = options.MediasiteOptions?.RootFolder,
                MediasiteUsername = options.MediasiteOptions?.Username,
                MediasitePlayerId = options.MediasiteOptions?.PlayerId,
                MediasiteTemplateId = options.MediasiteOptions?.TemplateId,
                MediasiteUploadDirectory = options.MediasiteOptions?.UploadDirectory,
                MediasiteCoursesDirectory = options.MediasiteOptions?.CoursesDirectory,
                MediasitePassword = String.IsNullOrEmpty(options.MediasiteOptions?.Password.Value) ? "" : _passwordPlaceholder,
                MediasiteApiKey = String.IsNullOrEmpty(options.MediasiteOptions?.ApiKey.Value) ? "" : _passwordPlaceholder,
                MediasiteReportToEmail = options.MediasiteOptions?.ReportToEmail,
                MediasiteReportReplyToEmail = options.MediasiteOptions?.ReportReplyToEmail,
            };

            CheckRequirements(viewModel);

            if (viewModel.FailedBannerRequirements.Count() == 0)
            {
                viewModel = PopulateSelectLists(viewModel);
            }
            else
            {
                viewModel.BannerSubjects = new List<stvsubj>();
                viewModel.BannerTerms = new List<stvterm>();
            }

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Index(BannerOptionsViewModel model, [FromServices] SizedCache sizedCache,
            [FromServices] CanvasApi canvasApi)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var options = _secretOptions.GetValue().Result;
            canvasApi.Options = options.CanvasApi.CreateCanvasOptions();

            options.Banner.Instance = model.Instance;
            options.Banner.Username = new SecretStruct(model.Username);
            if (model.Password != _passwordPlaceholder && !String.IsNullOrEmpty(model.Password))
            {
                options.Banner.Password = new SecretStruct(model.Password);
            }

            // process term changes - new start/end dates. otherwise allow preferred start/end.
            if (options.CurrentTerm != model.CurrentTerm)
            {
                options.CurrentTerm = model.CurrentTerm;

                // look up new start/end dates
                try
                {
                    var term = _termRepository.GetAll().FirstOrDefault(t => t.code == model.CurrentTerm);
                    options.TermStart = term?.start_date ?? DateTime.Now;
                    options.TermEnd = term?.end_date ?? DateTime.Now;
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error looking up Term repo in Setup POST. {ex.Message}");
                }
            }
            else
            {
                options.TermStart = model.TermStart;
                options.TermEnd = model.TermEnd;
            }

            // find term in Canvas and store its id in hidden field
            try
            {
                var bannerTerm = _termRepository.GetAll().FirstOrDefault(t => t.code == options.CurrentTerm);
                var canvasTerm = canvasApi.ListEnrollmentTerms().FirstOrDefault(t => t.MatchesBannerTermDesc(bannerTerm?.description));

                if (canvasTerm != null)
                {
                    options.CanvasApi.EnrollmentTerm = canvasTerm.id;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in Setup POST connecting to Banner or Canvas: {ex.Message}.");
            }

            options.DownloadDirectory = model.DownloadDirectory;
            options.ProfilePhotoDirectory = model.ProfilePhotoDirectory;
            options.ExtraProfEmails = model.ExtraProfEmails;

            options.CurrentSubject = model.CurrentSubject;

            if (model.ZoomAccountId != _passwordPlaceholder && !String.IsNullOrEmpty(model.ZoomAccountId))
            {
                options.ZoomApi.AccountId= new SecretStruct(model.ZoomAccountId);
            }

            if (model.ZoomClientId != _passwordPlaceholder && !String.IsNullOrEmpty(model.ZoomClientId))
            {
                options.ZoomApi.ClientId = new SecretStruct(model.ZoomClientId);
            }

            if (model.ZoomClientSecret != _passwordPlaceholder && !String.IsNullOrEmpty(model.ZoomClientSecret))
            {
                options.ZoomApi.ClientSecret = new SecretStruct(model.ZoomClientSecret);
            }

            options.ZoomApi.RequireMeetingAuthentication = model.ZoomRequireMeetingAuthentication;
            options.ZoomApi.AuthenticationOptionId = model.ZoomAuthenticationOptionId;
            options.ZoomApi.AuthenticationDomains = model.ZoomAuthenticationDomains;
            options.ZoomApi.AlternateHosts = model.ZoomAlternateHosts;

            options.CanvasApi.UseCanvas = model.UseCanvas;
            options.CanvasApi.SelectedAccount = model.CanvasAccountId;
            if (model.CanvasAccessToken != _passwordPlaceholder && !String.IsNullOrEmpty(model.CanvasAccessToken))
            {
                options.CanvasApi.ApiAccessToken = new SecretStruct(model.CanvasAccessToken);
            }

            options.EmailOptions.SmtpHost = model.SmtpHost;
            options.EmailOptions.Username = model.SmtpUsername;
            if (model.SmtpPassword != _passwordPlaceholder && !String.IsNullOrEmpty(model.SmtpPassword))
            {
                options.EmailOptions.Password = new SecretStruct(model.SmtpPassword);
            }
            options.EmailOptions.ParticipantReportCcList = model.ParticipantReportCcList;

            // mediasite settings
            options.MediasiteOptions.UseMediasite = model.UseMediasite;
            options.MediasiteOptions.Endpoint = model.MediasiteEndpoint;
            options.MediasiteOptions.RootFolder = model.MediasiteRootFolder;
            options.MediasiteOptions.Username = model.MediasiteUsername;
            options.MediasiteOptions.PlayerId = model.MediasitePlayerId;
            options.MediasiteOptions.TemplateId = model.MediasiteTemplateId;
            options.MediasiteOptions.UploadDirectory = model.MediasiteUploadDirectory;
            options.MediasiteOptions.ReportToEmail = model.MediasiteReportToEmail;
            options.MediasiteOptions.ReportReplyToEmail = model.MediasiteReportReplyToEmail;
            if (model.MediasitePassword != _passwordPlaceholder && !String.IsNullOrEmpty(model.MediasitePassword))
            {
                options.MediasiteOptions.Password = new SecretStruct(model.MediasitePassword);
            }
            if (model.MediasiteApiKey != _passwordPlaceholder && !string.IsNullOrEmpty(model.MediasiteApiKey))
            {
                options.MediasiteOptions.ApiKey = new SecretStruct(model.MediasiteApiKey);
            }

            _secretOptions.Save();

            // re-check requirements and redirect Home only if all pass
            if (CheckRequirements(model))
            {
                sizedCache.ResetCache();
                _logger.LogInformation("Dumping cache after settings update");

                return RedirectToAction("Index", "Home");
            }

            model = PopulateSelectLists(model);

            return View(model);
        }

        private BannerOptionsViewModel PopulateSelectLists(BannerOptionsViewModel model)
        {
            try
            {
                model.BannerSubjects = _subjectRepository.GetAll()
                    .OrderBy(s => s.code)
                    .ToList();

                model.BannerTerms = _termRepository.GetAll()
                    .Where(t => t.start_date.Subtract(DateTime.Now).TotalDays < 90 &&
                        DateTime.Now.Subtract(t.end_date).TotalDays < 20)
                    .OrderBy(t => t.code)
                    .ToList();
            }
            catch (Exception ex)
            {
                model.BannerSubjects = new List<stvsubj>();
                model.BannerTerms = new List<stvterm>();
            }

            return model;
        }

        public IActionResult CmdKey()
        {
            var options = _secretOptions.GetValue().Result;
            var cmdKeyOptions = options?.CmdKeyOptions ?? new CmdKeyOptions();

            return View(cmdKeyOptions);
        }

        [HttpPost]
        public IActionResult CmdKey(bool confirm, [FromServices] CmdKeyService cmdKeyService)
        {
            if (confirm)
            {
                _logger.LogInformation("Generating new CmdKey by request.");

                // generate and save new key
                var newKey = cmdKeyService.GenerateKey();
                var options = _secretOptions.GetValue().Result;
                var cmdKeyOptions = options?.CmdKeyOptions ?? new CmdKeyOptions();

                cmdKeyOptions.CmdKey = new SecretStruct(newKey);
                cmdKeyOptions.CreatedBy = User.Identity.Name;
                cmdKeyOptions.CreatedDate = DateTime.Now;

                options.CmdKeyOptions = cmdKeyOptions;

                _secretOptions.Save();

                TempData["Message"] = $"New CmdKey Created: '{cmdKeyOptions.CmdKey.Value}'";
            }

            return RedirectToAction("Index", "Home");
        }

        private bool CheckRequirements(BannerOptionsViewModel model)
        {
            var success = _requirementManager.CheckAllRequirements();

            model.FailedBannerRequirements = GetMissingRequirement(RequirementType.Banner);
            model.FailedZoomRequirements = GetMissingRequirement(RequirementType.Zoom);
            model.FailedCanvasRequirements = GetMissingRequirement(RequirementType.Canvas);
            model.FailedEmailRequirements = GetMissingRequirement(RequirementType.Email);
            model.FailedMediasiteRequirements = GetMissingRequirement(RequirementType.Mediasite);

            return success;
        }

        private List<ISetupRequirement> GetMissingRequirement(RequirementType requirementType)
        {
            return _requirementManager.MissingRequirements()
                .Where(r => r.Type == requirementType)
                .ToList();
        }
    }
}
