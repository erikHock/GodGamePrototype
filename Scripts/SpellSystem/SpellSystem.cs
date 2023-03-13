using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SpellSystem : MonoBehaviour
{
    [SerializeField] List<BaseSpell> _spells = new List<BaseSpell>();

    private void Awake()
    {
        GetSpellbyID(1).Initialize();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            GetSpellbyID(1).ActivateSpell();
        }

        //GetSpellbyID(1).OnSpellDeactivate += ActivateNextSpell;
    }

    public void ActivateNextSpell(BaseSpell baseSpell)
    {
        GetSpellbyID(2).ActivateSpell();
    }
    public BaseSpell GetSpellbyID(int id)
    {
        if (_spells.Count.Equals(0))
        {
            return null;
        }

        var result = _spells.Where(x => x.GetSpellID() == id).FirstOrDefault();
        return result;
    }
}
