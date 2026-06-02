using UnityEngine;

public class OrginalAudioManager : MonoBehaviour
{
    public static OrginalAudioManager Instance;
    [SerializeField] protected AudioSource music;
    [SerializeField] protected AudioSource SFX;
    [SerializeField] protected AudioClip introMusic;
    [SerializeField] protected AudioClip startBattleMusic;
    [SerializeField] protected AudioClip bGMusic;
    [SerializeField] protected AudioClip startGameMusic;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void PlayIntroMusic()
    {
        music.PlayOneShot(introMusic);
    }
    public void PlayStartBattleMusic()
    {
        music.PlayOneShot(startBattleMusic);
    }
    public void PlayBGMusic()
    {
        music.clip = bGMusic;
        music.loop = true;
        music.Play();
    }
    public void StopMusic()
    {
        if (music != null)
        {
            music.Stop();
        }
    }
    public void PlayStartGameMusic()
    {
        music.PlayOneShot(startGameMusic);
    }

}

