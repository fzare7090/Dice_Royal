using UnityEngine;
using System.Collections.Generic;


public class Player : MonoBehaviour
{
    [SerializeField] private Board board;
    [SerializeField] private Elexir elexir;
    [SerializeField] private Dice dice;


    public Board GetBoard() {  return board; }

    public Elexir GetElexir() {  return elexir; }

    public KingCastleConfig GetKingCastleConfig() { return board.GetKingCastle(); }

  


}
