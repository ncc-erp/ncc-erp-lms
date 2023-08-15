using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Abp.Configuration;
using Abp.Runtime.Session;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using RMALMS.Configuration;
using RMALMS.SymLinker;
using RMALMS.Web;

namespace RMALMS.Helper
{
    public class UploadHelper : IUploadHelper
    {
        readonly IConfiguration _configuration;
        readonly ISettingManager _settingManager;
        readonly IAbpSession _abpSession;
        readonly IHostingEnvironment _environment;
        public UploadHelper(
            IConfiguration configuration,
            ISettingManager settingManager,
            IAbpSession abpSession,
            IHostingEnvironment environment)
        {
            _configuration = configuration;
            _settingManager = settingManager;
            _abpSession = abpSession;
            _environment = environment;
        }
        public async Task<IEnumerable<FileUploadInfo>> UploadFiles(IEnumerable<IFormFile> files, string subFolder, string prefixName = "")
        {
            var mediaShortFolder = _configuration["MediaShortFolder"];
            var defaultMedia = _configuration["DefaultMedia"];

            var settingMedia = await _settingManager.GetSettingValueForTenantAsync(AppSettingNames.CourceFolder, _abpSession.TenantId.Value);
            bool hasSettingMedia = !string.IsNullOrEmpty(settingMedia);
            var mediaFolder = hasSettingMedia ? mediaShortFolder : defaultMedia;

            var targetPath = GetMediaFolderPath(subFolder, false);
            List<FileUploadInfo> fileInfos = new List<FileUploadInfo>();
            foreach (var file in files)
            {
                var fileInfo = CreateFileInfo(file, targetPath, $"{mediaFolder}/{subFolder}", prefixName);
                if (fileInfo != null)
                    fileInfos.Add(fileInfo);
            }
            return fileInfos;
        }

        public async Task<FileUploadInfo> UploadFile(IFormFile file, string subFolder, string prefixName = "", bool isSCORM = false)
        {
            var mediaFolder = isSCORM ? await _settingManager.GetSettingValueForTenantAsync(AppSettingNames.SCORMCourceResourceFolder, _abpSession.TenantId.Value) 
                : await _settingManager.GetSettingValueForTenantAsync(AppSettingNames.CourceFolder, _abpSession.TenantId.Value);
            //var storageLocation = isSCORM ? AppSettingNames.SCORMCourceResourceFolder : AppSettingNames.CourceFolder;
            //var defaultMedia = _configuration["DefaultMedia"];
            //var settingMedia = "";
            //if (_abpSession.TenantId.HasValue)
            //{
            //    settingMedia = await _settingManager.GetSettingValueForTenantAsync(storageLocation, _abpSession.TenantId.Value);
            //}
            //else
            //{
            //    settingMedia = await _settingManager.GetSettingValueAsync(storageLocation);
            //}

            //bool hasSettingMedia = !string.IsNullOrEmpty(settingMedia);
            //var mediaFolder = hasSettingMedia ? mediaShortFolder : defaultMedia;

            var targetPath = GetMediaFolderPath(subFolder, false);
            return CreateFileInfo(file, targetPath, $"{mediaFolder}/{subFolder}", prefixName);
        }


        private FileUploadInfo CreateFileInfo(IFormFile file, string locaFullPath, string serverPath, string prefixName = "")
        {
            FileUploadInfo fileInfo = null;
            if (file.Length > 0)
            {
                fileInfo = new FileUploadInfo();
                var contentDisposition = ContentDispositionHeaderValue.Parse(file.ContentDisposition);
                string fileName = contentDisposition.FileName.Trim('"');
                if (!string.IsNullOrEmpty(prefixName))
                {
                    var fileextension = Path.GetExtension(getFileName(fileName));
                    fileName = string.Join("", prefixName, fileextension);
                }
                else
                {
                    fileName = getFileName(fileName);
                }
                string fullPath = Path.Combine(locaFullPath, fileName);
                fullPath = AppendFileNumberIfExists(fullPath, string.Empty);
                fileName = getFileName(fullPath);
                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }

                fileInfo.FileName = fileName;
                fileInfo.FilePath = fullPath;
                fileInfo.ServerPath = $"data/{GetTenantFolder()}/{serverPath}/{fileName}";
                fileInfo.FileSize = file.Length;
                fileInfo.MineType = file.ContentType;
            }
            return fileInfo;
        }

        public string GetMediaFolderPath(string subFolder, bool isSCORM = false)
        {
            var webroot = _environment.WebRootPath;
            var dataWRoot = Path.Combine(webroot, "data", GetTenantFolder());

            var mediaShortFolder = isSCORM ?  _settingManager.GetSettingValueForTenantAsync(AppSettingNames.SCORMCourceResourceFolder, _abpSession.TenantId.Value)
                :  _settingManager.GetSettingValueForTenantAsync(AppSettingNames.CourceFolder, _abpSession.TenantId.Value);
           
            var storagePath = Path.GetFullPath(Path.Combine(dataWRoot, mediaShortFolder.Result.ToString()));

            string targetPath = storagePath;
            if (!string.IsNullOrEmpty(subFolder))
            {
                targetPath = Path.Combine(storagePath, subFolder);
                if (!System.IO.Directory.Exists(targetPath))
                {
                    System.IO.Directory.CreateDirectory(targetPath);
                }
            }

            if (!System.IO.Directory.Exists(targetPath))
            {
                System.IO.Directory.CreateDirectory(targetPath);
            }
            return targetPath;
        }

        private string getFileName(string fullName)
        {
            return Path.GetFileName(fullName);
        }

        private void CreateSymLink(string name, string target, bool isFile = false)
        {
            if (!SymLinkCreator.GetSymLinkCreator().CreateSymLink(name, target, isFile))
            {
                throw new Exception("Could not create");
            }
        }

        private string AppendFileNumberIfExists(string file, string ext)
        {
            if (System.IO.File.Exists(file))
            {
                string folderPath = Path.GetDirectoryName(file);
                string fileName = Path.GetFileNameWithoutExtension(file);
                string extension = string.Empty;
                if (ext == string.Empty)
                {
                    extension = Path.GetExtension(file);
                }
                else
                {
                    extension = ext;
                }

                int fileNumber = 0;
                Regex r = new Regex(@"\(([0-9]+)\)$");
                Match m = r.Match(fileName);
                if (m.Success)
                {
                    string s = m.Groups[1].Captures[0].Value;
                    fileNumber = int.Parse(s);
                    fileName = fileName.Replace("(" + s + ")", "");
                }
                do
                {
                    fileNumber += 1;
                    file = Path.Combine(folderPath,
                                            String.Format("{0}({1}){2}",
                                                                      fileName,
                                                                      fileNumber,
                                                                      extension));
                }
                while (System.IO.File.Exists(file));
            }
            return file;
        }

        public string GetTenantFolder()
        {
            return $"{_abpSession.TenantId}";
        }

        private bool IsSymbolic(string path)
        {
            DirectoryInfo pathInfo = new DirectoryInfo(path);
            return pathInfo.Attributes.HasFlag(FileAttributes.ReparsePoint);

        }

        public async void DeleteFile(string path, string filename)
        {
            var mediaShortFolder = _configuration["MediaShortFolder"];
            var defaultMedia = _configuration["DefaultMedia"];
            var settingMedia = await _settingManager.GetSettingValueForTenantAsync(AppSettingNames.CourceFolder, _abpSession.TenantId.Value);
            bool hasSettingMedia = !string.IsNullOrEmpty(settingMedia);
            var mediaFolder = hasSettingMedia ? mediaShortFolder : defaultMedia;
            var targetPath = GetMediaFolderPath(path, false);
            string fullPath = Path.Combine(targetPath, filename);

            if (System.IO.File.Exists(fullPath))
            {
                System.IO.File.Delete(fullPath);
            }
        }


    }
}
