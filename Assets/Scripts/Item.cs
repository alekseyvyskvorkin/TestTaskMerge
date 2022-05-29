using UnityEngine;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections.Generic;
using System.Threading.Tasks;

public class Item : Interactable
{
    public delegate void OnMergeComplete();
    public event OnMergeComplete OnCompleteMerge;

    private const int DoubleTapDurationTime = 500;
    private const float MaxSpawnDistance = 20f;
    private const float MoveSpeed = 100f;

    public Tile CurrentTile { get; set; }

    public GameObject Lining => _lining;
    public int Level => _level;
    public ItemType ItemType => _itemType;

    [SerializeField] private ItemType _itemType;
    [SerializeField] private int _level;

    [SerializeField] private GameObject _lining;

    [SerializeField] private ParticleSystem _mergeParticle;

    [SerializeField] private Item _createdItem;
    [SerializeField] private Coin _coinPrefab;

    private bool _isClicked;
    private bool _canMergeItems;

    private List<Item> _contactItems = new List<Item>();    

    private Vector3 _endMovePosition => CurrentTile.transform.position + CurrentTile.transform.up / 2;

    private void OnDestroy()
    {
        OnCompleteMerge -= CreateItem;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Item>(out var item))
        {
            _contactItems.Add(item);

            if (item.Level == Level && item.ItemType == ItemType
                && item.CurrentTile.CanMergeItems(Level, ItemType))
            {
                _canMergeItems = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<Item>(out var item))
        {
            _contactItems.Remove(item);
            if (_contactItems.Count == 0)
            {
                _canMergeItems = false;
                Tile.DeactivateLining();
            }
        }
    }

    public override async void OnClick()
    {
        _lining.SetActive(true);

        if (_isClicked)
        {
            if (CurrentTile.NearestEmptyTile() != null)
            {
                Tile tile = CurrentTile.NearestEmptyTile();
                Vector3 createPosition = tile.transform.position + tile.transform.up / 2;
                var newCoin = Instantiate(_coinPrefab, createPosition, CurrentTile.NearestEmptyTile().transform.rotation);
                tile.CurrentCoin = newCoin;
            }
            _isClicked = false;
        }
        else
        {
            _isClicked = true;
            await UniTask.Delay(DoubleTapDurationTime);
            _isClicked = false;
        }
    }

    public override void OnMove(Vector3 position)
    {
        transform.position = position;
    }

    public override void OnPointerUp()
    {        
        if (_canMergeItems && HasContactWithMergableItem())
        {
            Tile.MergeItems();
        }
        else
        {
            if (_contactItems.Count == 0 && Physics.Raycast(transform.position, -transform.up, out var hit) 
                && hit.collider.TryGetComponent<Tile>(out var tile) && tile.IsEmpty())
            {
                CurrentTile.CurrentItem = null;
                CurrentTile = tile;
                tile.CurrentItem = this;
            }

            transform.DOMove(_endMovePosition, MoveSpeed).SetSpeedBased();
            Tile.DeactivateLining();
        }
        
        _lining.SetActive(false);
        InputController.CurrentInteractable = null;
    }

    public async void Merge(Vector3 mergePosition)
    {
        await MergeAnimationPlaying(mergePosition);

        if (OnCompleteMerge == null)
        {
            Destroy(gameObject);
        }
        else
        {
            OnCompleteMerge.Invoke();
            OnCompleteMerge -= CreateItem;
        }
    }

    private async Task MergeAnimationPlaying(Vector3 mergePosition)
    {
        transform.DOMove(mergePosition, 0.5f);
        await UniTask.Delay(500);
        transform.DOPunchScale(Vector3.one, 0.25f);
        _mergeParticle.Play();
        await UniTask.Delay(260);
        transform.DOScale(Vector3.one / 10f, 0.25f);
        transform.DOMove(mergePosition, 0.25f);
        await UniTask.Delay(260);
    }

    public void CreateItem()
    {
        Physics.Raycast(transform.position, -transform.up, out var hit);
        hit.collider.TryGetComponent<Tile>(out var tile);
        Vector3 createPosition = tile.transform.position + tile.transform.up / 2;

        if (_createdItem != null)
        {
            var newItem = Instantiate(_createdItem, createPosition, tile.transform.rotation);
            tile.CurrentItem = newItem;
            newItem.CurrentTile = tile;
        }
        else
        {
            CreateCoins(tile, createPosition);
            Tile.DeactivateLining();
        }
        Destroy(gameObject);
    }

    private void CreateCoins(Tile tile, Vector3 createPosition)
    {
        for (int i = 0; i < Tile.MergableTiles.Count; i++)
        {
            var newCoin = Instantiate(_coinPrefab, createPosition, tile.transform.rotation);            
            Vector3 movePosition = Tile.MergableTiles[i].transform.position + Tile.MergableTiles[i].transform.up / 2;

            if ((newCoin.transform.position - movePosition).sqrMagnitude > MaxSpawnDistance && tile.NearestEmptyTile())
            {
                movePosition = tile.NearestEmptyTile().transform.position + tile.transform.up / 2;
                tile.NearestEmptyTile().CurrentCoin = newCoin;
            }
            else
            {
                Tile.MergableTiles[i].CurrentCoin = newCoin;
            }
            
            newCoin.transform.DOMove(movePosition, 0.5f);
        }
    }

    private bool HasContactWithMergableItem()
    {
        foreach (var item in _contactItems)
        {
            if (item.Level == Level && item.ItemType == ItemType)
            {
                return true;
            }              
        }
        return false;
    }
}
