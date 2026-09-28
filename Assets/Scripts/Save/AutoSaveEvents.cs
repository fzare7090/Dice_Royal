using System;

public static class AutoSaveEvents
{
    public static event Action OnCoinChanged;
    public static event Action OnGemChanged;

    public static event Action OnCardChanged;
    public static event Action OnCastleChanged;
    public static event Action OnDiceChanged;

    public static void CoinChanged()
    {
        OnCoinChanged?.Invoke();
    }

    public static void GemChanged()
    {
        OnGemChanged?.Invoke();
    }

    public static void CardChanged()
    {
        OnCardChanged?.Invoke();
    }

    public static void CastleChanged()
    {
        OnCastleChanged?.Invoke();
    }

    public static void DiceChanged()
    {
        OnDiceChanged?.Invoke();
    }
}