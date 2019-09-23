using System;
using System.Configuration;
using System.IO;
using System.Text;
using System.Transactions;
using FiiiPay.Foundation.Data;
using FiiiPay.Framework;
using ImageProcessor;
using File = FiiiPay.Foundation.Entities.File;

namespace FiiiPay.Foundation.Business
{
    public class FileComponent
    {
        private static readonly string _path = ConfigurationManager.AppSettings.Get("StorageFolder");

        /// <summary>
        /// Creates the specified file.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public File Create(File file, byte[] data)
        {
            var fileMd5 = GetMD5HashFromByte(data);
            
            var fileDAC = new FileDAC();
            var oldFile = fileDAC.SelectByMd5(fileMd5);

            var id = Guid.NewGuid();
            var subFolder = CreateSubFolder(file.FileType);
            var fullFolder = Path.Combine(_path, subFolder);
            var extName = Path.GetExtension(file.FileName)?.ToLower();
            var fullFilePath = Path.Combine(fullFolder, id + extName);

            if (!Directory.Exists(fullFolder))
            {
                Directory.CreateDirectory(fullFolder);
            }

            if (oldFile == null)
            {
                file.FilePath = Path.Combine(subFolder, id + extName);
                using (var stream = System.IO.File.Create(fullFilePath))
                {
                    stream.Write(data, 0, data.Length);
                    stream.Flush();
                    stream.Close();
                }
            }
            else
            {
                file.FilePath = oldFile.FilePath;
            }

            file.Id = id;
            file.MimeType = FileContentType.GetMimeType(extName);
            file.Md5 = fileMd5;
            fileDAC.Create(file);

            return file;
        }

        public Guid[] CreateWithThumbnail(File file, byte[] data)
        {
            var originId = Guid.NewGuid();
            var subFolder = CreateSubFolder(file.FileType);
            var fullFolder = Path.Combine(_path, subFolder);
            var extName = Path.GetExtension(file.FileName)?.ToLower();
            var originFilePath = Path.Combine(subFolder, originId + extName);
            var originfullFilePath = Path.Combine(fullFolder, originId + extName);


            if (!Directory.Exists(fullFolder))
            {
                Directory.CreateDirectory(fullFolder);
            }

            using (var stream = System.IO.File.Create(originfullFilePath))
            {
                stream.Write(data, 0, data.Length);
                stream.Flush();
                stream.Close();
            }

            var compressFile = Compress(data, file);

            var fileDAC = new FileDAC();

            file.Id = originId;
            file.FilePath = originFilePath;
            file.MimeType = FileContentType.GetMimeType(extName);
            file.Md5 = GetMD5HashFromByte(data);

            using (var scope = new TransactionScope(TransactionScopeOption.Required, new TimeSpan(0, 0, 1, 30)))
            {
                fileDAC.Create(file);
                fileDAC.Create(compressFile);

                scope.Complete();
            }

            return new Guid[] { file.Id, compressFile.Id };
        }

        public Guid CreateWithCompress(Guid fileId)
        {
            var file = new FileDAC().SelectById(fileId);

            var subFolder = CreateSubFolder(file.FileType);
            var fullFolder = Path.Combine(_path, subFolder);
            var extName = Path.GetExtension(file.FileName)?.ToLower();
            var filePath = Path.Combine(subFolder, file.Id + extName);
            var fullFilePath = Path.Combine(fullFolder, file.Id + extName);

            if (!Directory.Exists(fullFolder))
            {
                Directory.CreateDirectory(fullFolder);
            }

            using (var stream = System.IO.File.Open(fullFilePath, FileMode.Open))
            {
                var bytes = new byte[stream.Length];

                stream.Read(bytes, 0, bytes.Length);

                // 设置当前流的位置为流的开始 

                stream.Seek(0, SeekOrigin.Begin);
                var compressFile = Compress(bytes, file);

                new FileDAC().Create(compressFile);
                return compressFile.Id;
            }

        }

        /// <summary>
        /// Deletes the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        public void Delete(Guid id)
        {
            // Data access component declarations.
            var fileDAC = new FileDAC();

            var file = fileDAC.SelectById(id);

            var fullPath = Path.Combine(_path, file.FilePath);
            System.IO.File.Delete(fullPath);

            // Step 1 - Calling Delete on FileDAC.
            fileDAC.Delete(id);
        }

        /// <summary>
        /// Copy a file into the same folder
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public File CopyInFolder(Guid id)
        {
            File fileNew = null;
            var fileDAC = new FileDAC();
            var file = fileDAC.SelectById(id);
            if (file != null)
            {
                var toFileId = Guid.NewGuid();
                var fullPath = Path.Combine(_path, file.FilePath);
                var dirPath = Path.GetDirectoryName(file.FilePath);
                var extName = Path.GetExtension(file.FileName)?.ToLower();
                var newFileName = toFileId + extName;
                var newFullPath = Path.Combine(_path, dirPath ?? throw new InvalidOperationException(), newFileName);

                System.IO.File.Copy(fullPath, newFullPath);

                file.Id = toFileId;
                file.FilePath = Path.Combine(dirPath, newFileName);
                file.Timestamp = DateTime.Now;

                fileDAC.Create(file);
                fileNew = file;
                fileNew.Id = toFileId;
            }
            return fileNew;
        }

        /// <summary>
        /// Replace file by fileId
        /// </summary>
        /// <param name="sourceId"></param>
        /// <param name="targetId"></param>
        public void ReplaceById(Guid sourceId, Guid targetId)
        {
            var fileDAC = new FileDAC();
            var sourceFile = fileDAC.SelectById(sourceId);
            var targetFile = fileDAC.SelectById(targetId);
            var stargetFileName = Path.GetFileName(targetFile.FilePath)?.ToLower();
            var sourcePath = Path.Combine(_path, sourceFile.FilePath);
            var targetPath = Path.Combine(_path, targetFile.FilePath);
            var targetDir = Path.GetDirectoryName(targetPath);
            var copyPath = Path.Combine(targetDir ?? throw new InvalidOperationException(), stargetFileName ?? throw new InvalidOperationException());

            if (System.IO.File.Exists(targetPath))
                System.IO.File.Delete(targetPath);
            System.IO.File.Copy(sourcePath, copyPath);
        }

        /// <summary>
        /// Checks the file exists.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public bool CheckFileExists(Guid id)
        {
            // Data access component declarations.
            var fileDAC = new FileDAC();

            var file = fileDAC.SelectById(id);

            if (file != null)
            {
                var fullPath = Path.Combine(_path, file.FilePath);
                return System.IO.File.Exists(fullPath);
            }
            return false;
        }

        /// <summary>
        /// GetById business method. 
        /// </summary>
        /// <param name="id">A id value.</param>
        /// <returns>Returns a File object.</returns>
        public File GetById(Guid id)
        {
            // Data access component declarations.
            var fileDAC = new FileDAC();

            // Step 1 - Calling SelectById on FileDAC.
            var result = fileDAC.SelectById(id);
            return result;
        }

        public byte[] GetFileById(Guid id)
        {
            if (id == Guid.Empty) return null;

            var file = GetById(id);

            if (file != null)
            {
                var fullPath = Path.Combine(_path, file.FilePath);

                if (System.IO.File.Exists(fullPath))
                {
                    byte[] bytes;
                    using (var stream = System.IO.File.Open(fullPath, FileMode.Open))
                    {
                        bytes = new byte[stream.Length];
                        stream.Read(bytes, 0, bytes.Length);
                        stream.Close();
                    }

                    return bytes;
                }
            }

            return null;
        }

        public File GetByMd5(string md5)
        {
            return new FileDAC().SelectByMd5(md5);
        }

        public string CreateSubFolder(string fileType)
        {
            var type = fileType == "image" ? "img\\" : "doc\\";
            var path = type + "common";

            return path;
        }

        public string GetMD5HashFromFile(string fileName)
        {
            try
            {
                var file = new FileStream(fileName, FileMode.Open);
                System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
                var retVal = md5.ComputeHash(file);
                file.Close();

                var sb = new StringBuilder();
                foreach (var item in retVal)
                {
                    sb.Append(item.ToString("x2"));
                }

                return sb.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("GetMD5HashFromFile() fail,error:" + ex.Message);
            }
        }

        public File Compress(byte[] data, File file)
        {
            var id = Guid.NewGuid();
            var subFolder = CreateSubFolder(file.FileType);
            var fullFolder = Path.Combine(_path, subFolder);
            var extName = Path.GetExtension(file.FileName)?.ToLower();
            var filePath = Path.Combine(subFolder, id + extName);
            var fullFilePath = Path.Combine(fullFolder, id + extName);

            if (!Directory.Exists(fullFolder))
            {
                Directory.CreateDirectory(fullFolder);
            }

            var compressFile = new File
            {
                Id = id,
                FilePath = filePath,
                FileType = file.FileType,
                FileName = file.FileName,
                MimeType = FileContentType.GetMimeType(extName),
                Timestamp = DateTime.Now
            };

            using (var inStream = new MemoryStream(data))
            {
                using (var outStream = new MemoryStream())
                {
                    // Initialize the ImageFactory using the overload to preserve EXIF metadata.
                    using (var imageFactory = new ImageFactory(preserveExifData: true))
                    {
                        // Load, resize, set the format and quality and save an image.
                        imageFactory.Load(inStream)
                            .Quality(30)
                            .Save(outStream);
                    }
                    using (var fileStream = new FileStream(fullFilePath, FileMode.Create, FileAccess.Write))
                    {
                        outStream.CopyTo(fileStream);

                        var compressData = new byte[outStream.Length];
                        outStream.Read(compressData, 0, compressData.Length);
                        outStream.Seek(0, SeekOrigin.Begin);
                        compressFile.Md5 = GetMD5HashFromByte(compressData);
                        return compressFile;
                    }
                }
            }
        }


        public string GetMD5HashFromByte(byte[] file)
        {
            try
            {
                System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
                var retVal = md5.ComputeHash(file);

                var sb = new StringBuilder();
                foreach (var item in retVal)
                {
                    sb.Append(item.ToString("x2"));
                }

                return sb.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("GetMD5HashFromFile() fail,error:" + ex.Message);
            }
        }
    }
}
