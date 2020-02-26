using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Soaring.AdminWeb.Models;
using Soaring.AdminWeb.Models.DatabaseModel;
using Soaring.AdminWeb.Models.Enums;
using Soaring.AdminWeb.Services;
using Soaring.AdminWeb.Wrappers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Soaring.AdminWeb.Controllers {
    public class DataApiController : Controller {
        public DataApiController(){

        }
        [HttpPost("uploadData")]
        public void UploadData(){

        }
    }
}