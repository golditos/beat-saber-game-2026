using UnityEngine;
using System;

namespace Ath.Beat.Audio
{
    public class BeatDetector : MonoBehaviour
    {
        public int sampleSize = 1024;
        public AudioSource audioSource;
        public float playbackAheadTime = 10f;
        public int bpm = 134;
        
        public double songStartDspTime { get; private set; }
        public int CurrentBeat { get; private set; }
        public float SecondsPerBeat => 60f / bpm;

        public event Action<int> OnBeat;
        public event Action<float[]> OnSpectrum;
        
        private float[] spectrumData;
        private int lastBeat = -1;
        private bool isPlaying = false;
        
        private void Awake()
        {
            spectrumData = new float[sampleSize];
            if (audioSource == null)
                audioSource = GetComponent<AudioSource>();
        }

        public void StartSong(double dspDelay = 0.1)
        {
            songStartDspTime = AudioSettings.dspTime + dspDelay;
            audioSource.PlayScheduled(songStartDspTime);
            isPlaying = true;
            lastBeat = -1;
            CurrentBeat = 0;
        }

        public void StopSong()
        {
            audioSource.Stop();
            isPlaying = false;
        }

        private void Update()
        {
            if (!isPlaying || !audioSource.isPlaying) return;
            double elapsed = AudioSettings.dspTime - songStartDspTime;
            if (elapsed < 0) return;
            int beat = Mathf.FloorToInt((float)(elapsed / SecondsPerBeat));

            if (beat != lastBeat)
            {
                lastBeat = beat;
                CurrentBeat = beat;
                OnBeat?.Invoke(beat);
            }
            
            audioSource.GetSpectrumData(spectrumData, 0, FFTWindow.BlackmanHarris);
            OnSpectrum?.Invoke(spectrumData);
        }

        public double GetBeatDspTime(int beatIndex)
        {
            return songStartDspTime + beatIndex * SecondsPerBeat;
        }

        public float GetBeatsUntilDspTime(double dspTime)
        {
            return (float)((dspTime - AudioSettings.dspTime) / SecondsPerBeat);
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

