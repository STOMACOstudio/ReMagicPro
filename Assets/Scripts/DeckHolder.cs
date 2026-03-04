using System.Collections.Generic;

public static class DeckHolder
{
    public static List<CardData> SelectedDeck;
    public static string FavouriteCardName;
    public static bool IsStarterDeckRewardCollected;
    public static string DeckEditorReturnSceneName;
    public static bool IsDeckEditorOpenedAdditively;
    public static bool RestoreGameplayCursorOnDeckEditorClose;

    public static string GetDeckEditorReturnScene(string fallbackSceneName = "MapScene")
    {
        return string.IsNullOrEmpty(DeckEditorReturnSceneName) ? fallbackSceneName : DeckEditorReturnSceneName;
    }
}
