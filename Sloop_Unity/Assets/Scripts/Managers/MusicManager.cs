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
    [Range(0f, 1f)]
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
    [SerializeField] private MusicClip[] fiddleTunes;
    [SerializeField] private MusicClip[] shanties;
    public AudioClip[] oceanClips;
    public AudioClip[] shoreClips;
    private AudioSource oceanSource;
    private AudioSource ambientSource;
    private AudioSource songSource;
    private MusicClip currentClip;
    private bool isInSailingScene = false;
    [SerializeField] private AudioMixerGroup musicGroup;
    [SerializeField] private AudioMixerGroup songGroup;
    [SerializeField] private AudioMixer mixer;
    private Coroutine shantyRoutine;
    public void SetMusicVolume(float value)
    {
        mixer.SetFloat("AmbientVolume", Mathf.Log10(value) * 20);
    }

    public void SetEventVolume(float value)
    {
        mixer.SetFloat("SongVolume", Mathf.Log10(value) * 20);
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

        ambientSource = gameObject.AddComponent<AudioSource>();
        ambientSource.loop = true;
        ambientSource.playOnAwake = false;

        songSource = gameObject.AddComponent<AudioSource>();
        songSource.loop = false;
        songSource.playOnAwake = false;
        ambientSource.outputAudioMixerGroup = musicGroup;
        songSource.outputAudioMixerGroup = songGroup;
        oceanSource = gameObject.AddComponent<AudioSource>();
        oceanSource.loop = true;
        oceanSource.playOnAwake = false;
        oceanSource.volume = 0.6f;
    }

    void Start()
    {
        
    }
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
                oceanSource.clip = oceanClips[Random.Range(0, oceanClips.Length)];
                break;

            case "R-IslandPort":
                newClip = islandMusic;
                oceanSource.clip = shoreClips[Random.Range(0, shoreClips.Length)];
                break;

            case "H-IslandPort":
                newClip = islandMusic;
                oceanSource.clip = shoreClips[Random.Range(0, shoreClips.Length)];
                break;

            case "N-IslandPort":
                newClip = islandMusic;
                oceanSource.clip = shoreClips[Random.Range(0, shoreClips.Length)];
                break;

        }
        if (sceneName != "StartScreen")
        {
            Debug.Log("Playing ocean for scene: " + sceneName);
            oceanSource.volume = 1f;
            oceanSource.Play();
        }

        if (newClip != null && newClip != currentClip)
        {
            currentClip = newClip;
            ambientSource.clip = currentClip.clip;
            ambientSource.volume = 0;
            ambientSource.Play();
            StartCoroutine(FadeAmbient(currentClip.volume, 2f));
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
        ambientSource.clip = sailingMusic.clip;
        ambientSource.volume = sailingMusic.volume;
        ambientSource.Play();
        StartCoroutine(FadeAmbient(sailingMusic.volume, 2f));
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
            yield return new WaitForSeconds(waitTime);
            float startVolume = ambientSource.volume;
            float time = 0f;

            while (time < 2f)
            {
                time += Time.deltaTime;
                ambientSource.volume = Mathf.Lerp(startVolume, 0f, time / 2f);
                yield return null;
            }
            ambientSource.volume = 0f;

            Debug.Log("Playing random shanty");
            

            MusicClip randomClip = fiddleTunes[Random.Range(0, fiddleTunes.Length)];



            songSource.clip = randomClip.clip;
            songSource.volume = randomClip.volume;
            songSource.Play();

            yield return new WaitForSeconds(randomClip.clip.length);
            startVolume = ambientSource.volume;
            time = 0f;

            while (time < 2f)
            {
                time += Time.deltaTime;
                ambientSource.volume = Mathf.Lerp(startVolume, 0.4f, time / 2f);
                yield return null;
            }

            yield return new WaitUntil(() => !songSource.isPlaying);
        }
    }
}
