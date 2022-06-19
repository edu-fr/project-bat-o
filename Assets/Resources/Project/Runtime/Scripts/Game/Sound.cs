using UnityEngine;

namespace Game
{
    [System.Serializable]
    public class Sound
    {
        public string Name;
        
        public AudioClip Clip;
        
        [Range(0f, 1f)]
        public float Volume;
        [Range(.1f, 3f)]
        public float Pitch;
        
        [Range(0f, 0.1f)]
        public float VolumeVariance;

        [Range(0f, 0.05f)]
        public float PitchVariance;

        public bool Loop;

        [HideInInspector]
        public AudioSource Source;
        
    }
}
