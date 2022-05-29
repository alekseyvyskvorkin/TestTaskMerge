using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [SerializeField] private Tile[] _tiles;

    [SerializeField] private SpawnSettings[] _spawnSettings;

    public void Awake()
    {
        foreach (var spawnSettings in _spawnSettings)
        {
            for (int i = 0; i < spawnSettings.SpawnCount; i++)
            {
                CreateItem(spawnSettings.SpawnItem);
            }
        }
    }

    private void CreateItem(Item item)
    {
        int randomTile = Random.Range(0, _tiles.Length);

        while (_tiles[randomTile].CurrentItem != null)
        {
            randomTile = Random.Range(0, _tiles.Length);
        }

        var tileTransform = _tiles[randomTile].transform;
        var newItem = Instantiate(item, tileTransform.position + tileTransform.up / 2, tileTransform.rotation);
        _tiles[randomTile].CurrentItem = newItem;
        newItem.CurrentTile = _tiles[randomTile];
    }
}
