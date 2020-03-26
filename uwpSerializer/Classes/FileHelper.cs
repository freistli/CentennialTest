using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace uwpsavefile.Classes
{
    class FileHelper
    {
        /// <summary>Returns if a file is found in the specified storage strategy</summary>
        /// <param name="key">Path of the file in storage</param>
        /// <param name="location">Location storage strategy</param>
        /// <returns>Boolean: true if found, false if not found</returns>
        public async Task<bool> FileExistsAsync(string key, StorageStrategies location = StorageStrategies.Local)
        {
            return (await GetIfFileExistsAsync(key, location)) != null;
        }

        public async Task<bool> FileExistsAsync(string key, Windows.Storage.StorageFolder folder)
        {
            return (await GetIfFileExistsAsync(key, folder)) != null;
        }

        /// <summary>Deletes a file in the specified storage strategy</summary>
        /// <param name="key">Path of the file in storage</param>
        /// <param name="location">Location storage strategy</param>
        public async Task<bool> DeleteFileAsync(string key, StorageStrategies location = StorageStrategies.Local)
        {
            var _File = await GetIfFileExistsAsync(key, location);
            if (_File != null)
                await _File.DeleteAsync();
            return !(await FileExistsAsync(key, location));
        }

        /// <summary>Reads and deserializes a file into specified type T</summary>
        /// <typeparam name="T">Specified type into which to deserialize file content</typeparam>
        /// <param name="key">Path to the file in storage</param>
        /// <param name="location">Location storage strategy</param>
        /// <returns>Specified type T</returns>
        public async Task<T> ReadFileAsync<T>(string key, StorageStrategies location = StorageStrategies.Local)
        {
            try
            {
                // fetch file
                var _File = await GetIfFileExistsAsync(key, location);
                if (_File == null)
                    return default(T);
                // read content
                var _String = await Windows.Storage.FileIO.ReadTextAsync(_File);
                // convert to obj
                var _Result = Deserialize<T>(_String);
                return _Result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>Serializes an object and write to file in specified storage strategy</summary>
        /// <typeparam name="T">Specified type of object to serialize</typeparam>
        /// <param name="key">Path to the file in storage</param>
        /// <param name="value">Instance of object to be serialized and written</param>
        /// <param name="location">Location storage strategy</param>
        public async Task<bool> WriteFileAsync<T>(string key, T value, StorageStrategies location = StorageStrategies.Local)
        {
            // convert to string
            var _String = Serialize(value);
            System.Diagnostics.Debug.WriteLine(_String);

            // create file
            var _File = await CreateFileAsync(key, location, Windows.Storage.CreationCollisionOption.ReplaceExisting).ConfigureAwait(false);
            Debug.WriteLine("File created/replaced");

            try
            {
                // save string to file
                await Windows.Storage.FileIO.WriteTextAsync(_File, _String).AsTask().ConfigureAwait(false);
                Debug.WriteLine("Wrote data to file");

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                throw ex;
            }
            Debug.WriteLine("Checking if File exists and returning");

            // result
            return await FileExistsAsync(key, location).ConfigureAwait(false);
        }

        private async Task<Windows.Storage.StorageFile> CreateFileAsync(string key, StorageStrategies location = StorageStrategies.Local,
            Windows.Storage.CreationCollisionOption option = Windows.Storage.CreationCollisionOption.OpenIfExists)
        {
            switch (location)
            {
                case StorageStrategies.Local:
                    return await Windows.Storage.ApplicationData.Current.LocalFolder.CreateFileAsync(key, option);
                case StorageStrategies.Roaming:
                    return await Windows.Storage.ApplicationData.Current.RoamingFolder.CreateFileAsync(key, option);
                case StorageStrategies.Temporary:
                    return await Windows.Storage.ApplicationData.Current.TemporaryFolder.CreateFileAsync(key, option);
                case StorageStrategies.LocalCache:
                    return await Windows.Storage.ApplicationData.Current.LocalCacheFolder.CreateFileAsync(key, option);
                case StorageStrategies.SharedLocalFolder:
                    return await Windows.Storage.ApplicationData.Current.SharedLocalFolder.CreateFileAsync(key, option);
                case StorageStrategies.DocumentLibrary:
                    return await Windows.Storage.KnownFolders.DocumentsLibrary.CreateFileAsync(key, option);
                case StorageStrategies.PublisherCacheFolder:
                    return await Windows.Storage.ApplicationData.Current.GetPublisherCacheFolder("freistli.com").CreateFileAsync(key, option);
                default:
                    throw new NotSupportedException(location.ToString());
            }
        }

        private async Task<Windows.Storage.StorageFile> GetIfFileExistsAsync(string key, Windows.Storage.StorageFolder folder,
            Windows.Storage.CreationCollisionOption option = Windows.Storage.CreationCollisionOption.FailIfExists)
        {
            Windows.Storage.StorageFile retval;
            try
            {
                retval = await folder.GetFileAsync(key);
            }
            catch (System.IO.FileNotFoundException)
            {
                System.Diagnostics.Debug.WriteLine("GetIfFileExistsAsync:FileNotFoundException");
                return null;
            }
            return retval;
        }

        /// <summary>Returns a file if it is found in the specified storage strategy</summary>
        /// <param name="key">Path of the file in storage</param>
        /// <param name="location">Location storage strategy</param>
        /// <returns>StorageFile</returns>
        public async Task<Windows.Storage.StorageFile> GetIfFileExistsAsync(string key,
            StorageStrategies location = StorageStrategies.Local,
            Windows.Storage.CreationCollisionOption option = Windows.Storage.CreationCollisionOption.FailIfExists)
        {
            Windows.Storage.StorageFile retval;
            try
            {
                switch (location)
                {
                    case StorageStrategies.Local:
                        retval = await Windows.Storage.ApplicationData.Current.LocalFolder.GetFileAsync(key);
                        break;
                    case StorageStrategies.Roaming:
                        retval = await Windows.Storage.ApplicationData.Current.RoamingFolder.GetFileAsync(key);
                        break;
                    case StorageStrategies.Temporary:
                        retval = await Windows.Storage.ApplicationData.Current.TemporaryFolder.GetFileAsync(key);
                        break;
                    case StorageStrategies.LocalCache:
                        retval = await Windows.Storage.ApplicationData.Current.LocalCacheFolder.GetFileAsync(key);
                        break;
                    case StorageStrategies.SharedLocalFolder:
                        retval = await Windows.Storage.ApplicationData.Current.SharedLocalFolder.GetFileAsync(key);
                        break;
                    case StorageStrategies.DocumentLibrary:
                        retval = await Windows.Storage.KnownFolders.DocumentsLibrary.GetFileAsync(key);
                        break;
                    case StorageStrategies.PublisherCacheFolder:
                        retval = await Windows.Storage.ApplicationData.Current.GetPublisherCacheFolder("freistli.com").GetFileAsync(key);
                        break;
                    default:
                        throw new NotSupportedException(location.ToString());
                }
            }
            catch (System.IO.FileNotFoundException)
            {
                System.Diagnostics.Debug.WriteLine("GetIfFileExistsAsync:FileNotFoundException");
                return null;
            }

            return retval;
        }

        private string Serialize<T>(T item)
        {
            return JsonConvert.SerializeObject(item,
                Formatting.None, new JsonSerializerSettings()
                {
                    TypeNameHandling = TypeNameHandling.Objects,
                    TypeNameAssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple
                });
        }

        private T Deserialize<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        public enum StorageStrategies { Local, Roaming, Temporary, LocalCache, SharedLocalFolder, DocumentLibrary,PublisherCacheFolder }
    }
}
