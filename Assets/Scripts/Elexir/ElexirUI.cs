using UnityEngine;
using UnityEngine.UI;

public class ElexirUI : MonoBehaviour
{
    [SerializeField] private Elexir elexir;

    private Slider elexirSlider;

    void Awake()
    {
        elexirSlider = GetComponent<Slider>();
    }

    void Update()
    {
        if (elexir != null && elexirSlider != null)
        {
            elexirSlider.maxValue = elexir.GetMaxElexir();
            elexirSlider.value = elexir.GetElexir();
        }
    }
}
