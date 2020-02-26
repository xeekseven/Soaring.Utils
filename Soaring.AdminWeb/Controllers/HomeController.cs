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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Soaring.AdminWeb.Controllers {
    [Authorize]
    public class HomeController : Controller {
        private XomlTaskService _xomlService;
        public HomeController (XomlTaskService xomlService) {
            this._xomlService = xomlService;
        }
        public IActionResult Index () {
            return View ();
        }
        public async Task<IActionResult> TaskOverview () {
            ViewData["CurrentMenu"] = MenuType.TaskOverview.ToString ();
            var list = await this._xomlService.GetAll<XomlTaskModel> ();

            return View (list);
        }
        public IActionResult TaskUpload () {
            ViewData["CurrentMenu"] = MenuType.TaskUpload.ToString ();
            return View ();
        }

        [HttpPost ("/home/upload")]
        public async Task<IActionResult> UploadFiles ([FromForm] XomlTaskViewModel model) {
            if (ModelState.IsValid) {
                foreach (var file in model.TaskFiles) {
                    var taskId = Guid.NewGuid ().ToString ();
                    var path = Path.Combine (@"TaskFile",  taskId + "-" + file.FileName);
                    using (var stream = System.IO.File.Create (path)) {
                        //保存到本地
                        await file.CopyToAsync (stream);
                    }
                    
                    var xomlTask = new XomlTaskModel () {
                        XomlTaskId = taskId,
                        XomlTaskName = model.TaskName,
                        XomlTaskPath = Path.Combine (AppDomain.CurrentDomain.BaseDirectory,path),
                        CreateTime = DateTime.Now
                    };
                    await this._xomlService.InsertOne (xomlTask);
                }
                ViewData["OperationResult"] = "上传文件，操作成功！";
            } else {
                ViewData["OperationResult"] = "上传文件失败！";
            }
            return View ("~/Views/Partial/OperationResult.cshtml");
        }
        public IActionResult TaskSetting () {
            ViewData["CurrentMenu"] = MenuType.TaskSetting.ToString ();
            return View ();
        }
        public IActionResult CollectOverview () {
            ViewData["CurrentMenu"] = MenuType.CollectOverview.ToString ();
            return View ();
        }
        [HttpPost("/home/deleteXomlTask")]
        public async Task<IActionResult> DeleteXomlTask(string taskId){
            await this._xomlService.Delete(taskId);
            ViewData["OperationResult"] = "删除文件成功！";
            return View ("~/Views/Partial/OperationResult.cshtml");
        }
        public IActionResult Privacy () {
            return View ();
        }

        [ResponseCache (Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error () {
            return View (new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}