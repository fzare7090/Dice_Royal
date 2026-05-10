using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] private List<Unit> team1Units = new List<Unit>();
    [SerializeField] private List<Unit> team2Units = new List<Unit>();

    [Header("Prefabs")]
    [SerializeField] private GameObject unitPrefab;

    [Header("Unit Data")]
    [SerializeField] private UnitData playerWizardData;
    [SerializeField] private UnitData enemyWizardData;

    [Header("UI Buttons")]
    [SerializeField] private Button buttonP1;
    [SerializeField] private Button buttonP2;

    [Header("Spawn Parent")]
    [SerializeField] private Transform backGroundTransform;

    [SerializeField] private Image drageCardP1;
    [SerializeField] private Image drageCardP2;
    void Start()
    {
        buttonP1.onClick.AddListener(SpawnUnitForPlayer1);
        buttonP2.onClick.AddListener(SpawnUnitForPlayer2);
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
            team1Units.Add(unit);
            unit.SetEnemies(team2Units);
        }
        else
        {   
            spawnedUnit.transform.localPosition = drageCardP2.transform.localPosition;
            image.color = Color.red;
            unit.SetData(enemyWizardData);
            team2Units.Add(unit);
            unit.SetEnemies(team1Units);
        }
    }

}
