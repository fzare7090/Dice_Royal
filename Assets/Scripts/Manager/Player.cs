using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private Board board;
    [SerializeField] private Elexir elexir;
    [SerializeField] private Dice dice;
    [SerializeField] private int teamId;

    private int level;

    public Board GetBoard() { return board; }
    public void SetBoard(Board newBoard) { board = newBoard; }

    public Elexir GetElexir() { return elexir; }

    public int GetTeamId() { return teamId; }

    public void SetTeamId(int newTeamId) { teamId = newTeamId; }

    public KingCastleConfig GetKingCastleConfig()
    {
        return board != null ? board.GetKingCastle() : null;
    }

    public int GetLevel(){ return level; }

    public void SetLevel(int NewLevel)
    {
        level = NewLevel;
    }
}
