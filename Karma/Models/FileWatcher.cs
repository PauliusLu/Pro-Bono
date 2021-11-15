using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Karma.Models
{
    public class FileWatcher
    {
        private static IWebHostEnvironment _iWebHostEnv;
        private static FileSystemWatcher _watcher;

        public FileWatcher(IWebHostEnvironment iWebHostEnv)
        {
            _iWebHostEnv = iWebHostEnv;

            if (_watcher == null)
            {
                _watcher = new FileSystemWatcher(Path.Combine(_iWebHostEnv.ContentRootPath, ItemTypes.ItemTypesPath));

            _watcher.NotifyFilter = NotifyFilters.Attributes
                    | NotifyFilters.CreationTime
                    | NotifyFilters.DirectoryName
                    | NotifyFilters.FileName
                    | NotifyFilters.LastAccess
                    | NotifyFilters.LastWrite
                    | NotifyFilters.Security
                    | NotifyFilters.Size;

                _watcher.Filter = Path.GetFileName(ItemTypes.ItemTypesFileName);
                _watcher.Changed += ItemTypes.OnChangedEventHandler;
            _watcher.EnableRaisingEvents = true;
        }

        }
    }
}
