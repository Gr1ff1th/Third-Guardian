using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public AudioMixer masterMixer;
    //for music changes
    //public AudioMixer musicMixer;

    //musics
    //public List<AudioSource> dayMusic;
    //public List<AudioSource> nightMusic;
    public AudioSource scoreMusic;
    public AudioSource defeatMusic;
    public AudioClip defeatMusic2;

    public AudioClip epilogueMusic1;
    public AudioClip epilogueMusic2;

    public AudioClip storyMusic1;
    public AudioClip storyMusic2;

    public List<AudioClip> dayMusics;
    public List<AudioClip> nightMusics;

    public float sfxLevel;
    public float sfxVol;
    public float musicLevel;
    public float musicVol;

    public AudioSource mainMusicHolder;
    public AudioSource encounterMusicHolder;

    public void SetSfxLevel(float sfxLvl)
    {
        //masterMixer.SetFloat("sfxVol", sfxLvl);
        masterMixer.SetFloat("sfxVol", Mathf.Log10(sfxLvl) *20);

        sfxLevel = GameObject.Find("SFX Volume Slider").GetComponent<Slider>().value;
        sfxVol = Mathf.Log10(sfxLvl) * 20;

        if (GameObject.Find("AMMRoomController").GetComponent<PhotonRoom>().currentScene == 1)
        {
            PlayerPrefs.SetFloat("SfxLevel", GameObject.Find("SoundManager").GetComponent<SoundManager>().sfxLevel);
            PlayerPrefs.SetFloat("SfxVol", GameObject.Find("SoundManager").GetComponent<SoundManager>().sfxVol);
        }
        //unfortunately i had named these differently on different scenes..
        if (GameObject.Find("AMMRoomController").GetComponent<PhotonRoom>().currentScene > 2)
        {
            //PlayerPrefs.SetFloat("SfxLevel", GameObject.Find("SFX Player").GetComponent<SoundManager>().sfxLevel);
            //PlayerPrefs.SetFloat("SfxVol", GameObject.Find("SFX Player").GetComponent<SoundManager>().sfxVol);

            PlayerPrefs.SetFloat("SfxLevel", GameManager.ins.references.soundManager.sfxLevel);
            PlayerPrefs.SetFloat("SfxVol", GameManager.ins.references.soundManager.sfxVol);
        }
    }

    public void SetMusicLevel(float musicLvl)
    {
        masterMixer.SetFloat("musicVol", Mathf.Log10(musicLvl) *20);
        musicVol = Mathf.Log10(musicLvl) * 20;

        musicLevel = GameObject.Find("Music Volume Slider").GetComponent<Slider>().value;

        if (GameObject.Find("AMMRoomController").GetComponent<PhotonRoom>().currentScene == 1)
        {
            PlayerPrefs.SetFloat("MusicLevel", GameObject.Find("SoundManager").GetComponent<SoundManager>().musicLevel);
            PlayerPrefs.SetFloat("MusicVol", GameObject.Find("SoundManager").GetComponent<SoundManager>().musicVol);
        }

        if (GameObject.Find("AMMRoomController").GetComponent<PhotonRoom>().currentScene > 2)
        {
            PlayerPrefs.SetFloat("MusicLevel", GameManager.ins.references.soundManager.musicLevel);
            PlayerPrefs.SetFloat("MusicVol", GameManager.ins.references.soundManager.musicVol);
        }
    }
    /*
    public void SetMasterLevel(float masterLvl)
    {
        masterMixer.SetFloat("masterVol", Mathf.Log10(masterLvl) * 20);
    }
    */

    #region old music system
    /*
    public void PlayDayMusic()
    {
        StartCoroutine(StartFade(masterMixer, "fadeOutVol", 3f, 0.0001f));
        Invoke("PlayDayMusicAfterDelay", 3.5f);
    }

    void PlayDayMusicAfterDelay()
    {
        //stop all night musics
        for (int i = 0; i < nightMusic.Count; i++)
        {
            nightMusic[i].GetComponent<AudioSource>().Stop();
        }
        //stop all day musics
        for (int i = 0; i < dayMusic.Count; i++)
        {
            dayMusic[i].GetComponent<AudioSource>().Stop();
        }

        //select new random day track to play on loop
        int number = Random.Range(0, dayMusic.Count);
        dayMusic[number].GetComponent<AudioSource>().Play();

        StartCoroutine(StartFade(masterMixer, "fadeOutVol", 3f, 1f));

        int number2 = Random.Range(150, 250);
        //switches the track, incase turn drags on
        Invoke("SwitchDayTrack", number2);
    }

    void SwitchDayTrack()
    {
        if (Clock.clock.isNight == false && Clock.clock.ulrimanInPlay == false)
            PlayDayMusic();
    }

    public void PlayNightMusic()
    {
        StartCoroutine(StartFade(masterMixer, "fadeOutVol", 3f, 0.0001f));
        Invoke("PlayNightMusicAfterDelay", 3.5f);
    }

    void PlayNightMusicAfterDelay()
    {
        //stop all night musics
        for (int i = 0; i < nightMusic.Count; i++)
        {
            nightMusic[i].GetComponent<AudioSource>().Stop();
        }
        //stop all day musics
        for (int i = 0; i < dayMusic.Count; i++)
        {
            dayMusic[i].GetComponent<AudioSource>().Stop();
        }

        //select new random night track to play on loop
        int number = Random.Range(0, nightMusic.Count);
        nightMusic[number].GetComponent<AudioSource>().Play();

        StartCoroutine(StartFade(masterMixer, "fadeOutVol", 3f, 1f));

        int number2 = Random.Range(150, 250);
        //switches the track, incase turn drags on
        Invoke("SwitchNightTrack", number2);
    }

    void SwitchNightTrack()
    {
        //should add condition here to not switch track if game has ended
        if ((Clock.clock.isNight == true || Clock.clock.ulrimanInPlay == true) && Clock.clock.totalTurnsPlayed != GameManager.ins.endTime)
            PlayNightMusic();
    }
    */
    #endregion

    //when playing end score music
    //could use for story musics too actually
    public void PlayScoreMusic(int musicType)
    {
        mainMusicHolder.Stop();
        encounterMusicHolder.Stop();

        if (musicType == 1)
        {
            scoreMusic.clip = epilogueMusic1;
        }
        if (musicType == 2)
        {
            scoreMusic.clip = epilogueMusic2;
        }
        if (musicType == 3)
        {
            scoreMusic.clip = storyMusic1;
        }
        if (musicType == 4)
        {
            scoreMusic.clip = defeatMusic.clip;
        }
        if (musicType == 5)
        {
            scoreMusic.clip = storyMusic2;
        }
        if (musicType == 6)
        {
            scoreMusic.clip = defeatMusic2;
        }

        StartCoroutine(StartFade(masterMixer, "fadeOutVol", 1f, 0.0001f));
        Invoke("PlayScoreMusicAfterDelay", 1.5f);
    }

    void PlayScoreMusicAfterDelay()
    {
        mainMusicHolder.Stop();

        //mainMusicHolder.volume = 0f;

        //switch to score music
        scoreMusic.GetComponent<AudioSource>().Play();

        StartCoroutine(StartFade(masterMixer, "fadeOutVol", 2f, 1f));
    }

    //when playing defeat usic
    public void PlayDefeatMusic()
    {
        //StartCoroutine(StartFade(masterMixer, "fadeOutVol", 1f, 0.0001f));
        //Invoke("PlayDefeatMusicAfterDelay", 0.5f);
        mainMusicHolder.Stop();

        defeatMusic.volume = 0f;
        defeatMusic.GetComponent<AudioSource>().Play();

        StartCoroutine(StartMusicFade(defeatMusic, 1f, 1f));
    }


    //used for fading whole channel
    public static IEnumerator StartFade(AudioMixer audioMixer, string exposedParam, float duration, float targetVolume)
    {
        float currentTime = 0;
        float currentVol;
        audioMixer.GetFloat(exposedParam, out currentVol);
        currentVol = Mathf.Pow(10, currentVol / 20);
        float targetValue = Mathf.Clamp(targetVolume, 0.0001f, 1);

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            float newVol = Mathf.Lerp(currentVol, targetValue, currentTime / duration);
            audioMixer.SetFloat(exposedParam, Mathf.Log10(newVol) * 20);
            yield return null;
        }
        yield break;
    }

    //when changing music volume
    //used by certain encounters
    public void ChangeMusicVolume(float fadeTime, float volume)
    {
        StopCoroutine("StartMusicFade");
        StartCoroutine(StartMusicFade(mainMusicHolder, fadeTime, volume));
        //Invoke("PlayScoreMusicAfterDelay", 1.5f);
    }

    public void ChangeEncounterMusicVolume(float fadeTime, float volume)
    {
        //dunno if this stop command actually does anything
        StopCoroutine("StartMusicFade");
        StartCoroutine(StartMusicFade(encounterMusicHolder, fadeTime, volume));
        //Invoke("PlayScoreMusicAfterDelay", 1.5f);
    }

    /* old
     * when changing music volume
    //used by certain encounters
    public void ChangeMusicVolume(float fadeTime, float volume)
    {
        StartCoroutine(StartFade(masterMixer, "fadeOutVol", fadeTime, volume));
        //Invoke("PlayScoreMusicAfterDelay", 1.5f);
    }
    */

    public void PlayDayMusic2()
    {
        StartCoroutine(StartMusicFade(mainMusicHolder, 3f, 0.0001f));
        Invoke("PlayDayMusicAfterDelay2", 3.5f);
    }

    void PlayDayMusicAfterDelay2()
    {
        //select new random day track to play on loop
        int number = Random.Range(0, dayMusics.Count);
        //dayMusic[number].GetComponent<AudioSource>().Play();
        mainMusicHolder.clip = dayMusics[number];
        mainMusicHolder.Play();

        //dont bring main music volume back, if encounter music is playing
        //also dont bring it back if player is defeated
        if (GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().encounterMusicHolder.isPlaying == false && 
            GameManager.ins.references.soundManager.scoreMusic.isPlaying == false)// || GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().encounterMusicHolder.volume < 0.2)
        {
            StartCoroutine(StartMusicFade(mainMusicHolder, 3f, 1f));
        }

        int number2 = Random.Range(150, 250);

        //switches the track, incase turn drags on
        Invoke("SwitchDayTrack2", number2);
    }

    void SwitchDayTrack2()
    {
        if (Clock.clock.isNight == false)
            PlayDayMusic2();
    }

    public void PlayNightMusic2()
    {
        StartCoroutine(StartMusicFade(mainMusicHolder, 3f, 0.0001f));
        Invoke("PlayNightMusicAfterDelay2", 3.5f);
    }

    void PlayNightMusicAfterDelay2()
    {
        //select new random night track to play on loop
        int number = Random.Range(0, nightMusics.Count);
        //nightMusic[number].GetComponent<AudioSource>().Play();
        mainMusicHolder.clip = nightMusics[number];
        mainMusicHolder.Play();

        //dont bring main music volume back, if encounter music is playing
        if (GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().encounterMusicHolder.isPlaying == false &&
            GameManager.ins.references.soundManager.scoreMusic.isPlaying == false)// || GameManager.ins.exploreHandler.GetComponent<ExploreHandler>().encounterMusicHolder.volume < 0.2)
        {
            StartCoroutine(StartMusicFade(mainMusicHolder, 3f, 1f));
        }

        int number2 = Random.Range(150, 250);
        //switches the track, incase turn drags on
        Invoke("SwitchNightTrack2", number2);
    }

    void SwitchNightTrack2()
    {
        //should add condition here to not switch track if game has ended
        if ((Clock.clock.isNight == true || Clock.clock.ulrimanInPlay == true) && Clock.clock.totalTurnsPlayed != GameManager.ins.endTime)
            PlayNightMusic2();
    }

    //can be used for any audiosource music fadeout
    public static IEnumerator StartMusicFade(AudioSource audioSource, float duration, float targetVolume)
    {
        float currentTime = 0;
        float currentVol = audioSource.volume;
        //audioSource.GetFloat(exposedParam, out currentVol);
        //float currentVol = Mathf.Pow(10, audioSource.volume / 20);
        float targetValue = Mathf.Clamp(targetVolume, 0.0001f, 1);

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            float newVol = Mathf.Lerp(currentVol, targetValue, currentTime / duration);
            audioSource.volume = newVol;
            //audioSource.volume = Mathf.Log10(newVol) * 20;
            yield return null;
        }
        yield break;
    }
}

