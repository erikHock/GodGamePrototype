using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TakeAndDropItem : BaseSpell
{
    [SerializeField] private float _grabbingHeight;
    [SerializeField] private float _groundHeightMin = 0.5f;

    private GameObject _grabedItem;
    private ItemGrabber _grabber;
    private bool _isGrabbingActive;

    public override void SpellFunction()
    {
        if (_isGrabbingActive)
        {
            SetGrabbingActive(this);
        }
    }


    private void OnEnable()
    {
        this.OnSpellActivate += SetGrabbingActive;
    }

    private void OnDisable()
    {
        this.OnSpellActivate -= SetGrabbingActive;
    }

    private void SetGrabbingActive(BaseSpell spell)
    {

    }

    private void SetGrabbingInactive(BaseSpell spell)
    {

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
