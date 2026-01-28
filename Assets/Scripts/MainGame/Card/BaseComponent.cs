using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseComponent
{
    public abstract void DealDamage(int damage);

    public abstract void HealDamage(int heal);
}
