using DG.Tweening;
using UnityEngine;

public class LightsTransitions : MonoBehaviour
{
    [SerializeField] private Light Spotlight1;
    [SerializeField] private Light Spotlight2;
    [SerializeField] private Light Spotlight3;
    [SerializeField] private Light Spotlight4;

    void Start()
    {
        LightTransition(Spotlight1, 19);
        LightTransition(Spotlight2, 19);
        LightTransition(Spotlight3, 10);
        LightTransition(Spotlight4, 10);
    }

    private void LightTransition(Light light, float intensity)
    {
        Sequence lightSeq = DOTween.Sequence();
        lightSeq.Join(light.DOColor(Color.white, 2f).SetEase(Ease.OutCubic));
        lightSeq.Join(light.DOIntensity(intensity, 2f).SetEase(Ease.OutCubic));
    }
}
