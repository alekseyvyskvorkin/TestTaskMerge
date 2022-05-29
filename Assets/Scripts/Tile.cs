using UnityEngine;
using System.Collections.Generic;

public class Tile : MonoBehaviour
{
    public static List<Tile> MergableTiles => _mergableTiles;

    public Item CurrentItem { get; set; }
    public Coin CurrentCoin { get; set; }

    [SerializeField] private List<Tile> _tiles = new List<Tile>();

    private static List<Tile> _mergableTiles = new List<Tile>();

    public static void MergeItems()
    {
        if (_mergableTiles.Count > 2)
        {
            var currentItem = InputController.CurrentInteractable.GetComponent<Item>();
            currentItem.OnCompleteMerge += currentItem.CreateItem;

            foreach (var tile in _mergableTiles)
            {
                tile.CurrentItem.Merge(currentItem.transform.position);
            }
        }
    }

    [ContextMenu("AddNearestTile")]
    public void AddNearestTile()
    {
        _tiles.Clear();
        TryAddNearestTile(transform.forward, transform.localScale.z);
        TryAddNearestTile(-transform.forward, transform.localScale.z);
        TryAddNearestTile(transform.right, transform.localScale.x);
        TryAddNearestTile(-transform.right, transform.localScale.x);
    }

    public bool CanMergeItems(int level, ItemType type)
    {
        List<Tile> tiles = new List<Tile>();

        if (_mergableTiles.Contains(this) == false)
        {
            _mergableTiles.Add(this);
            if(_mergableTiles.Count > 2)
            {
                ActivateLining(true);
            }            
        }        

        foreach (var tile in _tiles)
        {
            if (_mergableTiles.Contains(tile) == false && tile.CurrentItem != null 
                && tile.CurrentItem.ItemType == type && tile.CurrentItem.Level == level)
            {
                tiles.Add(tile);
            }
        }
        if (tiles.Count > 0)
        {
            foreach (var tile in tiles)
            {
                tile.CanMergeItems(level, type);
            }
            return true;
        }

        return false;
    }

    public Tile NearestEmptyTile()
    {
        foreach (var tile in _tiles)
        {
            if (tile.IsEmpty())
            {
                return tile;
            }
        }
        return null;
    }

    public bool IsEmpty()
    {
        if (CurrentItem == null && CurrentCoin == null)
            return true;
        return false;
    }

    public static void DeactivateLining() 
    {
        ActivateLining(false);
        _mergableTiles.Clear();
    }

    private static void ActivateLining(bool isActive)
    {
        foreach (var tile in _mergableTiles)
        {
            if (tile.CurrentItem && tile.CurrentItem.Lining && tile.CurrentItem != InputController.CurrentInteractable)
            {
                tile.CurrentItem.Lining.SetActive(isActive);
            }                
        }
    }

    private void TryAddNearestTile(Vector3 direction, float maxDistance)
    {
        RaycastHit[] hits = Physics.RaycastAll(transform.position, direction, maxDistance);
        foreach (var hit in hits)
        {
            if (hit.collider.TryGetComponent<Tile>(out var tile) && tile != this)
            {
                _tiles.Add(tile);
            }
        }
    }
}