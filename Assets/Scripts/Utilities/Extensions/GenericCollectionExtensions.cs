using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Provides utility extension methods for collections.
/// </summary>
public static class GenericCollectionExtensions
{
    #region IList Extensions
    /// <summary>
    /// Retrieves a random element from this collection.
    /// </summary>
    /// <param name="collection">The collection to pull from.</param>
    /// <returns>Any element from the collection base on an evenly distributed random function.</returns>
    public static T RandomElement<T>(this IList<T> collection)
    {
        return collection[Random.Range(0, collection.Count)];
    }
    #endregion
    #region Array Extensions
    /// <summary>
    /// Returns a copy of the array with an index of the array removed.
    /// </summary>
    /// <param name="collection">The array to remove from.</param>
    /// <param name="indexToRemove">The index to remove from the array.</param>
    /// <returns>A new array without the specified index.</returns>
    public static T[] WithIndexRemoved<T>(this T[] collection, int indexToRemove)
    {
        T[] newCollection = new T[collection.Length - 1];
        int retrieverIndex = 0;
        for (int i = 0; i < newCollection.Length; i++)
        {
            // If the removed element has been hit,
            // have the retriever index step over it.
            if (i == indexToRemove)
                retrieverIndex++;
            newCollection[i] = collection[retrieverIndex];
            // Increment retriever index.
            retrieverIndex++;
        }
        return newCollection;
    }
    #endregion
}
