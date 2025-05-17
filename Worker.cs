namespace FileMoverService
{
    public class Worker : BackgroundService
    {
        #region MyRegion
        private readonly ILogger<Worker> _logger;
        private FileSystemWatcher _watcher;
        private readonly string folder1 = @"C:\Folder1";
        private readonly string folder2 = @"C:\Folder2";
        #endregion

        #region Constructor
        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;

            if (!Directory.Exists(folder1))
                Directory.CreateDirectory(folder1);

            if (!Directory.Exists(folder2))
                Directory.CreateDirectory(folder2);

            _watcher = new FileSystemWatcher(folder1);
            _watcher.Created += OnCreated;
            _watcher.EnableRaisingEvents = true;
        }
        #endregion

        #region Action
        private void OnCreated(object sender, FileSystemEventArgs e)
        {
            try
            {
                WaitForFile(e.FullPath);

                string fileName = Path.GetFileName(e.FullPath);
                string destPath = Path.Combine(folder2, fileName);

                File.Move(e.FullPath, destPath);

                _logger.LogInformation($"Moved file '{fileName}' from Folder1 to Folder2");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error moving file '{e.FullPath}'");
            }
        }

        private void WaitForFile(string filePath)
        {
            int retries = 10;
            while (retries > 0)
            {
                try
                {
                    using (FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.None))
                    {
                        if (stream.Length > 0)
                            break;
                    }
                }
                catch (IOException)
                {
                    Thread.Sleep(500);
                }
                retries--;
            }
        }
        #endregion

        #region Execute
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("FileMoverService started. Monitoring folder: {folder1}", folder1);

            try
            {
                await Task.Delay(Timeout.Infinite, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("FileMoverService is stopping.");
            }
        }
        #endregion

        #region Dispose
        public override void Dispose()
        {
            base.Dispose();
            if (_watcher != null)
            {
                _watcher.Dispose();
                _watcher = null;
            }
        }
        #endregion

    }
}
