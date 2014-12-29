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
        public async static Task<bool> IsExistFolder(string dirName)
        {
            bool isExistFolder = false;
            try
            {
                //当前应用程序包位置
                IStorageFolder local = Windows.ApplicationModel.Package.Current.InstalledLocation;

                StorageFolder storageFolder = await local.GetFolderAsync(dirName).AsTask().ConfigureAwait(false);
                if (storageFolder != null)
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
        /// <param name="dirName">目录名称，目录为null时，搜索根目录是否存在文件</param>
        /// <param name="fileName">文件名称</param>
        /// <returns></returns>
        public async static Task<bool> IsExistFile(string filePath)
        {
            bool isExistFile = false;
            try
            {
                //当前应用程序包位置
                IStorageFolder local = Windows.ApplicationModel.Package.Current.InstalledLocation;
                IStorageFile storageFile = await local.GetFileAsync(filePath).AsTask().ConfigureAwait(false);
                if (storageFile != null)
                {
                    isExistFile = true;
                }
                else
                {
                    isExistFile = false;
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
        public async static Task<bool> CreateDirectory(string dirName)
        {
            bool isCreateDir = false;
            try
            {
                //当前应用程序包位置
                IStorageFolder local = Windows.ApplicationModel.Package.Current.InstalledLocation;
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
        /// <param name="dirname">目录名称</param>
        /// <param name="filename">文件名称</param>
        /// <param name="getDataStream">文件内容</param>
        /// <returns>是否创建</returns>
        public async static Task<bool> CreateFileForFolderAsync(string dirname, string filename, string content)
        {
            bool isCreateFile = false;
            try
            {
                //当前应用程序包位置
                IStorageFolder local = Windows.ApplicationModel.Package.Current.InstalledLocation;
                string filePath = dirname + "\\" + filename;
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
        /// 删除文件
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static async Task DeleteFile(string fileName)
        {
            var file = await GetFileAccess(fileName).ConfigureAwait(false);
            if (file != null)
            {
                await file.DeleteAsync();
            }
        }

        /// <summary>
        /// 读取Txt文件
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="fileFolder"></param>
        /// <returns></returns>
        public static async Task<string> ReadTxtFile(string fileName, string fileFolder)
        {
            string text = null;
            IStorageFolder local = Windows.ApplicationModel.Package.Current.InstalledLocation;
            string filePath = fileFolder + "\\" + fileName;
            IStorageFile storageFile = await local.GetFileAsync(filePath).AsTask().ConfigureAwait(false);
            var buffer = await Windows.Storage.FileIO.ReadBufferAsync(storageFile).AsTask().ConfigureAwait(false);
            using (DataReader dataReader = Windows.Storage.Streams.DataReader.FromBuffer(buffer))
            {
              text= dataReader.ReadString(buffer.Length);
            }
            return text;
        }

        public async static Task<IInputStream> GetOpenFileSequentialStream(string fileName)
        {
            var file = await GetFileAccess(fileName);

            return await file.OpenSequentialReadAsync();
        }

        public async static Task<IInputStream> GetOpenFileRandomAccesStream(string fileName)
        {
            var data = await GetFileRandomAccessStream(fileName, FileAccessMode.Read);

            return data.GetInputStreamAt(0);
        }

        public async static Task<IOutputStream> GetSaveFileStream(string fileName)
        {
            var data = await GetFileRandomAccessStream(fileName, FileAccessMode.ReadWrite);

            return data.GetOutputStreamAt(0);
        }

        public async static Task<IRandomAccessStream> GetFileRandomAccessStream(string fileName, FileAccessMode accessMode)
        {
            var file = await GetFileAccess(fileName);

            return await file.OpenAsync(accessMode);
        }

        public async static Task<StorageFile> GetFileAccess(string fileName)
        {
            var storageFolder = ApplicationData.Current.LocalFolder;

            StorageFile file = null;

            try
            {
                file = await storageFolder.GetFileAsync(fileName);
            }
            catch (Exception)
            {

            }

            if (file == null)
            {
                file = await storageFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
            }

            return file;
        }
    }


}
