using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voxon;
using Random = UnityEngine.Random;

namespace Voxon.Examples._19_Lighting
{
	/// <summary>
	/// 
	/// 
	/// </summary>
	public class LightVoxels
	{
		/// <summary>
		/// Structure to hold metadata regarding a pixel. Referencing
		/// it's position in a 2D array, it's ratio (% position), and positions
		/// in world space
		/// </summary>
		private struct PmCache
		{
			public float X;
			public float Z;
			public float XRatio;
			public float ZRatio;
			public float XPos;
			public float ZPos;

			public PmCache(float x, float z, float xRatio, float zRatio, float xPos, float zPos)
			{
				X = x;
				Z = z;
				XRatio = xRatio;
				ZRatio = zRatio;
				XPos = xPos;
				ZPos = zPos;
			}
		}

		/// <summary>
		/// texture width in world space
		/// </summary>
		private uint _width = 1;
		/// <summary>
		/// Texture height in world space
		/// </summary>
		private uint _height = 1;
		/// <summary>
		/// Number of pixels 'wide'
		/// </summary>
		public int x_units;
		/// <summary>
		/// Number of pixels 'high'
		/// </summary>
		public int z_units;

		/// <summary>
		/// Default pixel color
		/// </summary>
		public Color32 colour = Color.white;

		/// <summary>
		/// Color per pixel
		/// </summary>
		public Color32[] colours;
		/// <summary>
		/// Position per pixel
		/// </summary>
		private Vector3[] positions;

		private int iCol;
		private VXVoxelBatch _voxelBatch;

		/// <summary>
		/// Voxon representation of pixels (voxels)
		/// </summary>
		private VXVoxels _voxelVoxels;

		/// <summary>
		/// Half width, used to determine center and offsets
		/// </summary>
		private float halfWidth;
		/// <summary>
		/// Half height, used to determine center and offsets
		/// </summary>
		private float halfHeight;
		/// <summary>
		/// Total pixels = <see cref="x_units"/> * <see cref="z_units"/>
		/// </summary>
		private int totalUnits;
		/// <summary>
		/// All Pixel Metadata
		/// </summary>
		private PmCache[] _cache;

		/// <summary>
		/// Generates a collection of voxels to be coloured and positioned based
		/// on incoming image color and depth data.
		/// </summary>
		/// <param name="Twidth">Texture width (pixels)</param>
		/// <param name="Theight">Texture height (pixels)</param>
		public LightVoxels(int Twidth = 500, int Theight = 500)
		{
			x_units = Twidth;
			z_units = Theight;

			halfWidth = _width * .5f;
			halfHeight = _height * .5f;

			totalUnits = x_units * z_units;
			positions = new Vector3[totalUnits];
			_cache = new PmCache[totalUnits];

			for (var x = 0; x < x_units; ++x)
			{
				float xRatio = (float)x / (x_units - 1);
				float xPos = xRatio * _width - halfWidth;
				for (var z = 0; z < z_units; ++z)
				{
					float zRatio = (float)z / (z_units - 1);
					float zPos = zRatio * _height - halfHeight;
					long idx = x * z_units + z;
					positions[idx] = new Vector3()
					{
						x = zPos,
						y = 0,
						z = xPos,
					};

					_cache[idx] = new PmCache(x, z, xRatio, zRatio, xPos, zPos);
				}
			}

			colours = new Color32[totalUnits];
			for (int i = 0; i < totalUnits; i++)
			{
				colours[i] = colour;
			}
			// _voxelBatch = new VXVoxelBatch(ref positions, colour);
			_voxelVoxels = new VXVoxels(ref positions, ref colours);
		}

		/// <summary>
		/// Update all pixels with new color data and height data
		/// (height data is stored in alpha channel)
		/// </summary>
		/// <param name="data">Pixel Data</param>
		public void Update(ref Color32[] data)
		{
			_voxelVoxels.set_colors(ref data);

			for (var x = 0; x < x_units; ++x)
			{
				for (var z = 0; z < z_units; ++z)
				{
					int idx = x * z_units + z;
					float y = -((float)data[idx].a - 128) / 256;
					// Translation mistake
					_voxelVoxels.set_posz(idx, y);

				}
			}



		}
	}
}