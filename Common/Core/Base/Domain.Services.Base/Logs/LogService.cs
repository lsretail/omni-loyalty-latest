using System.Collections.Generic;
using System.Threading.Tasks;
using LSRetail.Omni.Domain.DataModel.Base.Log;

namespace LSRetail.Omni.Domain.Services.Base.Logs
{
    public class LogService
    {
        private ILogRepository logRepository;

        public LogService(ILogRepository logRepository)
        {
            this.logRepository = logRepository;
        }

        private void Save(DataModel.Base.Log.Log log)
        {
            logRepository.Save(log);
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

        public async Task SaveAsync(DataModel.Base.Log.Log log)
        {
            await Task.Run(() => Save(log));
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
