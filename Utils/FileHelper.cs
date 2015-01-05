using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;

namespace Weather.Utils
{
    /// <summary>
    /// 文件操作类
    /// </summary>
    public class FileHelper
    {
        /// <summary>
        /// 文件目录是否存在
        /// </summary>
        /// <param name="dirName">目录名称</param>
        public async static Task<bool> IsExistFolderAsync(string dirName)
        {
            bool isExistFolder = false;
            try
            {
                IStorageFolder local = Windows.Storage.ApplicationData.Current.LocalFolder;

                IReadOnlyList<StorageFolder> folderList = await local.GetFoldersAsync();

                StorageFolder existFolder = folderList.FirstOrDefault(x => x.Name == dirName);

                if (existFolder != null)
                {
                    isExistFolder = true;
                }
            }
            catch (Exception)
            {
                isExistFolder = false;
            }

            return isExistFolder;
        }

        /// <summary>
        /// 文件是否存在
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public async static Task<bool> IsExistFileAsync(string filePath)
        {
            bool isExistFile = false;
            try
            {
                string fileFolder = filePath.Split('\\')[0];
                string fileName = filePath.Split('\\')[1];

                bool isExistFolder = await IsExistFolderAsync(fileFolder);

                if (isExistFolder)
                {
                    IStorageFolder local = Windows.Storage.ApplicationData.Current.LocalFolder;
                    StorageFolder folder = await local.GetFolderAsync(fileFolder);
                    IReadOnlyList<StorageFile> fileList = await folder.GetFilesAsync();
                    StorageFile existFile = fileList.FirstOrDefault(x => x.Name == fileName);

                    if (existFile != null)
                    {
                        isExistFile = true;
                    }
                    else
                    {
                        isExistFile = false;
                    }
                }
            }
            catch (Exception)
            {
                isExistFile = false;
            }

            return isExistFile;
        }

        /// <summary>
        /// 创建文件目录
        /// </summary>
        /// <param name="dirName">目录名称</param>
        public async static Task<bool> CreateDirectoryAsync(string dirName)
        {
            bool isCreateDir = false;
            try
            {
                //当前应用程序包位置
                IStorageFolder local = Windows.Storage.ApplicationData.Current.LocalFolder;
                StorageFolder sampleFile = await local.CreateFolderAsync(dirName, CreationCollisionOption.OpenIfExists);
                isCreateDir = true;
            }
            catch (Exception)
            {
                isCreateDir = false;
            }

            return isCreateDir;
        }

        /// <summary>
        /// 创建文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public async static Task<bool> CreateFileAsync(string filePath)
        {
            bool isCreateFile = false;
            try
            {
                //当前应用程序包位置
                IStorageFolder local = Windows.Storage.ApplicationData.Current.LocalFolder;
                StorageFile storageFile = await local.CreateFileAsync(filePath, CreationCollisionOption.ReplaceExisting);
                isCreateFile = true;
            }
            catch (Exception)
            {
                isCreateFile = false;
            }

            return isCreateFile;
        }


        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static async Task DeleteFileAsync(string filePath)
        {
            var x = await IsExistFileAsync(filePath);
            if (x)
            {
                IStorageFile file = await GetFileAccess(filePath);
                await file.DeleteAsync();
            }
        }


        /// <summary>
        /// 创建并写入文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public async static Task<bool> CreateAndWriteFileAsync(string filePath, string content)
        {
            bool isCreateFile = false;
            try
            {
                //当前应用程序包位置
                IStorageFolder local = Windows.Storage.ApplicationData.Current.LocalFolder;
                StorageFile storageFile = await local.CreateFileAsync(filePath, CreationCollisionOption.ReplaceExisting);
                var buffer = Windows.Security.Cryptography.CryptographicBuffer.ConvertStringToBinary(
    content, Windows.Security.Cryptography.BinaryStringEncoding.Utf8);
                await Windows.Storage.FileIO.WriteBufferAsync(storageFile, buffer);
                isCreateFile = true;
            }
            catch (Exception)
            {
                isCreateFile = false;
            }

            return isCreateFile;
        }



        /// <summary>
        /// 读取文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static async Task<string> ReadTxtFileByInstalledLocationAsync(string filePath)
        {
            string text = null;
            IStorageFolder local = Windows.ApplicationModel.Package.Current.InstalledLocation;
            IStorageFile storageFile = await local.GetFileAsync(filePath).AsTask().ConfigureAwait(false);
            var buffer = await Windows.Storage.FileIO.ReadBufferAsync(storageFile).AsTask().ConfigureAwait(false);
            using (DataReader dataReader = Windows.Storage.Streams.DataReader.FromBuffer(buffer))
            {
                text = dataReader.ReadString(buffer.Length);
            }
            return text;
        }

        /// <summary>
        /// 读取文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static async Task<string> ReadTxtFileAsync(string filePath)
        {
            try
            {
                string text = null;
                IStorageFolder local = Windows.Storage.ApplicationData.Current.LocalFolder;
                IStorageFile storageFile = await local.GetFileAsync(filePath).AsTask().ConfigureAwait(false);
                var buffer = await Windows.Storage.FileIO.ReadBufferAsync(storageFile).AsTask().ConfigureAwait(false);
                using (DataReader dataReader = Windows.Storage.Streams.DataReader.FromBuffer(buffer))
                {
                    text = dataReader.ReadString(buffer.Length);
                }
                return text;
            }
            catch (Exception)
            {
                
                throw;
            }
         
        }


        /// <summary>
        /// 获取文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public async static Task<StorageFile> GetFileAccess(string filePath)
        {
            var storageFolder = ApplicationData.Current.LocalFolder;

            StorageFile file = null;

            try
            {
                file = await storageFolder.GetFileAsync(filePath);
            }
            catch (Exception)
            {

            }

            if (file == null)
            {
                file = await storageFolder.CreateFileAsync(filePath, CreationCollisionOption.ReplaceExisting);
            }

            return file;
        }
    }


}
