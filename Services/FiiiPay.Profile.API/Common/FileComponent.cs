using System;
using System.Configuration;
using System.IO;
using FiiiPay.Framework;
using FiiiPay.Profile.Data;

namespace FiiiPay.Profile.API.Common
{
    internal class FileComponent
    {
        private readonly string _path = ConfigurationManager.AppSettings.Get("StorageFolder");

        /// <summary>
        /// Creates the specified file.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public Entities.File Create(Entities.File file, byte[] data)
        {
            var id = Guid.NewGuid();
            var subFolder = (file.FileType.ToLower() == "image" ? "img/" : "doc/") + (file.AccountId.HasValue ? file.AccountId.ToString() : "common");
            var fullFolder = Path.Combine(_path, subFolder);
            var extName = Path.GetExtension(file.FileName)?.ToLower();
            var filePath = Path.Combine(subFolder, id + extName);
            var fullFilePath = Path.Combine(fullFolder, id + extName);

            if (!Directory.Exists(fullFolder))
            {
                Directory.CreateDirectory(fullFolder);
            }

            using (var stream = File.Create(fullFilePath))
            {
                stream.Write(data, 0, data.Length);
                stream.Flush();
                stream.Close();
            }

            // Data access component declarations.
            var fileDAC = new FileDAC();

            // Step 1 - Calling Create on FileDAC.
            file.Id = id;
            file.FilePath = filePath;
            file.MimeType = FileContentType.GetMimeType(extName);

            fileDAC.Create(file);
            return file;
        }

        public Entities.File GetByMd5(string md5)
        {
            var fileDAC = new FileDAC();

            return fileDAC.SelectByMd5(md5);
        }

        /// <summary>
        /// GetById business method. 
        /// </summary>
        /// <param name="id">A id value.</param>
        /// <returns>Returns a File object.</returns>
        public Entities.File GetById(Guid id)
        {
            // Data access component declarations.
            var fileDAC = new FileDAC();

            // Step 1 - Calling SelectById on FileDAC.
            var result = fileDAC.SelectById(id);
            return result;
        }
    }
}
