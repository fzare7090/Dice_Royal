using UnityEngine;

public class CastlesPosition : MonoBehaviour
{
    private Vector3 kingCastleP1 = new Vector3(0, -1300, 0);
    private Vector3 rightPrincessCastleP1 = new Vector3(400, -1100, 0);
    private Vector3 leftPrincessCastleP1 = new Vector3(-400, -1100, 0);
    private Vector3 kingCastleP2 = new Vector3(0, 1300, 0);
    private Vector3 rightPrincessCastleP2 = new Vector3(400, 1100, 0);
    private Vector3 leftPrincessCastleP2 = new Vector3(-400, 1100, 0);

    public Vector3 GetKingCastleP1()
    {
        return kingCastleP1;
    }

    public Vector3 GetRightPrincessCastleP1()
    {
        return rightPrincessCastleP1;
    }

    public Vector3 GetLeftPrincessCastleP1()
    {
        return leftPrincessCastleP1;
    }

    public Vector3 GetKingCastleP2()
    {
        return kingCastleP2;
    }

    public Vector3 GetRightPrincessCastleP2()
    {
        return rightPrincessCastleP2;
    }

    public Vector3 GetLeftPrincessCastleP2()
    {
        return leftPrincessCastleP2;
    }
}
