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
    [SerializeField] private GameObject towerPrefab;


    [Header("Unit Data")]
    [SerializeField] private UnitData playerWizardData;
    [SerializeField] private UnitData enemyWizardData;
    [Header("Tower Data")]
    [SerializeField] private TowerData kingTowerP1;
    [SerializeField] private TowerData princessTowerP1;
    [SerializeField] private TowerData kingTowerP2;
    [SerializeField] private TowerData princessTowerP2;


    [Header("UI Buttons")]
    [SerializeField] private Button buttonP1;
    [SerializeField] private Button buttonP2;

    [Header("Spawn Parent")]
    [SerializeField] private Transform backGroundTransform;

    [Header("Drag Card")]
    [SerializeField] private Image drageCardP1;
    [SerializeField] private Image drageCardP2;

    [Header("Tower Manager")]
    [SerializeField] private GameObject towerManager;

    void Start()
    {
        buttonP1.onClick.AddListener(SpawnUnitForPlayer1);
        buttonP2.onClick.AddListener(SpawnUnitForPlayer2);
        SpawnTower(1);
        SpawnTower(2);
    }
    private void SpawnUnitForPlayer1()
    {
        SpawnUnit(1);
    }
    private void SpawnUnitForPlayer2()
    {
        SpawnUnit(2);
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
        // if (teamId == 1)
        // {
        //     unit.GetElexir();
        // }
        // else
        // {
        //     unit.GetElexir();
        // }
    }
    private void SpawnTower(int teamId)
    {

        GameObject kingTower = Instantiate(towerPrefab, backGroundTransform);
        GameObject rightPrincessTower = Instantiate(towerPrefab, backGroundTransform);
        GameObject leftPrincessTower = Instantiate(towerPrefab, backGroundTransform);
        RectTransform kingRT = kingTower.GetComponent<RectTransform>();
        RectTransform rightRT = rightPrincessTower.GetComponent<RectTransform>();
        RectTransform leftRT = leftPrincessTower.GetComponent<RectTransform>();
        Image kingImg = kingTower.GetComponent<Image>();
        Image rightImg = rightPrincessTower.GetComponent<Image>();
        Image leftImg = leftPrincessTower.GetComponent<Image>();
        Tower kingTowerScript = kingTower.GetComponent<Tower>();
        Tower rightPrincessTowerScript = rightPrincessTower.GetComponent<Tower>();
        Tower leftPrincessTowerScript = leftPrincessTower.GetComponent<Tower>();
        TowersPosition towerPosition = towerManager.GetComponent<TowersPosition>();


        Color teamColor;
        if (teamId == 1)
        {
            ColorUtility.TryParseHtmlString("#006906", out teamColor);
        }
        else
        {
            ColorUtility.TryParseHtmlString("#FF0097", out teamColor);
        }


        kingImg.color = teamColor;
        rightImg.color = teamColor;
        leftImg.color = teamColor;
        kingRT.sizeDelta = new Vector2(300, 300);
        rightRT.sizeDelta = new Vector2(200, 200);
        leftRT.sizeDelta = new Vector2(200, 200);


        if (teamId == 1)
        {
            kingTower.transform.localPosition = towerPosition.GetKingTowerP1();
            rightPrincessTower.transform.localPosition = towerPosition.GetRightPrincessTowerP1();
            leftPrincessTower.transform.localPosition = towerPosition.GetLeftPrincessTowerP1();
            kingTowerScript.SetEnemies(team1Enemies);
            rightPrincessTowerScript.SetEnemies(team1Enemies);
            leftPrincessTowerScript.SetEnemies(team1Enemies);
            kingTowerScript.SetData(kingTowerP1);
            rightPrincessTowerScript.SetData(princessTowerP1);
            leftPrincessTowerScript.SetData(princessTowerP1);
            team2Enemies.Add(kingTowerScript.transform);
            team2Enemies.Add(rightPrincessTowerScript.transform);
            team2Enemies.Add(leftPrincessTowerScript.transform);

        }
        else
        {
            kingTower.transform.localPosition = towerPosition.GetKingTowerP2();
            rightPrincessTower.transform.localPosition = towerPosition.GetRightPrincessTowerP2();
            leftPrincessTower.transform.localPosition = towerPosition.GetLeftPrincessTowerP2();
            kingTowerScript.SetEnemies(team2Enemies);
            rightPrincessTowerScript.SetEnemies(team2Enemies);
            leftPrincessTowerScript.SetEnemies(team2Enemies);
            kingTowerScript.SetData(kingTowerP2);
            rightPrincessTowerScript.SetData(princessTowerP2);
            leftPrincessTowerScript.SetData(princessTowerP2);
            team1Enemies.Add(kingTowerScript.transform);
            team1Enemies.Add(rightPrincessTowerScript.transform);
            team1Enemies.Add(leftPrincessTowerScript.transform);
        }
        


        kingTowerScript.SetTeamId(teamId);
        rightPrincessTowerScript.SetTeamId(teamId);
        leftPrincessTowerScript.SetTeamId(teamId);
    }

}



