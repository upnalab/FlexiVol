using UnityEngine;

namespace Voxon.Examples._1_SpawningObject
{
    /// <summary>
    /// Monobehaviour that triggers a tone and recolors the parent gameObject on collision.
    /// The recoloring will then fade over time, back to the base color of white
    /// </summary>
    public class Touched : MonoBehaviour {
        /// <summary>
        /// transpose in semitones
        /// </summary>
        private int _transpose = -4;
        /// <summary>
        /// Associated Audio Source (Plays the Tone)
        /// </summary>
        private AudioSource _sound;

        /// <summary>
        /// Position of Audio Source
        /// </summary>
        public int position;
        /// <summary>
        /// Sample Rate of Audio
        /// </summary>
        public int sampleRate = 44100;
        /// <summary>
        /// Frequency of Audio Tone
        /// </summary>
        public int frequency = 440;
        /// <summary>
        /// Audio Clip which will be played by <see cref="_sound"/>
        /// </summary>
        private AudioClip _myClip;

        /// <summary>
        /// Initialise Touched. Adding / Finding an AudioSource, configuring the source options and Generating Tone to be played on collision
        /// </summary>
        private void Start () {
            if(GetComponent<AudioSource>() == null)
            {
                gameObject.AddComponent<AudioSource>();
            }

            AudioClip myClip = AudioClip.Create("MyTone", 44100, 1, 24500, true, OnAudioRead, OnAudioSetPosition);

            sampleRate = AudioSettings.outputSampleRate;
            _sound = GetComponent<AudioSource>();
            _sound.clip = myClip;
        
            _sound.minDistance = 1;
            _sound.volume = 0.25f;
            _sound.spatialBlend = 0f;
        }
	
        /// <summary>
        /// Called once per frame. If the parent isn't white, transitions back towards this color while reducing tone volume.
        /// Otherwise makes sure the tone isn't playing
        /// </summary>
        void Update () {

            if (gameObject.GetComponent<Renderer>().sharedMaterial.color != Color.white)
            {
                Color tmp = gameObject.GetComponent<Renderer>().sharedMaterial.color;
                if (tmp.r < 1.0)
                {
                    tmp.r += 0.01f;
                }
                if (tmp.b < 1.0)
                {
                    tmp.b += 0.01f;
                }
                if (tmp.g < 1.0)
                {
                    tmp.g += 0.01f;
                }

                _sound.volume = _sound.volume - 0.025f;
                gameObject.GetComponent<Renderer>().sharedMaterial.color = tmp;
            }
            else if (_sound.isPlaying)
            {
                _sound.Stop();
            }

        }

        /// <summary>
        /// Triggered on collision with another object. Sets the parent to the colliding objects color, and plays a note / tone
        /// based on the colliding objects material color.
        /// </summary>
        /// <param name="collision"></param>
        private void OnCollisionEnter(Collision collision)
        {
            Color col = collision.gameObject.GetComponent<Renderer>().sharedMaterial.color;
            gameObject.GetComponent<Renderer>().sharedMaterial.color = col;

            float note = -1; // invalid value to detect when note is pressed

            if (col == Color.red) note = 0;  // C
            else if (col == Color.green) note = 2;  // D
            else if (col == Color.blue) note = 4;  // E
            else if (col == Color.cyan) note = 5;  // G
            else if (col == Color.magenta) note = 9;  // B
            else if (col == Color.white) note = 11;  // C
            else
            {
                note = 7;
            }

            if (note >= 0)
            {
                _sound.volume = 0.25f;
                _sound.pitch = Mathf.Pow(2, (note + _transpose) / 12.0f);
                _sound.Play();

            }
        }

        /// <summary>
        /// Generates audio data based on current position, frequency and sample rate
        /// </summary>
        /// <param name="data"></param>
        void OnAudioRead(float[] data)
        {
            int count = 0;
            while (count < data.Length)
            {
                data[count] = Mathf.Sign(Mathf.Sin(2 * Mathf.PI * frequency * position / sampleRate));
                position++;
                count++;
            }
        }

        /// <summary>
        /// Sets position of Audio Source
        /// </summary>
        /// <param name="newPosition"></param>
        void OnAudioSetPosition(int newPosition)
        {
            position = newPosition;
        }
    }
}
