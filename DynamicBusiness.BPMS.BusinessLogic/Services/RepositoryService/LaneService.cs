using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicBusiness.BPMS.Domain;

namespace DynamicBusiness.BPMS.BusinessLogic
{
    public class LaneService : ServiceBase
    {
        public LaneService(IUnitOfWork unitOfWork = null) : base(unitOfWork) { }

        public ResultOperation Add(sysBpmsLane lane)
        {
            ResultOperation resultOperation = new ResultOperation();
            if (resultOperation.IsSuccess)
            {
                this.UnitOfWork.Repository<ILaneRepository>().Add(lane);
                this.UnitOfWork.Save();
            }
            return resultOperation;
        }

        public ResultOperation Update(sysBpmsLane lane)
        {
            ResultOperation resultOperation = new ResultOperation();
            if (resultOperation.IsSuccess)
            {
                this.UnitOfWork.Repository<ILaneRepository>().Update(lane);
                this.UnitOfWork.Save();
            }
            return resultOperation;
        }

        public ResultOperation Delete(Guid ID)
        {
            ResultOperation resultOperation = new ResultOperation();
            try
            {
                sysBpmsLane lane = this.GetInfo(ID);
                this.BeginTransaction();
                this.UnitOfWork.Repository<ILaneRepository>().Delete(ID);
                this.UnitOfWork.Save();
                resultOperation = new ElementService(this.UnitOfWork).Delete(lane.ElementID, lane.ProcessID);
            }
            catch (Exception ex)
            {
                return base.ExceptionHandler(ex);
            }
            base.FinalizeService(resultOperation);

            return resultOperation;
        }

        public sysBpmsLane GetInfo(Guid ID)
        {
            return this.UnitOfWork.Repository<ILaneRepository>().GetInfo(ID);
        }

        public List<sysBpmsLane> GetList(Guid? ProcessID)
        {
            return this.UnitOfWork.Repository<ILaneRepository>().GetList(ProcessID);
        }

        public void Update(Guid processID, WorkflowProcess _WorkflowProcess)
        {
            List<sysBpmsLane> Lanes = this.GetList(processID);
            var CurrentWorkflowLane = WorkflowLane.GetAllLanes(_WorkflowProcess.LaneSet);
            foreach (sysBpmsLane item in Lanes.Where(c => !CurrentWorkflowLane.Any(d => d.ID == c.ElementID)))
            {
                this.Delete(item.ID);
            }

            //StartEvents
            foreach (WorkflowLane item in CurrentWorkflowLane)
            {
                sysBpmsLane lane = Lanes.FirstOrDefault(c => c.ElementID == item.ID);
                if (lane != null)
                {
                    this.Update(lane);
                    //Element
                    lane.sysBpmsElement.Name = item.Name;
                    new ElementService(this.UnitOfWork).Update(lane.sysBpmsElement);
                }
                else
                {
                    lane = new sysBpmsLane()
                    {
                        ID = Guid.NewGuid(),
                        ElementID = item.ID,
                        ProcessID = processID,
                        //Element
                        sysBpmsElement = new sysBpmsElement()
                        {
                            ID = item.ID,
                            Name = item.Name,
                            ProcessID = processID,
                            TypeLU = (int)sysBpmsElement.e_TypeLU.Lane,
                        }
                    };
                    this.Add(lane);
                    new ElementService(this.UnitOfWork).Update(lane.sysBpmsElement);
                }
            }

        }


    }
}
