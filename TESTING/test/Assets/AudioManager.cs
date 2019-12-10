using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance = null; //used to check if only one in scene

    public Sound[] SoundEffects; //number of sfx

    //easier to find soundeffect
    public Dictionary<string, AudioSource> AudioSources = new Dictionary<string, AudioSource>();

    public AudioSource OneShotAudioSource;
    private void Awake()
    {
        //Check if only one in scene
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if(Instance != this)
        {
            //already one in scene
            Destroy(gameObject);
            return;
        }
        else
        {
            return;
        }

        
        //Create number of AudioSources based on number of sfx in SoundEffects
        foreach(Sound entry in SoundEffects)
        {
            //add an audio source for the given sound
            AudioSource source = gameObject.AddComponent<AudioSource>();

            source.volume = entry.Volume;
            source.clip = entry.Clip;
            source.pitch = entry.Pitch;
            source.playOnAwake = entry.PlayOnAwake;
            source.loop = entry.Loop;

            //play on awake if indicated
            if (source.playOnAwake)
            {
                source.Play();
            }

            //save audio sources references into dictionary
            AudioSources.Add(entry.Name, source);
        }
        //sPlaySound("Arc");
    }
   
    //find audiosource based on string, used with PlaySound()
    public AudioSource GetAudioSource(string soundName)
    {
        AudioSource source;
        if (AudioSources.TryGetValue(soundName, out source))
        {
            return source;
        }
        else
        {
            Debug.LogWarning("No sound with name " + soundName + " was found");
            return null;
        }
    }
    
    //PLAY ONESHOT SOUND:
    public void PlayOneShotSound(string soundName)
    {
        if (AudioSources.TryGetValue(soundName, out OneShotAudioSource))
        {
            //OneShotAudioSource.PlayOneShot();
            //OneShotAudioSource.PlayOneShot(Eyes, );
        }
        //if not, send debug log
        else
        {
            Debug.LogWarning("No sound with name " + soundName + " was found");
        }
    }
    
    
    //play sound of given string
    public void PlaySound(string soundName)
    {
        AudioSource source;
        //See if audiosource and sound exists
        if (AudioSources.TryGetValue(soundName, out source))
        {

            source.Play(); //Play sound
        }
        //if not, send debug log
        else
        {
            Debug.LogWarning("No sound with name " + soundName + " was found");
        }
    }
    

    //Used to stop sound
    public void StopSound(string soundName)
    {
        AudioSource source;

        //See if audiosource and sound exists
        if (AudioSources.TryGetValue(soundName, out source))
        {
            source.Stop(); //Play sound
        }
        //if not, send debug log
        else
        {
            Debug.LogWarning("No sound with name " + soundName + " was found");
        }
    }

    //Play sound at specific location
    public void PlayPositionalSound(int index, Vector3 position)
    {
        AudioSource.PlayClipAtPoint(SoundEffects[index].Clip, position);
    }
}

//Modify variables of sound played
[System.Serializable]
public class Sound
{
    public string Name; //used to search sounds
    public AudioClip Clip; //audio to be played

    [Range(0f, 1f)]
    public float Volume = 1f; //loudness

    [Range(0f, 3f)]
    public float Pitch = 1f; //pitch
    public bool PlayOnAwake = false; //play immediately
    public bool Loop; //loop audio
}
