using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

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

                StorageFolder storageFolder = await local.GetFolderAsync(dirName);
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
        public async static Task<bool> IsExistFile(string dirName, string fileName)
        {
            bool isExistFile = false;
            try
            {
                //当前应用程序包位置
                IStorageFolder local = Windows.ApplicationModel.Package.Current.InstalledLocation;
                if (!string.IsNullOrEmpty(dirName))
                {
                    StorageFolder storageFolder = await local.GetFolderAsync(dirName);

                    if (storageFolder != null)
                    {
                        StorageFile storageFile = await storageFolder.GetFileAsync(fileName);
                        if (storageFile != null)
                        {
                            isExistFile = true;
                        }
                    }
                }
                else
                {
                    StorageFile storageFile = await local.GetFileAsync(fileName);
                    if (storageFile != null)
                    {
                        isExistFile = true;
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
        /// 存在目录时创建txt文件
        /// </summary>
        /// <param name="dirname">目录名称</param>
        /// <param name="filename">文件名称</param>
        /// <param name="getDataStream">文件内容</param>
        /// <returns>是否创建</returns>
        public async static Task<bool> CreateFileForFolder(string dirname, string filename, string content)
        {
            bool isCreateFile = false;
            try
            {
                //当前应用程序包位置
                IStorageFolder local = Windows.ApplicationModel.Package.Current.InstalledLocation;
                string filePath = filename + ".txt";
                StorageFolder storageFolder = await local.GetFolderAsync(dirname);
                StorageFile storageFile = await storageFolder.CreateFileAsync(filePath, CreationCollisionOption.ReplaceExisting);
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
        /// 不存在目录时创建txt文件
        /// </summary>
        /// <param name="dirname">目录名称</param>
        /// <param name="filename">文件名称</param>
        /// <param name="getDataStream">文件内容</param>
        /// <returns>是否创建</returns>
        public async static Task<bool> CreateFileForNoFolder(string dirname, string filename, string content)
        {
            bool isCreateFile = false;
            try
            {
                //当前应用程序包位置
                IStorageFolder local = Windows.ApplicationModel.Package.Current.InstalledLocation;
                string filePath = filename + ".txt";
                StorageFolder storageFolder = await local.CreateFolderAsync(dirname);
                StorageFile storageFile = await storageFolder.CreateFileAsync(filePath, CreationCollisionOption.ReplaceExisting);
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
        /// 创建txt文件
        /// </summary>
        /// <param name="dirname">目录名称</param>
        /// <param name="filename">文件名称</param>
        /// <param name="getDataStream">文件内容</param>
        /// <returns>是否创建</returns>
        public async static Task<bool> CreateFile(string dirname, string filename, string content)
        {
            bool isCreateFile = false;
            try
            {
                //当前应用程序包位置
                IStorageFolder local = Windows.ApplicationModel.Package.Current.InstalledLocation;
                string filePath = filename + ".txt";
                StorageFolder storageFolder = await local.GetFolderAsync(dirname);
                if (storageFolder != null)
                {
                    StorageFile storageFile = await storageFolder.CreateFileAsync(filePath, CreationCollisionOption.ReplaceExisting);
                    var buffer = Windows.Security.Cryptography.CryptographicBuffer.ConvertStringToBinary(
        content, Windows.Security.Cryptography.BinaryStringEncoding.Utf8);
                    await Windows.Storage.FileIO.WriteBufferAsync(storageFile, buffer);
                }
                else
                {
                    StorageFolder s = await local.CreateFolderAsync(dirname);
                    StorageFile storageFile = await storageFolder.CreateFileAsync(filePath, CreationCollisionOption.ReplaceExisting);
                    var buffer = Windows.Security.Cryptography.CryptographicBuffer.ConvertStringToBinary(
        content, Windows.Security.Cryptography.BinaryStringEncoding.Utf8);
                    await Windows.Storage.FileIO.WriteBufferAsync(storageFile, buffer);

                }
                isCreateFile = true;
            }
            catch (Exception)
            {
                isCreateFile = false;
            }

            return isCreateFile;
        }



    }
}
