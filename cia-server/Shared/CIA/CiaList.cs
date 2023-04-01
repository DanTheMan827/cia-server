using System.Collections.ObjectModel;

namespace cia_server.Shared.CIA
{
    public class CiaList : IDisposable
    {
        private static CiaList? _shared = null;
        private FileSystemWatcher _watcher;
        public static CiaList Shared
        {
            get
            {
                if (_shared == null)
                {
                    InitializeShared();
                }

                return _shared;
            }
        }

        public static void InitializeShared() => _shared = new CiaList();
        public ObservableCollection<CiaFile> Files { get; private set; } = new ObservableCollection<CiaFile>();
        private CiaList()
        {
            _watcher = new FileSystemWatcher(AppPaths.CiaServerPath, "*.cia");
            _watcher.Created += this.CiaCreated;
            _watcher.Changed += this.CiaChanged;
            _watcher.Renamed += this.CiaRenamed;
            _watcher.Deleted += this.CiaDeleted;
            _watcher.IncludeSubdirectories = true;
            _watcher.EnableRaisingEvents = true;

            foreach (var file in Directory.GetFiles(AppPaths.CiaServerPath, "*.cia", SearchOption.AllDirectories))
            {
                AddCia(file).Wait();
            }
        }

        private async Task AddCia(string path)
        {
            await Task.Run(() =>
            {
                try
                {
                    var cia = new CiaFile(path);
                    Files.Add(cia);
                }
                catch (Exception ex) { }
            });
        }

        private async Task RemoveCia(string path)
        {
            await Task.Run(() =>
            {
                foreach (var cia in Files.Where(cia => cia.Path == path).ToList())
                {
                    Files.Remove(cia);
                }
            });
        }

        private async void CiaDeleted(object sender, FileSystemEventArgs e)
        {
            await RemoveCia(e.FullPath);
        }

        private async void CiaRenamed(object sender, RenamedEventArgs e)
        {
            await RemoveCia(e.OldFullPath);
            await AddCia(e.FullPath);
        }

        private async void CiaChanged(object sender, FileSystemEventArgs e)
        {
            await RemoveCia(e.FullPath);
            await AddCia(e.FullPath);
        }

        private async void CiaCreated(object sender, FileSystemEventArgs e)
        {
            await AddCia(e.FullPath);
        }

        public void Dispose()
        {
            _watcher.Dispose();
        }
    }
}
