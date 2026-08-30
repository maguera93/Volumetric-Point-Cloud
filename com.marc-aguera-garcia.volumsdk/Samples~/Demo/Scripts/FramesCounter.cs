using UnityEngine;

namespace Volum.SDK.Samples.Demo
{
    public class FramesCounter : MonoBehaviour
    {
        private const float SAMPLE_INTERNAL = 0.5F;

        private float _acturalFrameRate = 0;
        private float _timer;
        private int _frames;
        private string _label = "Volum SDK Demo";

        private VolumStream _stream;

        /// <summary>
        /// Stars frame tracking of a Volum Stream. Can be called at the Start of the Volum Stream
        /// </summary>
        /// <param name="stream"></param>
        public void Track(VolumStream stream)
        {
            _stream = stream;
            _frames = stream != null ? stream.FrameIndex : 0;
            _timer = 0;
            _acturalFrameRate = 0;
        }

        /// <summary>
        /// Stops frame tracking
        /// </summary>
        public void StopTracking()
        {
            _stream = null;
        }

        private void Update()
        {
            if (_stream == null)
                return;


            _timer += Time.unscaledDeltaTime;
            if (_timer < SAMPLE_INTERNAL)
                return;

            int currentFrameIndex = _stream.FrameIndex;
            int framesProduced = currentFrameIndex - _frames;

            _acturalFrameRate = framesProduced / _timer;
            _frames = currentFrameIndex;
            _timer = 0f;
            _label = $"Volum SDK Demo\n{_acturalFrameRate:0.0} FPS";
        }

        private void OnGUI()
        {
            GUI.Label(new Rect(12, 12, 320, 40), _label);
        }
    }
}