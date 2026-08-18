using Cysharp.Threading.Tasks;
using UnityEngine;

public class SoundManager : SingletonBase<SoundManager>
{
    [SerializeField] private AudioSource AudioSource_BGM;
    [SerializeField] private AudioSource AudioSource_SFX;

    private async UniTaskVoid LoadAndPlayAudioClip(AudioSource audioSource, string path, bool isLoop = false)
    {
        AudioClip clip = await ResourceManager.Instance.LoadAsset<AudioClip>(path);

        if (isLoop)
        {
            audioSource.clip = clip;
            audioSource.loop = true;
            audioSource.Play();
        }
        else
        {
            audioSource.PlayOneShot(clip);
        }
    }

    private string GetPathSFX(string soundID)
    {
        return $"Audio/SFX/{soundID}";
    }

    private string GetPathBGM(string soundID)
    {
        return $"Audio/BGM/{soundID}";
    }

    public void PlaySFX(string soundID)
    {
        LoadAndPlayAudioClip(AudioSource_SFX, GetPathSFX(soundID)).Forget();
    }

    public void PlayBGM(string soundID)
    {
        LoadAndPlayAudioClip(AudioSource_BGM, GetPathBGM(soundID), true).Forget();
    }

    public void StopSFX()
    {
        AudioSource_SFX.Stop();
    }

    public void StopBGM()
    {
        AudioSource_BGM.Stop();
    }
}
