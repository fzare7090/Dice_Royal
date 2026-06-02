using UnityEngine;
using System.Collections.Generic;
using UnityEditor.Rendering;

public class AreaManager : MonoBehaviour
{

    [SerializeField] private List<Area> allAreas = new List<Area>();

    private Dictionary<int, List<Area>> teamAreasMap;

    private void Start()
    {
        SetAreasByTeam();
        Debug.Log($"Total Areas registered: {allAreas.Count}");

        foreach (var area in allAreas)
        {
            RectTransform rt = area.GetRectTransform();
            if (rt == null)
            {
                Debug.LogWarning($"AREA {area.name} has no RectTransform!");
                continue;
            }


            Vector3[] corners = new Vector3[4];
            rt.GetWorldCorners(corners);

        }
    }


    public void SetAreasByTeam()
    {

        teamAreasMap = new Dictionary<int, List<Area>>();

        foreach (Area area in allAreas)
        {
            int teamId = area.GetTeamId();
            if (!teamAreasMap.ContainsKey(teamId)) /// ??
            {
                teamAreasMap[teamId] = new List<Area>();
            }
            teamAreasMap[(teamId)].Add(area);
            Debug.Log($"area of team{teamId} set");

        }

    }




    public Area GetAreaAtPosition(RectTransform spawner)
    {
        foreach (Area area in allAreas)
        {
            if (area == null)
            {
                Debug.LogWarning("Encountered a null Area in allAreas list.");
                continue;
            }
            if (IsInsideArea(area, spawner))
            {
                Debug.Log($"Spawner is inside Area: {area.name}");
                return area;
            }
        }
        Debug.Log("spawner is not at area ");///به نظر میرسه مشکل اینجاست 
        return null;
    }




    public bool IsInsideArea(Area area, RectTransform spawner)
    {
        RectTransform rt = area.GetRectTransform();
        if (rt == null)
        {
            Debug.Log("rt is null");
            return false;
        }

        Vector3[] areaCorners = new Vector3[4];
        rt.GetWorldCorners(areaCorners);

        Vector3[] spawnerCorners = new Vector3[4];
        spawner.GetWorldCorners(spawnerCorners);

        Vector2 areaMin = areaCorners[0]; // Bottom-left
        Vector2 areaMax = areaCorners[2]; // Top-right

        Vector2 spawnerCenter = spawner.position;

        bool isInside = spawnerCenter.x >= areaMin.x && spawnerCenter.x <= areaMax.x &&
                        spawnerCenter.y >= areaMin.y && spawnerCenter.y <= areaMax.y;
        Debug.Log($"the isinside bool is {isInside}");/// همش فالس میده تو کنسول
        return isInside;
    }





    public bool CanPlayerSpawnAtPosition(RectTransform spawner, int playerTeamId)
    {
        bool foundAnyValidArea = false;
        bool spawnerInSomeArea = false;

        foreach (Area area in allAreas)
        {
            if (area != null && IsInsideArea(area, spawner))
            {
                spawnerInSomeArea = true;
                if (area.GetTeamId() == playerTeamId)
                {
                    foundAnyValidArea = true;
                    break;
                }
            }
        }

        if (!spawnerInSomeArea)
        {
            Debug.Log("area is null or spawner is not in any area");
            return false;
        }

        Debug.Log($"the spawnerTeamId is {playerTeamId} and valid area found: {foundAnyValidArea}");
        return foundAnyValidArea;
    }


    public List<Area> GetTeamArea(int teamId)
    {
        if (teamAreasMap.ContainsKey(teamId))
        {
            return new List<Area>(teamAreasMap[teamId]);
        }
        return new List<Area>();
    }

    public void ChangeAreaTeam(Area area, int newTeamId)
    {
        if (area != null)
        {
            int oldTeamId = area.GetTeamId();
            if (teamAreasMap != null)
            {
                if (teamAreasMap.ContainsKey(oldTeamId))
                {
                    teamAreasMap[oldTeamId].Remove(area);
                }
                if (!teamAreasMap.ContainsKey(newTeamId))
                {
                    teamAreasMap[newTeamId] = new List<Area>();
                }
                if (!teamAreasMap[newTeamId].Contains(area))
                {
                    teamAreasMap[newTeamId].Add(area);
                }
            }
            area.SetTeamId(newTeamId);
        }
    }

    public void ShowValidAreas(int teamId, bool show)
    {
        foreach (Area area in allAreas)
        {
            if (area != null && area.GetTeamId() == teamId)
            {
                area.ShowHighlight(show);
            }
        }
    }

}