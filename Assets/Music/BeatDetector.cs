using UnityEngine;

namespace Ath.Beat.Audio
{
    public class BeatDetector : MonoBehaviour
    {
        public int sampleSize = 1024;
        public AudioSource audioSource;
        public float playbackAheadTime = 10f;
        public double songStartDsptime;
        public int bpm = 134;

        private float[] spectrumData;

        private void Awake()
        {
            spectrumData = new float[sampleSize];
            audioSource = GetComponent<AudioSource>();
        }
    }

    public interface IBeatState
    {
        void Enter();
        void Tick();
        void Exit();
    }

    public class NoopBeatState : IBeatState
    {
        public void Enter()
        {
            return;
        }

        public void Tick()
        {
            return;
        }

        public void Exit()
        {
            return;
        }
    }
    
}

