using UnityEngine;
using UnityEditor;
using com.relativedistance.UnityAVIExport;
using System.IO;

public class AVIExportEditor : EditorWindow
{
	Camera cam;
	int width = 1280;
	int height = 720;
	int quality = 90;
	float fps = 30;
	string path = "";
	bool enterPlayMode = true;
	bool currentlyRecording = false;
	bool waitingForPlayMode = false;
	
	static bool inPlayMode = false;
	static string windowIconPath; 
	
	AVIExport avi;
	
	[MenuItem("Window/AVI Export")]
	static void Init()
	{
		AVIExportEditor window = (AVIExportEditor)EditorWindow.GetWindow(typeof(AVIExportEditor));
		GUIContent titleContent = new GUIContent ("AVI Export", AssetDatabase.LoadAssetAtPath<Texture> (windowIconPath));
		window.titleContent = titleContent;
		window.Show();
	}

	void OnEnable()
	{
		avi = new AVIExport();
		string dekstopPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop).Replace('\\','/');
		iconAssetPath();
		cam = Camera.main;
		path = dekstopPath + "/" + "Video.avi";
		EditorApplication.playModeStateChanged += PlayModeStateChanged;
	}
	
	void OnGUI()
	{
		GUIStyle guiStyle = new GUIStyle();
		guiStyle.padding = new RectOffset( 10, 10, 10, 10 );
		
		EditorGUILayout.BeginVertical(guiStyle);
		
		GUILayout.Label( "AVI Export Settings" , EditorStyles.boldLabel );
		
		GUILayout.Space(8);
		EditorGUI.indentLevel++;
		
		cam = EditorGUILayout.ObjectField("Camera" , cam,  typeof(Object),true) as Camera;
		width = EditorGUILayout.IntField("Width", width);
		height = EditorGUILayout.IntField("Height", height);
		fps = EditorGUILayout.FloatField("Framerate", fps); 
		quality = EditorGUILayout.IntSlider("Quality", quality, 50,100 ); 
		
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.TextField("File",path,GUILayout.ExpandWidth(true));
		if(GUILayout.Button("Browse",GUILayout.ExpandWidth(false)))
			path = EditorUtility.SaveFilePanel("File",path,"Video.avi","avi");
		EditorGUILayout.EndHorizontal();
		
		enterPlayMode = EditorGUILayout.Toggle("Enter Play Mode", enterPlayMode);
		
		GUILayout.Space(10);
		
		string buttonName = "Start Recording";
		if (currentlyRecording) { buttonName = "Stop Recording";}
		if (GUILayout.Button(buttonName, GUILayout.Height(40)))
		{
			currentlyRecording = !currentlyRecording;
			if (!currentlyRecording)
			{
				avi.stopRecording();
				File.WriteAllBytes(path, avi.getAVIByteArray());
			}
			else
			{
				if (enterPlayMode)
				{
					waitingForPlayMode = true;
					EditorApplication.isPlaying = true;
				}
				else
				{
					avi.Init(cam,width,height,fps,60,quality);
					avi.startRecording();	
				}
			}
		}
		
		// Waits for fully in play mode before starting to record.
		if (waitingForPlayMode)
		{
			if (inPlayMode)
			{
				waitingForPlayMode = false;
				avi.Init(cam,width,height,fps,60,quality);
				avi.startRecording();	
			}
		}
		
		EditorGUILayout.EndVertical();
	}
	
	void Update()
	{
		avi.DoUpdate();
	}
	 
	void iconAssetPath()
	{
		windowIconPath = AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(this));
		
		string iconFilename = "aviwindowicon_light.png";
		if (EditorGUIUtility.isProSkin)
		{
			iconFilename = "aviwindowicon_dark.png";
		}
		
		windowIconPath = windowIconPath.Substring(0, windowIconPath.LastIndexOf('/')) + "/EditorResources/" + iconFilename;
	}
	
	private static void PlayModeStateChanged(PlayModeStateChange state)
	{
		if (state == PlayModeStateChange.EnteredPlayMode)
		{
			inPlayMode = true;
		}
		else
		{
			inPlayMode = false;
		}
	}
}