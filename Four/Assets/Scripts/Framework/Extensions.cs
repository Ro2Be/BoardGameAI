using System.Collections.Generic;
using UnityEngine;

public static class Extensions
{
    public static T GetRandom<T>(this List<T> list)
        => list[Random.Range(0, list.Count)];
}