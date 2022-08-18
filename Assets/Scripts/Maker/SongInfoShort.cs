using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SongInfoShort : MonoBehaviour
{
	public static SongInfoShort Instance { get; set; }

	private void Awake()
	{
		Instance = this;
	}

	[Header("Game Object Set")] 
	public Image bg;
	public Image cover;
	public Text title;
	public Text artist;
	public Image[] ImageList;
	// 0: masked_bottom, 1: masked_center, 2: masked_top, 3: bottom_left, 4: bottom_right, 6: masked_top_left
	// Group: 0: [0,1,2], 1: [3,4], 2: [6]
	public Image Bottom;
	public Image coverShadow;
	public Image DiffImg;
	public Text DiffText;
	public RectTransform SongShadow;

	[Header("Value Set")]
	public float[] minValue;
	// 0: Color Subtract, 1: Color Subtract, 2: Color Subtract
	public float[] minValueAlpha;
	// 0: Bottom, 1: ImageList_Group_1, 2: ImgList_Group_0, 3: DiffText
	public float[] alphas;
	// 0: SongShadow, 1: coverShadow, 2: DiffText

	[Header("Diff Set")]
	public Color[] DiffColor;
	// 0: Past, 1: Present, 2: Future, 3: Beyond, 4: Custom

	[Header("User Set")]
	public string titleSt;
	public string artistSt;
	// Song Info
	public bool enableDiff;
	public bool enableCustomDiff;
	public string CustomDiff;
	public int DiffClass;
	public int Rating;
	public bool RatingPlus;
	// Diff Info
	public Color mainC;
	public Color shadowC;
	// Colors
	public Sprite bgS;
	public Sprite coverS;
	// Sprite >> Image.sprite

	[Header("Other")]
	public Text TopTitle;
	public bool OverrideTopTitle;
	public string TopTitleStr;
	
	public void SetInfo()
	{
		TopTitle.text = OverrideTopTitle ? TopTitleStr : "Arcaea Fanmade";
		// Check value and Set Top Title Text (Default: "Arcaea Fanmade")
		bg.sprite = bgS;
		cover.sprite = coverS;
		// Set Image
		title.text = titleSt;
		artist.text = artistSt;
		// Set Text
		foreach(var i in ImageList) if(i != null) i.color = mainC;
		Bottom.color = new Color(mainC.r-minValue[0],mainC.g-minValue[0],mainC.b-minValue[0],mainC.a-minValueAlpha[0]);
		ImageList[3].color = ImageList[4].color = new Color(mainC.r-minValue[1],mainC.g-minValue[1],mainC.b-minValue[1],mainC.a-minValueAlpha[1]);
		// ImgList_Group_1
		ImageList[0].color = ImageList[1].color = ImageList[2].color = new Color(mainC.r-minValue[2],mainC.g-minValue[2],mainC.b-minValue[2],mainC.a-minValueAlpha[2]);
		// ImgList_Group_0
		SongShadow.GetComponent<Image>().color = new Color(1,1,1,alphas[0]);
		coverShadow.color = new Color(shadowC.r,shadowC.g,shadowC.b,alphas[1]);
		// Set Color
		ResizeText(title.GetComponent<RectTransform>(),title,4290f,0.42f);
		ResizeText(artist.GetComponent<RectTransform>(),artist,5900f,0.3f);
		// Resize Text
		TextToSize(titleSt,title.font,300,artistSt,artist.font,300);
		// Text To Size Value, Set Shadow Length
		SetDiff(DiffClass);
		// Set Diff Info
	}
	//Set Song Info Here

	private void SetDiff(int Diff)
	{
		DiffImg.gameObject.SetActive(enableDiff);
		DiffText.gameObject.SetActive(enableDiff);
		// Set Object Active
		Diff = Mathf.Clamp(Diff,0,4);
		// Diff = 4 is custom Diff Info
		DiffImg.color = new Color(DiffColor[Diff].r,DiffColor[Diff].g,DiffColor[Diff].b,DiffColor[Diff].a-minValueAlpha[3]);
		DiffText.GetComponent<Outline>().effectColor = new Color(DiffColor[Diff].r,DiffColor[Diff].g,DiffColor[Diff].b,alphas[2]);
		string[] DiffStr = new string[4]
		{
			"Past",
			"Present",
			"Future",
			"Beyond"
		};
		// Diff Text
		DiffText.text = Diff == 4 ? $"{CustomDiff}" : $"{(enableCustomDiff ? CustomDiff : DiffStr[Diff])} {(Rating == 0 ? "?" : Rating.ToString())}{(RatingPlus ? "+" : "")}";
		// Check and Set
	}
	
	private void Update()
	{
		if(Input.GetKeyDown(KeyCode.A)) GetCover();
		// Export Image
	}
	
	public string AppPath
	{
		get
		{
			return new DirectoryInfo(Application.dataPath).Parent.FullName + "/ExportCover/";
		}
	}
	private void GetCover()
	{
		if(!Directory.Exists(AppPath)) Directory.CreateDirectory(AppPath);
		// Check Path
		ScreenCapture.CaptureScreenshot(AppPath + $"SongCover_{(titleSt)}_{DateTime.Now.ToString("yyyy.MM.dd.HH.mm.ss")}.png", 1);
		// Set Image Info and Get Image
		Debug.Log("GetCover");
		// Get Image Tips
	}
	
	private void ResizeText(RectTransform r,Text t,float d,float s)
	{
		float p = LayoutUtility.GetPreferredWidth(t.rectTransform);
		if(p>d)
		{
			r.localScale = new Vector3((d/p)*s,s,1);
		}
		else
		{
			r.localScale = new Vector3(s,s,1);
		}
	}
	
	private void TextToSize(string text, Font font, int fontsize, string textA, Font fontA, int fontsizeA)
	{
		float totalLength = 0;
		font.RequestCharactersInTexture(text,fontsize,FontStyle.Normal);
		CharacterInfo characterInfo;
		for(int i = 0; i<text.Length;i++)
		{
			font.GetCharacterInfo(text[i],out characterInfo,fontsize);
			totalLength = totalLength + (characterInfo.advance / 2f);
		}
		
		float totalLengthA = 0;
		fontA.RequestCharactersInTexture(textA,fontsizeA,FontStyle.Normal);
		CharacterInfo characterInfoA;
		for(int i = 0; i<textA.Length;i++)
		{
			fontA.GetCharacterInfo(textA[i],out characterInfoA,fontsizeA);
			totalLengthA = totalLengthA + (characterInfoA.advance / 2f);
		}
		
		float f = 0;
		// Final Length
		
		if(totalLength > totalLengthA)
		{
			f = totalLength/10f;
			if(f > 200)
			{
				f = 200;
			}
			else if(f < 20f)
			{
				f = 20f;
			}
		}
		else if(totalLengthA > totalLength)
		{
			f = totalLengthA/10f;
			if(f > 200)
			{
				f = 200;
			}
			else if(f < 20f)
			{
				f = 20f;
			}
		}
		else if(totalLengthA == totalLength)
		{
			f = totalLength/10f;
		}
		
		SongShadow.sizeDelta = new Vector2(f,100);
		// Set Shadow Length
	}
}
