using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;

namespace Weather.Utils
{
    public static class JsonSerializeHelper
    {
        #region 通用序列化方法
        /// <summary>
        /// 序列化Json
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target"></param>
        /// <returns></returns>
        public static string JsonSerialize<T>(T target)
        {

            DataContractJsonSerializer serializer = new DataContractJsonSerializer(target.GetType());

            using (MemoryStream stream = new MemoryStream())
            {
                serializer.WriteObject(stream, target);
                stream.Position = 0;
                using (StreamReader reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }

            }
        }



        /// <summary>
        /// 反序列化Json
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target"></param>
        /// <returns></returns>
        public static T JsonDeserialize<T>(string target) where T : class
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
            using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(target)))
            {
                return serializer.ReadObject(ms) as T;
            }
        }
        #endregion

        #region 文件化Json序列化/反序列化

        /// <summary>
        /// 文件化Json序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target"></param>
        /// <param name="fileName"></param>
        /// <param name="fileFolder"></param>
        public static async void JsonSerializeForFile<T>(T target, string fileName, string fileFolder)
        {
            try
            {
                //序列化
                string jsonContent = JsonSerialize<T>(target);
                //获取本地文件夹，目录文件夹
                IStorageFolder local = Windows.Storage.ApplicationData.Current.LocalFolder;
                IStorageFolder storageFolder = await local.GetFolderAsync(fileFolder);
                IStorageFile storageFile = await storageFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
                //Uri uri = new Uri("ms-appx:///" + fileFolder + "/" + fileName + "");
                //IStorageFile storageFile = await StorageFile.GetFileFromApplicationUriAsync(uri);
                await FileIO.WriteTextAsync(storageFile, jsonContent);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        /// <summary>
        /// 文件化Json反序列化
        /// </summary>
        /// <param name="type"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static async Task<T> JsonDeSerializeForFile<T>(string fileName, string fileFolder) where T : class
        {
            try
            {
                Uri uri = new Uri(@"ms-appx:///" + fileFolder + "/" + fileName + "");
                IStorageFile storageFile = await StorageFile.GetFileFromApplicationUriAsync(uri).AsTask().ConfigureAwait(false);
                //string filePath = fileFolder + "\\" + fileName;
                //IStorageFile storageFile = await FileHelper.GetFileAccess(filePath);

                //using (IRandomAccessStream readStream = await storageFile.OpenAsync(FileAccessMode.Read))
                //{
                //    using (DataReader dataReader=new DataReader(readStream))
                //    {
                //        await dataReader.LoadAsync(sizeof(Int32));
                //        Int32 stringSize = dataReader.ReadInt32();
                //        await dataReader.LoadAsync((UInt32)stringSize);
                //        string fileContent = dataReader.ReadString((uint)stringSize);
                //        return JsonDeserialize<T>(fileContent);
                //    }
                //}

                //using (var fs = await storageFile.OpenAsync(FileAccessMode.Read))
                //{
                //    using (var inStream = fs.GetInputStreamAt(0))
                //    {
                //        using (var reader = new DataReader(inStream))
                //        {
                //            await reader.LoadAsync((uint)fs.Size);
                //            string data = reader.ReadString((uint)fs.Size);
                //            reader.DetachStream();
                //            return JsonDeserialize<T>(data); ;
                //        }
                //    }
                //}


                //IBuffer buffer = await FileIO.ReadBufferAsync(storageFile);
                //using (DataReader dataReader = DataReader.FromBuffer(buffer))
                //{
                //    int stringSize = dataReader.ReadInt32();
                //    string fileContent = dataReader.ReadString((uint)stringSize);
                //    return JsonDeserialize<T>(fileContent);
                //}


                string text = await FileIO.ReadTextAsync(storageFile);
                return JsonDeserialize<T>(text);
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        #endregion

        #region Cities.json和WeatherTypes.json反序列化

        /// <summary>
        /// Cities.json反序列化
        /// </summary>
        /// <param name="type"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static async Task<T> JsonDeSerializeForCities<T>() where T : class
        {
            try
            {
                IStorageFile storageFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Data/Cities.txt"));
                string text = await FileIO.ReadTextAsync(storageFile);
                return JsonDeserialize<T>(text);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// WeatherTypes.json反序列化
        /// </summary>
        /// <param name="type"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static async Task<T> JsonDeSerializeForWeatherTypes<T>() where T : class
        {
            try
            {
                IStorageFile storageFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Data/WeatherTypes.txt"));
                string text = await FileIO.ReadTextAsync(storageFile);
                return JsonDeserialize<T>(text);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        #endregion

    }
}
