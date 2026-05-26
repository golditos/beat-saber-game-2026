using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Ath.Beat.Audio;
using Ath.Beat.Gameplay;

namespace Ath.Beat.Gameplay
{
    public class NoteSpawner : MonoBehaviour
    {
        public BeatDetector beatDetector;
        public GameObject leftBlockPrefab;
        public GameObject rightBlockPrefab;
        public Transform spawnPoint;
        public float spawnDistance = 15f;
        public float laneWidth = 0.6f;
        public float rowHeight = 0.6f;
        public float baseY = 1.0f;
        public float noteSpeed = 6f;
        public int poolSize = 20;
        public BlockScript.BlockType[] pattern = new BlockScript.BlockType[]
        {
            BlockScript.BlockType.Right,
            BlockScript.BlockType.Left,
            BlockScript.BlockType.Right,
            BlockScript.BlockType.Left,
        };
        public int spawnEveryNBeats = 1;
        public int[] patternLanes = new int[] { 2, 1, 3, 0 };
        public int[] patternRows = new int[] { 1, 1, 1, 1 };
        private Queue<BlockScript> leftPool  = new();
        private Queue<BlockScript> rightPool = new();
        private int patternIndex = 0;
        
        private void Awake()
        {
            InitPool(leftPool, leftBlockPrefab,  poolSize);
            InitPool(rightPool, rightBlockPrefab, poolSize);
        }

        private void OnEnable()
        {
            if (beatDetector != null)
                beatDetector.OnBeat += HandleBeat;
        }

        private void OnDisable()
        {
            if (beatDetector != null)
                beatDetector.OnBeat -= HandleBeat;
        }
        
        private void InitPool(Queue<BlockScript> pool, GameObject prefab, int size)
        {
            if (prefab == null) return;
            for (int i = 0; i < size; i++)
            {
                var go = Instantiate(prefab);
                var bloc = go.GetComponent<BlockScript>();
                go.SetActive(false);
                pool.Enqueue(bloc);
            }
        }

        private BlockScript GetFromPool(BlockScript.BlockType type)
        {
            var pool = type == BlockScript.BlockType.Left ? leftPool : rightPool;
            if (pool.Count == 0) return null;
            var bloc = pool.Dequeue();
            bloc.gameObject.SetActive(true);
            return bloc;
        }

        public void ReturnToPool(BlockScript bloc)
        {
            bloc.gameObject.SetActive(false);
            var pool = bloc.blockType == BlockScript.BlockType.Left ? leftPool : rightPool;
            pool.Enqueue(bloc);
        }
        
        private void HandleBeat(int beat)
        {

            if (beat % spawnEveryNBeats != 0) return;
            
            int idx = patternIndex % pattern.Length;
            var type = pattern[idx];

            int lane = patternLanes.Length > idx ? patternLanes[idx] : 1;
            int row = patternRows.Length > idx ? patternRows[idx]  : 1;

            patternIndex++;
            
            double hitDspTime = beatDetector.GetBeatDspTime(beat);

            SpawnBlock(type, lane, row, hitDspTime);
        }

        private void SpawnBlock(BlockScript.BlockType type, int lane, int row, double hitDspTime)
        {
            var bloc = GetFromPool(type);
            if (bloc == null) return;
            float x = (lane - 1.5f) * laneWidth;
            float y = baseY + row * rowHeight;
            float z = spawnPoint != null
                ? spawnPoint.position.z
                : Camera.main.transform.position.z + spawnDistance;
            bloc.transform.position = new Vector3(x, y, z);
            bloc.Init(type, hitDspTime, noteSpeed);
        }
    }
}