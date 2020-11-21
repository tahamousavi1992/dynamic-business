using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.Domain
{
    public interface IUserCodeHelper
    {
        sysBpmsUser GetUserByUserName(string userName);
        sysBpmsUser GetUserByID(object id);
        string GetUserPropertyByID(Guid id, string propertyName);
        string GetUserPropertyByUserName(string userName, string propertyName);
        DotNetNuke.Entities.Users.UserInfo GetSiteUser(string userName);
        bool CreateBpmsUser(string userName, string firstName, string LastName, string email, string mobile, string telePhone);
        bool CreateSiteUser(string userName, string firstName, string LastName, string email, string password, bool doLogin = true, bool createBpms = true);
        bool CreateSiteUser(DotNetNuke.Entities.Users.UserInfo userInfo, string password, bool doLogin = true);
    }
}
