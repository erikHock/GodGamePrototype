using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BaseSpell : MonoBehaviour
{
    [SerializeField] protected int _spellID;
    [SerializeField] protected string _spellName;

    protected bool _isActive = false;
    public Action<BaseSpell> OnSpellActivate;
    public Action<BaseSpell> OnSpellDeactivate;
        
    public virtual void SpellFunction()
    {
        return;
    }

    public virtual void Update()
    {
        if (!this.IsActive())
        {
            return;
        }
        else
        {
            SpellFunction();
        }
    }

    public bool IsActive()
    {
        return this._isActive;
    }

    public virtual void ActivateSpell()
    {
        if (!this._isActive)
        {
            this._isActive = true;
            OnSpellActivate?.Invoke(this);
        }
    }

    // Muset be called on the end of spell
    public virtual void DeactivateSpell()
    {
        this._isActive = false;
        OnSpellDeactivate?.Invoke(this);    
    }

    // Muset be called before ActivateSpell
    public virtual void Initialize() 
    { 
        if (this._spellID == 0 && String.IsNullOrEmpty(_spellName))
        {
            return;
        }
    }

    public int GetSpellID()
    {
        return _spellID;
    }
}
