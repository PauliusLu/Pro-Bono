using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Karma.Models
{
    public class FileWatcher
    {
        public static readonly string ItemTypesPath = Path.Combine("Data", "ItemTypes.txt");

        private static FileSystemWatcher _watcher;

        static FileWatcher()
        {
            _watcher = new FileSystemWatcher(ItemTypesPath);

            _watcher.NotifyFilter = NotifyFilters.Attributes
                    | NotifyFilters.CreationTime
                    | NotifyFilters.DirectoryName
                    | NotifyFilters.FileName
                    | NotifyFilters.LastAccess
                    | NotifyFilters.LastWrite
                    | NotifyFilters.Security
                    | NotifyFilters.Size;

            _watcher.Changed += OnChangedEventHandler;
            _watcher.EnableRaisingEvents = true;
        }

        public static void OnChangedEventHandler(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType != WatcherChangeTypes.Changed)
                return;

            ItemTypes.LoadItemTypes(e.FullPath);
        }
    }
}
