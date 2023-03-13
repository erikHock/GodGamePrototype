using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public interface IRequirement 
{
    protected event Action OnRequirementIsFullfilled;
    protected void RequirementIsFullfilled();
}
