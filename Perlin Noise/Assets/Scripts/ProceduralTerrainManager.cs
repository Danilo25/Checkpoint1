using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralTerrainManager : MonoBehaviour
{
    public int profundidade = 20; // Profundidade do terreno
    public int largura = 256; // Largura do terreno
    public int altura = 256; // Altura do terreno

    public float scale = 5f; // Escala para o Perlin Noise
    public int octaves = 4; // Número de oitavas
    public float persistence = 0.5f; // Persistência para as oitavas
    public float lacunarity = 2f; // Lacunaridade para as oitavas
    public int seed = 42; // Semente para geração do Perlin Noise
    public Vector2 offset = Vector2.zero; // Deslocamento do mapa

    public float[,] heightMap;

    public Transform player; // Referência ao jogador
    public Material terrainMaterial; // Material do terreno

    private Vector2Int currentRegion; // Região atual do jogador
    private Dictionary<Vector2Int, Terrain> activeTerrains = new Dictionary<Vector2Int, Terrain>();
    private Dictionary<Vector2Int, List<GameObject>> activeElements = new Dictionary<Vector2Int, List<GameObject>>(); // Elementos como árvores ou cavernas

    public float elementChance = 1f; // Chance para a criação de elementos (0 a 1)
    public GameObject treePrefab; // Prefab da árvore
    public GameObject caveEntrancePrefab; // Prefab da entrada da caverna

    public Vector3 StartTerrain()
    {
        currentRegion = GetRegionFromPosition(player.position);
        UpdateTerrains();

        return new Vector3(
            currentRegion.x * (largura - 1) + (largura - 1) / 2f,
            10,
            currentRegion.y * (altura - 1) + (altura - 1) / 2f
        );
    }

    public void UpdateMap()
    {
        Vector2Int newRegion = GetRegionFromPosition(player.position);

        if (newRegion != currentRegion)
        {
            currentRegion = newRegion;

            UpdateTerrains();
        }
    }

    private Vector2Int GetRegionFromPosition(Vector3 position)
    {
        int x = Mathf.FloorToInt(position.x / largura);
        int z = Mathf.FloorToInt(position.z / altura);
        return new Vector2Int(x, z);
    }

    private void UpdateTerrains()
    {
        HashSet<Vector2Int> regionsToKeep = new HashSet<Vector2Int>();

        for (int x = -1; x <= 1; x++)
        {
            for (int z = -1; z <= 1; z++)
            {
                Vector2Int region = currentRegion + new Vector2Int(x, z);
                regionsToKeep.Add(region);

                if (!activeTerrains.ContainsKey(region))
                {
                    CreateTerrain(region);
                    CreateElements(region);
                }
            }
        }

        List<Vector2Int> regionsToRemove = new List<Vector2Int>();
        foreach (var region in activeTerrains.Keys)
        {
            if (!regionsToKeep.Contains(region))
            {
                regionsToRemove.Add(region);
            }
        }

        foreach (var region in regionsToRemove)
        {
            Destroy(activeTerrains[region].gameObject);
            activeTerrains.Remove(region);

            if (activeElements.ContainsKey(region))
            {
                foreach (var element in activeElements[region])
                {
                    Destroy(element);
                }
                activeElements.Remove(region);
            }
        }
    }

    private void CreateTerrain(Vector2Int region)
    {
        GameObject terrainObject = new GameObject($"Terrain_{region.x}_{region.y}");
        terrainObject.transform.position = new Vector3(
            region.x * (largura - 1),
            0,
            region.y * (altura - 1)
        );

        Terrain terrain = terrainObject.AddComponent<Terrain>();
        TerrainData terrainData = new TerrainData
        {
            heightmapResolution = largura + 1,
            size = new Vector3(largura, profundidade, altura)
        };

        Vector2 globalOffset = new Vector2(region.x * largura, region.y * altura);
        Vector2 regionOffset = offset + globalOffset;

        heightMap = PerlinNoise.GenerateHeightMap(
            largura,
            altura,
            seed,
            scale,
            octaves,
            persistence,
            lacunarity,
            regionOffset
        );

        terrainData.SetHeights(0, 0, heightMap);

        terrain.terrainData = terrainData;
        terrain.materialTemplate = terrainMaterial;

        TerrainCollider terrainCollider = terrainObject.AddComponent<TerrainCollider>();
        terrainCollider.terrainData = terrainData;

        activeTerrains[region] = terrain;
    }

    private void CreateElements(Vector2Int region)
    {
        // Criar árvores ou entradas de cavernas com uma chance
        List<GameObject> elementsInRegion = new List<GameObject>();

        // Exemplo de gerar árvores com chance
        if (UnityEngine.Random.value < elementChance)
        {
            Vector3 treePosition = new Vector3(
                region.x * largura + UnityEngine.Random.Range(-largura / 2, largura / 2),
                10,  // Fora do terreno (acima)
                region.y * altura + UnityEngine.Random.Range(-altura / 2, altura / 2)
            );
            GameObject tree = Instantiate(treePrefab, treePosition, Quaternion.identity);
            elementsInRegion.Add(tree);
        }

        // Exemplo de gerar entradas de cavernas com chance
        if (UnityEngine.Random.value < elementChance)
        {
            Vector3 cavePosition = new Vector3(
                region.x * largura + UnityEngine.Random.Range(-largura / 2, largura / 2),
                10,  // Fora do terreno (acima)
                region.y * altura + UnityEngine.Random.Range(-altura / 2, altura / 2)
            );
            GameObject caveEntrance = Instantiate(caveEntrancePrefab, cavePosition, Quaternion.identity);
            elementsInRegion.Add(caveEntrance);
        }

        // Armazenar os elementos criados para posterior remoção
        activeElements[region] = elementsInRegion;
    }
}