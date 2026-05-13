using TMPro;
using UnityEngine;

public class Castle : MonoBehaviour
{

    [SerializeField] private TMP_Text hpText;

    public CastleHp castleHP;


    private void Start()
    {
        SetCastleHPText();
    }


    public CastleHp GetCastleHP()
    {
        return castleHP;
    }


    public Transform GetCastleTransform()
    {
        return transform;
    }
 
    private void SetCastleHPText()
    {
        hpText.SetText(castleHP.GetHPCastle().ToString());
    }

    /// level & upgrade


}

