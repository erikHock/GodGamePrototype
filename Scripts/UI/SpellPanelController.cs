using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SpellPanelController : MonoBehaviour
{
    [SerializeField] private SpellButtonSO _spellButtonSO;
    [SerializeField] private GameObject _spellButtonPrefab;

    public static event Action OnSpellAdded;
    public static event Action OnSpellDeleted;

    public void AddSpell()
    {
        // Add spell
        OnSpellAdded?.Invoke();
    }

    public void DeleteSpell()
    {
        // Delete spell
        OnSpellDeleted?.Invoke();
    }

    private void SpawnButton()
    {
        // Instantiate button
        // Set Parent
        // Set name
        // Set Sprite
    }
}
