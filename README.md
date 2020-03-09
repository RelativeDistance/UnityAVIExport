# UnityAVIExport

Export an MJPG encoded AVI at runtime or from the editor.  It is a Unity wrapper around this code: [MotionJPEGWriter](https://github.com/secile/MotionJPEGWriter) AVIs produced by this plugin will play natively on MacOS and Windows without any additional codecs.   

## Runtime Usage

[WebGL Demo Scene](https://relativedistance.github.io//UnityAVIExport/index)

Add AVIExport script to a game object the get a reference to it:

```csharp
AVIExport avi = GetComponent<AVIExport>();
```

Then use the following methods:

### Initialize AVI

```csharp
avi.Init(camera,width,height,AVIFps,gameFps,quality);
```
- **Camera** - Camera reference, if null main camera will be used
- **Width/Height** - Dimensions of the AVI
- **AVIFps** - Framerate of AVI, decimals ok such as 29.97 NTSC
- **GameFPS** - The frame rate the project is currently running at on target platform.
- **Quality** - Integer between 0 (worst) - 100 (best) for jpg quality of each frame. 

### Start Recording

```csharp
avi.startRecording();
```

### Stop Recording

```csharp
avi.stopRecording();
```

### Get Byte Array

```csharp
avi.getAVIByteArray();
```
Then save the byte array to a file with something like this:
```csharp
File.WriteAllBytes(fileName, aviExport.getAVIByteArray());
```
See the included demo scene for an example.

## Editor Usage

## TODO
Currently video only, add audio
Improve performance with AsyncGPUReadback (something like 10-20% increase expected)
Look into WebGL 2 Render Texture bug and see if there's a workaround
