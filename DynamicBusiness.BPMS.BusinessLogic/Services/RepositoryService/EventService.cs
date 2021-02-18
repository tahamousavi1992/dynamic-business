using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicBusiness.BPMS.Domain;
namespace DynamicBusiness.BPMS.BusinessLogic
{
    public class EventService : ServiceBase
    {
        public EventService(IUnitOfWork unitOfWork = null) : base(unitOfWork) { }

        public ResultOperation Add(sysBpmsEvent Event)
        {
            this.UnitOfWork.Repository<IEventRepository>().Add(Event);
            this.UnitOfWork.Save();

            return new ResultOperation();
        }

        public ResultOperation Update(sysBpmsEvent Event)
        {
            this.UnitOfWork.Repository<IEventRepository>().Update(Event);
            this.UnitOfWork.Save();

            return new ResultOperation();
        }

        public ResultOperation Delete(Guid id)
        {
            ResultOperation resultOperation = new ResultOperation();
            try
            {
                this.BeginTransaction();
                sysBpmsEvent @event = this.GetInfo(id);
                this.UnitOfWork.Repository<IEventRepository>().Delete(id);
                this.UnitOfWork.Save();
                new ElementService(this.UnitOfWork).Delete(@event.ElementID, @event.ProcessID);

            }
            catch (Exception ex)
            {
                return base.ExceptionHandler(ex);
            }
            base.FinalizeService(resultOperation);

            return resultOperation;

        }

        public sysBpmsEvent GetInfo(Guid id)
        {
            return this.UnitOfWork.Repository<IEventRepository>().GetInfo(id);
        }

        public sysBpmsEvent GetInfo(string elementId, Guid processId)
        {
            return this.UnitOfWork.Repository<IEventRepository>().GetInfo(elementId, processId);
        }

        public List<sysBpmsEvent> GetList(int? TypeLU, Guid? ProcessID, string RefElementID, int? SubType, string[] Includes = null)
        {
            return this.UnitOfWork.Repository<IEventRepository>().GetList(TypeLU, ProcessID, RefElementID, SubType, Includes);
        }

        public List<sysBpmsEvent> GetList(int? TypeLU, Guid? ProcessID, string RefElementID, int? SubType, int? ProcessStatusLU, string[] Includes = null)
        {
            return this.UnitOfWork.Repository<IEventRepository>().GetList(TypeLU, ProcessID, RefElementID, SubType, ProcessStatusLU, Includes);
        }

        public List<sysBpmsEvent> GetListStartMessage(Guid? notProcessID, string key, Guid messageTypeID, string[] includes = null)
        {
            return this.UnitOfWork.Repository<IEventRepository>().GetListStartMessage(notProcessID, key, messageTypeID, includes);
        }

        public ResultOperation Update(Guid processID, WorkflowProcess _WorkflowProcess)
        {
            ResultOperation resultOperation = new ResultOperation();
            try
            {
                List<Guid> listDeleted = new List<Guid>();
                ElementService elementService = new ElementService(this.UnitOfWork);
                List<sysBpmsEvent> events = this.GetList(null, processID, string.Empty, null);
                foreach (sysBpmsEvent item in events.Where(c => c.TypeLU == (int)sysBpmsEvent.e_TypeLU.StartEvent && !_WorkflowProcess.StartEvents.Any(d => d.ID == c.ElementID)))
                {
                    resultOperation = this.Delete(item.ID);
                    listDeleted.Add(item.ID);
                    if (!resultOperation.IsSuccess)
                        return resultOperation;
                }

                foreach (sysBpmsEvent item in events.Where(c => c.TypeLU == (int)sysBpmsEvent.e_TypeLU.EndEvent && !_WorkflowProcess.EndEvents.Any(d => d.ID == c.ElementID)))
                {
                    resultOperation = this.Delete(item.ID);
                    listDeleted.Add(item.ID);
                    if (!resultOperation.IsSuccess)
                        return resultOperation;
                }

                foreach (sysBpmsEvent item in events.Where(c => c.TypeLU == (int)sysBpmsEvent.e_TypeLU.IntermediateThrow && !_WorkflowProcess.IntermediateThrowEvents.Any(d => d.ID == c.ElementID)))
                {
                    resultOperation = this.Delete(item.ID);
                    listDeleted.Add(item.ID);
                    if (!resultOperation.IsSuccess)
                        return resultOperation;
                }

                foreach (sysBpmsEvent item in events.Where(c => c.TypeLU == (int)sysBpmsEvent.e_TypeLU.IntermediateCatch && !_WorkflowProcess.IntermediateCatchEvents.Any(d => d.ID == c.ElementID)))
                {
                    resultOperation = this.Delete(item.ID);
                    listDeleted.Add(item.ID);
                    if (!resultOperation.IsSuccess)
                        return resultOperation;
                }

                foreach (sysBpmsEvent item in events.Where(c => c.TypeLU == (int)sysBpmsEvent.e_TypeLU.boundary && !_WorkflowProcess.BoundaryEvents.Any(d => d.ID == c.ElementID)))
                {
                    resultOperation = this.Delete(item.ID);
                    listDeleted.Add(item.ID);
                    if (!resultOperation.IsSuccess)
                        return resultOperation;
                }
                events = events.Where(c => !listDeleted.Contains(c.ID)).ToList();
                //StartEvents
                foreach (WorkflowStartEvent item in _WorkflowProcess.StartEvents)
                {
                    sysBpmsEvent _event = events.FirstOrDefault(c => c.ElementID == item.ID);
                    if (_event != null)
                    {
                        //If _event.SubType was changed, It updates _event.
                        if (_event.SubType != (int)item.StartEventType)
                        {
                            _event.SubType = (int)item.StartEventType;
                            resultOperation = this.Update(_event);
                            if (!resultOperation.IsSuccess)
                                return resultOperation;
                        }
                        //Element
                        _event.Element.Name = item.Name;
                        resultOperation = elementService.Update(_event.Element);
                        if (!resultOperation.IsSuccess)
                            return resultOperation;
                    }
                    else
                    {
                        _event = new sysBpmsEvent()
                        {
                            TypeLU = (int)sysBpmsEvent.e_TypeLU.StartEvent,
                            ElementID = item.ID,
                            ID = Guid.NewGuid(),
                            ProcessID = processID,
                            SubType = (int)item.StartEventType,
                            //Element
                            Element = new sysBpmsElement()
                            {
                                ID = item.ID,
                                Name = item.Name,
                                ProcessID = processID,
                                TypeLU = (int)sysBpmsElement.e_TypeLU.Event,
                            }
                        };
                        resultOperation = this.Add(_event);
                        if (!resultOperation.IsSuccess)
                            return resultOperation;

                        resultOperation = elementService.Update(_event.Element);
                        if (!resultOperation.IsSuccess)
                            return resultOperation;
                    }
                }
                //EndEvents
                foreach (WorkflowEndEvent item in _WorkflowProcess.EndEvents)
                {
                    sysBpmsEvent _event = events.FirstOrDefault(c => c.ElementID == item.ID);
                    if (_event != null)
                    {
                        //If _event.SubType was changed, It updates _event.
                        if (_event.SubType != (int)item.EndEventType)
                        {
                            _event.SubType = (int)item.EndEventType;
                            resultOperation = this.Update(_event);
                            if (!resultOperation.IsSuccess)
                                return resultOperation;
                        }
                        //If _event.Element was changed, It updates Element.
                        if (_event.Element.Name != item.Name)
                        {
                            _event.Element.Name = item.Name;
                            resultOperation = elementService.Update(_event.Element);
                            if (!resultOperation.IsSuccess)
                                return resultOperation;
                        }
                    }
                    else
                    {
                        _event = new sysBpmsEvent()
                        {
                            TypeLU = (int)sysBpmsEvent.e_TypeLU.EndEvent,
                            ElementID = item.ID,
                            ID = Guid.NewGuid(),
                            ProcessID = processID,
                            SubType = (int)item.EndEventType,
                            //Element
                            Element = new sysBpmsElement()
                            {
                                ID = item.ID,
                                Name = item.Name,
                                ProcessID = processID,
                                TypeLU = (int)sysBpmsElement.e_TypeLU.Event,
                            }
                        };
                        resultOperation = this.Add(_event);
                        if (!resultOperation.IsSuccess)
                            return resultOperation;
                    }
                }
                //IntermediateThrow
                foreach (WorkflowIntermediateThrowEvent item in _WorkflowProcess.IntermediateThrowEvents)
                {
                    sysBpmsEvent _event = events.FirstOrDefault(c => c.ElementID == item.ID);
                    if (_event != null)
                    {
                        //If _event.SubType was changed, It updates _event.
                        if (_event.SubType != (int)item.IntermediateThrowType)
                        {
                            _event.SubType = (int)item.IntermediateThrowType;
                            resultOperation = this.Update(_event);
                            if (!resultOperation.IsSuccess)
                                return resultOperation;
                        }
                        //If _event.Element was changed, It updates Element.
                        if (_event.Element.Name != item.Name)
                        {
                            _event.Element.Name = item.Name;
                            resultOperation = elementService.Update(_event.Element);
                            if (!resultOperation.IsSuccess)
                                return resultOperation;
                        }
                    }
                    else
                    {
                        _event = new sysBpmsEvent()
                        {
                            TypeLU = (int)sysBpmsEvent.e_TypeLU.IntermediateThrow,
                            ElementID = item.ID,
                            ID = Guid.NewGuid(),
                            ProcessID = processID,
                            SubType = (int)item.IntermediateThrowType,
                            //Element
                            Element = new sysBpmsElement()
                            {
                                ID = item.ID,
                                Name = item.Name,
                                ProcessID = processID,
                                TypeLU = (int)sysBpmsElement.e_TypeLU.Event,
                            }
                        };
                        resultOperation = this.Add(_event);
                        if (!resultOperation.IsSuccess)
                            return resultOperation;
                    }
                }

                //IntermediateCatch
                foreach (WorkflowIntermediateCatchEvent item in _WorkflowProcess.IntermediateCatchEvents)
                {
                    sysBpmsEvent _event = events.FirstOrDefault(c => c.ElementID == item.ID);
                    if (_event != null)
                    {
                        //If _event.SubType was changed, It updates _event.
                        if (_event.SubType != (int)item.IntermediateCatchType)
                        {
                            _event.SubType = (int)item.IntermediateCatchType;
                            resultOperation = this.Update(_event);
                            if (!resultOperation.IsSuccess)
                                return resultOperation;
                        }
                        //If _event.Element was changed, It updates Element.
                        if (_event.Element.Name != item.Name)
                        {
                            _event.Element.Name = item.Name;
                            resultOperation = elementService.Update(_event.Element);
                            if (!resultOperation.IsSuccess)
                                return resultOperation;
                        }
                    }
                    else
                    {
                        _event = new sysBpmsEvent()
                        {
                            TypeLU = (int)sysBpmsEvent.e_TypeLU.IntermediateCatch,
                            ElementID = item.ID,
                            ID = Guid.NewGuid(),
                            ProcessID = processID,
                            SubType = (int)item.IntermediateCatchType,
                            //Element
                            Element = new sysBpmsElement()
                            {
                                ID = item.ID,
                                Name = item.Name,
                                ProcessID = processID,
                                TypeLU = (int)sysBpmsElement.e_TypeLU.Event,
                            }
                        };
                        resultOperation = this.Add(_event);
                        if (!resultOperation.IsSuccess)
                            return resultOperation;
                    }
                }

                //Boundary
                foreach (WorkflowBoundaryEvent item in _WorkflowProcess.BoundaryEvents)
                {
                    sysBpmsEvent _event = events.FirstOrDefault(c => c.ElementID == item.ID);
                    if (_event != null)
                    {
                        //If _event's properties were changed, It updates _event.
                        if (_event.SubType != (int)item.BoundaryType ||
                            _event.RefElementID != item.AttachedToRef ||
                            _event.CancelActivity != item.CancelActivity)
                        {
                            _event.SubType = (int)item.BoundaryType;
                            _event.RefElementID = item.AttachedToRef;
                            _event.CancelActivity = item.CancelActivity;
                            resultOperation = this.Update(_event);
                            if (!resultOperation.IsSuccess)
                                return resultOperation;
                        }
                        //If _event.Element was changed, It updates Element.
                        if (_event.Element.Name != item.Name)
                        {
                            _event.Element.Name = item.Name;
                            resultOperation = elementService.Update(_event.Element);
                            if (!resultOperation.IsSuccess)
                                return resultOperation;
                        }
                    }
                    else
                    {
                        _event = new sysBpmsEvent()
                        {
                            TypeLU = (int)sysBpmsEvent.e_TypeLU.boundary,
                            ElementID = item.ID,
                            ID = Guid.NewGuid(),
                            ProcessID = processID,
                            SubType = (int)item.BoundaryType,
                            RefElementID = item.AttachedToRef,
                            CancelActivity = item.CancelActivity,
                            //Element
                            Element = new sysBpmsElement()
                            {
                                ID = item.ID,
                                Name = item.Name,
                                ProcessID = processID,
                                TypeLU = (int)sysBpmsElement.e_TypeLU.Event,
                            }
                        };
                        resultOperation = this.Add(_event);
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
    }
}
