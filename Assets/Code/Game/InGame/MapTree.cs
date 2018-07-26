using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTree
{
    public Vector3 position;
    public MapTree parent = null;
    public List<MapTree> children = new List<MapTree>();

    public MapTree(Vector3 position)
    {
        this.position = position;
    }

    public void AddChile(MapTree child)
    {
        child.parent = this;
        children.Add(child);
    }
}