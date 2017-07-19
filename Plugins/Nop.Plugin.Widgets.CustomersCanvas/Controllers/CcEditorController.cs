using Nop.Web.Framework.Controllers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web;
using System.Net.Http;
using System.Configuration;
using Nop.Plugin.Widgets.CustomersCanvas.Data;
using System.IO.Compression;
using Newtonsoft.Json;
using System.Web.Hosting;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using Nop.Core;
using Nop.Plugin.Widgets.CustomersCanvas.Infrastructure;
using Nop.Plugin.Widgets.CustomersCanvas.Services;
using Nop.Services.Logging;
using System.Net;

namespace Nop.Plugin.Widgets.CustomersCanvas.Controllers
{

    public static class ZipArchiveExtensions
    {
        public static void ExtractToDirectory(this ZipArchive archive, string destinationDirectoryName, bool overwrite)
        {
            if (!overwrite)
            {
                archive.ExtractToDirectory(destinationDirectoryName);
                return;
            }
            foreach (ZipArchiveEntry file in archive.Entries)
            {
                string completeFileName = Path.Combine(destinationDirectoryName, file.FullName);
                Directory.CreateDirectory(Path.GetDirectoryName(completeFileName));
                if (file.Name == "")
                {// Assuming Empty for Directory
                    Directory.CreateDirectory(Path.GetDirectoryName(completeFileName));
                    continue;
                }
                file.ExtractToFile(completeFileName, true);
            }
        }
    }

    public class CcEditorController : BasePluginController
    {
        private readonly ICcService _ccService;
        private readonly ILogger _logger;
        
        public CcEditorController(ICcService ccService, ILogger logger)
        {
            _ccService = ccService;
            _logger = logger;
        }
        [AdminAuthorize]
        public ActionResult Index()
        {
            var editorsList = _ccService.GetAllEditors();
            return View("~/Plugins/Widgets.CustomersCanvas/Views/CcEditor/Index.cshtml", editorsList);
        }

        #region editor

        [AdminAuthorize]
        [HttpPost]
        public JsonResult UploadEditor(bool isNew = false)
        {
            try
            {
                var file = System.Web.HttpContext.Current.Request.Files[0];
                var fileName = Path.GetFileName(file.FileName);
                var filePath = Path.Combine(CommonHelper.MapPath(PluginPaths.PluginFolder), PluginPaths.EditorsFolder, fileName);
                var folderName = filePath.Substring(0, filePath.LastIndexOf("."));
                Directory.CreateDirectory(folderName);
                file.SaveAs(filePath);

                ExtractArchive(filePath, isNew);
                return Json(new { status = "success" });
            }
            catch (Exception ex)
            {
                _logger.Error("UploadEditor exception", ex);
                return Json(new { status = "error", message = ex.Message });
            }
        }

        [HttpPost]
        [AdminAuthorize]
        public JsonResult DeleteEditor(string folderName)
        {
            try
            {
                var filePath = Path.Combine(CommonHelper.MapPath(PluginPaths.PluginFolder), PluginPaths.EditorsFolder, folderName);
                Directory.Delete(filePath, true);
                return Json(new { status = "success" });
            }
            catch (Exception ex)
            {
                _logger.Error("DeleteEditor exception", ex);
                return Json(new { status = "error", message = ex.Message });
            }
        }


        #endregion

        #region config
        [AdminAuthorize]
        [HttpPost]
        public JsonResult UploadConfig(string editorFolder, bool isNew = false)
        {
            try
            {
                var file = System.Web.HttpContext.Current.Request.Files[0];
                var fileName = Path.GetFileName(file.FileName);
                var filePath = Path.Combine(CommonHelper.MapPath(PluginPaths.PluginFolder), PluginPaths.EditorsFolder, editorFolder, PluginPaths.ConfigsFolder, fileName);

                if (System.IO.File.Exists(filePath))
                {
                    if (isNew)
                    {
                        System.IO.File.Delete(filePath);
                    }
                    else
                    {
                        return Json(new { status = "warning", message = "Config already exist. Replace?" });
                    }
                }

                file.SaveAs(filePath);
                return Json(new { status = "success" });
            }
            catch (Exception ex)
            {
                _logger.Error("UploadConfig exception", ex);
                return Json(new { status = "error", message = ex.Message });
            }
        }

        [HttpPost]
        [AdminAuthorize]
        public JsonResult DeleteConfig(string editorFolder, string fileName)
        {
            try
            {
                var filePath = Path.Combine(CommonHelper.MapPath(PluginPaths.PluginFolder), PluginPaths.EditorsFolder, editorFolder, PluginPaths.ConfigsFolder, fileName);
                System.IO.File.Delete(filePath);
                return Json(new { status = "success" });
            }
            catch (Exception ex)
            {
                _logger.Error("DeleteConfig exception", ex);
                return Json(new { status = "error", message = ex.Message });
            }
        }
        #endregion

        #region archive
        private void ExtractArchive(string path, bool isNew)
        {
            // TODO 
            // сделать выворачивание в темп папку, чтение джона, и переименование папки в нужное имя.
            try
            {
                var filename = Path.GetFileName(path);
                var folderName = filename.Split('.')[0];
                var folderPath = path.Replace(filename,folderName);

                using (var archive = ZipFile.Open(path, ZipArchiveMode.Update))
                {
                    archive.ExtractToDirectory(folderPath, true);
                }
                
                System.IO.File.Delete(path);
            }
            catch (Exception ex)
            {
                _logger.Error("ExtendArchive exception:", ex);
                throw;
            }
        }

        #endregion

    }
}