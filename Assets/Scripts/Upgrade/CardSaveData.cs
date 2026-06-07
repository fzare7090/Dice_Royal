using UnityEngine;

[CreateAssetMenu(menuName = "Save Data/Card Save")]
public class CardSaveData : ScriptableObject
{
    public string cardId;
    public int level = 1;
    public int ownedCopies;
}