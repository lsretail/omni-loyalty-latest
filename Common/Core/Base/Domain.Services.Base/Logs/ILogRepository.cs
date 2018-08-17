using System.Collections.Generic;
using LSRetail.Omni.Domain.DataModel.Base.Log;

namespace LSRetail.Omni.Domain.Services.Base.Logs
{
    public interface ILogRepository
    {
        void Save(DataModel.Base.Log.Log log);
        void Delete(int id);
        void DeleteOldLogs();
        void DeleteAll();
        DataModel.Base.Log.Log Get(int id);
        List<DataModel.Base.Log.Log> Get(LogLevel level, LogType type);
        List<DataModel.Base.Log.Log> GetAll();
    }
}
