using BodyReport.Message;
using BodyReport.ServiceLayers.Interfaces;

namespace BodyReport.Framework
{
    public static class AppUtils
    {
        public static TUnitType GetUserUnit(IUserInfosService userInfosService, string userId)
        {
            TUnitType result = TUnitType.Imperial;

            if (userId != null && userInfosService != null)
            {
                var userInfo = userInfosService.GetUserInfo(new UserInfoKey() { UserId = userId });
                if (userInfo != null)
                    result = userInfo.Unit;
            }
            return result;
        }
    }
}
