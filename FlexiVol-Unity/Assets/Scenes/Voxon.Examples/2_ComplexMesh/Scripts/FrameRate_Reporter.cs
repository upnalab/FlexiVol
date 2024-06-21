using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace Voxon.Examples._2_ComplexMesh
{
    /// <summary>
    /// Reports average framerate to Voxon touch screen
    /// </summary>
    public class FrameRateReporter : MonoBehaviour {
        /// <summary>
        /// Fixed percentage of standard deviation that framerate 
        /// must operate in to be reported (do not report if extremely unstable frame rate)
        /// </summary>
        private const float StdDevPercent = 0.05f;

        /// <summary>
        /// Total Frames
        /// </summary>
        private int _mFrameCounter;
        /// <summary>
        /// Total Time
        /// </summary>
        private float _mTimeCounter;
        /// <summary>
        /// Most recent framerate
        /// </summary>
        private float _mLastFramerate;
        /// <summary>
        /// How often to refresh time
        /// </summary>
        [FormerlySerializedAs("m_refreshTime")] public float mRefreshTime = 0.5f;

        /// <summary>
        /// Time of the last X frames
        /// </summary>
        private List<float> _frames;

        /// <summary>
        /// Mean of last X Frames
        /// </summary>
        private float _average;
        /// <summary>
        /// Standard Deviation of last X Frames
        /// </summary>
        private float _standardDeviation;

        /// <summary>
        /// Is the framerate stable?
        /// </summary>
        private bool _stabalised;

        /// <summary>
        /// Called on Start.
        /// Intialises Frames and Sets as unstable
        /// </summary>
        private void Start()
        {
            Reset();
        }

        /// <summary>
        /// Clears frames and sets as unstable
        /// </summary>
        public void Reset()
        {
            _frames = new List<float>();
            _stabalised = false;
        }

        /// <summary>
        /// Determine the Standard Deviation of the
        /// currently stored frame rates
        /// </summary>
        private void GetStandardDeviation()
        {
            _average = _frames.Average();
            if(_frames.Count < 2)
            {
                _standardDeviation = 0;
                return;

            }

            float sumOfDerivation = 0;
            foreach (float value in _frames)
            {
                sumOfDerivation += (value) * (value);
            }
            float sumOfDerivationAverage = sumOfDerivation / (_frames.Count - 1);
            float newStandardDeviation = Mathf.Sqrt(sumOfDerivationAverage - (_average * _average));
            if(!_stabalised && Mathf.Abs(_standardDeviation - newStandardDeviation) < StdDevPercent)
            {
                Debug.Log("Stablised");
                _stabalised = true;
            }

            _standardDeviation = newStandardDeviation;
        }

        /// <summary>
        /// Called per Frame.
        /// Determines if an update to framerate is required
        /// If so; 
        /// - determines framerate since last check; 
        /// - calculate STDDev
        /// - Report current values
        /// </summary>
        private void Update()
        {
            if (_mTimeCounter < mRefreshTime)
            {
                _mTimeCounter += Time.deltaTime;
                _mFrameCounter++;
            }
            else
            {
                //This code will break if you set your m_refreshTime to 0, which makes no sense.
                _mLastFramerate = _mFrameCounter / _mTimeCounter;
                _mFrameCounter = 0;
                _mTimeCounter = 0.0f;

                _frames.Add(_mLastFramerate);
                GetStandardDeviation();
                if(_stabalised)
                {
                    // Debug.Log(average + ", +/- " + standard_deviation);
                    VXProcess.add_log_line(_average + ", +/- " + _standardDeviation);
                }
                else
                {
                    VXProcess.add_log_line("Stabalising...");
                }
            
            }
        }
    }
}
