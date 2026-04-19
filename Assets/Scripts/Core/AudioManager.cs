using FMOD.Studio;
using FMODUnity;

public class AudioManager : Singleton<AudioManager>
{
    private const string CatLoadEvent = "event:/CatSFX/SFX/Catload";
    private const string ClickEvent = "event:/CatSFX/SFX/Click";
    private const string CorrectMatchEvent = "event:/CatSFX/SFX/Correctmatch";
    private const string ExplosionEvent = "event:/CatSFX/SFX/Explosion";
    private const string FailureEvent = "event:/CatSFX/SFX/Faliure";
    private const string GlassBreakEvent = "event:/CatSFX/SFX/Glassbreak";
    private const string VictoryEvent = "event:/CatSFX/SFX/Victory";
    private const string Attention1Event = "event:/CatSFX/Attention1";
    private const string Attention2Event = "event:/CatSFX/Attention2";
    private const string Food1Event = "event:/CatSFX/Food1";
    private const string Food2Event = "event:/CatSFX/Food2";
    private const string Toy1Event = "event:/CatSFX/Toy1";
    private const string Toy2Event = "event:/CatSFX/Toy2";
    private const string MusicEvent = "event:/Mx";

    private EventInstance _musicInstance;

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

        _musicInstance = RuntimeManager.CreateInstance(MusicEvent);
        _musicInstance.start();
    }

    public void StopMusic()
    {
        if (!_musicInstance.isValid())
        {
            return;
        }

        _musicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        _musicInstance.release();
        _musicInstance.clearHandle();
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
