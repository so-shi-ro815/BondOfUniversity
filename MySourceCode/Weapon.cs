using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * 各武器の親クラス
 */
public abstract class Weapon : MonoBehaviour
{
    protected int ammo;
    protected float damage;
    protected float effectiveTime;
    protected float speed;
    protected int Element;
}
