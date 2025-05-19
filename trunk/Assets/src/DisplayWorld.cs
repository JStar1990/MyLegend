using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-1)]
public class DisplayWorld : MonoBehaviour
{
    private List<EntityVisual> entityVisuals; // 实体列表

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddEntitVisual(EntityVisual entityVisual)
    {
        if (entityVisuals == null)
        {
            entityVisuals = new List<EntityVisual>();
        }
        entityVisuals.Add(entityVisual);
    }
}
