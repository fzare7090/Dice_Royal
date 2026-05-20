using Unity.VisualScripting;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance ;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip fireEffect;
    [SerializeField] private AudioClip projectileEffect;
    [SerializeField] private AudioClip giantAttackEffect;
    [SerializeField] private AudioClip distroyCastleEffect;
    

    void Awake()
    {
        Instance = this;
    }
    public void PlayFireEffect()
    {
        audioSource.PlayOneShot(fireEffect);
    }
    public void PlayProjectileEffect()
    {
        audioSource.PlayOneShot(projectileEffect);
    }
    public void PlayGiantAttackEffect()
    {
        audioSource.PlayOneShot(giantAttackEffect);
    }
    public void PlayDistroyCastleEffect()
    {
        audioSource.PlayOneShot(distroyCastleEffect);
    }
}
