using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Serialization;

namespace Voxon
{
	[Serializable]
	public class MeshData
    {
        // Mesh Data
        [SerializeField]
        public poltex[] vertices;

		[SerializeField]
		public string name;
        [FormerlySerializedAs("vertex_count")] [SerializeField]
        public int vertexCount;        // Number of vertices
        [FormerlySerializedAs("submesh_count")] [SerializeField]
        public int submeshCount;      // Count of Submeshes part of mesh
        [SerializeField]
        public int[][] indices;
        [FormerlySerializedAs("index_counts")] [SerializeField]
        public int[] indexCounts;


		/*  Structuring Data */
		//  int64_t byteSize of MeshData
		//  nameLength
		//	name
		//  vertexCount
		//  poltex[]
		//  submeshCount
		//  indexCounts[]
		//  indices[][]

		public byte[] toByteArray()
		{
			/* OutBuffer */
			int totalBytes = 0;
				totalBytes += sizeof(int); // totalBytes
				totalBytes += sizeof(int); // name byte_len
				totalBytes += sizeof(Char) * name.Length; // Name
				totalBytes += sizeof(int); // # vertices byte_len
				totalBytes += Marshal.SizeOf(typeof(poltex)) * vertexCount; // Poltex[]
				totalBytes += sizeof(int); // # submeshes count
				totalBytes += sizeof(int); // # indexCount byte_len
				totalBytes += sizeof(Int32) * indexCounts.Length; // indexCounts[]
				totalBytes += sizeof(int) * indices.Length; // indices byte_len
				foreach(int[] indArray in indices)
				{
					totalBytes += sizeof(Int32) * indArray.Length; // Indices
				}

			// Debug.Log($"Byte Buffer Size: {totalBytes}");

			byte[] arr = new byte[totalBytes];

			// Copy Actions
			IntPtr ptr;
			int offset = 0;

			// Size of Packet (buffer - size(sizeofPacket))
			int sizeofPacket = totalBytes - sizeof(int);
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

			// Vertex Size
			int vert_size = Marshal.SizeOf(typeof(poltex))*vertices.Length;
			ptr = Marshal.AllocHGlobal(sizeof(int));
			Marshal.StructureToPtr(vert_size, ptr, true);
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
				Debug.Log($"VertexSize: \t{vert_size}");
				Debug.Log($"\tBytes: \t{hex.ToString()}");
			}
			*/
			offset += sizeof(int);

			// Vertices / Poltex
			int pol_size = Marshal.SizeOf(typeof(poltex));
			int vert_length = vertices.Length;
			for(int i = 0; i < vert_length; i++)
			{
				poltex p = vertices[i];
				ptr = Marshal.AllocHGlobal(pol_size);
				Marshal.StructureToPtr(p, ptr, true);
				Marshal.Copy(ptr, arr, offset, pol_size);
				Marshal.FreeHGlobal(ptr);
				offset += pol_size;
			}
			/*
			{ // Debug
				int start = offset - vert_size;
				int end = offset;
				System.Text.StringBuilder hex = new System.Text.StringBuilder(end - start);
				for (int idx = start; idx < end; idx++)
				{
					hex.AppendFormat("{0:x2}", arr[idx]);
				}
				Debug.Log($"Vertices: \t{vert_size}");
				Debug.Log($"\tBytes: \t{hex.ToString()}");
			}
			*/

			// SubMesh Count
			ptr = Marshal.AllocHGlobal(sizeof(int));
			Marshal.StructureToPtr(submeshCount, ptr, true);
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
				Debug.Log($"SubMeshCount: \t{submeshCount}");
				Debug.Log($"\tBytes: \t{hex.ToString()}");
			}
			*/

			offset += sizeof(int);

			// Indice Counts Size
			// Debug.Log($"Indice Counts contains {indexCounts.Length} counts, and needs {sizeof(Int32) * indexCounts.Length} bytes to store each count");
			int indcount_size = sizeof(Int32) * indexCounts.Length;
			ptr = Marshal.AllocHGlobal(sizeof(int));
			Marshal.StructureToPtr(indcount_size, ptr, true);
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
				Debug.Log($"IndCountSize: \t{indcount_size}");
				Debug.Log($"\tBytes: \t{hex.ToString()}");
			}
			*/

			offset += sizeof(int);

			// Indice Counts
			int int_size = sizeof(Int32);
			int indCount_length = indexCounts.Length;
			for (int i = 0; i < indCount_length; i++)
			{
				Int32 p = indexCounts[i];
				ptr = Marshal.AllocHGlobal(int_size);
				Marshal.StructureToPtr(p, ptr, true);
				Marshal.Copy(ptr, arr, offset, int_size);
				Marshal.FreeHGlobal(ptr);
				offset += int_size;
			}

			/*
			{ // Debug
				int start = offset - indcount_size;
				int end = offset;
				System.Text.StringBuilder hex = new System.Text.StringBuilder(end - start);
				for (int idx = start; idx < end; idx++)
				{
					hex.AppendFormat("{0:x2}", arr[idx]);
				}
				Debug.Log($"Submesh Indices Counts: \t{indexCounts[0]}");
				Debug.Log($"\tBytes: \t{hex.ToString()}");
			}
			*/

			int ori_offset = offset;
			// Indice Counts
			for (int i = 0; i < submeshCount; i++)
			{
				int indices_size = sizeof(Int32) * indices[i].Length;
				ptr = Marshal.AllocHGlobal(sizeof(int));
				Marshal.StructureToPtr(indices_size, ptr, true);
				Marshal.Copy(ptr, arr, offset, sizeof(int));
				Marshal.FreeHGlobal(ptr);
				offset += sizeof(int);

				int ind_len = indices[i].Length;
				int indice_size = sizeof(Int32);
				int indice;
				for (int ii = 0; ii < ind_len; ii++) {
					indice = indices[i][ii];
					ptr = Marshal.AllocHGlobal(indice_size);
					Marshal.StructureToPtr(indice, ptr, true);
					Marshal.Copy(ptr, arr, offset, indice_size);
					Marshal.FreeHGlobal(ptr);
					offset += indice_size;
				}
			}

			/*
			{ // Debug
				int start = ori_offset;
				int end = offset;
				System.Text.StringBuilder hex = new System.Text.StringBuilder(end - start);
				for (int idx = start; idx < end; idx++)
				{
					hex.AppendFormat("{0:x2}", arr[idx]);
				}
				Debug.Log($"Indices: \t{indices[0].Length}");
				Debug.Log($"\tBytes: \t{hex.ToString()}");
			}
			*/

			if (offset != totalBytes)
			{
				Debug.Log("Error packing Mesh Data. Size Mismatch");
			}

			return arr;


		}

		public static MeshData fromByteArray(byte[] bytes)
		{
			MeshData md = new MeshData();

			// Copy Actions
			int offset = 0;

			// TotalBytes - (Cut by providing stream!)

			// NameLength
			int name_size = System.BitConverter.ToInt32(bytes, offset);
			offset += sizeof(int);

			// Debug.Log($"NameSize: {name_size}");

			// Name
			md.name = System.Text.Encoding.Unicode.GetString(bytes, offset, name_size);
			offset += name_size;

			// Debug.Log($"Name {md.name}");

			// Vertex Size -> Count
			int vertex_size = System.BitConverter.ToInt32(bytes, offset);
			int poltex_size = Marshal.SizeOf(typeof(poltex));
			md.vertexCount = vertex_size / poltex_size;
			offset += sizeof(int);
			

			// Debug.Log($"VertSize {vertex_size}");

			int float_size = sizeof(float);
			int int_size = sizeof(int);
			// Vertices / Poltex
			md.vertices = new poltex[md.vertexCount];
			for(int i = 0; i < md.vertexCount; i++)
			{

				md.vertices[i].x = BitConverter.ToSingle(bytes, offset); offset += float_size;
				md.vertices[i].y = BitConverter.ToSingle(bytes, offset); offset += float_size;
				md.vertices[i].z = BitConverter.ToSingle(bytes, offset); offset += float_size;
				md.vertices[i].u = BitConverter.ToSingle(bytes, offset); offset += float_size;
				md.vertices[i].v = BitConverter.ToSingle(bytes, offset); offset += float_size;
				md.vertices[i].col = BitConverter.ToInt32(bytes, offset); offset += int_size;
			}
			
			// Debug.Log($"First Vert: {md.vertices[0].ToString()}");

			// SubMesh Count
			md.submeshCount = System.BitConverter.ToInt32(bytes, offset);
			offset += sizeof(int);
			md.indexCounts = new int[md.submeshCount];
			// Debug.Log($"Submesh#: {md.submeshCount}");

			// Indices Count Size
			int indcount_size = System.BitConverter.ToInt32(bytes, offset);
			offset += sizeof(int);

			// Indice Counts
			for(int i = 0; i < md.submeshCount; i++)
			{
				md.indexCounts[i] = System.BitConverter.ToInt32(bytes, offset);
				offset += sizeof(int);
			}

			// Debug.Log($"First Index#: {md.indexCounts[0].ToString()}");

			// Indices
			md.indices = new int[md.submeshCount][];

			int indices_size = 0;
			
			for (int i = 0; i < md.submeshCount; i++)
			{
				md.indices[i] = new int[md.indexCounts[i]];

				indices_size = System.BitConverter.ToInt32(bytes, offset);
				offset += sizeof(int);

				int indices_count = indices_size / sizeof(int);

				md.indices[i] = new int[indices_count];
				for(int ii = 0; ii < indices_count; ii++) {
					md.indices[i][ii] = System.BitConverter.ToInt32(bytes, offset);
					offset += sizeof(int);
				}
			}

			// Debug.Log($"First Loop: {md.indices[0][0].ToString()},{md.indices[0][1].ToString()},{md.indices[0][2].ToString()},{md.indices[0][3].ToString()}");

			return md;
		}
	}
}

