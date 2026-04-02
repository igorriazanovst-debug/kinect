using System.Collections.Generic;
using UnityEngine;

namespace EduMotion.Games
{
    [System.Serializable]
    public class AudioEntry { public string Key; public AudioClip Clip; }

    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [SerializeField] private AudioEntry[] _voiceClips;
        [SerializeField] private AudioEntry[] _sfxClips;
        [SerializeField] private AudioSource  _voiceSource;
        [SerializeField] private AudioSource  _sfxSource;

        private Dictionary<string,AudioClip> _voiceMap = new();
        private Dictionary<string,AudioClip> _sfxMap   = new();

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            foreach (var e in _voiceClips) _voiceMap[e.Key] = e.Clip;
            foreach (var e in _sfxClips)   _sfxMap[e.Key]   = e.Clip;
        }

        public void PlayVoice(string key)
        {
            if (!_voiceMap.TryGetValue(key, out var clip)) return;
            _voiceSource.Stop(); _voiceSource.clip = clip; _voiceSource.Play();
        }

        public void PlaySFX(string key)
        {
            if (!_sfxMap.TryGetValue(key, out var clip)) return;
            _sfxSource.PlayOneShot(clip);
        }

        public void StopVoice() => _voiceSource.Stop();
    }
}
