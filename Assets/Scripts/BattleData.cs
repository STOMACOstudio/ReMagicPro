public static class BattleData
{
    private const string DefaultReturnSceneName = "MapScene";

    public static string CurrentZoneId = null;
    public static string LastCompletedZoneId = null;

    //public static MapZone CurrentZone = null;
    public static bool ZoneJustCompleted = false;

    public static string CurrentDeckKey = null;
    public static string ReturnSceneName = null;
    public static bool IsBattleOpenedAdditively = false;

    public static string GetReturnScene(string fallbackSceneName = DefaultReturnSceneName)
    {
        return string.IsNullOrEmpty(ReturnSceneName) ? fallbackSceneName : ReturnSceneName;
    }
}
