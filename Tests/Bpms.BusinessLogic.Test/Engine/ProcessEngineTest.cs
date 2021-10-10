using DynamicBusiness.BPMS.BusinessLogic;
using DynamicBusiness.BPMS.BusinessLogic.Migrations;
using DynamicBusiness.BPMS.Domain;
using DynamicBusiness.BPMS.EngineApi.Controllers;
using DynamicBusiness.BPMS.SharedPresentation;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using Xunit;
using System.Linq;

namespace Bpms.BusinessLogic.Test
{
    public class ProcessEngineTest : BaseIntegratedTest
    {
        public ProcessEngineTest()
        {
            new SeedDataBase().SeedAll();
        }

        [Fact]
        public void BeginTask_Recruitment_Process_With_Local_Variable_ShouldPassSuccessfully()
        {
            //Arrange
            string userName = "bpms_employee";
            using (ProcessEngine processEngine = new ProcessEngine(new EngineSharedModel(currentThread: null, currentProcessID: new Guid("3E9E69D2-9232-4F07-8B63-08FBDD778F45"), baseQueryModel: new List<QueryModel>(), currentUserName: userName, apiSessionId: "")))
            {
                using (ThreadTaskService threadTaskService = new ThreadTaskService())
                {
                    using (UserService userService = new UserService())
                    {
                        //Act 
                        (ResultOperation result, List<MessageModel> messageList) = processEngine.BegingProcess(userService.GetInfo(userName)?.ID);
                        //Assert
                        Assert.True(result.IsSuccess);
                        sysBpmsThreadTask threadTask = threadTaskService.GetList(((sysBpmsThread)result.CurrentObject).ID, (int)sysBpmsTask.e_TypeLU.UserTask, null, (int)sysBpmsThreadTask.e_StatusLU.New).LastOrDefault();
                        Assert.NotNull(threadTask);
                    }
                }
            }
        }

        [Fact]
        public void BeginTask_Recruitment_Process_With_Local_Variable_MustFail()
        {
            //Arrange
            string userName = "UnauthorizedUser";
            using (TaskService taskService = new TaskService())
            {
                var task = taskService.GetInfo(new Guid("7a5b0b25-9da5-4e4e-b743-4f51055b95e0"));
                task.UserID = "1A7BDA0B-1931-455D-A964-03A76708DB0F";
                task.OwnerTypeLU = (int)sysBpmsTask.e_OwnerTypeLU.User;
                taskService.Update(task);
            }
            using (ProcessEngine processEngine = new ProcessEngine(new EngineSharedModel(currentThread: null, currentProcessID: new Guid("3E9E69D2-9232-4F07-8B63-08FBDD778F45"), baseQueryModel: new List<QueryModel>(), currentUserName: userName, apiSessionId: "")))
            {
                using (ThreadTaskService threadTaskService = new ThreadTaskService())
                {
                    using (UserService userService = new UserService())
                    {
                        //Act 
                        (ResultOperation result, List<MessageModel> messageList) = processEngine.BegingProcess(userService.GetInfo(userName)?.ID);
                        //Assert
                        Assert.False(result.IsSuccess); 
                        Assert.Null(result.CurrentObject);
                    }
                }
            }
        }
    }
}
