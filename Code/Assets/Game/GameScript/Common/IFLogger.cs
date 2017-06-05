using System;
using System.Collections.Generic;
using UnityEngine;


class IFLogger : IFSingletonBehaviour<IFLogger>
{
	/// <summary>
	/// 是否显示的总开关
	/// </summary>
	public static bool IsShowLog = false;

	public enum LOG_TYPE
	{
		LOG,
		WARNNING,
		ERROR,
	}

	public struct stLog
	{
		public LOG_TYPE logType;
		public string logText;
	}
        		
	private static List<stLog> mLines = new List<stLog>();
	private static int maxCount = 300;
	private static int fontsize = 9999;
		
	public static bool IsShowConsole = false;
	private static int unSee = 0;
	private static int unSeeError = 0;
	private static bool isBottom = true;		

	private int xPos = 400;
	private int yPos = 300;
	private int wValue = 40;
	private int hValue = 40;
	private string buttonName = "拉";
	private bool bScale = false;

	public static Dictionary<string, string> mDebugShow = new Dictionary<string, string>();
	public static void SetDebugShow(string name, string v)
	{
		if (mDebugShow.ContainsKey(name))
		{				
			mDebugShow[name] = v;
		}
		else
		{				
			mDebugShow.Add(name, v);
		}
	}

	public static void DrawBounds(Bounds b)
	{
		Vector3 center = b.center;
		Vector3 vector2 = b.center - b.extents;
		Vector3 vector3 = b.center + b.extents;
		UnityEngine.Debug.DrawLine(new Vector3(vector2.x, vector2.y, center.z), new Vector3(vector3.x, vector2.y, center.z), Color.red);
		UnityEngine.Debug.DrawLine(new Vector3(vector2.x, vector2.y, center.z), new Vector3(vector2.x, vector3.y, center.z), Color.red);
		UnityEngine.Debug.DrawLine(new Vector3(vector3.x, vector2.y, center.z), new Vector3(vector3.x, vector3.y, center.z), Color.red);
		UnityEngine.Debug.DrawLine(new Vector3(vector2.x, vector3.y, center.z), new Vector3(vector3.x, vector3.y, center.z), Color.red);
	}

	void OnApplicationQuit()
	{
		mLines.Clear();
		mLines = null;
	}
        
	
	public static void Log(string format, params object[] arr)
	{
        UnityLog(format, arr);
    }
	private static void WindowsLog(string format, params object[] arr)
	{
		string log = string.Format(format, arr);
		Console.WriteLine(log);
	}

	private static void UnityLog(string format, params object[] arr)
	{
		if (mLines == null)
			return;			
		string log = string.Format(format, arr);
		log = DateTime.Now.ToLocalTime().ToString("HH:mm:ss(fff)") + ": " + log;

		if (mLines.Count > maxCount)
		{
			mLines.RemoveAt(0);
		}
#if CRASH_LOG
			UnityEngine.Debug.Log("[UnityLog]" + log);
#endif

		if (isBottom)
		{
			stLog stlog = new stLog();
			stlog.logType = LOG_TYPE.LOG;
			stlog.logText = log;
			mLines.Add(stlog);

			scrollPosition.y += fontsize;				
		}

        // Instailize
        Instance.ToString();
			
		if (IsShowConsole == false)
		{
			unSee++;
		}
	}
	public static void Exception(Exception e)
	{
		UnityException(e);
	}
	private static void WindowsExceiption(global::System.Exception e)
	{
		Console.WriteLine(e.StackTrace);
	}

	private static void UnityException(Exception e)
	{
		if (mLines == null) return;
		string log = e.StackTrace;
		//log = DateTime.Now.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss(fff)") + ": " + log;
		log = DateTime.Now.ToLocalTime().ToString("HH:mm:ss(fff)") + ": " + log;
		if (mLines.Count > maxCount)
		{
			mLines.RemoveAt(0);
		}


#if CRASH_LOG
			UnityEngine.Debug.LogException(e);
#endif


		if (isBottom)
		{
			stLog stlog = new stLog();
			stlog.logType = LOG_TYPE.WARNNING;
			stlog.logText = log;
			mLines.Add(stlog);

			scrollPosition.y += fontsize;
			//nowCount = mLines.Count;
		}
		//CreateInstance();

		if (IsShowConsole == false)
		{
			unSee++;
		}
	}

	public static void LogWarning(string format, params object[] arr)
	{
		if (mLines == null)
			return;


        string log = string.Format(format, arr);
        log = DateTime.Now.ToLocalTime().ToString("HH:mm:ss(fff)") + ": " + log;            
		if (mLines.Count > maxCount)
		{
			mLines.RemoveAt(0);
		}


#if CRASH_LOG
			UnityEngine.Debug.LogWarning("[UnityLog]" + log);
#endif


		if (isBottom)
		{
			stLog stlog = new stLog();
			stlog.logType = LOG_TYPE.WARNNING;
			stlog.logText = log;
			mLines.Add(stlog);

			scrollPosition.y += fontsize;
			//nowCount = mLines.Count;
		}

        // Instailize
        Instance.ToString();

		if (IsShowConsole == false)
		{
			unSee++;
		}
	}

	public static void LogError(string format, params object[] arr)
	{
		if (mLines == null)
			return;
		format = format.Replace("{}", "[zhongkuohao]");
		string log = string.Format(format, arr);
		log = log.Replace("[zhongkuohao]", "{}");

		log = DateTime.Now.ToLocalTime().ToString("HH:mm:ss(fff)") + ": " + log;
		if (mLines.Count > maxCount)
		{
			mLines.RemoveAt(0);
		}


#if CRASH_LOG
			UnityEngine.Debug.LogError("[UnityLog]" + log);
#endif


		if (isBottom)
		{
			stLog stlog = new stLog();
			stlog.logType = LOG_TYPE.ERROR;
			stlog.logText = log;
			mLines.Add(stlog);

			scrollPosition.y += fontsize;
			//nowCount = mLines.Count;
		}

        // Instailize
        Instance.ToString();

		if (IsShowConsole == false)
		{
			unSee++;
			unSeeError++;
		}
	}

	public static Vector2 scrollPosition = new Vector2(0, 0);
	public string textStr = "";
	private GameObject uiObj;
	private int rightClickCount = 0;
	private float firstRightClickTime = 0;
	private float lastRightClickTime = 0;
	private bool bOpnen = false;
	private void OnGUI()
	{
		if (Application.isEditor && Input.GetKeyDown(KeyCode.F8))
		{
			bOpnen = !bOpnen; 
		}

		if (!bOpnen)
			return ;

		if (!IsShowLog)
		{
			if (Input.GetMouseButtonUp(1))
			{
				if (Time.time - lastRightClickTime < 0.1f) return;
				if (firstRightClickTime == 0)
				{
					firstRightClickTime = Time.time;
					rightClickCount = 0;
				}
				rightClickCount++;
				lastRightClickTime = Time.time;					
				if (rightClickCount >= 20)
				{
					if (Time.time - firstRightClickTime < 5)
					{							
						IsShowLog = true;
					}
					firstRightClickTime = 0;
					rightClickCount = 0;
				}
			}
			return;
		}
		if (IsShowConsole)
		{
			unSee = 0;
			unSeeError = 0;
		}

		string strtext = "Log";
		if (unSee > 0)
		{
			strtext += " (" + unSee + ")";
		}
		if (unSeeError > 0)
		{
			strtext += " (" + unSeeError + ")";
		}
            			
		int tempY = 155;
		if (IFSystemMonitor.Instance.IsShowSystem)
		{
			GUILayout.Space(120);
			tempY = 155 + 30;
		}
		else
		{
			GUILayout.Space(30);
			tempY = 155 - 98;
		}

		GUILayout.BeginHorizontal();
		if (GUILayout.Button(strtext, GUILayout.Width(60), GUILayout.Height(40)))
		{				
			IsShowConsole = !IsShowConsole;
		}
		if (GUILayout.Button("UI", GUILayout.Width(60), GUILayout.Height(40)))
		{
            GameObject go = GameObject.Find("UI Root (2D)");
			if (go != null) uiObj = go;
			if (uiObj != null) uiObj.SetActive(!uiObj.activeSelf);
		}
			
		GUILayout.EndHorizontal();

		if (IsShowConsole == false || mLines.Count == 0)
		{
			//return;
		}
		else
		{
			if (GUI.RepeatButton(new Rect(xPos, yPos, wValue, hValue), buttonName))
			{
				bScale = true;
			}
			if (Input.GetMouseButtonUp(0))
			{
				bScale = false;
			}

			if (bScale)
			{
				Vector3 mousePos = Input.mousePosition;
				xPos = (int)mousePos.x - (int)(wValue / 2);
				yPos = (int)(Screen.height - mousePos.y) - (int)(hValue / 2);

				//buttonName = string.Format("x:{0}, y:{1}, z:{2}", mousePos.x, mousePos.y, mousePos.z);

				GUILayout.Box("拉伸中...", GUILayout.Width(xPos + 28), GUILayout.Height(yPos - tempY));
			}
			else
			{

				//GUI.Box(new Rect(0, 100, 450, 500), "");//外边框

				//GUILayout.Space(110);
				//scrollPosition = GUI.BeginScrollView(new Rect(0, 100, 450, 500), scrollPosition, new Rect(0, 0, 450, 500), false, false);

				//GUIStyle hs=new GUIStyle(GUI.skin.horizontalScrollbar);
				//hs.normal.
				//GUIStyle vs=new GUIStyle(GUI.skin.verticalScrollbar);
				//vs.fixedHeight = 30;
				//scrollPosition = GUILayout.BeginScrollView(scrollPosition, hs, vs, GUILayout.Width(500), GUILayout.Height(500));
				scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(xPos + 28), GUILayout.Height(yPos - tempY));


				int num = 0;
				int count = mLines.Count;
				//string strLog = "";
				while (num < count)
				{
					//string strTemp = mLines[num].logText;

					//GUI.Label(new Rect(0, num * 20, 450, 20), mLines[num]);

					//GUIStyle bb=new GUIStyle(GUI.skin.box);
					GUIStyle bb = GUI.skin.box;
					bb.richText = true;
					bb.alignment = TextAnchor.MiddleLeft;
					switch (mLines[num].logType)
					{
						case LOG_TYPE.LOG:
							bb.normal.textColor = Color.white;//new Color(1, 1, 1);
							bb.fontSize = 14;
							//strTemp = "<color=#ffffffff>" + strTemp + "</color>";
							break;
						case LOG_TYPE.WARNNING:
							bb.normal.textColor = Color.yellow;//new Color(1, 1, 0);
							bb.fontSize = 14;
							//strTemp = "<color=#ffff00ff>" + strTemp + "</color>";
							break;
						case LOG_TYPE.ERROR:
							bb.normal.textColor = Color.red;//new Color(1, 0, 0);
							bb.fontSize = 14;
							//strTemp = "<color=#ff0000ff>" + strTemp + "</color>";
							break;
					}

					GUILayout.Label(mLines[num].logText, bb);//, GUILayout.Height(fontsize));
					GUILayout.Space(-8);


					//strLog += strTemp + "\n";
					num++;

				}
				//GUIStyle bb=GUI.skin.box;
				//bb.richText = true;
				//bb.alignment = TextAnchor.MiddleLeft;
				//bb.fontSize = 14;
				//GUILayout.TextField(strLog, bb);


				//GUILayout.HorizontalScrollbar(30, 70, 0, 100, GUILayout.Height(100));


				//scrollPosition.y = num * 16;

				//GUIStyle bb=new GUIStyle();
				//bb.normal.background = null;    //这是设置背景填充的
				//bb.normal.textColor = new Color(1, 0, 0);   //设置字体颜色的
				//bb.fontSize = 40;       //当然，这是字体颜色

				//GUILayout.TextField(strLog, bb);

				//GUILayout.TextField(strLog);

				//GUILayout.Space(70);
				//GUILayout.Label(strLog, new GUILayoutOption[0]);
				//GUILayout.TextField(strLog);

				//GUI.EndScrollView();
				GUILayout.EndScrollView();
			}

			// 调试用显示
			int index = 1;
			foreach (KeyValuePair<string, string> show in mDebugShow)
			{
				string name = show.Key;
				string value = show.Value;

				GUI.Label(new Rect(xPos + 50, 50 + 20 * index, 300, 20), string.Format("{0}: {1}", name, value));
				index++;
			}
		}
	}
}
