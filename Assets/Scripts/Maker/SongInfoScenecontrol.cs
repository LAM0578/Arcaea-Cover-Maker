using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using Maker.Project;
using Maker.Util;

namespace Maker.Compose
{
    public class SongInfoScenecontrol : MonoBehaviour
    {
        public static SongInfoScenecontrol Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
            SetResolution(true);
            CheckFolder();
        }

        private void CheckFolder()
        {
            if (!Directory.Exists(BackgroundPath)) Directory.CreateDirectory(BackgroundPath);
            if (!Directory.Exists(CoverPath)) Directory.CreateDirectory(CoverPath);
            if (!Directory.Exists(SongInfoFolderPath)) Directory.CreateDirectory(SongInfoFolderPath);
            if (!File.Exists(JsonPath)) File.Create(JsonPath);
            // Check Folder and File, if retun false Create Folder or File
        }

        private void Start()
        {
            SetInfoByJson();
        }

        public Camera Camera;
        // Check "Is16By9" used
        public GameObject[] ObjList;
        // 0: 16:9, 1: 16:10
        public Material BlurMat;
        // Blur Background Material
        private float BlurRange = 14.3f;
        // Blur Shader Effect Range

        public SongInfoJson SongInfoJson { get; set; }
        // Song Info from Json

        private Vector2 screenResolution;
        // Used for Update

        public bool Is16By9
        {
            get
            {
                return 1.77777779f - 1f * (float)Camera.pixelWidth / (float)Camera.pixelHeight < 0.1f;
            }
        }

        private void Update()
        {
            if (ObjList.Length < 2) return;
            ObjList[0].SetActive(Is16By9);
            ObjList[1].SetActive(!Is16By9);
            // Set Obj Show

            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.F)) SetResolution(false);
            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.G)) SetResolution(true);
            // Set Resolution

            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.R)) SetInfoByJson();
            // Reload Info

#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.R)) SetInfoByJson();
            // Reload Info
#endif

            if (screenResolution != new Vector2(Camera.pixelWidth, Camera.pixelHeight))
            {
                screenResolution = new Vector2(Camera.pixelWidth, Camera.pixelHeight);
                BlurMat.SetFloat("_Size", BlurRange * ((float)Camera.pixelHeight) / (Is16By9 ? 1080f : 1000f));
                // Set Blur
            }
        }

        public string SongInfoFolderPath { get { return Path.Combine(new DirectoryInfo(Application.dataPath).Parent.FullName, "SongInfo/"); } }
        public string BackgroundPath { get { return Path.Combine(SongInfoFolderPath, "Background/"); } }
        public string CoverPath { get { return Path.Combine(SongInfoFolderPath, "Cover/"); } }
        public string JsonPath { get { return Path.Combine(SongInfoFolderPath, "Info.json"); } }

        public void SetInfoByJson()
        {
            try
            {
                SongInfoJson = JsonConvert.DeserializeObject<SongInfoJson>(File.ReadAllText(JsonPath));
                WriteJson();
                // Deserialize Object and Write Readed Json
            }
            catch
            {
                RefreshJson();
            }
            if (SongInfoJson == null)
            {
                RefreshJson();
            }
            // SongInfoEditManager.Instance.LoadCurrentInfo(SongInfoJson); 
            // Load Current Info
            BlurRange = SongInfoJson.SceneSettings.UseBlur ? SongInfoJson.SceneSettings.BlurRange : 0;
            BlurMat.SetFloat("_Size", BlurRange * ((float)Camera.pixelHeight) / (Is16By9 ? 1080f : 1000f));
            BlurMat.SetColor("_Color", SongInfoJson.SceneSettings.BlurColor.HexToColor());
            // Set Blur
            List<SongInfoShort> songInfoList = new List<SongInfoShort>();
            for (int i = 0; i < ObjList.Length; i++)
            {
                songInfoList.Add(ObjList[i].GetComponent<SongInfoShort>());
            }
            foreach (SongInfoShort s in songInfoList)
            {
                s.titleSt = SongInfoJson.Title;
                s.artistSt = SongInfoJson.Artist;
                // Base Info

                s.enableDiff = SongInfoJson.EnableDiff;
                s.enableCustomDiff = SongInfoJson.EnableCustomDiff;
                s.CustomDiff = SongInfoJson.CustomDiffString;
                s.DiffClass = SongInfoJson.RatingClass;
                s.Rating = SongInfoJson.Rating;
                s.RatingPlus = SongInfoJson.RatingPlus;
                // Diff Info

                s.mainC = SongInfoJson.MainColor.HexToColor(SongInfoJson.MainAlpha);
                s.shadowC = SongInfoJson.ShadowColor.HexToColor(SongInfoJson.ShadowAlpha);
                s.DiffColor[4] = SongInfoJson.CustomDiffColor.HexToColor(0.35f);
                // Color Info

                s.minValue = new float[] {
                SongInfoJson.ColorSubtract.Center,
                SongInfoJson.ColorSubtract.Side,
                SongInfoJson.ColorSubtract.Bottom
            };
                s.minValueAlpha = new float[] {
                SongInfoJson.AlphaSubtract.Bottom,
                SongInfoJson.AlphaSubtract.Center,
                SongInfoJson.AlphaSubtract.Side,
                SongInfoJson.AlphaSubtract.Diff
            };
                s.alphas = new float[] {
                SongInfoJson.Alphas.SongShadowAlpha,
                SongInfoJson.Alphas.CoverShadowAlpha,
                SongInfoJson.Alphas.DiffTextAlpha
            };
                // Some value set

                s.bg.preserveAspect = SongInfoJson.SceneSettings.BackgroundPreserveAspect;
                s.bg.gameObject.transform.localScale = SongInfoJson.SceneSettings.BackgroundScale.JsonVector2ToVector2();

                if (File.Exists(Path.Combine(BackgroundPath, SongInfoJson.BackgroundPath)))
                {
                    Texture2D bgt = new Texture2D(1, 1);
                    bgt.LoadImage(File.ReadAllBytes(Path.Combine(BackgroundPath, SongInfoJson.BackgroundPath)), true);
                    Sprite bgspr = Sprite.Create(bgt, new Rect(0, 0, bgt.width, bgt.height), new Vector2(0.5f, 0.5f));
                    s.bgS = bgspr;
                    if (SongInfoJson.UseAutoMainColor)
                    {
                        s.mainC = (bgt.GetReadableTexture()).GetAvgColor(SongInfoJson.MainAlpha);
                    }
                }
                else
                {
                    s.bgS = null;
                }
                // Check has background

                if (File.Exists(Path.Combine(CoverPath, SongInfoJson.CoverPath)))
                {
                    Texture2D covt = new Texture2D(1, 1);
                    covt.LoadImage(File.ReadAllBytes(Path.Combine(CoverPath, SongInfoJson.CoverPath)), true);
                    Sprite covspr = Sprite.Create(covt, new Rect(0, 0, covt.width, covt.height), new Vector2(0.5f, 0.5f));
                    s.coverS = covspr;
                }
                else
                {
                    s.coverS = null;
                }
                // Check has cover

                s.OverrideTopTitle = SongInfoJson.OverrideTopTitle;
                s.TopTitleStr = SongInfoJson.OverrideTopTitleString;
                // Set Top Title

                s.SetInfo();
                // Set SongInfo
            }
        }

        private void RefreshJson()
        {
            SongInfoJson = new SongInfoJson();
            WriteJson();
        }

        public void WriteJson()
        {
            File.WriteAllText(JsonPath, JsonConvert.SerializeObject(SongInfoJson).JsonFormatting());
        }

        private void SetResolution(bool isBili)
        {
            Resolution[] resolutions = Screen.resolutions;
            Vector2Int finalResolution = new Vector2Int();
            float widthBase = isBili ? 1600 : 1920;
            float heightBase = isBili ? 1000 : 1080;
            float width = resolutions[resolutions.Length - 1].width * 0.95f;
            float height = resolutions[resolutions.Length - 1].height * 0.95f;
            finalResolution.x = int.Parse((widthBase * (width / 1920)).ToString("f0"));
            finalResolution.y = int.Parse((heightBase * (height / 1080)).ToString("f0"));
            Screen.SetResolution(finalResolution.x, finalResolution.y, false);
            Debug.Log(finalResolution);
        }

#if UNITY_EDITOR
        private void OnApplicationQuit()
        {
            BlurMat.SetColor("_Color", Color.white);
            // Reset Blur Color when quit (on editor)
        }
#endif

    }
}
