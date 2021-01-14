using UnityEngine;
using System.Collections.Generic;

public static class GenericCollectionExtensions
{
    public static T RandomElement<T>(this IList<T> collection)
    {
        return collection[Random.Range(0, collection.Count)];
    }
}
