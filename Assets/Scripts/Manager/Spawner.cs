using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class Spawner : MonoBehaviour
{
    [SerializeField] private List<Transform> team1Enemies = new List<Transform>();
    [SerializeField] private List<Transform> team2Enemies = new List<Transform>();

    [Header("Prefabs")]
    [SerializeField] private GameObject unitPrefab;
    [SerializeField] private GameObject castlePrefab; // تغییر نام از towerPrefab به castlePrefab

    [Header("Unit Data")]
    [SerializeField] private UnitData playerWizardData;
    [SerializeField] private UnitData enemyWizardData;

    [Header("Board")]// تغییر از Tower Data به Castle Data
    [SerializeField] private Image player1Board;
    [SerializeField] private Image player2Board;


    [Header("Castle Data")]
    [SerializeField] private CastleData kingCastleP1;
    [SerializeField] private CastleData princessCastleP1;
    [SerializeField] private CastleData kingCastleP2;
    [SerializeField] private CastleData princessCastleP2;

    [Header("UI Buttons")]
    [SerializeField] private Button buttonP1;
    [SerializeField] private Button buttonP2;

    [Header("Spawn Parent")]
    [SerializeField] private Transform backGroundTransform;

    [Header("Drag Card")]
    [SerializeField] private Image drageCardP1;
    [SerializeField] private Image drageCardP2;

    [Header("Castle Manager")]
    [SerializeField] private GameObject castleManager; // تغییر نام از towerManager به castleManager

    private Player player1;
    private Player player2;

    void Awake()
    {
        player1 = buttonP1.GetComponent<Player>();
        player2 = buttonP2.GetComponent<Player>();
    }

    void Start()
    {
        buttonP1.onClick.AddListener(SpawnUnitForPlayer1);
        buttonP2.onClick.AddListener(SpawnUnitForPlayer2);
        SpawnCastle(1); // تغییر نام متد
        SpawnCastle(2); // تغییر نام متد
        SetPlayerTeamId();
    }

    void Update()
    {
        UpdateButtons();
    }

    private void SpawnUnitForPlayer1()
    {
        if (player1.GetElexir().GetElexir() >= playerWizardData.elexir)
        {
            SpawnUnit(1);
        }
    }

    private void SpawnUnitForPlayer2()
    {
        if (player2.GetElexir().GetElexir() >= enemyWizardData.elexir)
        {
            SpawnUnit(2);
        }
    }

    private void SpawnUnit(int teamId)
    {
        GameObject spawnedUnit = Instantiate(unitPrefab, backGroundTransform.parent);
        Unit unit = spawnedUnit.GetComponent<Unit>();
        Image image = spawnedUnit.GetComponent<Image>();
        unit.SetTeamId(teamId);

        if (teamId == 1)
        {
            spawnedUnit.transform.localPosition = drageCardP1.transform.localPosition;
            image.color = Color.green;
            unit.SetData(playerWizardData);
            team2Enemies.Add(unit.transform);
            unit.SetEnemies(team1Enemies);
        }
        else
        {
            spawnedUnit.transform.localPosition = drageCardP2.transform.localPosition;
            image.color = Color.red;
            unit.SetData(enemyWizardData);
            team1Enemies.Add(unit.transform);
            unit.SetEnemies(team2Enemies);
        }

        if (teamId == 1)
        {
            Elexir elexir = player1.GetElexir();
            elexir.UsingElexir(playerWizardData.elexir);
        }
        else
        {
            Elexir elexir = player2.GetElexir();
            elexir.UsingElexir(enemyWizardData.elexir);
        }
    }

    private void SpawnCastle(int teamId)
    {
        GameObject kingCastle =
    Instantiate(castlePrefab, backGroundTransform.parent);

        GameObject rightPrincessCastle =
            Instantiate(castlePrefab, backGroundTransform.parent);

        GameObject leftPrincessCastle =
            Instantiate(castlePrefab, backGroundTransform.parent);


        Image kingCastleImage = kingCastle.GetComponent<Image>();
        Image rightPrincessCastleImage = rightPrincessCastle.GetComponent<Image>();
        Image leftPrincessCastleImage = leftPrincessCastle.GetComponent<Image>();


        RectTransform kingRect = kingCastle.GetComponent<RectTransform>();
        kingRect.sizeDelta = new Vector2(200, 200);

        RectTransform rightRect = rightPrincessCastle.GetComponent<RectTransform>();
        rightRect.sizeDelta = new Vector2(150, 150);

        RectTransform leftRect = leftPrincessCastle.GetComponent<RectTransform>();
        leftRect.sizeDelta = new Vector2(150, 150);


        KingCastleConfig kingCastleScript =
            kingCastle.AddComponent<KingCastleConfig>();

        PrinceCastleConfig rightPrincessCastleScript =
            rightPrincessCastle.AddComponent<PrinceCastleConfig>();

        PrinceCastleConfig leftPrincessCastleScript =
            leftPrincessCastle.AddComponent<PrinceCastleConfig>();


        CastlesPosition castlePosition =
            castleManager.GetComponent<CastlesPosition>();

        // ست کردن TeamId
        kingCastleScript.SetTeamId(teamId);

        rightPrincessCastleScript.SetTeamId(teamId);

        leftPrincessCastleScript.SetTeamId(teamId);


        // ست کردن Enemy List
        if (teamId == 1)
        {
            kingCastleScript.SetEnemies(team1Enemies);

            rightPrincessCastleScript.SetEnemies(team1Enemies);

            leftPrincessCastleScript.SetEnemies(team1Enemies);
            kingCastleImage.color = Color.green;
            rightPrincessCastleImage.color = Color.green;
            leftPrincessCastleImage.color = Color.green;
        }
        else
        {
            kingCastleScript.SetEnemies(team2Enemies);

            rightPrincessCastleScript.SetEnemies(team2Enemies);

            leftPrincessCastleScript.SetEnemies(team2Enemies);
            kingCastleImage.color = Color.red;
            rightPrincessCastleImage.color = Color.red;
            leftPrincessCastleImage.color = Color.red;
        }

        // تنظیمات هر تیم
        if (teamId == 1)
        {
            // Position
            kingCastle.transform.localPosition =
                castlePosition.GetKingCastleP1();

            rightPrincessCastle.transform.localPosition =
                castlePosition.GetRightPrincessCastleP1();

            leftPrincessCastle.transform.localPosition =
                castlePosition.GetLeftPrincessCastleP1();

            // Data
            kingCastleScript.SetData(kingCastleP1);

            rightPrincessCastleScript.SetData(princessCastleP1);

            leftPrincessCastleScript.SetData(princessCastleP1);

            // اضافه شدن به Enemy List تیم مقابل
            team2Enemies.Add(kingCastleScript.transform);

            team2Enemies.Add(rightPrincessCastleScript.transform);

            team2Enemies.Add(leftPrincessCastleScript.transform);
        }
        else
        {
            // Position
            kingCastle.transform.localPosition =
                castlePosition.GetKingCastleP2();

            rightPrincessCastle.transform.localPosition =
                castlePosition.GetRightPrincessCastleP2();

            leftPrincessCastle.transform.localPosition =
                castlePosition.GetLeftPrincessCastleP2();

            // Data
            kingCastleScript.SetData(kingCastleP2);

            rightPrincessCastleScript.SetData(princessCastleP2);

            leftPrincessCastleScript.SetData(princessCastleP2);

            // اضافه شدن به Enemy List تیم مقابل
            team1Enemies.Add(kingCastleScript.transform);

            team1Enemies.Add(rightPrincessCastleScript.transform);

            team1Enemies.Add(leftPrincessCastleScript.transform);
        }

        // گرفتن Board موجود
        Board board;

        if (teamId == 1)
        {
            board = player1Board.GetComponent<Board>();
        }
        else
        {
            board = player2Board.GetComponent<Board>();
        }

        // ثبت Castle ها داخل Board
        board.SetKingCastleConfig(kingCastleScript);

        List<PrinceCastleConfig> princeCastles =
            new List<PrinceCastleConfig>();

        princeCastles.Add(rightPrincessCastleScript);

        princeCastles.Add(leftPrincessCastleScript);

        board.SetPrinceCastleConfig(princeCastles);
    }

    private void UpdateButtons()
    {
        buttonP1.interactable = player1.GetElexir().GetElexir() >= playerWizardData.elexir;
        buttonP2.interactable = player2.GetElexir().GetElexir() >= enemyWizardData.elexir;
    }

    private void SetPlayerTeamId()
    {
        Player player1 = buttonP1.GetComponent<Player>();
        Player player2 = buttonP2.GetComponent<Player>();
        player1.SetTeamId(1);
        player2.SetTeamId(2);
    }
}
