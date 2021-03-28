using DotNetNuke.Common;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security.Membership;
using DotNetNuke.Security.Roles;
using DotNetNuke.Services.Localization;
using DotNetNuke.Services.Mail;
using DotNetNuke.Services.Social.Notifications;
using DynamicBusiness.BPMS.Domain;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DynamicBusiness.BPMS.BusinessLogic
{
    public class UserCodeHelper : IUserCodeHelper
    {
        public EngineSharedModel EngineSharedModel { get; set; }
        public IUnitOfWork UnitOfWork { get; set; }
        public UserCodeHelper(EngineSharedModel engineSharedModel, IUnitOfWork unitOfWork)
        {
            this.EngineSharedModel = engineSharedModel;
            this.UnitOfWork = unitOfWork;
        }
        public sysBpmsUser GetUserByUserName(string userName)
        {
            return new UserService(this.UnitOfWork).GetInfo(userName);
        }

        public sysBpmsUser GetUserByID(object id)
        {
            return new UserService(this.UnitOfWork).GetInfo(id.ToGuidObj());
        }

        public string GetUserPropertyByID(Guid id, string propertyName)
        {
            return typeof(sysBpmsUser).GetProperty(propertyName).GetValue(new UserService(this.UnitOfWork).GetInfo(id), null).ToStringObjNull();
        }

        public string GetUserPropertyByUserName(string userName, string propertyName)
        {
            return typeof(sysBpmsUser).GetProperty(propertyName).GetValue(new UserService(this.UnitOfWork).GetInfo(userName), null).ToStringObjNull();
        }

        public UserInfo GetSiteUser(string userName)
        {
            return UserController.GetUserByName(DotNetNuke.Entities.Portals.PortalSettings.Current.PortalId, userName);
        }

        public bool CreateBpmsUser(string userName, string firstName, string LastName, string email, string mobile, string telePhone)
        {
            if (string.IsNullOrWhiteSpace(userName))
                return false;
            UserService userService = new UserService(this.UnitOfWork);
            if (userService.GetInfo(userName) == null)
            {
                sysBpmsUser user = new sysBpmsUser(Guid.NewGuid(), userName, firstName.ToStringObj().Trim(), LastName.ToStringObj().Trim(), email.ToStringObj().Trim(), telePhone.ToStringObj().Trim(), mobile.ToStringObj().Trim());
                userService.Add(user, null);
                if (this.GetSiteUser(user.Username) == null)
                {
                    bool createResult = this.CreateSiteUser(user.Username, user.FirstName, user.LastName, user.Email, UserController.GeneratePassword(), false);
                }
            }
            return true;
        }

        public bool CreateSiteUser(string userName, string firstName, string LastName, string email, string password, bool doLogin = true, bool createBpms = true)
        {
            UserInfo userInfo = new UserInfo();
            userInfo.Username = userName;
            userInfo.FirstName = firstName.ToStringObj().Trim();
            userInfo.LastName = LastName.ToStringObj().Trim();
            userInfo.Email = email.ToStringObj().Trim();
            userInfo.Membership.Approved = DotNetNuke.Entities.Portals.PortalSettings.Current.UserRegistration == (int)Globals.PortalRegistrationType.PublicRegistration;
            userInfo.Membership.CreatedDate = System.DateTime.Now;
            userInfo.Membership.Password = password;
            userInfo.IsDeleted = false;
            userInfo.PortalID = DotNetNuke.Entities.Portals.PortalSettings.Current.PortalId;
            bool result = this.CreateDnnUserInfo(userInfo, doLogin);
            if (createBpms)
            {
                UserService userService = new UserService(this.UnitOfWork);
                if (userService.GetInfo(userInfo.Username) == null)
                {
                    sysBpmsUser user = new sysBpmsUser(Guid.NewGuid(), userInfo.Username, userInfo.FirstName, userInfo.LastName, userInfo.Username, userInfo.Email, userInfo.Profile.Cell.ToStringObj());
                    userService.Add(user, null);
                }
            }
            return result;
        }

        public bool CreateSiteUser(UserInfo userInfo, string password, bool doLogin = true)
        {
            userInfo.Membership.Approved = true;
            userInfo.Membership.CreatedDate = System.DateTime.Now;
            userInfo.Membership.Password = password;
            userInfo.IsDeleted = false;
            return this.CreateDnnUserInfo(userInfo, doLogin);
        }

        private bool CreateDnnUserInfo(UserInfo userInfo, bool doLogin)
        {
            var portalSettings = DotNetNuke.Entities.Portals.PortalSettings.Current;
            UserCreateStatus userCreateStatus = UserController.CreateUser(ref userInfo);
            if (userCreateStatus == UserCreateStatus.Success)
            {
                //send notification to portal administrator of new user registration
                //check the receive notification setting first, but if register type is Private, we will always send the notification email.
                //because the user need administrators to do the approve action so that he can continue use the website.
                if (portalSettings.EnableRegisterNotification || portalSettings.UserRegistration == (int)DotNetNuke.Common.Globals.PortalRegistrationType.PrivateRegistration)
                {
                    Mail.SendMail(userInfo, DotNetNuke.Services.Mail.MessageType.UserRegistrationAdmin, portalSettings);
                    SendAdminNotification(userInfo, portalSettings);
                }
                //because of some bugs in dnn, sometimes it must be approved after getting it from dnn. 
                if (!userInfo.Membership.Approved)
                { 
                    var current = UserController.GetUserByName(userInfo.Username);
                    //due to dnn bug I have to update user again.
                    current.FirstName = userInfo.FirstName.ToStringObj().Trim();
                    current.LastName = userInfo.LastName.ToStringObj().Trim();

                    current.Membership.Approved = true;
                    UserController.UpdateUser(userInfo.PortalID, current);
                    UserController.ApproveUser(current);
                }
                if (doLogin)
                {
                    UserLoginStatus loginStatus = UserLoginStatus.LOGIN_FAILURE;
                    UserController.UserLogin(portalSettings.PortalId, userInfo.Username, userInfo.Membership.Password, "", portalSettings.PortalName, "", ref loginStatus, false);
                }
                Mail.SendMail(userInfo, DotNetNuke.Services.Mail.MessageType.UserRegistrationPublic, portalSettings);

                return true;
            }
            else
                return false;
        }

        private void SendAdminNotification(UserInfo newUser, PortalSettings portalSettings)
        {
            var notificationType = newUser.Membership.Approved ? "NewUserRegistration" : "NewUnauthorizedUserRegistration";
            var locale = LocaleController.Instance.GetDefaultLocale(portalSettings.PortalId).Code;
            var notification = new Notification
            {
                NotificationTypeID = NotificationsController.Instance.GetNotificationType(notificationType).NotificationTypeId,
                IncludeDismissAction = newUser.Membership.Approved,
                SenderUserID = portalSettings.AdministratorId,
                Subject = GetNotificationSubject(locale, newUser, portalSettings),
                Body = GetNotificationBody(locale, newUser, portalSettings),
                Context = newUser.UserID.ToString(CultureInfo.InvariantCulture)
            };
            var adminrole = RoleController.Instance.GetRoleById(portalSettings.PortalId, portalSettings.AdministratorRoleId);
            var roles = new List<RoleInfo> { adminrole };
            NotificationsController.Instance.SendNotification(notification, portalSettings.PortalId, roles, new List<UserInfo>());
        }

        private string GetNotificationBody(string locale, UserInfo newUser, PortalSettings portalSettings)
        {
            const string text = "EMAIL_USER_REGISTRATION_ADMINISTRATOR_BODY";
            return LocalizeNotificationText(text, locale, newUser, portalSettings);
        }

        private string LocalizeNotificationText(string text, string locale, UserInfo user, PortalSettings portalSettings)
        {
            //This method could need a custom ArrayList in future notification types. Currently it is null
            return Localization.GetSystemMessage(locale, portalSettings, text, user, Localization.GlobalResourceFile, null, "", portalSettings.AdministratorId);
        }

        private string GetNotificationSubject(string locale, UserInfo newUser, PortalSettings portalSettings)
        {
            const string text = "EMAIL_USER_REGISTRATION_ADMINISTRATOR_SUBJECT";
            return LocalizeNotificationText(text, locale, newUser, portalSettings);
        }

    }
}
