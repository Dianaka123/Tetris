using System.Collections.Generic;
using Models.Interfaces;
using strange.extensions.mediation.impl;
using UnityEngine;

namespace Models.Managers
{
    public class SoundManager: View, ISoundManager
    {
        private Dictionary<SoundType, (AudioClip clip, AudioSource audioSource)> soundDictionary;
        
        [SerializeField] private AudioClip shapeMoveSound;
        [SerializeField] private AudioClip shapeRotateRound;
        [SerializeField] private AudioClip destroySound;
        [SerializeField] private AudioClip musicSound;

        private AudioSource effectAudioSource;
        private AudioSource musicAudioSource;

        protected override void Start()
        {
            base.Start();
            
            effectAudioSource = gameObject.AddComponent<AudioSource>();
            musicAudioSource = gameObject.AddComponent<AudioSource>();
           
            soundDictionary = new Dictionary<SoundType, (AudioClip clip, AudioSource audioSource)>
            {
                {SoundType.ShapeMove, (shapeMoveSound, effectAudioSource)},
                {SoundType.ShapeRotate, (shapeRotateRound, effectAudioSource)},
                {SoundType.Destroy, (destroySound, effectAudioSource)},
                {SoundType.Background, (musicSound, musicAudioSource)}
            };
        }

        public void PlaybackSound(SoundType soundType)
        {
            var soundInfo = soundDictionary[soundType];
            soundInfo.audioSource.clip = soundInfo.clip;
            soundInfo.audioSource.Play();
        }
    }
}