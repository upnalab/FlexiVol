using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Serialization;

namespace Voxon
{
	[Serializable]
	public class TextureData
	{
		public string name;
		// Texture Data
		[SerializeField]
		public tiletype Texture;


		/*  Structuring Data */
		//  int64_t byteSize of TextureData
		//  nameSize
		//  name
		//  pitch
		//	width
		//  height
		//  pixelDataSize
		//  pixel_data[]

		public byte[] toByteArray()
		{
			/* OutBuffer */
			Int64 totalBytes = 0;
			totalBytes += sizeof(int); // totalBytes
			totalBytes += sizeof(int); // nameSize
			totalBytes += sizeof(char) * name.Length; // Name
			totalBytes += sizeof(Int64); // pitch
			totalBytes += sizeof(Int64); // width
			totalBytes += sizeof(Int64); // height
			totalBytes += sizeof(int); // pixelDataSize
			totalBytes += sizeof(byte) * ((int)Texture.height * (int)Texture.pitch); // # indexCount byte_len
			
			// Debug.Log($"Byte Buffer Size: {totalBytes}");

			byte[] arr = new byte[totalBytes];

			// Copy Actions
			IntPtr ptr;
			int offset = 0;

			// Size of Packet (buffer - size(sizeofPacket))
			Int64 sizeofPacket = totalBytes - sizeof(int);
			ptr = Marshal.AllocHGlobal(sizeof(int));
			Marshal.StructureToPtr(sizeofPacket, ptr, true);
			Marshal.Copy(ptr, arr, offset, sizeof(int));
			Marshal.FreeHGlobal(ptr);
			
			/*
			{ // Debug
				int start = offset;
				int end = offset + sizeof(int);
				System.Text.StringBuilder hex = new System.Text.StringBuilder(end - start);
				for (int idx = start; idx < end; idx++)
				{
					hex.AppendFormat("{0:x2}", arr[idx]);
				}
				Debug.Log($"Packet Size: \t{hex.ToString()}");
			}
			*/
			
			offset += sizeof(int);

			// Name_Bytes
			int name_size = sizeof(char) * name.Length;
			ptr = Marshal.AllocHGlobal(sizeof(int));
			Marshal.StructureToPtr(name_size, ptr, true);
			Marshal.Copy(ptr, arr, offset, sizeof(int));
			Marshal.FreeHGlobal(ptr);
			
			/*
			{ // Debug
				int start = offset;
				int end = offset + sizeof(int);
				System.Text.StringBuilder hex = new System.Text.StringBuilder(end - start);
				for (int idx = start; idx < end; idx++)
				{
					hex.AppendFormat("{0:x2}", arr[idx]);
				}
				Debug.Log($"NameSize: \t{name_size}");
				Debug.Log($"\tBytes: \t{hex.ToString()}");
			}
			*/
			
			offset += sizeof(int);

			// Name
			ptr = Marshal.StringToHGlobalUni(name);
			Marshal.Copy(ptr, arr, offset, name_size);
			Marshal.FreeHGlobal(ptr);
			
			/*
			{ // Debug
				int start = offset;
				int end = offset + name_size;
				System.Text.StringBuilder hex = new System.Text.StringBuilder(end - start);
				for (int idx = start; idx < end; idx++)
				{
					hex.AppendFormat("{0:x2}", arr[idx]);
				}
				Debug.Log($"Name: \t{name}");
				Debug.Log($"\tBytes: \t{hex.ToString()}");
			}
			*/

			offset += name_size;

			// Pitch
			ptr = Marshal.AllocHGlobal(sizeof(Int64));
			Marshal.StructureToPtr(Texture.pitch, ptr, true);
			Marshal.Copy(ptr, arr, offset, sizeof(Int64));
			Marshal.FreeHGlobal(ptr);

			/*
			{ // Debug
				int start = offset;
				int end = offset + sizeof(int);
				System.Text.StringBuilder hex = new System.Text.StringBuilder(end - start);
				for (int idx = start; idx < end; idx++)
				{
					hex.AppendFormat("{0:x2}", arr[idx]);
				}
				Debug.Log($"Pitch: \t{Texture.pitch}");
				Debug.Log($"\tBytes: \t{hex.ToString()}");
			}
			*/

			offset += sizeof(Int64);

			// Width
			ptr = Marshal.AllocHGlobal(sizeof(Int64));
			Marshal.StructureToPtr(Texture.width, ptr, true);
			Marshal.Copy(ptr, arr, offset, sizeof(Int64));
			Marshal.FreeHGlobal(ptr);

			/*
			{ // Debug
				int start = offset;
				int end = offset + sizeof(int);
				System.Text.StringBuilder hex = new System.Text.StringBuilder(end - start);
				for (int idx = start; idx < end; idx++)
				{
					hex.AppendFormat("{0:x2}", arr[idx]);
				}
				Debug.Log($"Width: \t{Texture.pitch}");
				Debug.Log($"\tBytes: \t{hex.ToString()}");
			}
			*/

			offset += sizeof(Int64);

			// Height
			ptr = Marshal.AllocHGlobal(sizeof(Int64));
			Marshal.StructureToPtr(Texture.height, ptr, true);
			Marshal.Copy(ptr, arr, offset, sizeof(Int64));
			Marshal.FreeHGlobal(ptr);

			/*
			{ // Debug
				int start = offset;
				int end = offset + sizeof(int);
				System.Text.StringBuilder hex = new System.Text.StringBuilder(end - start);
				for (int idx = start; idx < end; idx++)
				{
					hex.AppendFormat("{0:x2}", arr[idx]);
				}
				Debug.Log($"Width: \t{Texture.pitch}");
				Debug.Log($"\tBytes: \t{hex.ToString()}");
			}
			*/

			offset += sizeof(Int64);

			// Pixel Size
			// Name_Bytes
			int pixel_size = (int)Texture.pitch * (int)Texture.height;
			ptr = Marshal.AllocHGlobal(sizeof(int));
			Marshal.StructureToPtr(pixel_size, ptr, true);
			Marshal.Copy(ptr, arr, offset, sizeof(int));
			Marshal.FreeHGlobal(ptr);

			/*
			{ // Debug
				int start = offset;
				int end = offset + sizeof(int);
				System.Text.StringBuilder hex = new System.Text.StringBuilder(end - start);
				for (int idx = start; idx < end; idx++)
				{
					hex.AppendFormat("{0:x2}", arr[idx]);
				}
				Debug.Log($"PixelSize: \t{pixel_size}");
				Debug.Log($"\tBytes: \t{hex.ToString()}");
			}
			*/

			offset += sizeof(int);

			// Pixels
			ptr = Texture.first_pixel;
			Marshal.Copy(ptr, arr, offset, pixel_size);
			// DO NOT FREE PIXEL, STILL IN USE
			/*
			{ // Debug
				int start = offset;
				int end = offset + name_size;
				System.Text.StringBuilder hex = new System.Text.StringBuilder(end - start);
				for (int idx = start; idx < end; idx++)
				{
					hex.AppendFormat("{0:x2}", arr[idx]);
				}
				Debug.Log($"Pixels: \t{name}");
				Debug.Log($"\tBytes: \t{hex.ToString()}");
			}
			*/

			offset += pixel_size;

			if (offset != totalBytes)
			{
				Debug.Log("Error packing Mesh Data. Size Mismatch");
			}

			return arr;
		}

		public static TextureData fromByteArray(byte[] bytes)
		{
			/*  Structuring Data */
			//  nameSize
			//  name
			//  pitch
			//	width
			//  height
			//  pixelDataSize
			//  pixel_data[]
			TextureData td = new TextureData();

			// Copy Actions
			int offset = 0;

			// NameLength
			int name_size = System.BitConverter.ToInt32(bytes, offset);
			offset += sizeof(int);

			// Debug.Log($"NameSize: {name_size}");

			// Name
			td.name = System.Text.Encoding.Unicode.GetString(bytes, offset, name_size);
			offset += name_size;

			// Debug.Log($"Name {td.name}");

			// Pitch
			td.Texture.pitch = (IntPtr)System.BitConverter.ToInt64(bytes, offset);
			offset += sizeof(Int64);

			// Debug.Log($"Pitch {td.Texture.pitch}");
			
			// Width
			td.Texture.width = (IntPtr)System.BitConverter.ToInt64(bytes, offset);
			offset += sizeof(Int64);

			// Debug.Log($"Width {td.Texture.width}");
			
			// Height
			td.Texture.height = (IntPtr)System.BitConverter.ToInt64(bytes, offset);
			offset += sizeof(Int64);

			// Debug.Log($"Height {td.Texture.height}");

			// Pixel Size
			int pixel_size = System.BitConverter.ToInt32(bytes, offset);
			offset += sizeof(int);

			// Debug.Log($"PixelSize {pixel_size}");

			// Assign Pixels
			td.Texture.first_pixel = Marshal.AllocHGlobal(pixel_size);
			Marshal.Copy(bytes, offset, td.Texture.first_pixel, pixel_size);

			return td;
		}
	}
}

