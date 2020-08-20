using System.Collections.Generic;
using UnityEngine;

public static class Extensions
{
    public static T GetRandom<T>(T[] array)
        => array[Random.Range(0, array.Length)];

    public static T GetRandom<T>(this List<T> list)
        => list[Random.Range(0, list.Count)];

    public static void Swap<T>(this T[] array, int index0, int index1)
    {
        T t = array[index0];
        array[index0] = array[index1];
        array[index1] = t;
    }

    public static void Swap<T>(this List<T> list, int index0, int index1)
    {
        T t = list[index0];
        list[index0] = list[index1];
        list[index1] = t;
    }
}