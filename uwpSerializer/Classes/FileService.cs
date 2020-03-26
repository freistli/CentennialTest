using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static uwpsavefile.Classes.FileHelper;

namespace uwpsavefile.Classes
{
    class FileService
    {
        FileHelper _helper = new FileHelper();
        public StorageStrategies? storageStrategy { get; set; }
        public async Task<List<T>> ReadAsync<T>(string key)
        {
            try { return await _helper.ReadFileAsync<List<T>>(key, storageStrategy ?? StorageStrategies.Local); }
            catch { return new List<T>(); }
        }

        public async Task WriteAsync<T>(string key, List<T> items)
        {

            await _helper.WriteFileAsync(key, items, storageStrategy ?? StorageStrategies.Local);
        }

        public async Task<Windows.Storage.StorageFile> GetFile (string key)
        {
            return await _helper.GetIfFileExistsAsync(key, storageStrategy ?? StorageStrategies.Local);
        }
    }
}
