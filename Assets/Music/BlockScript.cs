using UnityEngine;
using System;

namespace Ath.Beat.Gameplay
{
    public class BlockScript : MonoBehaviour
    {
        public enum BlockType { Left, Right }
        public static event Action<BlockScript, HitRating> OnBlockHit;
        public static event Action<BlockScript>            OnBlockMiss;
        public BlockType blockType { get; private set; }
        private double _hitDspTime;
        private float _speed;
        private float _hitZ;
        private bool     _alive = false;
        private const float WindowPerfect = 0.06f;
        private const float WindowGood    = 0.12f;
        private const float WindowOk      = 0.20f;
        private const float WindowMiss    = 0.30f;
        private NoteSpawner _spawner;
        
        public void Init(BlockType type, double hitDspTime, float speed, float hitZ = 0f)
        {
            blockType = type;
            _hitDspTime = hitDspTime;
            _speed = speed;
            _hitZ = hitZ;
            _alive = true;
            if (_spawner == null)
                _spawner = FindObjectOfType<NoteSpawner>();
            float secondsUntilHit = (float)(_hitDspTime - AudioSettings.dspTime);
            Vector3 pos = transform.position;
            pos.z = _hitZ + secondsUntilHit * _speed;
            transform.position = pos;
        }

        private void Update()
        {
            if (!_alive) return;
            transform.Translate(0f, 0f, -_speed * Time.deltaTime, Space.World);
            double elapsed = AudioSettings.dspTime - _hitDspTime;
            if (elapsed > WindowMiss)
            {
                Miss();
            }
        }
        
        public void TryHit()
        {
            if (!_alive) return;
            double delta = Math.Abs(AudioSettings.dspTime - _hitDspTime);
            HitRating rating;
            if (delta <= WindowPerfect) rating = HitRating.Perfect;
            else if (delta <= WindowGood) rating = HitRating.Good;
            else if (delta <= WindowOk) rating = HitRating.Ok;
            else rating = HitRating.Miss;
            if (rating == HitRating.Miss)
            {
                Miss();
                return;
            }
            _alive = false;
            OnBlockHit?.Invoke(this, rating);
            ReturnToPool();
        }
        
        private void Miss()
        {
            _alive = false;
            OnBlockMiss?.Invoke(this);
            ReturnToPool();
        }

        private void ReturnToPool()
        {
            if (_spawner != null)
                _spawner.ReturnToPool(this);
            else
                gameObject.SetActive(false);
        }
    }
    public enum HitRating { Perfect, Good, Ok, Miss }
}