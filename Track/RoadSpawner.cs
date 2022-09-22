using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoadSpawner : MonoBehaviour
{
    public List<GameObject> roads;
    private float xRoadOffset = 5f;
    public float roadsOffset = 55f;
    // Start is called before the first frame update
    void Start()
    {
        if (roads != null && roads.Count > 0)
        {
            roads = roads.OrderBy(r => r.transform.position.z).ToList();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void MoveRoads()
    {
        GameObject moveRoad = roads[0];
        roads.Remove(moveRoad);
        float roadNewZ = roads[roads.Count - 1].transform.position.z + roadsOffset;
        moveRoad.transform.position = new Vector3(xRoadOffset, 0, roadNewZ);
        roads.Add(moveRoad);
    }
}