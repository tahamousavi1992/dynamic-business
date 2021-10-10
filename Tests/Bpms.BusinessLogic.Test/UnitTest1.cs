using System;
using Xunit;

namespace Bpms.BusinessLogic.Test
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            Assert.Equal("", "");
        }
    }
}
//using DynamicBusiness.BPMS.BusinessLogic;
//using DynamicBusiness.BPMS.Domain;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using Moq;
//using System;

//namespace Bpms.Test.Engine
//{
//    [TestClass]
//    public class UnitTest1
//    {
//        ConfigurationService configurationService = null;
//        private readonly Mock<IConfigurationRepository> configurationRepoMock = new Mock<IConfigurationRepository>();
//        UnitOfWork unitOfWork = new UnitOfWork(null);
//        public UnitTest1()
//        {
//            configurationService = new ConfigurationService(unitOfWork);
//        }

//        [TestMethod]
//        public void TestMethod_Mock()
//        {
//            //Arrange
//            unitOfWork.Repository<IConfigurationRepository>(configurationRepoMock.Object);

//            configurationRepoMock.Setup(x => x.GetValue(It.IsAny<string>())).Returns("");

//            //Act
//            var result = configurationService.GetValue("");

//            //Assert
//            Assert.AreEqual("", result);
//        }
//    }
//}
