using UnityEngine;

public static class DeviceUtils
{
    public static bool IsMobile()
    {
        return Application.isMobilePlatform || SystemInfo.deviceType == DeviceType.Handheld;
    }

}
