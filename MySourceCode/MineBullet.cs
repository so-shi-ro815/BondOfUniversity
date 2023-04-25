using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineBullet : BazBullet
{
    public override void VelocityDown()
    {
        rBody.AddForce((rBody.velocity/rBody.velocity.magnitude) * (0 - rBody.velocity.magnitude), ForceMode.Acceleration);
    }
    private void Awake()
    {
        base.EnemyListObj = GameObject.Find("MisilePod").gameObject;
    }
}
