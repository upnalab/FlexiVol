using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voxon;
using Random = UnityEngine.Random;

namespace Voxon.Examples._16_PixelMesh
{
    /// <summary>
    /// Generates a large number of voxels to be displayed via the Voxon Runtime
    /// </summary>
    public class PixelMesh : MonoBehaviour
    {
        /*
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
        */

        /// <summary>
        /// Width (x) of pixel collection in world space
        /// </summary>
        public uint width = 1;
        /// <summary>
        /// Height (z) of pixels in world space
        /// </summary>
        public uint height = 1;
        /// <summary>
        /// Number of pixels along x axis
        /// </summary>
        public int x_units = 500;
        /// <summary>
        /// Number of pixels along z axis
        /// </summary>
        public int z_units = 500;

        /// <summary>
        /// Default color for all pixels
        /// </summary>
        public Color32 colour;
        /// <summary>
        /// Color per pixel (updated during runtime)
        /// </summary>
        public Color32[] colours;
        /// <summary>
        /// Position of each pixel (updated during runtime)
        /// </summary>
        private Vector3[] positions;

        /// <summary>
        /// Unused. Alternative call for drawing groups of voxels
        /// </summary>
        private VXVoxelBatch _voxelBatch;
        /// <summary>
        /// Voxon representation of voxels. Where draw call is initialised from
        /// </summary>
        private VXVoxels _voxelVoxels;

        /// <summary>
        /// Half width (x axis) - Used for setting central position
        /// </summary>
        private float halfWidth;
        /// <summary>
        /// Half height (z axis) - Used for setting central position
        /// </summary>
        private float halfHeight;

        /// <summary>
        /// Total number of pixels. = <see cref="x_units"/> * <see cref="z_units"/>
        /// </summary>
        private int totalUnits;
        // private PmCache[] _cache;

        /// <summary>
        /// Generates <see cref="totalUnits"/> of positions and a cache of 
        /// </summary>
        void Start()
        {
            halfWidth = width * .5f;
            halfHeight = height * .5f;

            totalUnits = x_units * z_units;
            positions = new Vector3[totalUnits];
            // _cache = new PmCache[totalUnits];
            for (var x = 0; x < x_units; ++x)
            {
                float xRatio = (float)x / (x_units - 1);
                float xPos = xRatio * width - halfWidth;
                for (var z = 0; z < z_units; ++z)
                {
                    float zRatio = (float)z / (z_units - 1);
                    float zPos = zRatio * height - halfHeight;
                    long idx = x * z_units + z;
                    positions[idx] = new Vector3()
                    {
                        x = xPos,
                        y = 0,
                        z = zPos,
                    };

                    // _cache[idx] = new PmCache(x, z, xRatio, zRatio, xPos, zPos);
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
        /// Runs per Frame.
        /// Updates pixel positions based on Sine wave of pixel positions and current time
        /// </summary>
        void Update()
        {
            float x_mid = x_units * .5f;
            float z_mid = z_units * .5f;
            for (int x = 0; x < x_units; ++x)
            {
                for (int z = 0; z < z_units; ++z)
                {
                    int idx = x * z_units + z;

                    positions[idx].y = Mathf.Sin(x * .1f + Time.time * 2);

                    int red = (x > x_mid ? 0xFF : 0);
                    int green = (positions[idx].y > 0 ? 0xFF : 0);
                    int blue = (z > z_mid ? 0xFF : 0); ;
                    int rgb24 = (red << 16) | (green << 8) | (blue); // the colour

                    //int rgb24 = (255 << 16); //RED ONLY
                    //int rgb24 = (255 << 8); //GREEN ONLY
                    //int rgb24 = (255); // BLUE ONLY
                    // return (col.r << 16) | (col.g << 8) | col.b;
                    // colours[idx] = new Color32(rgb24);
                    // Debug.Log(red + " : " + green + " : " + blue);
                    _voxelVoxels.set_icolor(idx, rgb24);
                    // colours[idx] = new Color32(255, 0, 0, 255);
                }
            }

            // _voxelBatch.update_transform();
            _voxelVoxels.update_transform();
            // _voxelVoxels.set_colors(ref colours);
        }
    }
}