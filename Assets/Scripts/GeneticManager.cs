using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GeneticManager : MonoBehaviour
{
    private int[] tape = new int[8];

    public void UpdateGeneticTape(){
        System.Random rnd = new System.Random();
        tape = Enumerable.Range(0, 8).OrderBy(c => rnd.Next()).ToArray();
    }

    public int[] GetGeneticTape()
    {
        return tape;
    }

}
