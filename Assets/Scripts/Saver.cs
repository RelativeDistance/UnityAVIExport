using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GitHub.secile.Avi;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine.Rendering;
using System.Diagnostics;

public class Saver : MonoBehaviour
{
	[DllImport("__Internal")]
	private static extern void DownloadFile(byte[] array, int byteLength, string fileName);
	
	//public Playhead ph;
	public Texture watermark;
	bool currentlyRecording = false;
	RenderTexture rt;
	MjpegWriter writer;
	
	Texture2D tex;
	//Texture2D tempTex;
	Rect r;
	 
	//long nums = 0;
	//long total = 0; s 
	  
	bool isFull,seamlessloop;
	int totalSeconds,width,height,cfTime,totalFrames,currentFrame;
	float framerate;
	List<Texture2D> crossFadeList= new List<Texture2D>();

 
    void Start()
	{ 
		
	    Application.targetFrameRate = 60;
    }
  
	void Update()
	{
		if ((currentFrame>=totalFrames) && currentlyRecording)
		{
			currentFrame = 1;
			currentlyRecording = false;
			stopRecording();
		}

		if (currentlyRecording)
		{

			recordFrame();
			currentFrame++;
		}
			
    }
       
	public void saveTest()
	{
		saveIt(true,10,"7680x4320","30",false,2);
	}
 
 
	public byte[] createThumbnail(int w,int h)
	{
		rt = new RenderTexture(w,h,24);
		tex = new Texture2D(rt.width, rt.height);
		r = new Rect(0, 0, tex.width, tex.height);
		
		
		Camera.main.targetTexture = rt;
		RenderTexture.active = rt;
		
		//stopwatch.Start();
		Camera.main.Render();
		//stopwatch.Stop();
		tex.ReadPixels(r, 0, 0);
		tex.Apply();
		
		byte[] bytes = tex.EncodeToJPG(70);		
		
		
		RenderTexture.active = null;
		Camera.main.targetTexture = null;
		
		return bytes;
	}
 
 
	void saveIt(bool full, int len, string res, string fps,  bool loop, int crossfade )
	{
		// full download - bool
		// length - string
		// resolution - string
		// fps - string
		// container format - number
		// loop - boolean
		// crossfade - number

		currentFrame=1;
		isFull = full;
		totalSeconds = len;
		string[] resParts = res.Split(new string[]{"x"},System.StringSplitOptions.None);
		width = int.Parse(resParts[0]);
		height = int.Parse(resParts[1]);
		framerate = float.Parse(fps);
		seamlessloop = loop;
		cfTime = crossfade;

		totalFrames = (int)(totalSeconds * framerate);

		if (!isFull)
		{
			width = 1280;
			height = 720;
		}
		rt = new RenderTexture(width,height,24);
		tex = new Texture2D(rt.width, rt.height);
		r = new Rect(0, 0, tex.width, tex.height);

		startRecording();
	}

	void recordFrame()
	{
		//Stopwatch stopwatch = new Stopwatch();
		
		Camera.main.targetTexture = rt;
		RenderTexture.active = rt;
		
		//stopwatch.Start();
		Camera.main.Render();
		//stopwatch.Stop();
		

		
		//var a = stopwatch.ElapsedMilliseconds;
		
		//stopwatch.Reset();
		//stopwatch.Start();
		
		if ((seamlessloop) && (currentFrame>totalFrames - ((int)(cfTime*framerate))))
		{
			if (crossFadeList.Count>0)
			{
				GL.PushMatrix();
				GL.LoadPixelMatrix(0, width, height,0);
				Graphics.DrawTexture (new Rect (0, 0, width, height), crossFadeList[0]);
				GL.PopMatrix ();     
				crossFadeList.RemoveAt(0);
			}
		}
		
		
		if (!isFull)
		{
			GL.PushMatrix();
			GL.LoadPixelMatrix(0, width, height,0);
			Graphics.DrawTexture (new Rect (0, 0, width, height), watermark);
			GL.PopMatrix ();          
		}
		
		tex.ReadPixels(r, 0, 0);
		

		if ((seamlessloop) && (currentFrame<(int)(cfTime*framerate)))
		{
			var data = tex.GetRawTextureData<Color32>();
			for (int x = 0; x < data.Length; x++)
			{
				Color32 a = data[x];
				a.a = (byte)(0.5f*255.0f); //alpah first param
				data[x] = a;
			}
			tex.Apply();
			crossFadeList.Add(tex);
		}
		else
		{
			tex.Apply();
		}
		
		//stopwatch.Stop();
		//var b = stopwatch.ElapsedMilliseconds;
		
		//stopwatch.Reset();
		//stopwatch.Start();
		var bytes = tex.EncodeToJPG(90);		
		writer.AddImage(bytes);
		
		
		
		
		
		
		
		
		
		
		
		
		
		//stopwatch.Stop();
		//var c = stopwatch.ElapsedMilliseconds;
		
		//nums = nums + a +b +c;
		//total++;
			
		RenderTexture.active = null;
		Camera.main.targetTexture = null;
	}

	public void startRecording()
	{
		// 1/4 Time
		//Application.targetFrameRate = 15;		
		//Time.timeScale = 0.25f;
		
	//	ph.dragPlayhead(0);
	//	ph.togglePlayPause();
		MemoryStream m = new MemoryStream();
		writer = new MjpegWriter( m, rt.width, rt.height, framerate);
		currentlyRecording = true;
	}
	
	public void stopRecording()
	{
		
		// Normal Time
		//UnityEngine.Debug.Log("Stop "+ (float)nums /(float)total);
		Application.targetFrameRate = 60;

		Time.timeScale = 1f;
		currentlyRecording = false;
		
		byte[] b = writer.Close();
		writer =null;
				
		#if UNITY_EDITOR || UNITY_STANDALONE
			
			File.WriteAllBytes("tester.avi", b);
		#endif
		#if UNITY_WEBGL && !UNITY_EDITOR

			DownloadFile(b, b.Length, "good.avi");
		#endif
	}
}















		//while (_requests.Count > 0)
		//{
		//	var req = _requests.Peek();

		//	if (req.hasError)
		//	{
		//		Debug.Log("GPU readback error detected.");
		//		_requests.Dequeue();
		//	}
		//	else if (req.done)
		//	{
		//		var buffer = req.GetData<Color32>();

		//		Texture2D tex = new Texture2D(rt.width, rt.height);
		//		tex.SetPixels32(buffer.ToArray());
		//		tex.Apply();
		//		var bytes = tex.EncodeToJPG(99);		
		//		writer.AddImage(bytes);
		
		//		Destroy(tex);
		//		_requests.Dequeue();
		//	}
		//	else
		//	{
		//		break;
		//	}
		//}


	//void OnRenderImage(RenderTexture source, RenderTexture destination)
	//{
	//	if (currentlyRecording)
	//		recordFrame();
	//}
       
	//void OnRenderImage(RenderTexture source, RenderTexture destination)
	//{
	//	if (currentlyRecording)
	//	{
		
	//		//	Debug.Log(source.width + " " +source.height);
		
	//		//var tempRT = RenderTexture.GetTemporary(1920, 1080);
	//		//Graphics.Blit(source, tempRT);
	//		//tempTex = new Texture2D(1920, 1080);
	//		//tempTex.ReadPixels(new Rect(0, 0, 1920, 1080), 0, 0, false);
	//		//tempTex.Apply();

	
	//		//RenderTexture.ReleaseTemporary(tempRT);

			
	//	}
	//	else
	//	{
	//		Graphics.Blit(source, destination);
		
	//	}
		
		
		
	//}
       
       
	//Queue<AsyncGPUReadbackRequest> _requests = new Queue<AsyncGPUReadbackRequest>();


	//int counter = 0;
	
	//List<byte[]> combinedArray = new List<byte[]>();
	
	
	
//combinedArray.Clear();
//	StartCoroutine(sendData());
//	combinedArray.Add(bytes);
		

	//private byte[] Combine(List<byte[]> arrays)
	//{
	//	byte[] rv = new byte[arrays.Sum(a => a.Length)];
	//	int offset = 0;
	//	foreach (byte[] array in arrays) {
	//		System.Buffer.BlockCopy(array, 0, rv, offset, array.Length);
	//		offset += array.Length;
	//	}
	//	return rv;
	//}
    
    
	//IEnumerator sendData()
	//{
	//	WWWForm postForm = new WWWForm();
	//	// version 1
	//	//postForm.AddBinaryData("theFile",localFile.bytes);
     
	//	// version 2
	//	//	postForm.AddBinaryData("fileUpload",Combine(combinedArray),"frame"+counter+".mjpeg","image/jpg");
	//	postForm.AddBinaryData("fileUpload",Combine(combinedArray),"frame"+counter+".mjpeg","video/x-motion-jpeg");
     
	//	WWW upload = new WWW("http://relativedistance.com/intromaker/saveframe.php",postForm);        
	//	yield return upload;
	//	if (upload.error == null)
	//		Debug.Log("upload done :" + upload.text);
	//	else
	//		Debug.Log("Error during upload: " + upload.error);
	//}
	
	