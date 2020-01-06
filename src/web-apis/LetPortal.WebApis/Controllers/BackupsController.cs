using LetPortal.Core.Logger;
using LetPortal.Portal.Repositories.Recoveries;
using LetPortal.Portal.Services.Recoveries;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LetPortal.PortalApis.Controllers
{
    [Route("api/backups")]
    [ApiController]
    public class BackupsController : ControllerBase
    {
        private readonly IBackupService _backupService;

        private readonly IBackupRepository _backupRepository;

        private readonly IServiceLogger<BackupsController> _logger;

        public BackupsController(
            IBackupService backupService,
            IBackupRepository backupRepository,
            IServiceLogger<BackupsController> logger)
        {
            _backupRepository = backupRepository;
            _backupService = backupService;
            _logger = logger;
        }
    }
}
