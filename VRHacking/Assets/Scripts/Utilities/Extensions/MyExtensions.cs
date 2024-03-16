using UnityEngine;
using System;
using System.Collections.Generic;

public static class MyExtensions {
    //Fisher-Yates Shuffle
    //https://jamesshinevar.medium.com/shuffle-a-list-c-fisher-yates-shuffle-32833bd8c62d
    private static System.Random rnd = new System.Random();

    public static void Shuffle<T> (this IList<T> list) {
        int n = list.Count;
        while(n > 1) {
            n--;

            int k = rnd.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}