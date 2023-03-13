using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "ScritableObjects/SpellPanelButtonSO")]
public class SpellButtonSO : ScriptableObject
{
    public string buttonName;
    public Sprite icon;
}
