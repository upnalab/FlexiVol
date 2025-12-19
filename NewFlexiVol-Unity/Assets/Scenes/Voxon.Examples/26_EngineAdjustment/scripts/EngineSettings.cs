using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voxon.Examples._26_EngineAdjustment
{
    /// <summary>
    /// Applies a variety of Graphical changes to the engine
    /// during runtime. There will alter visuals as well as
    /// runtime performance
    /// </summary>
    public class EngineSettings : MonoBehaviour
    {
        /// <summary>
        /// Active timer value
        /// </summary>
        float timer = 0;

        [Header("Density")]
        /// <summary>
        /// demonstrate Density Variation
        /// </summary>
        public bool densityExample = false;
        /// <summary>
        /// Density Demo cycle time (seconds)
        /// </summary>
        public float DensityCycleTime = 3;
        /// <summary>
        /// Density Demo target (max) value
        /// </summary>
        float densityTarget = 4;
        /// <summary>
        /// Density Demo initial (below min) value
        /// </summary>
        float initialDensity = -1;


        [Header("Gamma")]
        /// <summary>
        /// demonstrate Gamma Variation
        /// </summary>
        public bool gammaExample = false;
        /// <summary>
        /// Gamma Demo cycle time (seconds)
        /// </summary>
        public float GammaCycleTime = 3; // Seconds
        /// <summary>
        /// Gamma Demo target (max) value
        /// </summary>
        float gammaTarget = 4;
        /// <summary>
        /// Gamma Demo initial (below min) value
        /// </summary>
        float initialGamma = -1;

        [Header("DotSize")]
        /// <summary>
        /// demonstrate DotSize Variation
        /// </summary>
        public bool dotSizeExample = false;
        /// <summary>
        /// DotSize Demo cycle time (seconds)
        /// </summary>
        public float DotSizeCycleTime = 3;
        /// <summary>
        /// DotSize Demo target (max) value
        /// </summary>
        int dotSizeTarget = 4;
        /// <summary>
        /// DotSize Demo initial (below min) value
        /// </summary>
        int initialDotSize = -1;

        /// <summary>
        /// Called per Frame
        /// If Density Example: Raise and Lower Density value over time
        /// If Gamma Example: Raise and Lower Gamma value over time
        /// If DotSize Example: Raise and Lower DotSize value over time
        /// Reports on current Density, Gamme, DotSize and AspectRatio
        /// </summary>
        void Update()
        {
            timer += Time.deltaTime;
            if (VXProcess.Runtime == null || VXProcess.Instance == null) return;
            if (densityExample)
            {
                if (initialDensity < 0)
                {
                    initialDensity = Voxon.VXProcess.Runtime.GetDensity();
                }

                float newDensity = Mathf.Lerp(initialDensity, densityTarget, timer / DensityCycleTime);
                newDensity = Mathf.Clamp(newDensity, 0.1f, 4);
                Voxon.VXProcess.Runtime.SetDensity(newDensity);

                if (newDensity >= 4 || newDensity <= 0.1f)
                {
                    if (densityTarget == 4)
                    {
                        densityTarget = 0.1f;
                    }
                    else
                    {
                        densityTarget = 4;
                    }
                    timer = 0;
                    initialDensity = newDensity;
                }
            }

            if (gammaExample)
            {
                if (initialGamma < 0)
                {
                    initialGamma = Voxon.VXProcess.Runtime.GetGamma();
                }

                float newGamma = Mathf.Lerp(initialGamma, gammaTarget, timer / GammaCycleTime);
                newGamma = Mathf.Clamp(newGamma, 0.1f, 4);

                Voxon.VXProcess.Runtime.SetGamma(newGamma);

                if (newGamma >= 4 || newGamma <= 0.1f)
                {
                    if (gammaTarget == 4)
                    {
                        gammaTarget = 0.1f;
                    }
                    else
                    {
                        gammaTarget = 4;
                    }
                    timer = 0;
                    initialGamma = newGamma;
                }
            }

            if (dotSizeExample)
            {


                if (initialDotSize < 0)
                {
                    initialDotSize = Voxon.VXProcess.Runtime.GetDotSize();
                }

                float possible = Mathf.Lerp(initialDotSize, dotSizeTarget, timer / GammaCycleTime);
                int newDotSize = Mathf.RoundToInt(possible);

                if (newDotSize != Voxon.VXProcess.Runtime.GetDotSize())
                {
                 
                    int convertedval = packDotSize(newDotSize);

                    Voxon.VXProcess.Runtime.SetDotSize(convertedval);
                }


                if (newDotSize == dotSizeTarget)
                {
                    if (dotSizeTarget == 4)
                    {
                        dotSizeTarget = 0;
                    }
                    else
                    {
                        dotSizeTarget = 0;
                    }
                    timer = 0;
                    initialDotSize = newDotSize;
                }
                
            }

            /* Report Engine Values */
            float[] aspf = Voxon.VXProcess.Runtime.GetAspectRatio();
            Vector3 asp = new Vector3(aspf[0], aspf[1], aspf[2]);
            Voxon.VXProcess.add_log_line($"");
            Voxon.VXProcess.add_log_line($"");
            Voxon.VXProcess.add_log_line($"Gamma: {Voxon.VXProcess.Runtime.GetGamma()}");
            Voxon.VXProcess.add_log_line($"Density: {Voxon.VXProcess.Runtime.GetDensity()}");
            Voxon.VXProcess.add_log_line($"Dot Size - r:{(Voxon.VXProcess.Runtime.GetDotSize() >> 16) & 0xFF}, g:{(Voxon.VXProcess.Runtime.GetDotSize() >> 8)  & 0xff}, b:{Voxon.VXProcess.Runtime.GetDotSize() & 0xff }");
            Voxon.VXProcess.add_log_line($"Aspect Ratio: {asp}");
            Voxon.VXProcess.add_log_line($"");
            Voxon.VXProcess.add_log_line($"");
            Voxon.VXProcess.add_log_line($"");
        }
         
       // Dotsize is expressed as three hex values (RGB) 0 - 4... 4 is max. 
       // example 0x010204 would mean  R = 1, G = 2, B = 4; 
        private int packDotSize(int rgbValue)
        {
            // Extract individual color components

            int red = rgbValue;
            int green = rgbValue;
            int blue = rgbValue;
            

            return (int)((red << 16)  | (green << 8)  | (blue) );


        }
    }


}