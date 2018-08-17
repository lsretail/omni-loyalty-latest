using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LSRetail.Omni.Domain.DataModel.Base.OmniTasks;
using LSRetail.Omni.Domain.Services.Base.OmniTasks;
using LSRetail.Omni.Infrastructure.Data.Omniservice.Base;

namespace LSRetail.Omni.Infrastructure.Data.Omniservice.OmniTasks
{
    public class OmniTaskRepository : BaseRepository, IOmniTasksRepository
    {
        public List<OmniTask> TasksGetAll(Boolean includeLines)
        {
            string methodName = "TaskGet";
            OmniTask filter = new OmniTask() { ModifyTime = DateTime.MinValue };
            var jObject = new { lookupFilter = filter, includeLines = includeLines, maxTasks = 0 };
            return base.PostData<List<OmniTask>>(jObject, methodName);
        }

        public List<OmniTask> TaskGet(OmniTask task, Boolean includeLines)
        {
            string methodName = "TaskGet";
            var jObject = new { lookupFilter = task, includeLines = includeLines, maxTasks = 0 };
            return base.PostData<List<OmniTask>>(jObject, methodName);
        }

        public OmniTask TaskGetById(String taskId, Boolean includeLines)
        {
            string methodName = "TaskGet";
            var jObject = new
            {
                lookupFilter = new OmniTask(taskId) { ModifyTime = DateTime.MinValue },
                includeLines = includeLines,
                maxTasks = 1
            };
            var omniList = base.PostData<List<OmniTask>>(jObject, methodName);

            if (omniList.Count > 0)
            {
                return omniList.First();
            }
            return null;
        }

        public OmniTask TaskSave(OmniTask task)
        {
            string methodName = "TaskSave";
            var jObject = new { task = task };
            return base.PostData<OmniTask>(jObject, methodName);
        }
    }
}
