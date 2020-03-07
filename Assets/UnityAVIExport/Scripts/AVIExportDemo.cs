using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using com.relativedistance.UnityAVIExport;

[RequireComponent(typeof(AVIExport))]
public class AVIExportDemo : MonoBehaviour
{
	public Camera cam;
	public int width = 1280;
	public int height = 720;
	public float AVIFps = 30;
	public int gameFps = 60;
	
	[Range(50,100)]
	public int quality = 90;
	public string fileName = "test.avi";
	
	AVIExport aviExport;
	
	void Start()
	{
		aviExport = GetComponent<AVIExport>();
		fileName = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/" + fileName;
	}
	
	public void startRecording()
	{
		aviExport.Init(cam,width,height,AVIFps,gameFps,quality);
		aviExport.startRecording();
	}
	
	public void stopRecording()
	{
		aviExport.stopRecording();
	}
	
	public void saveAVI()
	{
		aviExport.saveAVI(fileName);
	}
}
