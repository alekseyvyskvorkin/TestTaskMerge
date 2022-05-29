using UnityEngine;

[System.Serializable]
public class SpawnSettings
{
    public Item SpawnItem => _spawnItem;
    public int SpawnCount => _spawnCount;

    [SerializeField] private Item _spawnItem;
    [SerializeField] private int _spawnCount;
}