using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterComponent : EntityVisual
{
    // Start is called before the first frame update
    public new void Start()
    {
        config = (IEntityConfig)Table.monster.Get(TableId);
        base.Start();
    }

    // Update is called once per frame
    public new void Update()
    {
        base.Update();
    }
}
