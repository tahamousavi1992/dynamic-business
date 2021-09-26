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
        public void BeginTask_ShouldPassSuccessfully()
        {
            //Arrange
            string userName = "bpms_expert";
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
    }
}
