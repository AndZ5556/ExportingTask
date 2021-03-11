using ExportingTask.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ExportingTask.Controllers
{
    public class HomeController : Controller
    {
        private readonly IWebHostEnvironment environment;
        static Task<PhysicalFileResult> printTask;
        static Task<PhysicalFileResult> downloadTask;
        static Task<PhysicalFileResult> fileTask;

        public HomeController(IWebHostEnvironment environment)
        {
            this.environment = environment;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult BeginLoadingFile(string mode)
        {
            fileTask = new Task<PhysicalFileResult>(() =>
            {
                Thread.Sleep(3000);
                var type = "application/pdf";
                if (mode == "download")
                {
                    var path = Path.Combine(environment.ContentRootPath, "Files/doc.pdf");
                    return PhysicalFile(path, type, "doc.pdf");
                }
                if (mode == "print")
                {
                    var path = Path.Combine(environment.ContentRootPath, "Files/docPrint.pdf");
                    return PhysicalFile(path, type);
                }
                return null;
            });
            fileTask.Start();
            return Json("ok");
        }

        [HttpPost]
        public IActionResult GetFileTaskStatus()
        {
            var dict = new Dictionary<string, bool>
            {
                ["success"] = fileTask != null && fileTask.IsCompleted
            };
            return Json(dict);
        }       

        [HttpPost]
        public IActionResult GetFile()
        {
            return fileTask.Result;
        }

    }
}
