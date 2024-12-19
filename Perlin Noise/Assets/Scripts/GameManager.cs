using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public ProceduralTerrainManager TerrainManager;
    public Transform player;
    // Start is called before the first frame update
    void Start()
    {
        player.position = TerrainManager.StartTerrain();
    }

    // Update is called once per frame
    void Update()
    {
        TerrainManager.UpdateMap();
    }
}

