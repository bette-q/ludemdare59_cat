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

    public void PlayCatRequest(E_CatRequestSound requestSound)
    {
        switch (requestSound)
        {
            case E_CatRequestSound.Food:
                PlayOneShot(UnityEngine.Random.Range(0, 2) == 0 ? Food1Event : Food2Event);
                break;
            case E_CatRequestSound.Petting:
                PlayOneShot(UnityEngine.Random.Range(0, 2) == 0 ? Attention1Event : Attention2Event);
                break;
            case E_CatRequestSound.Toy:
                PlayOneShot(UnityEngine.Random.Range(0, 2) == 0 ? Toy1Event : Toy2Event);
                break;
        }
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
}
