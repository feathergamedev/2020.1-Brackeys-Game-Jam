using UnityEngine;
using System.Collections;

public class CameraFade : MonoBehaviour
{
    public static CameraFade instance;

    public bool FadeInOnAwake;

    public float FadeInTime = 1;
	public float FadeOutTime = 1;
	public Color FadeInColor = new Color(0.1F, 0.1F, 0.1F, 1);
	public Color FadeOutColor = new Color(0.1F, 0.1F, 0.1F, 1);

	public static bool FadeInIsStart = false;
	public static bool FadeOutIsStart = false;
	public static bool FadeInIsDone = false;
	public static bool FadeOutIsDone = false;

	private Texture2D t2d;
	private GUIStyle gs;

	private static float a = 0;

	void Awake()
	{
		if(instance==null){
			instance = this;
		}
		
		FadeInIsStart = false;
		FadeOutIsStart = false;
		FadeInIsDone = false;
		FadeOutIsDone = false;
		a = 0;

		t2d = new Texture2D(1, 1);
		t2d.SetPixel(0, 0, FadeInColor);
		t2d.Apply();

		gs = new GUIStyle();
		gs.normal.background = t2d;
	}

	void OnGUI()
	{
		GUI.depth = -1000;
		GUI.Label(new Rect(0, 0, Screen.width, Screen.height), t2d, gs);
	}

    void Start(){
        if (FadeInOnAwake)
        {
            FadeIn();
        }
        else
        {
            t2d.SetPixel(0, 0, new Color(FadeInColor.r, FadeInColor.g, FadeInColor.b, 0));
            t2d.Apply();
        }
    }


	void Update()
	{
		if (FadeInIsStart)
		{
			if (a > 0)
			{
				a -= Time.deltaTime / FadeInTime;
				t2d.SetPixel(0, 0, new Color(FadeInColor.r, FadeInColor.g, FadeInColor.b, a));
				t2d.Apply();
			}
			else
			{
				FadeInIsStart = false;
				FadeInIsDone = true;
			}
		}

		if (FadeOutIsStart)
		{
			if (a < 1)
			{
				a += Time.deltaTime / FadeOutTime;
				t2d.SetPixel(0, 0, new Color(FadeOutColor.r, FadeOutColor.g, FadeOutColor.b, a));
				t2d.Apply();
			}
			else
			{
				FadeOutIsStart = false;
				FadeOutIsDone = true;
			}
		}

	}

	// 淡入
	public void FadeIn()
	{
		a = 1;
		FadeInIsStart = true;
		FadeInIsDone = false;
	}

	// 淡出
	public void FadeOut()
	{
		a = 0;
		FadeOutIsStart = true;
		FadeOutIsDone = false;
	}

}