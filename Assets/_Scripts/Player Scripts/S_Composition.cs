using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Scripts.Player_Scripts
{
    class S_Composition : MonoBehaviour
    {
        public delegate void BeatLostHandler(object source, EventArgs e);
        public event BeatLostHandler BeatLost;

        public delegate void BeatHitHandler(object source, EventArgs e);
        public event BeatHitHandler BeatHit;

        private AudioClip audioClip;

        private LinkedList<RectTransform> beatScrollers;

        private float[] beatEvents;
        private bool[] beatHits;
        private int iBeat;
        public float bpm;
        private float spb;
        private float translation;
        private float beatCounter;
        private float lastBeat;
        private float nextBeat;
        private float forgiveness;

        private const float distancePerBeat = 150;

        public S_Composition(AudioClip clip, float bpm, float beginningOffset)
        {
            this.audioClip = clip;
            this.bpm = bpm;

            float beatsPerSec = bpm / 60f; // 2 bps
            float secsPerBeat = 1 / beatsPerSec; // 1/2 spb
            float framesPerBeat = 60f * secsPerBeat;

            float clipLengthInFrames = clip.length * 60f;
            beatEvents = new float[Mathf.CeilToInt(clipLengthInFrames / framesPerBeat)];
            beatHits = new bool[beatEvents.Length];
            forgiveness = 0.125f;

            int i = 0;
            for (float j = beginningOffset; j < clip.length; j += secsPerBeat)
            {
                beatEvents[i] = j;
                i++;
            }

            beatScrollers = new LinkedList<RectTransform>();
            beatScrollers.AddLast(transform.GetChild(0).GetComponent<RectTransform>());
            beatScrollers.AddLast(transform.GetChild(1).GetComponent<RectTransform>());
            beatScrollers.AddLast(transform.GetChild(2).GetComponent<RectTransform>());
            beatScrollers.AddLast(transform.GetChild(3).GetComponent<RectTransform>());
            beatScrollers.AddLast(transform.GetChild(4).GetComponent<RectTransform>());
            beatScrollers.AddLast(transform.GetChild(5).GetComponent<RectTransform>());
        }

        public S_Composition(AudioClip clip, float[] beatEvents)
        {
            this.audioClip = clip;
            this.beatEvents = beatEvents;
        }

        public void Initialize(AudioClip clip, float bpm, float beginningOffset, float beatForgiveness)
        {
            this.audioClip = clip;
            this.bpm = bpm;

            float beatsPerSec = bpm / 60f; // 2 bps
            float secsPerBeat = 1 / beatsPerSec; // 1/2 spb
            spb = secsPerBeat;
            float framesPerBeat = 60f * secsPerBeat;
            translation = distancePerBeat / spb;
            if (beginningOffset < 0)
                beginningOffset += secsPerBeat;

            float clipLengthInFrames = clip.length * 60f;
            beatEvents = new float[Mathf.CeilToInt(clipLengthInFrames / framesPerBeat)];
            beatHits = new bool[beatEvents.Length];
            forgiveness = beatForgiveness;

            int i = 0;
            for (float j = beginningOffset; j < clip.length; j += secsPerBeat)
            {
                beatEvents[i] = j;
                i++;
            }

            beatScrollers = new LinkedList<RectTransform>();
            beatScrollers.AddLast(transform.GetChild(0).GetComponent<RectTransform>());
            beatScrollers.AddLast(transform.GetChild(1).GetComponent<RectTransform>());
            beatScrollers.AddLast(transform.GetChild(2).GetComponent<RectTransform>());
            beatScrollers.AddLast(transform.GetChild(3).GetComponent<RectTransform>());
            beatScrollers.AddLast(transform.GetChild(4).GetComponent<RectTransform>());
            beatScrollers.AddLast(transform.GetChild(5).GetComponent<RectTransform>());
        }

        public void Updater(float deltaTime)
        {
            beatCounter += deltaTime;
            if (beatEvents[iBeat] <= beatCounter)
            {
                iBeat++;
                lastBeat = beatEvents[iBeat - 1];
                nextBeat = beatEvents[iBeat];
                OnBeatHit();
            }

            if (iBeat > 0 && !beatHits[iBeat - 1] && beatEvents[iBeat - 1] <= beatCounter - forgiveness)
            {
                beatHits[iBeat - 1] = true;
                OnBeatLost();
            }

            LinkedListNode<RectTransform> r = beatScrollers.First;
            for (int i = 0; i < 6; i++)
            {
                float f = beatEvents[i + iBeat];
                float diff = f - beatCounter;
                float distance = diff * translation;
                distance -= 297;
                r.Value.localPosition = new Vector3(distance, 0, 0);
                r = r.Next;
            }
        }

        protected virtual void OnBeatHit()
        {
            if (BeatHit != null)
                BeatHit(this, EventArgs.Empty);
        }

        protected virtual void OnBeatLost()
        {
            if (BeatLost != null)
                BeatLost(this, EventArgs.Empty);
        }

        public bool CheckForBeatHit(float forgiveness)
        {
            return (!beatHits[iBeat - 1] && ApproximatelyEqual(beatCounter, lastBeat, forgiveness)) ||
                   (!beatHits[iBeat] && ApproximatelyEqual(beatCounter, nextBeat, forgiveness));
        }

        public void HitBeat(float forgiveness)
        {
            if (ApproximatelyEqual(beatCounter, lastBeat, forgiveness))
                beatHits[iBeat - 1] = true;
            else if (ApproximatelyEqual(beatCounter, nextBeat, forgiveness))
                beatHits[iBeat] = true;
        }

        private bool ApproximatelyEqual(float a, float b, float forgiveness)
        {
            return Mathf.Abs(b - a) <= forgiveness;
        }
    }
}
