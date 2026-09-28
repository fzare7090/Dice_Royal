using System;

[Serializable]
public class CardSaveInfo
{
    public string cardId;
    public int level;
    public int ownedCopies;

    public CardSaveInfo()
    {
    }

    public CardSaveInfo(string cardId, int level, int ownedCopies)
    {
        this.cardId = cardId;
        this.level = level;
        this.ownedCopies = ownedCopies;
    }
}