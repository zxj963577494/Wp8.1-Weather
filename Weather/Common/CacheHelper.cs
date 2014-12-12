using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Weather.Common
{

    /// <summary>
    /// 缓存实体类
    /// </summary>
    public class CacheEntity
    {
        /// <summary>
        /// 缓存起始时间
        /// </summary>
        public string StartDate { get; set; }

        /// <summary>
        /// 缓存周期
        /// </summary>
        public string CacheDate { get; set; }

        /// <summary>
        /// 唯一存储Key
        /// </summary>
        public string CacheKey { get; set; }

        /// <summary>
        /// 存储模块
        /// </summary>
        public string CacheDirName { get; set; }

        /// <summary>
        /// 存储文件名称
        /// </summary>
        public string CacheFileName { get; set; }//文件名称

        /// <summary>
        /// 缓存数据类型
        /// </summary>
        public string CacheContext { get; set; }//缓存数据类型
    }


    /// <summary>
    /// 缓存操作类
    /// </summary>
    public static class CacheHelper
    {
        /// <summary>
        /// 缓存是否过期
        /// </summary>
        /// <param name="getCacheEntity">Cache Entity</param>
        /// <returns>Is out Of Date</returns>
        public static bool CacheEntityIsOutDate(CacheEntity getCacheEntity)
        {
            bool isOutOfDate = false;
            if (getCacheEntity != null)
            {
                DateTime currentDate = DateTime.Now;
                TimeSpan getTimeSpan = currentDate - Convert.ToDateTime(getCacheEntity.StartDate);

                int compareValue = getTimeSpan.CompareTo(new TimeSpan(0, Convert.ToInt32(getCacheEntity.CacheDate), 0));
                if (compareValue == -1)
                    isOutOfDate = false;//未过期
                else
                    isOutOfDate = true;//过期
            }
            return isOutOfDate;
        }


        /// <summary>
        /// 添加缓存
        /// </summary>
        /// <param name="getCacheEntity">cache Entity</param>
        public static bool AddCacheEntity(CacheEntity getCacheEntity)
        {
            bool isCache = false;
            if (getCacheEntity != null)
            {
                //isCache = UniversalCommon_operator.AddIsolateStorageObj(getCacheEntity.CacheKey, getCacheEntity);
            }
            return isCache;
        }
    }
}
