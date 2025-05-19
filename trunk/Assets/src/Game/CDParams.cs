using System;
using UnityEngine;

public class CDParams
{
    public float time;
    public float cd;
    public int type;

    public void set(float cd, int type = 0)
    {
        time = Time.time;
        this.cd = cd;
        this.type = type;
    }

    public bool isCD
    {
        get { return Time.time - time < cd; }
    }

    public float remain
    {
        get { return Mathf.Clamp(cd - (Time.time - time), 0, cd); }
    }
    
    public float percent
    {
        get { return Mathf.Clamp((Time.time - time) / cd, 0, 1); }
    }
}
