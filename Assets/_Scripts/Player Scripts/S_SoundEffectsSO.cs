using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Sound Effects", menuName = "Sounds/Sound Effects", order = 1)]
public class S_SoundEffectsSO : ScriptableObject
{
    static S_SoundEffectsSO _instance = null;

    [SerializeField] private AudioClip[] bashSoundEffects;
    [SerializeField] private AudioClip[] swingSoundEffects;
    [SerializeField] private AudioClip[] shingSoundEffects;
    [Space]
    [SerializeField] private AudioClip clockTickEffect;
    [Space]
    [SerializeField] private AudioClip bellDingEffect;

    public static S_SoundEffectsSO Instance
    {
        get
        {
            if (!_instance)
                _instance = Resources.FindObjectsOfTypeAll<S_SoundEffectsSO>().FirstOrDefault();
            return _instance;
        }
    }

    public AudioClip GetRandomBashSoundEffect()
    {
        return bashSoundEffects[Random.Range(0, bashSoundEffects.Length - 1)];
    }

    public AudioClip GetRandomSwingSoundEffect()
    {
        return swingSoundEffects[Random.Range(0, swingSoundEffects.Length - 1)];
    }

    public AudioClip GetRandomShingSoundEffect()
    {
        return shingSoundEffects[Random.Range(0, shingSoundEffects.Length - 1)];
    }

    public AudioClip GetClockTickSoundEffect()
    {
        return clockTickEffect;
    }

    public AudioClip GetBellDingSoundEffect()
    {
        return bellDingEffect;
    }
}
