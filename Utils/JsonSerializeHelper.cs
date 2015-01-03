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
    public class JsonSerializeHelper
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
        public static async Task JsonSerializeForFileAsync<T>(T target, string filePath)
        {
            try
            {
                //序列化
                string jsonContent = JsonSerialize<T>(target);
                await FileHelper.CreateAndWriteFileAsync(filePath, jsonContent).ConfigureAwait(false);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// 文件化Json序列化
        /// </summary>
        /// <param name="jsonContent"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static async Task JsonSerializeForFileAsync(string jsonContent, string filePath)
        {
            try
            {
                //序列化
                await FileHelper.CreateAndWriteFileAsync(filePath, jsonContent).ConfigureAwait(false);

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
        public static async Task<T> JsonDeSerializeForFileAsync<T>(string filePath) where T : class
        {
            try
            {
                string text = await FileHelper.ReadTxtFileAsync(filePath);
                return JsonDeserialize<T>(text);
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
        public static async Task<T> JsonDeSerializeForFileByInstalledLocationAsync<T>(string filePath) where T : class
        {
            try
            {
                string text = await FileHelper.ReadTxtFileByInstalledLocationAsync(filePath);
                return JsonDeserialize<T>(text);
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        #endregion


        #region 文件化Json序列化/反序列化


        #endregion
    }
}
