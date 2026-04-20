using FMOD.Studio;
using FMODUnity;

public class AudioManager : Singleton<AudioManager>
{
    private const string CatLoadEvent = "event:/SFX/Catload";
    private const string ClickEvent = "event:/SFX/Click";
    private const string CorrectMatchEvent = "event:/SFX/Correctmatch";
    private const string ExplosionEvent = "event:/SFX/Explosion";
    private const string FailureEvent = "event:/SFX/Faliure";
    private const string GlassBreakEvent = "event:/SFX/Glassbreak";
    private const string VictoryEvent = "event:/SFX/Victory";
    private const string Attention1Event = "event:/CatSFX/Attention1";
    private const string Attention2Event = "event:/CatSFX/Attention2";
    private const string Food1Event = "event:/CatSFX/Food1";
    private const string Food2Event = "event:/CatSFX/Food2";
    private const string Toy1Event = "event:/CatSFX/Toy1";
    private const string Toy2Event = "event:/CatSFX/Toy2";
    private const string MusicEvent = "event:/Mx";
    private const string MusicTransitionParameter = "Transition";
    private const string MusicMenuTransition = "Menu";
    private const string MusicInGameTransition = "InGame";
    private const string MasterBusPath = "bus:/";

    private EventInstance _musicInstance;
    private Bus _masterBus;
    private float _masterVolume = 1f;

    public float MasterVolume => _masterVolume;

    public void PlayCatLoad()
    {
        PlayOneShot(CatLoadEvent);
    }

    public void PlayClick()
    {
        PlayOneShot(ClickEvent);
    }

    public void PlayCorrectMatch()
    {
        PlayOneShot(CorrectMatchEvent);
    }

    public void PlayExplosion()
    {
        PlayOneShot(ExplosionEvent);
    }

    public void PlayFailure()
    {
        PlayOneShot(FailureEvent);
    }

    public void PlayGlassBreak()
    {
        PlayOneShot(GlassBreakEvent);
    }

    public void PlayVictory()
    {
        PlayOneShot(VictoryEvent);
    }

    public void PlayAttention1()
    {
        PlayOneShot(Attention1Event);
    }

    public void PlayAttention2()
    {
        PlayOneShot(Attention2Event);
    }

    public void PlayFood1()
    {
        PlayOneShot(Food1Event);
    }

    public void PlayFood2()
    {
        PlayOneShot(Food2Event);
    }

    public void PlayToy1()
    {
        PlayOneShot(Toy1Event);
    }

    public void PlayToy2()
    {
        PlayOneShot(Toy2Event);
    }

    public float PlayCatRequest(E_CatRequestSound requestSound)
    {
        var eventPath = GetCatRequestEventPath(requestSound);
        if (string.IsNullOrEmpty(eventPath))
        {
            return 0f;
        }

        return PlayOneShotWithDuration(eventPath);
    }

    public void PlayMusic()
    {
        if (_musicInstance.isValid())
        {
            return;
        }

        StartMusic(MusicMenuTransition);
    }

    public void PlayMenuMusic()
    {
        if (!_musicInstance.isValid())
        {
            StartMusic(MusicMenuTransition);
            return;
        }

        SetMusicTransition(MusicMenuTransition);
    }

    public void PlayInGameMusic()
    {
        if (!_musicInstance.isValid())
        {
            StartMusic(MusicInGameTransition);
            return;
        }

        SetMusicTransition(MusicInGameTransition);
    }

    public void RestartMenuMusic()
    {
        RestartMusic(MusicMenuTransition);
    }

    public void RestartInGameMusic()
    {
        RestartMusic(MusicInGameTransition);
    }

    public void SetMasterVolume(float volume)
    {
        _masterVolume = UnityEngine.Mathf.Clamp01(volume);
        GetMasterBus().setVolume(_masterVolume);
    }

    public void StopMusic()
    {
        StopMusic(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

    public void StopAndResetMusic()
    {
        StopMusic(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }

    private void StopMusic(FMOD.Studio.STOP_MODE stopMode)
    {
        if (!_musicInstance.isValid())
        {
            return;
        }

        _musicInstance.stop(stopMode);
        _musicInstance.release();
        _musicInstance.clearHandle();
    }

    private void RestartMusic(string transition)
    {
        StopAndResetMusic();
        StartMusic(transition);
    }

    private void StartMusic(string transition)
    {
        _musicInstance = RuntimeManager.CreateInstance(MusicEvent);
        SetMusicTransition(transition);
        _musicInstance.start();
    }

    private Bus GetMasterBus()
    {
        if (!_masterBus.isValid())
        {
            _masterBus = RuntimeManager.GetBus(MasterBusPath);
        }

        return _masterBus;
    }

    private void SetMusicTransition(string transition)
    {
        if (!_musicInstance.isValid())
        {
            return;
        }

        _musicInstance.setParameterByNameWithLabel(MusicTransitionParameter, transition);
    }

    private static void PlayOneShot(string eventPath)
    {
        RuntimeManager.PlayOneShot(eventPath);
    }

    private static string GetCatRequestEventPath(E_CatRequestSound requestSound)
    {
        switch (requestSound)
        {
            case E_CatRequestSound.Food:
                return UnityEngine.Random.Range(0, 2) == 0 ? Food1Event : Food2Event;
            case E_CatRequestSound.Petting:
                return UnityEngine.Random.Range(0, 2) == 0 ? Attention1Event : Attention2Event;
            case E_CatRequestSound.Toy:
                return UnityEngine.Random.Range(0, 2) == 0 ? Toy1Event : Toy2Event;
            default:
                return null;
        }
    }

    private static float PlayOneShotWithDuration(string eventPath)
    {
        var instance = RuntimeManager.CreateInstance(eventPath);
        instance.getDescription(out var description);
        description.getLength(out var lengthMs);
        instance.start();
        instance.release();
        return lengthMs / 1000f;
    }
}
