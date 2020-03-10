using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using com.relativedistance.UnityAVIExport;
using System.Runtime.InteropServices;
using System.IO;

public class AVIExportDemo : MonoBehaviour
{
	// Plugin is used for downloading AVI directly from the browser
	[DllImport("__Internal")]
	private static extern void DownloadFile(byte[] array, int byteLength, string fileName);
	
	public Camera cam;
	public int width = 1280;
	public int height = 720;
	public float AVIFps = 30;
	public int gameFps = 60;
	
	[Range(50,100)]
	public int quality = 90;
	public string fileName = "test.avi";
	
	AVIExport avi;
	
	void Start()
	{
		avi = new AVIExport();
		
		#if UNITY_EDITOR || UNITY_STANDALONE	
		fileName = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/" + fileName;
		#endif
	}
	
	void Update()
	{
		avi.DoUpdate();
	}
	
	public void startRecording()
	{
		avi.Init(cam,width,height,AVIFps,gameFps,quality);
		avi.startRecording();
	}
	
	public void stopRecording()
	{
		avi.stopRecording();
	}
	
	public void saveAVI()
	{
		byte[] b = avi.getAVIByteArray();
		
		#if UNITY_WEBGL && !UNITY_EDITOR
		DownloadFile(b, b.Length, fileName);
		#elif UNITY_EDITOR || UNITY_STANDALONE
		File.WriteAllBytes(fileName, b);
		#endif
	}
	
}
