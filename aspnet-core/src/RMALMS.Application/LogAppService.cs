using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RMALMS
{
    public class LogAppService : ApplicationBaseService
    {
        private IHostingEnvironment _hostingEnvironment;
        public LogAppService(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }
        public string DownloadLog()
        {
            //string root = Environment.CurrentDirectory;
            var root = AppDomain.CurrentDomain.BaseDirectory;
            var webRoot = _hostingEnvironment.WebRootPath;
            var pathLog = Path.Combine(root, @"App_Data\Logs\Logs.txt");
            Logger.Info("pathLog: " + pathLog);
            var pathWebLog = Path.Combine(webRoot, "logs");
            CopyLog(pathLog, pathWebLog);
            var fileName = Path.GetFileNameWithoutExtension(pathLog);
            return Path.Combine("logs", fileName);
        }

        private void CopyLog(string sourceFile, string toFolder)
        {
            if (!Directory.Exists(toFolder))
            {
                Directory.CreateDirectory(toFolder);
            }

            var toFolderFile = Path.Combine(toFolder, Path.GetFileName(sourceFile));

            System.IO.File.Copy(sourceFile, toFolderFile, true);
        }
    }
}
