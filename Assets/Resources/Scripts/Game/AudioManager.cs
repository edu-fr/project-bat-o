using UnityEngine;
using UnityEngine.Audio;
using System;
using UI;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Game
{
    public class AudioManager : MonoBehaviour
    {
        public Sound[] Sounds;

        public static AudioManager instance;
        
        private void Awake()
        {
            if (instance == null)
                instance = this;
            else
            {
                Destroy(gameObject);
                return;
            }
            
            //DontDestroyOnLoad(gameObject);
            
            foreach (Sound s in Sounds)
            {
                s.Source = gameObject.AddComponent<AudioSource>();
                s.Source.clip = s.Clip;
                s.Source.volume = s.Volume;
                s.Source.pitch = s.Pitch;
                s.Source.loop = s.Loop;
            }
            
        }
        
        public void Play(string name)
        {
            Sound s = Array.Find(Sounds, sound => sound.Name == name);
            if (s == null)
            {
                Debug.LogWarning("Sound: " + name + " not found!");
            }

            // adding some randomness to the volume and pitch of the sfx
            s.Source.volume = s.Volume * (1f + Random.Range(-s.VolumeVariance, s.VolumeVariance));
            s.Source.pitch = s.Pitch * (1f + Random.Range(-s.PitchVariance, s.PitchVariance));

            if (PauseMenu.GameIsPaused)
            {
                s.Source.pitch *= .5f;
            }
             
            
            s.Source.Play();
        }
        
        public void Stop(string name)
        {
            Sound s = Array.Find(Sounds, item => item.Name == name);
            if (s == null)
            {
                Debug.LogWarning("Sound: " + ((Object) this).name + " not found!");
                return;
            }

            s.Source.volume = s.Volume * (1f + UnityEngine.Random.Range(-s.VolumeVariance / 2f, s.VolumeVariance / 2f));
            s.Source.pitch = s.Pitch * (1f + UnityEngine.Random.Range(-s.PitchVariance / 2f, s.PitchVariance / 2f));
            
            
            s.Source.Stop ();
        }
    }
}
