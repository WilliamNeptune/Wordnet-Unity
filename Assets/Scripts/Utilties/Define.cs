using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security;
using UnityEngine;

public static class Define
{
    public enum PART_OF_SPEECH
    {
        NOUN = 1,
        VERB = 2,
        ADJECTIVE = 3,
        ADVERB = 4,
        SATELLITE = 5,
        ADJSAT = 6,
        PREPOSITION = 7,   //7 and 8 was unused
        CONJUNCTION = 8,
    }

    public static string ColorPOSSelected = "#FF4E00";
    public static string ColorPOSUnselected = "#ffffff";
    public static string ColorSynsetMarkerSelected = "#FF6023";
    public static string ColorSynsetMarkerUnselected = "#8D9398";
    public enum AdType
    {
        TrendingAdBanner = 1,
        WordNetAdBanner = 2,
        SynonymAdBanner = 3,
        SynonymAdNative = 4,
    }

    public enum CurrentPage
    {
        WordNetTrending = 4,
        WordNet = 0,
        Bookmark = 1,
        Setting = 2,
    }
    public static string PRODUCT_ID = "com.linwood.synonymnet.membership";
    public static Color ColorWhite = new Color(245/255f, 245/255f, 247/255f, 1);
    public static Color ColorBlack = new Color(0, 0, 0, 1);

    public enum SettingPage
    {
        Login = 0,
        Subscribe = 1,
        LangSetting = 2,
        ColorModeSetting = 3,
        Rating = 4,
        Feedback = 5,
        About = 6,
        AccountSettings = 7,
    }
}
