using UnityEngine;

public class PopUpManager : MonoBehaviour
{
    public static PopUpManager Instance;
    [SerializeField] private Transform upgradeCardParent;
    void Awake()
    {
        Instance = this;
    }
    public Transform GetUpgradeCardParent()
    {
        return upgradeCardParent;
    }
}
