using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

[System.Serializable]
public class MusicClip
{
    public AudioClip clip;
    [Range(0f, 2f)]
    public float volume = 1f;
}

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;
    [SerializeField] private int songfrequency;
    [SerializeField] private int songfrequencyMax;
    [SerializeField] private MusicClip mainMenuMusic;
    [SerializeField] private MusicClip sailingMusic;
    [SerializeField] private MusicClip islandMusic;
    [SerializeField] private MusicClip fishingMusic;
    [SerializeField] private MusicClip cannonMusic;
    [SerializeField] private MusicClip barrelMusic;
    [SerializeField] private MusicClip[] fiddleTunes;
    [SerializeField] private MusicClip[] shanties;
    public AudioClip[] oceanClips;
    public AudioClip[] shoreClips;
    public AudioClip[] dockClips;
    private AudioSource oceanSource;
    private AudioSource ambientSource;
    private AudioSource ambientSource2;
    private AudioSource songSource;
    private MusicClip currentClip;
    private bool isInSailingScene = false;
    [SerializeField] private AudioMixerGroup musicGroup;
    [SerializeField] private AudioMixerGroup songGroup;
    [SerializeField] private AudioMixerGroup OceanGroup;
    [SerializeField] private AudioMixer mixer;
    
    private Coroutine shantyRoutine;
    private List<int> fiddleQueue = new List<int>();
    private int fiddleIndex = 0;

    public float MasterVolume {get; private set;} = 1f;
    public float SongVolume {get; private set;} = 1f;
    public float SFXVolume {get; private set;} = 1f;

    public void SetMasterVolume(float value)
    {
        MasterVolume=value;
        mixer.SetFloat("MasterVolume", Mathf.Log10(Mathf.Max(value,0.0001f))*20);
        PlayerPrefs.SetFloat("MasterVolume",value);
    }

    public void SetSFXVolume(float value)
    {
        SFXVolume=value;
        mixer.SetFloat("SFXVolume", Mathf.Log10(Mathf.Max(value,0.0001f))*20);
        PlayerPrefs.SetFloat("SFXVolume",value);
    }

    public void SetSongVolume(float value)
    {
        SongVolume=value;
        mixer.SetFloat("SongVolume", Mathf.Log10(Mathf.Max(value,0.0001f))*20);
        PlayerPrefs.SetFloat("SongVolume",value);
    }
    public void SetMusicVolume(float value)
    {
        mixer.SetFloat("AmbientVolume", Mathf.Log10(Mathf.Max(value,0.0001f)) * 20);
    }

    public void SetEventVolume(float value)
    {
        mixer.SetFloat("SongVolume", Mathf.Log10(Mathf.Max(value,0.0001f)) * 20);
    }

    private void InitializeFiddleQueue()
    {
        fiddleQueue.Clear();
        for (int i = 0; i < fiddleTunes.Length; i++)
            fiddleQueue.Add(i);

        // Shuffle the list
        for (int i = 0; i < fiddleQueue.Count; i++)
        {
            int temp = fiddleQueue[i];
            int randomIndex = Random.Range(i, fiddleQueue.Count);
            fiddleQueue[i] = fiddleQueue[randomIndex];
            fiddleQueue[randomIndex] = temp;
        }

        fiddleIndex = 0;
    }
    void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        //LoadSettings();

        ambientSource = gameObject.AddComponent<AudioSource>();
        ambientSource.loop = true;
        ambientSource.playOnAwake = false;
        
        ambientSource2 = gameObject.AddComponent<AudioSource>();
        ambientSource2.loop = true;
        ambientSource2.playOnAwake = false;

        songSource = gameObject.AddComponent<AudioSource>();
        songSource.loop = false;
        songSource.playOnAwake = false;
        ambientSource.outputAudioMixerGroup = musicGroup;
        songSource.outputAudioMixerGroup = songGroup;
        ambientSource2.outputAudioMixerGroup = OceanGroup;
        
        oceanSource = gameObject.AddComponent<AudioSource>();
        oceanSource.outputAudioMixerGroup = OceanGroup;
        oceanSource.loop = true;
        oceanSource.playOnAwake = false;
        oceanSource.volume = 0.6f;
        InitializeFiddleQueue();
    }

    void Start(){}

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    private void PlayMusicForScene(string sceneName)
    {
        MusicClip newClip = null;

        switch (sceneName)
        {
            case "StartScreen":
                newClip = mainMenuMusic;
                break;

            case "PRODUCTION":
                newClip = sailingMusic;
                oceanSource.volume = 1f;
                oceanSource.clip = oceanClips[Random.Range(0, oceanClips.Length)];
                ambientSource2.clip = null;
                ambientSource2.volume = 0f;
                break;

            case "R-IslandPort":
                newClip = islandMusic;
                oceanSource.volume = 1f;
                oceanSource.clip = shoreClips[Random.Range(0, shoreClips.Length)];
                ambientSource2.clip = dockClips[Random.Range(0, dockClips.Length)];
                ambientSource2.volume = 1f;
                break;

            case "H-IslandPort":
                newClip = islandMusic;
                oceanSource.volume = 1f;
                oceanSource.clip = shoreClips[Random.Range(0, shoreClips.Length)];
                ambientSource2.clip = dockClips[Random.Range(0, dockClips.Length)];
                ambientSource2.volume = 1f;
                break;

            case "N-IslandPort":
                newClip = islandMusic;
                oceanSource.volume = 1f;
                oceanSource.clip = shoreClips[Random.Range(0, shoreClips.Length)];
                ambientSource2.clip = dockClips[Random.Range(0, dockClips.Length)];
                ambientSource2.volume = 1f;
                break;

            case "FishingMinigame":
                newClip = fishingMusic;
                //oceanSource.clip = oceanClips[Random.Range(0, oceanClips.Length)];
                oceanSource.volume = 0.65f;
                ambientSource2.clip = null;
                ambientSource2.volume = 0f;
                break;

            case "CannonPractice":
                newClip = cannonMusic;
                //oceanSource.clip = oceanClips[Random.Range(0, oceanClips.Length)];
                ambientSource2.clip = null;
                ambientSource2.volume = 0f;
                oceanSource.volume = 0.5f;
                break;

            case "BarrelDodging":
                newClip = barrelMusic;
                //oceanSource.clip = oceanClips[Random.Range(0, oceanClips.Length)];
                oceanSource.volume = 0f;
                ambientSource2.clip = null;
                ambientSource2.volume = 0f;
                break;

        }
        if (sceneName != "StartScreen")
        {
            Debug.Log("Playing ocean for scene: " + sceneName);
            oceanSource.Play();
            if (ambientSource2.clip != null)
             {
                  ambientSource2.volume = 1.0f;
                    ambientSource2.Play();
             }
            else{ 
                ambientSource2.Stop();
            }
        }

        if (newClip != null && newClip != currentClip)
        {
            currentClip = newClip;
            ambientSource.clip = currentClip.clip;
            ambientSource.volume = 0;
            ambientSource.Play();
            StartCoroutine(FadeAmbient(currentClip.volume, 1f));
        }
    }
    private IEnumerator FadeAmbient(float targetVolume, float duration)
    {
        float startVolume = ambientSource.volume;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            ambientSource.volume = Mathf.Lerp(startVolume, targetVolume, time / duration);
            yield return null;
        }

        ambientSource.volume = targetVolume;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)

    {
        Debug.Log("Scene loaded event fired");
        isInSailingScene = scene.name == "PRODUCTION";

        if (isInSailingScene)
        {
            StartSailingMusic();
        }
        else
        {
            if (shantyRoutine != null)
            {
                StopCoroutine(shantyRoutine);
                shantyRoutine = null;
            }
            songSource.Stop();
            ambientSource.Stop();
            currentClip = null;
            PlayMusicForScene(scene.name);
        }
    }

    void StartSailingMusic()
    {
        ambientSource2.Stop();   // stop island dock ambience
        ambientSource2.clip = null;
        ambientSource.clip = sailingMusic.clip;
        ambientSource.volume = sailingMusic.volume;
        ambientSource.Play();
        StartCoroutine(FadeAmbient(sailingMusic.volume, 1f));
        oceanSource.clip = oceanClips[Random.Range(0, oceanClips.Length)];
        oceanSource.volume = 0.6f;
        oceanSource.Play();
        if (shantyRoutine != null)
        {
            StopCoroutine(shantyRoutine);
            shantyRoutine = null;
        }
        songSource.Stop();

        shantyRoutine = StartCoroutine(RandomShantyRoutine());
    }

    IEnumerator RandomShantyRoutine()
    {
        Debug.Log("RandomShantyRoutine started");
        while (isInSailingScene)
        {

            float waitTime = Random.Range(songfrequency, songfrequencyMax);
            Debug.Log("Waiting " + waitTime + " seconds before next shanty");
            yield return new WaitForSecondsRealtime(waitTime);
            float startVolume = ambientSource.volume;
            float originalVolume = startVolume;
            float time = 0f;

            while (time < 1f)
            {
                time += Time.unscaledDeltaTime;
                ambientSource.volume = Mathf.Lerp(startVolume, 0f, time / 1f);
                yield return null;
            }
            ambientSource.volume = 0f;

            Debug.Log("Playing random shanty");


            MusicClip randomClip = fiddleTunes[fiddleQueue[fiddleIndex]];
            fiddleIndex++;

            if (fiddleIndex >= fiddleQueue.Count)
            {
                InitializeFiddleQueue();
            }

            songSource.clip = randomClip.clip;
            songSource.volume = randomClip.volume;
            songSource.Play();

            yield return new WaitForSecondsRealtime(randomClip.clip.length);
            
            startVolume = ambientSource.volume;
            time = 0f;
            while (time < 1f)
            {
                time += Time.unscaledDeltaTime;
                ambientSource.volume = Mathf.Lerp(startVolume, originalVolume, time / 1f);
                yield return null;
            }

            yield return new WaitUntil(() => !songSource.isPlaying);
        }
    }

    // private void LoadSettings()
    // {
    //     MasterVolume = PlayerPrefs.GetFloat("MasterVolume",1f);
    //     MusicVolume = PlayerPrefs.GetFloat("MusicVolume",1f);
    //     SFXVolume = PlayerPrefs.GetFloat("SFXVolume",1f);

    //     SetMasterVolume(MasterVolume);
    //     SetMusicVolume(MusicVolume);
    //     SetSFXVolume(SFXVolume);
    // }
}
