using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicBusiness.BPMS.Domain;

namespace DynamicBusiness.BPMS.BusinessLogic
{
    public class TaskService : ServiceBase
    {
        public TaskService(IUnitOfWork unitOfWork = null) : base(unitOfWork) { }

        public ResultOperation Add(Domain.sysBpmsTask Task)
        {
            ResultOperation resultOperation = new ResultOperation();
            if (resultOperation.IsSuccess)
            {
                if (string.IsNullOrEmpty(Task.RoleName))
                    Task.RoleName = string.Empty;
                else
                    Task.RoleName = "," + Task.RoleName.Trim(',') + ",";

                this.UnitOfWork.Repository<ITaskRepository>().Add(Task);
                this.UnitOfWork.Save();
            }
            return resultOperation;
        }

        public ResultOperation Update(Domain.sysBpmsTask Task)
        {
            ResultOperation resultOperation = new ResultOperation();
            if (resultOperation.IsSuccess)
            {
                if (string.IsNullOrEmpty(Task.RoleName))
                    Task.RoleName = string.Empty;
                else
                    Task.RoleName = "," + Task.RoleName.Trim(',') + ",";

                this.UnitOfWork.Repository<ITaskRepository>().Update(Task);
                this.UnitOfWork.Save();
            }
            return resultOperation;
        }

        public ResultOperation Delete(Guid taskId)
        {
            ResultOperation resultOperation = null;
            try
            {
                resultOperation = new ResultOperation();
                if (resultOperation.IsSuccess)
                {
                    sysBpmsTask sysBpmsTask = this.GetInfo(taskId);
                    this.BeginTransaction();

                    if (new ThreadTaskService(base.UnitOfWork).HasAny(sysBpmsTask.ProcessID, sysBpmsTask.ID))
                    {
                        resultOperation.AddError(LangUtility.Get("CannotDelete.Text", nameof(sysBpmsTask)));
                        return resultOperation;
                    }

                    foreach (sysBpmsStep item in new StepService(base.UnitOfWork).GetList(taskId, null))
                    {
                        resultOperation = new StepService(base.UnitOfWork).Delete(item.ID);
                        if (!resultOperation.IsSuccess)
                            break;
                    }
                    if (resultOperation.IsSuccess)
                    {
                        this.UnitOfWork.Repository<ITaskRepository>().Delete(taskId);
                        this.UnitOfWork.Save();
                        resultOperation = new ElementService(base.UnitOfWork).Delete(sysBpmsTask.ElementID, sysBpmsTask.ProcessID);
                    }
                }
            }
            catch (Exception ex)
            {
                return base.ExceptionHandler(ex);
            }
            base.FinalizeService(resultOperation);

            return resultOperation;
        }

        public Domain.sysBpmsTask GetInfo(Guid ID)
        {
            return this.UnitOfWork.Repository<ITaskRepository>().GetInfo(ID);
        }
        public Domain.sysBpmsTask GetInfo(string elementId, Guid processId)
        {
            return this.UnitOfWork.Repository<ITaskRepository>().GetInfo(elementId, processId);
        }
        public List<Domain.sysBpmsTask> GetList(int? typeLU, Guid? processID)
        {
            return this.UnitOfWork.Repository<ITaskRepository>().GetList(typeLU, processID);
        }

        public ResultOperation Update(Guid processID, WorkflowProcess _WorkflowProcess)
        {
            ResultOperation resultOperation = new ResultOperation();
            try
            {
                List<Domain.sysBpmsTask> Tasks = this.GetList(null, processID);
                List<WorkflowLane> WorkflowLanes = WorkflowLane.GetAllLanes(_WorkflowProcess.LaneSet);
                List<Guid> listDeleted = new List<Guid>();
                //delete userTask
                foreach (Domain.sysBpmsTask item in Tasks.Where(c => c.TypeLU == (int)Domain.sysBpmsTask.e_TypeLU.UserTask && !_WorkflowProcess.UserTasks.Any(d => d.ID == c.ElementID)))
                {
                    resultOperation = this.Delete(item.ID);
                    listDeleted.Add(item.ID);
                    if (!resultOperation.IsSuccess)
                        return resultOperation;
                }

                //delete serviceTask
                foreach (Domain.sysBpmsTask item in Tasks.Where(c => c.TypeLU == (int)Domain.sysBpmsTask.e_TypeLU.ServiceTask && !_WorkflowProcess.ServiceTasks.Any(d => d.ID == c.ElementID)))
                {
                    resultOperation = this.Delete(item.ID);
                    listDeleted.Add(item.ID);
                    if (!resultOperation.IsSuccess)
                        return resultOperation;
                }

                //delete scriptTask
                foreach (Domain.sysBpmsTask item in Tasks.Where(c => c.TypeLU == (int)Domain.sysBpmsTask.e_TypeLU.ScriptTask && !_WorkflowProcess.ScriptTasks.Any(d => d.ID == c.ElementID)))
                {
                    resultOperation = this.Delete(item.ID);
                    listDeleted.Add(item.ID);
                    if (!resultOperation.IsSuccess)
                        return resultOperation;
                }

                //delete task
                foreach (Domain.sysBpmsTask item in Tasks.Where(c => c.TypeLU == (int)Domain.sysBpmsTask.e_TypeLU.Task && !_WorkflowProcess.Tasks.Any(d => d.ID == c.ElementID)))
                {
                    resultOperation = this.Delete(item.ID);
                    listDeleted.Add(item.ID);
                    if (!resultOperation.IsSuccess)
                        return resultOperation;
                }
                Tasks = Tasks.Where(c => !listDeleted.Contains(c.ID)).ToList();
                //userTask
                foreach (WorkflowUserTask item in _WorkflowProcess.UserTasks)
                {
                    Domain.sysBpmsTask task = Tasks.FirstOrDefault(c => c.ElementID == item.ID);
                    if (task != null)
                    {
                        resultOperation = this.UpdateTask(task, WorkflowLanes, item.ID, item.Name, item.MarkerTypeLU);
                        if (!resultOperation.IsSuccess)
                            return resultOperation;
                    }
                    else
                    {
                        task = this.InitTask(Domain.sysBpmsTask.e_TypeLU.UserTask, item.ID, item.MarkerTypeLU, WorkflowLanes, item.Name, processID);
                        resultOperation = this.Add(task);
                        if (!resultOperation.IsSuccess)
                            return resultOperation;
                        new ElementService(this.UnitOfWork).Update(task.Element);
                        if (!resultOperation.IsSuccess)
                            return resultOperation;
                    }
                }

                //serviceTask
                foreach (WorkflowServiceTask item in _WorkflowProcess.ServiceTasks)
                {
                    Domain.sysBpmsTask task = Tasks.FirstOrDefault(c => c.ElementID == item.ID);
                    if (task != null)
                    {
                        resultOperation = this.UpdateTask(task, WorkflowLanes, item.ID, item.Name, item.MarkerTypeLU);
                        if (!resultOperation.IsSuccess)
                            return resultOperation;
                    }
                    else
                    {
                        task = this.InitTask(Domain.sysBpmsTask.e_TypeLU.ServiceTask, item.ID, item.MarkerTypeLU, WorkflowLanes, item.Name, processID);
                        resultOperation = this.Add(task);
                        if (!resultOperation.IsSuccess)
                            return resultOperation;
                        new ElementService(this.UnitOfWork).Update(task.Element);
                        if (!resultOperation.IsSuccess)
                            return resultOperation;
                    }
                }

                //scriptTask
                foreach (WorkflowScriptTask item in _WorkflowProcess.ScriptTasks)
                {
                    Domain.sysBpmsTask _Task = Tasks.FirstOrDefault(c => c.ElementID == item.ID);
                    if (_Task != null)
                    {
                        resultOperation = this.UpdateTask(_Task, WorkflowLanes, item.ID, item.Name, item.MarkerTypeLU);
                        if (!resultOperation.IsSuccess)
                            return resultOperation;
                    }
                    else
                    {
                        _Task = this.InitTask(Domain.sysBpmsTask.e_TypeLU.ScriptTask, item.ID, item.MarkerTypeLU, WorkflowLanes, item.Name, processID);
                        resultOperation = this.Add(_Task);
                        if (!resultOperation.IsSuccess)
                            return resultOperation;
                        new ElementService(this.UnitOfWork).Update(_Task.Element);
                        if (!resultOperation.IsSuccess)
                            return resultOperation;
                    }
                }

                //Task
                foreach (WorkflowTask item in _WorkflowProcess.Tasks)
                {
                    Domain.sysBpmsTask _Task = Tasks.FirstOrDefault(c => c.ElementID == item.ID);
                    if (_Task != null)
                    {
                        resultOperation = this.UpdateTask(_Task, WorkflowLanes, item.ID, item.Name, item.MarkerTypeLU);
                        if (!resultOperation.IsSuccess)
                            return resultOperation;
                    }
                    else
                    {
                        _Task = this.InitTask(Domain.sysBpmsTask.e_TypeLU.Task, item.ID, item.MarkerTypeLU, WorkflowLanes, item.Name, processID);
                        resultOperation = this.Add(_Task);
                        if (!resultOperation.IsSuccess)
                            return resultOperation;
                        new ElementService(this.UnitOfWork).Update(_Task.Element);
                        if (!resultOperation.IsSuccess)
                            return resultOperation;
                    }
                }
            }
            catch (Exception ex)
            {
                return base.ExceptionHandler(ex);
            }
            if (resultOperation.IsSuccess)
                this.UnitOfWork.Save();
            base.FinalizeService(resultOperation);

            return resultOperation;
        }

        private ResultOperation UpdateTask(Domain.sysBpmsTask task, List<WorkflowLane> workflowLanes, string id, string name, Domain.sysBpmsTask.e_MarkerTypeLU? markerTypeLU)
        {
            ResultOperation resultOperation = new ResultOperation();
            task.MarkerTypeLU = markerTypeLU.HasValue ? (int)markerTypeLU : (int?)null;
            resultOperation = this.Update(task);
            if (!resultOperation.IsSuccess)
                return resultOperation;
            //Element
            task.Element.Name = name;
            resultOperation = new ElementService(this.UnitOfWork).Update(task.Element);
            return resultOperation;
        }

        private Domain.sysBpmsTask InitTask(Domain.sysBpmsTask.e_TypeLU e_TypeLU, string id, Domain.sysBpmsTask.e_MarkerTypeLU? markerTypeLU, List<WorkflowLane> workflowLanes, string name, Guid processId)
        {
            return new Domain.sysBpmsTask()
            {
                Code = string.Empty,
                TypeLU = (int)e_TypeLU,
                ID = Guid.NewGuid(),
                ElementID = id,
                ProcessID = processId,
                MarkerTypeLU = markerTypeLU.HasValue ? (int)markerTypeLU : (int?)null,
                //Element
                Element = this.InitElement(id, name, processId),
            };
        }

        private sysBpmsElement InitElement(string id, string name, Guid processId)
        {
            return new sysBpmsElement()
            {
                ID = id,
                Name = name,
                ProcessID = processId,
                TypeLU = (int)sysBpmsElement.e_TypeLU.Task,
            };
        }
    }
}
