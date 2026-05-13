using UnityEngine;
using UnityEngine.UI;

public class ElexirUI : MonoBehaviour
{
    [SerializeField] private Slider elexirSlider;
    [SerializeField] private Elexir elexir;

    void Update()
    {
        if (elexir != null && elexirSlider != null)
        {
            elexirSlider.value = elexir.GetElexir();
        }
    }
}

