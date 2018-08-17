using System.Collections.Generic;
using System.Threading.Tasks;
using LSRetail.Omni.Domain.DataModel.Base.OmniTasks;

namespace LSRetail.Omni.Domain.Services.Base.OmniTasks
{
	public class OmniTasksService
	{
		private IOmniTasksRepository repository;

		public OmniTasksService(IOmniTasksRepository repository)
		{
			this.repository = repository;
		}

		public List<OmniTask> TasksGetAll(bool includeLines)
		{
			return repository.TasksGetAll(includeLines);
		}

		public List<OmniTask> TasksGetByFilter(OmniTask filterTask, bool includeLines)
		{
			return repository.TaskGet(filterTask, includeLines);
		}

        public List<OmniTask> GetForUser(string userID, bool includeLines)
		{
			OmniTask task = new OmniTask();
			task.RequestUser = userID;

			return repository.TaskGet(task, includeLines);
		}

		public OmniTask TaskGetById(string taskId, bool includeLines)
		{
			return repository.TaskGetById(taskId, includeLines);
		}

		public OmniTask TaskSave(OmniTask task)
		{
			return repository.TaskSave(task);
		}

        #region Async Functions

        public async Task<List<OmniTask>> TasksGetAllAsync(bool includeLines)
        {
            return await Task.Run(() => TasksGetAll(includeLines));
        }

        public async Task<List<OmniTask>> TasksGetByFilterAsync(OmniTask filterTask, bool includeLines)
        {
            return await Task.Run(() => TasksGetByFilter(filterTask, includeLines));
        }

        public async Task<List<OmniTask>> GetForUserAsync(string userID, bool includeLines)
        {
            return await Task.Run(() => GetForUser(userID, includeLines));
        }

        public async Task<OmniTask> TaskGetByIdAsync(string taskId, bool includeLines)
        {
            return await Task.Run(() => TaskGetById(taskId, includeLines));
        }

        public async Task<OmniTask> TaskSaveAsync(OmniTask task)
		{
			return await Task.Run(() => TaskSave(task));
		}

        #endregion
    }
}
