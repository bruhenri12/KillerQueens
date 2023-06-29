using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public static class GeneticManager
{
  public static int[] GenerateRandomGeneticTape()
  {
    System.Random rnd = new System.Random();
    int[] tape = Enumerable.Range(0, 8).OrderBy(c => rnd.Next()).ToArray();
    return ConvertToBinaryTape(tape);
  }

  public static int[] ConvertToBinaryTape(int[] tape)
  {
    int[] genTape = new int[24];

    int temp = 0;
    for (int i = 0; i < 8; i++)
    {
      string bitString = Convert.ToString(tape[i], 2);

      int[] bitQueen = bitString.PadLeft(3, '0')
                       .Select(c => int.Parse(c.ToString()))
                       .ToArray();

      for (int j = 0; j < 3; j++) 
      { 
        genTape[temp + j] = bitQueen[j]; 
      }
      temp += 3;
    }

    return genTape;
  }

  public static int[] ConvertToIntTape(int[] genTape)
  {
    int[] decTape = new int[8];

    int temp = 0;
    for (int i = 0; i < 8; i++)
    {
      string conv = "";
      for (int j = 0; j < 3; j++) 
      {
         conv += genTape[temp + j];
      }
      decTape[i] = Convert.ToInt16(conv, 2);
      temp += 3;
    }
    return decTape;
  }

  public static BoardSetting[] GenerateOffspring(
    BoardSetting parent1, BoardSetting parent2,
    int offspringSize, int geneSlices=1,
    bool cloneParents = false
  )
  {
    BoardSetting[] offspring = new BoardSetting[offspringSize];

    if (cloneParents || geneSlices == 0)
    {
      for (int i = 0; i < offspringSize; i++)
      {
        if (i % 2 == 0) { offspring[i] = parent1; }
        else { offspring[i] = parent2; }
      }
    }
    else
    {
      for (int i = 0; i < offspringSize; i+=2)
      {
        var curOffspring = Crossover(parent1, parent2, geneSlices);
        offspring[i] = curOffspring.firstChild;

        if(i+1 < offspringSize) { 
          offspring[i+i] = curOffspring.secondChild;
        }
      }
    }

    return offspring;
  }

  public static (BoardSetting firstChild, BoardSetting secondChild) Crossover(BoardSetting parent1, BoardSetting parent2, int geneSlices=1)
  {
    int[] sliceIndexes = new int[geneSlices];

    // Set indexes for slicing the gene tape
    for (int i = 0; i < geneSlices; i++)
    {
      int sliceIndex = new System.Random().Next(0, 8) * 3; //Get the start of the encoded position
      if (!sliceIndexes.Contains(sliceIndex)) { sliceIndexes[i] = sliceIndex; }
    }

    Array.Sort(sliceIndexes);
    int[] firstChildTape = parent1.GetGeneticBinaryTape()[0..sliceIndexes[0]];
    int[] secondChildTape = parent2.GetGeneticBinaryTape()[0..sliceIndexes[0]];

    // If only 1 slice, add last genes
    if (geneSlices == 1) { 
      firstChildTape = firstChildTape.Concat(parent2.GetGeneticBinaryTape()[sliceIndexes[0]..(24)]).ToArray(); 
      secondChildTape = secondChildTape.Concat(parent1.GetGeneticBinaryTape()[sliceIndexes[0]..(24)]).ToArray();
    }

    int currentIndex = sliceIndexes[0];
    for (int i = 1; i < geneSlices; i++)
    {
      if (i % 2 == 0)
      { 
        firstChildTape = firstChildTape.Concat(parent1.GetGeneticBinaryTape()[currentIndex..sliceIndexes[i]]).ToArray();
        secondChildTape = secondChildTape.Concat(parent2.GetGeneticBinaryTape()[currentIndex..sliceIndexes[i]]).ToArray();
      }
      else
      {
        firstChildTape = firstChildTape.Concat(parent2.GetGeneticBinaryTape()[currentIndex..sliceIndexes[i]]).ToArray();
        secondChildTape = secondChildTape.Concat(parent1.GetGeneticBinaryTape()[currentIndex..sliceIndexes[i]]).ToArray();
      }

      // Adding last genes
      if (i == geneSlices - 1)
      {
        if (i % 2 == 0) 
        {
          firstChildTape = firstChildTape.Concat(parent1.GetGeneticBinaryTape()[sliceIndexes[i]..(24)]).ToArray();
          secondChildTape = secondChildTape.Concat(parent2.GetGeneticBinaryTape()[sliceIndexes[i]..(24)]).ToArray();
        }
        else
        { 
          firstChildTape = firstChildTape.Concat(parent2.GetGeneticBinaryTape()[sliceIndexes[i]..(24)]).ToArray();
          secondChildTape = secondChildTape.Concat(parent1.GetGeneticBinaryTape()[sliceIndexes[i]..(24)]).ToArray();
        }
      }
      currentIndex = sliceIndexes[i];
    }

    BoardSetting firstChild = new BoardSetting(firstChildTape);
    BoardSetting secondChild = new BoardSetting(secondChildTape);

    Debug.Log($" p1={parent1} | p2= {parent2} | point = {sliceIndexes[0]/3}\n fst child = {firstChild} | snd child ={secondChild}");

    return (firstChild: firstChild, secondChild: secondChild);
  }

  public static int[] MutateChild(int[] childGenes, float mutationProb)
  {
    // Amount of genes on the array of binaries converted do integers 
    int genesCount = childGenes.Length / 3;

    for (int i = 0; i < genesCount; i++)
    {
      //Checks if should mutate genes
      int mutationRandomNumber = new System.Random().Next(0, 101);

      if (mutationRandomNumber <= mutationProb * 100)
      {
        //Position of the gene in the binary vector
        int genePos = i * 3;

        //Array of positions that are not the index
        int[] validPositions = Enumerable.Range(0, 8).Where(pos => pos != i).ToArray();
        int swapRandomNumber = new System.Random().Next(0, 7);
        int swapPos = validPositions[swapRandomNumber] * 3;
        Debug.Log($"SWAP ({mutationRandomNumber}) {i}({genePos}) <-> {swapPos / 3}({swapPos})");

        // Swap the positions of the mutant  genes
        int swapAux;
        for (int j = 0; j < 3; j++)
        {
          swapAux = childGenes[genePos + j];
          childGenes[genePos + j] = childGenes[swapPos + j];
          childGenes[swapPos + j] = swapAux;
        }
      }
    }
    return childGenes;
  }

  public static void DebugMutateChild(int[] genTape)
  {
    //Method used only for test purpose of the MutateChild method. It can be safetly deleted later
    string originalGeneTxt = "| ";
    for (int i = 0; i < genTape.Length; i++)
    {
      int gene = genTape[i];
      originalGeneTxt += gene;
      originalGeneTxt += (i % 3 == 2) ? " | " : "";
    }

    int[] mutantChild = MutateChild(genTape, 0.4f);

    string mutantGeneTxt = "| ";
    for (int i = 0; i < genTape.Length; i++)
    {
      int gene = mutantChild[i];
      mutantGeneTxt += gene;
      mutantGeneTxt += (i % 3 == 2) ? " | " : "";
    }

    Debug.Log($"Original: {originalGeneTxt} \n Mutant: {mutantGeneTxt}");
  }

}
