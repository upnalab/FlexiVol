// https://www.codeproject.com/Articles/17110/Quick-Compression-Utility-for-C-Byte-Arrays
// Author: ronnotel
// License: The Code Project Open License (CPOL) 1.02 (https://www.codeproject.com/info/cpol10.aspx)

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace Compressor
{
	public class Compressor
	{		
		public static byte[] Compress(byte[] buffer)
		{
			MemoryStream ms = new MemoryStream();
			GZipStream zip = new GZipStream(ms, CompressionMode.Compress, true);
			zip.Write(buffer, 0, buffer.Length);
			zip.Close();
			ms.Position = 0;

			MemoryStream outStream = new MemoryStream();

			byte[] compressed = new byte[ms.Length];
			ms.Read(compressed, 0, compressed.Length);

			byte[] gzBuffer = new byte[compressed.Length + 4];
			Buffer.BlockCopy(compressed, 0, gzBuffer, 4, compressed.Length);
			Buffer.BlockCopy(BitConverter.GetBytes(buffer.Length), 0, gzBuffer, 0, 4);
			return gzBuffer;
		}

		public static byte[] Decompress(byte[] gzBuffer)
		{
			MemoryStream ms = new MemoryStream();
			int msgLength = BitConverter.ToInt32(gzBuffer, 0);
			ms.Write(gzBuffer, 4, gzBuffer.Length - 4);

			byte[] buffer = new byte[msgLength];

			ms.Position = 0;
			GZipStream zip = new GZipStream(ms, CompressionMode.Decompress);
			zip.Read(buffer, 0, buffer.Length);

			return buffer;
		}
	}
}