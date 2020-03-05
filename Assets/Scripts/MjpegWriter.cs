using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace GitHub.secile.Avi
{
    class MjpegWriter
    {
        private AviWriter aviWriter;
	    MemoryStream _outputAvi;
	    public MjpegWriter(MemoryStream outputAvi, int width, int height, float fps)
	    {
		    _outputAvi =  outputAvi;
            aviWriter = new AviWriter(_outputAvi, "MJPG", width, height, fps);
        }

	    public void AddImage(byte[] b)
        {
            using (var ms = new System.IO.MemoryStream())
            {
	            ms.Write(b, 0, b.Length);
                aviWriter.AddImage(ms.GetBuffer());
            }
        }

	

	    public byte[] Close()
	    {
		    aviWriter.Close();
		    return _outputAvi.ToArray();
		    //_outputAvi = null;
        }
    }
}
