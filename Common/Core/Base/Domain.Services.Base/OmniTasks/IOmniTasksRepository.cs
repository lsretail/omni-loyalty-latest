using System.Collections.Generic;
using LSRetail.Omni.Domain.DataModel.Base.OmniTasks;

namespace LSRetail.Omni.Domain.Services.Base.OmniTasks
{
	public interface IOmniTasksRepository
	{
		List<OmniTask> TasksGetAll(bool includeLines);
		List<OmniTask> TaskGet(OmniTask task, bool includeLines);
		OmniTask TaskGetById(string taskId, bool includeLines);
		OmniTask TaskSave(OmniTask task);
	}
}
