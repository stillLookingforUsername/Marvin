using NUnit.Framework;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;
using UnityEngine.Rendering;
using UnityEditor.SearchService;

public class Spawn : MonoBehaviour
{
    public enum ObjectList { smallGem, bigGem, Enemy }
    public Tilemap tilemap;
    public GameObject[] ObjectPrefabs;   //smallGem,bigGem,Enemy

    public float bigGemProbability = 0.2f;
    public float enemyProbability = 0.1f;
    public int maxObject = 3;
    public float gemLifeSpan = 10f;
    public float spawnInterval = 0.5f;

    private List<Vector3> _spawnPosition = new List<Vector3>();
    private List<GameObject> _spawnObjects = new List<GameObject>();
    private bool _isSpawning = false;

    private void Start()
    {
        GatherValidPosition();
        StartCoroutine(SpawnObjectIfNeeded());
    }

    private void Update()
    {
        if (!tilemap.gameObject.activeInHierarchy)
        {
            //level change

        }
        if (!_isSpawning && ActiveObjectCount() < maxObject)
        {
            StartCoroutine(SpawnObjectIfNeeded());
        }
    }
    private void LevelChange()
    {
        tilemap = GameObject.Find("Ground").GetComponent<Tilemap>();
        GatherValidPosition();

        //DestroyAllSpawnGameObjects;
        DestoryAllSpawnGameObjects();
    }
    private int ActiveObjectCount()
    {
        _spawnObjects.RemoveAll(item => item == null);
        return _spawnObjects.Count;
    }

    private IEnumerator SpawnObjectIfNeeded()
    {
        _isSpawning = true;
        while (ActiveObjectCount() < maxObject)
        {
            SpawnObject();
            yield return new WaitForSeconds(spawnInterval);
        }
        _isSpawning = false;
    }

    private bool PositionHasObject(Vector3 positionToCheck)
    {
        return _spawnObjects.Any(checkObj => checkObj && Vector3.Distance(checkObj.transform.position, positionToCheck) < 1.0f);
    }

    private ObjectList RandomObjectType()
    {
        float randomChoice = Random.value;

        if (randomChoice <= enemyProbability)
        {
            return ObjectList.Enemy;
        }
        else if (randomChoice <= (enemyProbability + bigGemProbability))
        {
            return ObjectList.bigGem;
        }
        else 
        {
            return ObjectList.smallGem;
        }
    } 

    private void SpawnObject()
    {
        if (_spawnPosition.Count == 0) return;

        Vector3 spawnPos = Vector3.zero;
        bool validPosFound = false;

        while (!validPosFound && _spawnPosition.Count > 0)
        {
            int randomIndex = Random.Range(0, _spawnPosition.Count);
            Vector3 potentialPosition = _spawnPosition[randomIndex];
            Vector3 leftPosition = potentialPosition + Vector3.left;
            Vector3 rightPosition = potentialPosition + Vector3.right;

            if (!PositionHasObject(leftPosition) && !PositionHasObject(rightPosition))
            {
                spawnPos = potentialPosition;
                validPosFound = true;   
            }
            _spawnPosition.RemoveAt(randomIndex);
        }
        if (validPosFound)
        {
            ObjectList objectType = RandomObjectType();
            GameObject gameObject = Instantiate(ObjectPrefabs[(int)objectType], spawnPos, Quaternion.identity);
            _spawnObjects.Add(gameObject);

            //Destroy gems only after time
            if (objectType != ObjectList.Enemy)
            {
                StartCoroutine(DestroyObjectAfterTime(gameObject, gemLifeSpan));
            }
        }
    }

    private IEnumerator DestroyObjectAfterTime(GameObject gameObject, float time)
    {
        yield return new WaitForSeconds(time);

        if (gameObject)
        {
            _spawnObjects.Remove(gameObject);
            _spawnPosition.Add(gameObject.transform.position);
            Destroy(gameObject);
        }
    }

    private void DestoryAllSpawnGameObjects()
    {
        foreach (GameObject obj in _spawnObjects)
        {
            if (obj != null)
            {
                Destroy(obj);
            }
        }
        _spawnObjects.Clear();
    }
    private void GatherValidPosition()
    {
        _spawnPosition.Clear();
        BoundsInt boundInt = tilemap.cellBounds;
        TileBase[] allTiles = tilemap.GetTilesBlock(boundInt);
        Vector3 start = tilemap.CellToWorld(new Vector3Int(boundInt.xMin, boundInt.yMin,0));

        for (int x = 0; x < boundInt.size.x; x++)
        {
            for (int y = 0; y < boundInt.size.y; y++)
            {
                TileBase tile = allTiles[x + y * boundInt.size.x];
                if (tile != null)
                {
                    Vector3 place = start + new Vector3(x + 0.5f, y + 2f, 0);
                    _spawnPosition.Add(place); 
                }
            }
        }
    }
}
