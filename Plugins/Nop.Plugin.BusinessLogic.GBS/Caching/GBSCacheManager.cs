using Microsoft.AspNet.OutputCache.SQLAsyncOutputCacheProvider;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using Newtonsoft.Json.Linq;
using Nop.Core.Caching;
using Nop.Core.Infrastructure;
using Nop.Plugin.BusinessDataAccess.GBS;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Reflection;
using System.Runtime.Caching;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Hosting;

namespace Nop.Plugin.BusinessLogic.GBS.Caching
{
    /// <summary>
    /// Represents a manager for caching between HTTP requests (long term caching)
    /// </summary>
    public partial class GBSCacheManager : ICacheManager
    {
        protected readonly string insert = "EXEC usp_Iinsert_tblNopCache @key, @value";
        protected readonly string select = "EXEC usp_Select_tblNopCache @key";
        protected readonly DBManager manager = new DBManager();
        protected ICacheManager memoryCacheManaager = EngineContext.Current.ContainerManager.Resolve<ICacheManager>("nop_cache_static");
        protected SQLAsyncOutputCacheProvider sqlProvider = new SQLAsyncOutputCacheProvider();

        /// <summary>
        /// Cache object
        /// </summary>
        //protected ObjectCache Cache
        //{
        //    get
        //    {
        //        return MemoryCache.Default;
        //    }
        //}

        [Serializable]
        public class MyCacheItem
        {
            public string key;
            public object data;
            public int cacheTime;
            public Type dataType;
            public MyCacheItem(string key, object data, int cacheTime, Type dataType)
            {
                this.key = key;
                this.data = data;
                this.cacheTime = cacheTime;
                this.dataType = dataType;
            }
        }

        /// <summary>
        /// Gets or sets the value associated with the specified key.
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="key">The key of the value to get.</param>
        /// <returns>The value associated with the specified key.</returns>
        public virtual T Get<T>(string key)
        {
            try
            {
                if (memoryCacheManaager.IsSet(key))
                {
                    return memoryCacheManaager.Get<T>(key);
                }
                //Dictionary<string, Object> paramDic = new Dictionary<string, Object>();
                //paramDic.Add("@key", key);
                //string dbResult = (string)manager.GetParameterizedScalar(select, paramDic);

                //return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(dbResult);
                MyCacheItem cacheItem = (MyCacheItem)sqlProvider.Get(key); 

                byte[] data = (byte[])cacheItem.data;
                using (MemoryStream ms = new MemoryStream(data))
                using (BsonReader reader = new BsonReader(ms))
                {
                    JsonSerializer serializer = new JsonSerializer(); 
                    if (typeof(T) == typeof(System.Data.DataView))
                    {
                        MyCacheItem dCacheItem = serializer.Deserialize<MyCacheItem>(reader);
                        System.Data.DataTable table = JsonConvert.DeserializeObject<System.Data.DataTable>(dCacheItem.data.ToString());
                        System.Data.DataSet dSet = new System.Data.DataSet();
                        dSet.Tables.Add(table);
                        T result = (T)Convert.ChangeType(dSet.Tables[0].DefaultView, typeof(T));
                        memoryCacheManaager.Set(key, new System.Data.DataView(table), cacheItem.cacheTime);
                        return result;
                    }
                    T item = serializer.Deserialize<T>(reader);
                    memoryCacheManaager.Set(key, item, cacheItem.cacheTime);
                    return item;
                }
            }catch (Exception ex)
            {
                return memoryCacheManaager.Get<T>(key);
            }
        }

        /// <summary>
        /// Gets or sets the value associated with the specified key.
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="key">The key of the value to get.</param>
        /// <returns>The value associated with the specified key.</returns>
        public virtual MyCacheItem GetCacheItem(string key)
        {
            return (MyCacheItem)sqlProvider.Get(key); ;
        }



        /// <summary>
        /// Adds the specified key and object to the cache.
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="data">Data</param>
        /// <param name="cacheTime">Cache time</param>
        public virtual void Set(string key, object data, int cacheTime)
        {
            if (data == null)
                return;
            memoryCacheManaager.Set(key, data, cacheTime);
            //dynamic cacheItem = new ExpandoObject();
            //cacheItem.key = key;
            //cacheItem.data = data;

            //ThreadPool.QueueUserWorkItem(new WaitCallback(SaveCacheItem), cacheItem);
            //var sqlProvider = new Microsoft.AspNet.OutputCache.SQLAsyncOutputCacheProvider.SQLAsyncOutputCacheProvider();
            Type dataType = data.GetType();
            if (data is System.Data.DataView)
            {
                data = ((System.Data.DataView)data).Table;
                MyCacheItem dCacheItem = new MyCacheItem(key, data, cacheTime, dataType);
                data = dCacheItem;
            }
            using (MemoryStream ms = new MemoryStream())
            using (BsonWriter datawriter = new BsonWriter(ms))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(datawriter, data);
                data = ms.ToArray();
            }

            var myCacheItem = new MyCacheItem(key, data, cacheTime, dataType);
            sqlProvider.Set(key, myCacheItem, DateTime.Now + TimeSpan.FromMinutes(cacheTime));
        }

        //protected void SaveCacheItem(object cacheItem)
        //{
        //    try
        //    {
        //        Dictionary<string, Object> paramDic = new Dictionary<string, Object>();

        //        var dataJson = JsonConvert.SerializeObject(((dynamic)cacheItem).data, new JsonSerializerSettings()
        //        {
        //            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        //        });
        //        paramDic.Add("@key", ((dynamic)cacheItem).key);
        //        paramDic.Add("@value", dataJson);
        //        manager.SetParameterizedQueryNoData(insert, paramDic);
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //}


        //protected void SaveCacheItem(object cacheItem)
        //{
        //    try
        //    {
        //        Dictionary<string, Object> paramDic = new Dictionary<string, Object>();

        //        var dataJson = JsonConvert.SerializeObject(((dynamic)cacheItem).data, new JsonSerializerSettings()
        //        {
        //            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        //        });
        //        paramDic.Add("@key", ((dynamic)cacheItem).key);
        //        paramDic.Add("@value", dataJson);
        //        manager.SetParameterizedQueryNoData(insert, paramDic);
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //}

        //protected T ToGenericType<T>(T obj)
        //{
        //    return (T)Convert.ChangeType(obj, typeof(T));
        //}


        /// <summary>
        /// Gets a value indicating whether the value associated with the specified key is cached
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>Result</returns>
        public virtual bool IsSet(string key)
        {
            var cacheItemB = sqlProvider.Get(key);
            return (cacheItemB != null);
        }

        /// <summary>
        /// Removes the value with the specified key from the cache
        /// </summary>
        /// <param name="key">/key</param>
        public virtual void Remove(string key)
        {
            memoryCacheManaager.Remove(key);
        }

        /// <summary>
        /// Removes items by pattern
        /// </summary>
        /// <param name="pattern">pattern</param>
        public virtual void RemoveByPattern(string pattern)
        {
            memoryCacheManaager.RemoveByPattern(pattern);
        }

        /// <summary>
        /// Clear all cache data
        /// </summary>
        public virtual void Clear()
        {
            memoryCacheManaager.Clear();
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public virtual void Dispose()
        {
            memoryCacheManaager.Dispose();
        }
    }
}