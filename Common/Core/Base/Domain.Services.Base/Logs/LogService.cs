using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LSRetail.Omni.Domain.DataModel.Base.Log;

namespace LSRetail.Omni.Domain.Services.Base.Logs
{
    public class LogService
    {
        private ILogRepository logRepository;
        private static SemaphoreSlim semaphore;

        public LogService(ILogRepository logRepository)
        {
            this.logRepository = logRepository;
        }

        public async Task SaveAsync(DataModel.Base.Log.Log log)
        {
            if (semaphore == null)
            {
                semaphore = new SemaphoreSlim(1, 1);
            }

            await semaphore.WaitAsync();

            try
            {
                await Task.Run(() => logRepository.Save(log));
            }
            finally
            {
                semaphore.Release();
            }
        }

        public async Task SaveAsync(LogType logType, LogLevel logLevel, string message)
        {
            await SaveAsync(new Log() { LogLevel = logLevel, Message = message, LogType = logType });
        }

        public async Task SaveAsync(LogType logType, LogLevel logLevel, LogLevel allowedLogLevel, string message)
        {
            if (logLevel >= allowedLogLevel)
            {
                await SaveAsync(new Log() { LogLevel = logLevel, Message = message, LogType = logType });
            }
        }

        public async void Save(LogType logType, LogLevel logLevel, LogLevel allowedLogLevel, string message)
        {
            if (logLevel <= allowedLogLevel)
            {
                await SaveAsync(new Log() { LogLevel = logLevel, Message = message, LogType = logType });
            }
        }

        private void Delete(int id)
        {
            logRepository.Delete(id);
        }

        private void DeleteOldLogs()
        {
            logRepository.DeleteOldLogs();
        }

        private void DeleteAll()
        {
            logRepository.DeleteAll();
        }

        private DataModel.Base.Log.Log Get(int id)
        {
            return logRepository.Get(id);
        }

        private List<DataModel.Base.Log.Log> Get(LogLevel level, LogType type)
        {
            return logRepository.Get(level, type);
        }

        private List<DataModel.Base.Log.Log> GetAll()
        {
            return logRepository.GetAll();
        }

        public async Task DeleteAsync(int id)
        {
            await Task.Run(() => Delete(id));
        }

        public async Task DeleteOldLogsAsync()
        {
            await Task.Run(() => DeleteOldLogs());
        }

        public async Task DeleteAllAsync()
        {
            await Task.Run(() => DeleteAll());
        }

        public async Task<DataModel.Base.Log.Log> GetAsync(int id)
        {
            return await Task.Run(() => Get(id));
        }

        public async Task<List<DataModel.Base.Log.Log>> GetAsync(LogLevel level, LogType type)
        {
            return await Task.Run(() => Get(level, type));
        }

        public async Task<List<DataModel.Base.Log.Log>> GetAllAsync()
        {
            return await Task.Run(() => GetAll());
        }
    }
}
