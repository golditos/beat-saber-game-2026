using UnityEngine;
using System;

namespace Ath.Beat.Gameplay
{
    public enum HitRating { Perfect, Good, Ok, Miss }
    public class BlockScript : MonoBehaviour
    {
        public enum BlockType { Left, Right, Bomb }
        public BlockType blockType = BlockType.Left;
        public bool WasHit { get; private set; }
        public double HitDspTime { get; private set; }
        public static event Action<BlockScript, HitRating> OnBlockHit;
        public static event Action<BlockScript> OnBlockMiss;
        private float speed;
        private bool active;
        private const float PrefectWindow = 0.060f;
        private const float GoodWindow = 0.120f;
        private const float OkWindow = 0.200f;

        public void Init(BlockType type, double hitDspTime, float noteSpeed)
        {
            blockType = type;
            HitDspTime = hitDspTime;
            speed = noteSpeed;
            WasHit = false;
            active = true;
        }

        private void Update()
        {
            if (!active) return;
            transform.Translate(0f, 0f, -speed * Time.deltaTime, Space.World);
            if (transform.position.z < -1.5f)
                Miss();
        }
        public void RegisterHit()
        {
            if (!active || WasHit) return;

            WasHit = true;
            active = false;

            HitRating rating = EvaluateTiming();
            OnBlockHit?.Invoke(this, rating);
        }

        private void Miss()
        {
            active = false;
        }

        private HitRating EvaluateTiming()
        {
            double diff = Math.Abs(AudioSettings.dspTime - HitDspTime);
            if (diff < PrefectWindow) return HitRating.Perfect;
            if (diff < GoodWindow) return HitRating.Good;
            if (diff < OkWindow) return HitRating.Ok;
            return HitRating.Miss;
        }
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = blockType switch
            {
                BlockType.Left  => Color.red,
                BlockType.Right => Color.blue,
                BlockType.Bomb  => Color.black,
                _               => Color.white
            };
            Gizmos.DrawWireCube(transform.position, transform.lossyScale);
        }
#endif
    }
}
