using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class GeneticManager : MonoBehaviour
{
    private int[] genTape = new int[24];

    public void RandomizeGeneticTape(){
        System.Random rnd = new System.Random();
        int[] tape = Enumerable.Range(0, 8).OrderBy(c => rnd.Next()).ToArray();
        GenerateGenTape(tape);
    }

    public int[] GetGeneticTape()
    {
        return TapeConverter();
    }

    public void GenerateGenTape(int[] tape)
    {
        int temp = 0;
        for (int i = 0; i < 8; i++)
        {
            string bitString = Convert.ToString(tape[i],2);

            int[] bitQueen = bitString.PadLeft(3,'0')
                             .Select(c => int.Parse(c.ToString()))
                             .ToArray();

            for (int j = 0; j < 3; j++) {genTape[temp+j] = bitQueen[j];}
            temp += 3;
        }
    }

    public int[] TapeConverter()
    {
        int[] decTape = new int[8];

        int temp = 0;
        for (int i = 0; i < 8; i++)
        {
            string conv = "";
            for (int j = 0; j < 3; j++) {conv += genTape[temp+j];}
            decTape[i] = Convert.ToInt16(conv,2);
            temp += 3;
        }
        return decTape;
    }

    public int[][] Crossover(
        int[] parent1, int[] parent2, 
        int offspringSize, int geneSlices,
        bool cloneParents=false 
    ) {
        int[][] offspring = new int[offspringSize][];

        if (cloneParents || geneSlices == 0) 
        {
            for (int i = 0; i < offspringSize; i++)
            {
                if (i % 2 == 0) {offspring[i] = parent1;} 
                else {offspring[i] = parent2;}
            }
        }
        else
        {
            for (int i = 0; i < offspringSize; i++)
            {
                offspring[i] = GenerateChild(parent1, parent2, geneSlices);
            }
        }

        return offspring;
    }

    public int[] GenerateChild(int[] parent1, int[] parent2, int geneSlices)
    {
        int[] sliceIndexes = new int[geneSlices];

        // Set indexes for slicing the gene tape
        for (int i = 0; i < geneSlices; i++)
        {
            int sliceIndex = new System.Random().Next(0, 24);
            if (!sliceIndexes.Contains(sliceIndex)) {sliceIndexes[i] = sliceIndex;}
        }

        Array.Sort(sliceIndexes);
        int[] newGeneTape = parent1[0..sliceIndexes[0]];

        // If only 1 slice, add last genes
        if (geneSlices == 1) {newGeneTape = newGeneTape.Concat(parent2[sliceIndexes[0]..(24)]).ToArray();}

        int currentIndex = sliceIndexes[0];
        for (int i = 1; i < geneSlices; i++)
        {
            if (i%2 == 0) {newGeneTape = newGeneTape.Concat(parent1[currentIndex..sliceIndexes[i]]).ToArray();}
            else {newGeneTape = newGeneTape.Concat(parent2[currentIndex..sliceIndexes[i]]).ToArray();}

            // Adding last genes
            if (i == geneSlices-1)
            {
                if (i%2 == 0) {newGeneTape = newGeneTape.Concat(parent1[sliceIndexes[i]..(24)]).ToArray();}
                else {newGeneTape = newGeneTape.Concat(parent2[sliceIndexes[i]..(24)]).ToArray();}
            }
            currentIndex = sliceIndexes[i];
        }

        return newGeneTape;
    }
}
