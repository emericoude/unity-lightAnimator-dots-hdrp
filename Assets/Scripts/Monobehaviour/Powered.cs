using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is the base class for everything that needs to be turned on/off by the power generator
/// </summary>
public abstract class Powered : MonoBehaviour
{
    [Header("Subsribe to")]
    [SerializeField] protected bool _subToPower = true;
    [SerializeField] protected bool _subToSecurity = true;
    [SerializeField] protected bool _subToWater = false;
    [SerializeField] protected bool _subToAir = false;
    
    public abstract void PowerOff();
    public abstract void PowerOn();
    
    /*
    protected virtual void Awake()
    {
        if (_subToPower)
            PowerManager.Instance.RegisterPoweredItem(this);

        if (_subToSecurity)
            SecurityManager.Instance.RegisterPoweredItem(this);

        if (_subToWater)
            WaterManager.Instance.RegisterPoweredItem(this);

        if (_subToAir)
            AirFilterManager.Instance.RegisterPoweredItem(this);
    }

    private void OnDisable()
    {
        if (_subToPower)
            PowerManager.Instance.UnregisterPoweredItem(this);

        if (_subToSecurity)
            SecurityManager.Instance.UnregisterPoweredItem(this);

        if (_subToWater)
            WaterManager.Instance.UnregisterPoweredItem(this);

        if (_subToAir)
            AirFilterManager.Instance.UnregisterPoweredItem(this);
    }
    */
    
}
