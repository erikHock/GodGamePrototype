using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnItem : BaseSpell
{

    [SerializeField] private GameObject _grabbedItemPrefab;
    [SerializeField] private int _itemMaxCount = 2;

    private int _count = 0;

    private GameObject _itemGameObject;
    private ItemGrabber _itemGrabber;
    public override void SpellFunction()
    {
        if (!IsItemSpawnable() && _itemGameObject == null)
        {
            DeactivateSpell();
        }
        else 
        {
            if (_itemGrabber.currentState != ItemGrabState.Grounded)
            {
                return;
            }
            else 
            {
                _itemGrabber = null;
                _itemGameObject = null;

                // Create item on activating spell and if is count < maxCount
                CreateItem(this);
            }
        }


    }
    bool IsItemSpawnable()
    {
        bool isItemSpawnable = _count < _itemMaxCount ? true : false;
        return isItemSpawnable;
    }
    private void CreateItem(BaseSpell baseSpell)
    {
        // Create item on mouse position
        if (IsItemSpawnable() && _grabbedItemPrefab != null)
        {
            _itemGameObject = Instantiate(_grabbedItemPrefab, Vector3.zero, Quaternion.identity);

            _itemGrabber = _itemGameObject.GetComponent<ItemGrabber>();
            _itemGrabber.currentState = ItemGrabState.Grabbed;

            _count++;
        }
    }

    private void OnEnable()
    {
        this.OnSpellActivate += CreateItem;
    }

    private void OnDisable()
    {
        this.OnSpellActivate -= CreateItem;
    }

    public override void ActivateSpell()
    {
        base.ActivateSpell();
    }

    public override void DeactivateSpell()
    {
        base.DeactivateSpell();
    }

    public override void Initialize()
    {
        base.Initialize();
    }

}
