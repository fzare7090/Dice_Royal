using UnityEngine;

public class TowersPosition : MonoBehaviour
{

    private Vector3 kingTowerP1 = new Vector3(0, -1300, 0);
    private Vector3 rightPrincessTowerP1 = new Vector3(400, -1100, 0);
    private Vector3 leftPrincessTowerP1 = new Vector3(-400, -1100, 0);
    private Vector3 kingTowerP2 = new Vector3(0, 1300, 0);
    private Vector3 rightPrincessTowerP2 = new Vector3(400, 1100, 0);
    private Vector3 leftPrincessTowerP2 = new Vector3(-400, 1100, 0);



    public Vector3 GetKingTowerP1()
    {
        return kingTowerP1;
    }

    public Vector3 GetRightPrincessTowerP1()
    {
        return rightPrincessTowerP1;
    }

    public Vector3 GetLeftPrincessTowerP1()
    {
        return leftPrincessTowerP1;
    }

    public Vector3 GetKingTowerP2()
    {
        return kingTowerP2;
    }

    public Vector3 GetRightPrincessTowerP2()
    {
        return rightPrincessTowerP2;
    }

    public Vector3 GetLeftPrincessTowerP2()
    {
        return leftPrincessTowerP2;
    }

}
