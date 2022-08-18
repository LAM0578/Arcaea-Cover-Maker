using System;

namespace Maker.Project
{
    [Serializable]
    public class SongInfoJson
    {
        public string Title = "";
        public string Artist = "";
        public bool EnableDiff;
        public bool EnableCustomDiff;
        public string CustomDiffString = "";
        public int RatingClass;
        public int Rating;
        public bool RatingPlus;
        public bool UseAutoMainColor;
        public string MainColor = "#FFFFFF";
        public float MainAlpha = 0.5f;
        public string ShadowColor = "#000000";
        public float ShadowAlpha = 0f;
        public string CustomDiffColor = "#000000";
        public bool OverrideTopTitle;
        public string OverrideTopTitleString = "";
        public string BackgroundPath = "";
        public string CoverPath = "";
        public ColorSubtract ColorSubtract = new ColorSubtract();
        public AlphaSubtract AlphaSubtract = new AlphaSubtract();
        public Alphas Alphas = new Alphas();
        public SceneSettings SceneSettings = new SceneSettings();
    }

    [Serializable]
    public class ColorSubtract
    {
        public float Center = 0.2f;
        public float Side = -0.1f;
        public float Bottom = -0.1f;
    }
    [Serializable]
    public class AlphaSubtract
    {
        public float Bottom = -0.1f;
        public float Side = -0.05f;
        public float Center = -0.1f;
        public float Diff = -0.2f;
    }
    [Serializable]
    public class Alphas
    {
        public float SongShadowAlpha = 0.75f;
        public float CoverShadowAlpha = 0.2f;
        public float DiffTextAlpha = 0.5f;
    }
    [Serializable]
    public class SceneSettings
    {
        public bool UseBlur = true;
        public string BlurColor = "#FFFFFF";
        public float BlurRange = 14.3f;
        public bool BackgroundPreserveAspect;
        public vector2 BackgroundScale = new vector2();
    }
    [Serializable]
    public class vector2
    {
        public float x = 1;
        public float y = 1;
    }
}
