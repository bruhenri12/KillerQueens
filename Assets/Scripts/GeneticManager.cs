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
    int intTapeLength = genTape.Length/3;
    int[] decTape = new int[intTapeLength];

    int temp = 0;
    for (int i = 0; i < intTapeLength; i++)
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
        offspring[i] = new BoardSetting(curOffspring.firstChild);

        if(i+1 < offspringSize) { 
          offspring[i+i] = new BoardSetting(curOffspring.secondChild);
        }
      }
    }

    return offspring;
  }

  public static (int[] firstChild, int[] secondChild) Crossover(BoardSetting parent1, BoardSetting parent2, int geneSlices=1)
    {
        int[] sliceIndexes = new int[geneSlices];

        // Set indexes for slicing the gene tape
        for (int i = 0; i < geneSlices; i++)
        {
            //Get the start of the encoded position
            int sliceIndex = 0;
            if (geneSlices == 1) 
            { 
                sliceIndex = new System.Random().Next(1, 7) * 3; //Excludes 0 and 7 to avoid parent copies 
            }
            else
            {
                sliceIndex = new System.Random().Next(0, 8) * 3;
            }

            if (!sliceIndexes.Contains(sliceIndex)) { sliceIndexes[i] = sliceIndex; }
        }

        Array.Sort(sliceIndexes);
        int[] firstChildTape = parent1.GetGeneticBinaryTape()[0..sliceIndexes[0]];
        int[] secondChildTape = parent2.GetGeneticBinaryTape()[0..sliceIndexes[0]];

        // If only 1 slice, add last genes
        if (geneSlices == 1)
        {
            firstChildTape = BuildChildrenTapes(parent2, parent1, sliceIndexes[0], parent2.GetGeneticBinaryTape().Length, firstChildTape); 
            secondChildTape = BuildChildrenTapes(parent1, parent2,  sliceIndexes[0], parent1.GetGeneticBinaryTape().Length, secondChildTape);       
        }

        int currentIndex = sliceIndexes[0];
        for (int i = 1; i < geneSlices; i++)
        {
            if (i % 2 == 0)
            {
                firstChildTape = BuildChildrenTapes(parent1, parent2, currentIndex, sliceIndexes[i], firstChildTape);
                secondChildTape = BuildChildrenTapes(parent2, parent1, currentIndex, sliceIndexes[i], secondChildTape);
            }
            else
            {
                firstChildTape = BuildChildrenTapes(parent2, parent1, currentIndex, sliceIndexes[i], firstChildTape);
                secondChildTape = BuildChildrenTapes(parent1, parent2, currentIndex, sliceIndexes[i], secondChildTape);
            }

            // Adding last genes
            if (i == geneSlices - 1)
            {
                if (i % 2 == 0)
                {
                    firstChildTape = BuildChildrenTapes(parent1, parent2, currentIndex, parent1.GetGeneticBinaryTape().Length, firstChildTape);
                    secondChildTape = BuildChildrenTapes(parent2, parent1, currentIndex, parent2.GetGeneticBinaryTape().Length, secondChildTape);
                }
                else
                {
                    firstChildTape = BuildChildrenTapes(parent2, parent1, currentIndex, parent2.GetGeneticBinaryTape().Length, firstChildTape);
                    secondChildTape = BuildChildrenTapes(parent1, parent2, currentIndex, parent1.GetGeneticBinaryTape().Length, secondChildTape);
                }
            }
            currentIndex = sliceIndexes[i];
        }

        Debug.Log($" p1={parent1} | p2= {parent2} | point = {sliceIndexes[0] / 3}\n" +
            $" fst child = {PrintArray(firstChildTape)} | snd child ={PrintArray(secondChildTape)}");

        return (firstChild: firstChildTape, secondChild: secondChildTape);
    }

    // Auxliar function to help build the genetic tape of the children on crossover 
    private static int[] BuildChildrenTapes(BoardSetting incomingParent, BoardSetting baseGeneParent, int currIndex, int sliceIndex, int[] childTape)
    {
        int[] resultTape = childTape.Concat(new int[sliceIndex - currIndex]).ToArray(); // Result child array
        int[] parentGeneSlice = incomingParent.GetGeneticBinaryTape()[currIndex..sliceIndex]; // The parent slice incoming to child

        // The tape that the crossover will use as base
        int[] concatedParentGeneSlice = parentGeneSlice.Concat(incomingParent.GetGeneticBinaryTape()).ToArray();

        int currChildGene = currIndex; // The current gene on child that will be updated
        int parentGeneSliceIndex = 0;  // The parent gene on analysis

        // Insert the genes on child avoiding repeating genes
        while (currChildGene < sliceIndex)
        {
            int[] gene = new int[2];
            try
            {
                gene = concatedParentGeneSlice[parentGeneSliceIndex..(parentGeneSliceIndex + 3)];
            }
            catch
            {
                Debug.Log(currIndex / 3);
            }

            bool contains = ContainsGene(resultTape, gene);

            if (!contains)
            {
                resultTape[currChildGene] = gene[0];
                resultTape[currChildGene + 1] = gene[1];
                resultTape[currChildGene + 2] = gene[2];
                currChildGene += 3;
            }

            parentGeneSliceIndex += 3;
        }

        return resultTape;
    }

    // Checks if a given gene is the genoma
    static bool ContainsGene(int[] genoma, int[] gene)
    {
        bool contains = false;
        for (int j = 0; j < genoma.Length; j += 3)
        {
            if (Enumerable.SequenceEqual(genoma[j..(j + 3)], gene))
            {
                contains = true;
            }
        }

        return contains;
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


    public static (BoardSetting parent1, BoardSetting parent2) ChooseParents(BoardSetting[] population)
    {
        var chooser = new System.Random();

        int bestFit1 = int.MaxValue;
        int bestFit2 = int.MaxValue;
        BoardSetting bestPop1 = null;
        BoardSetting bestPop2 = null;

        var tmp = "candidates: ";

        for(int i=0; i < 5; i++)
        {
            BoardSetting choosen = population[chooser.Next(population.Length)];
            tmp += choosen.Fitness + " | ";

            if(choosen.Fitness < bestFit1)
            {
                if (bestFit1 > bestFit2)
                {
                    bestFit2 = bestFit1;
                    bestPop2 = bestPop1;
                }

                bestFit1 = choosen.Fitness;
                bestPop1 = choosen;
            }
            else if(choosen.Fitness < bestFit2)
            {
                bestFit2 = choosen.Fitness;
                bestPop2 = choosen;
            }
        }

        Debug.Log(tmp);
        return (parent1: bestPop1, parent2: bestPop2);
    }

    public static BoardSetting[] SurvivorSelection(BoardSetting[] population, BoardSetting[] offspring)
    {
        int[] worstFitIndexes = new int[offspring.Length];

        for(int i = 0; i < offspring.Length; i++)
        {
            int currWorstFit = -1;
            int currWorstIndex = -1;

            for (int j = 0; j < population.Length; j++)
            {
                BoardSetting currPop = population[j];
                for (int k = 0; k < offspring.Length; k++)
                {
                    if (currPop.Fitness > currWorstFit && !worstFitIndexes.Contains(j))
                    {
                        currWorstFit = currPop.Fitness;
                        currWorstIndex = j;
                    }
                }
                worstFitIndexes[i] = currWorstIndex;
            }
        }

        for (int i=0; i < offspring.Length; i++)
        {
            int worstFitIndex = worstFitIndexes[i];
            Debug.Log($"({i}) {population[worstFitIndex]} <- {offspring[i]}");
            population[worstFitIndex] = offspring[i];
        }

        return population;
    }

    //Debug methods bellow

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


  public static string PrintArray(int[] array)
    {
        return string.Join(" ", array.Select(n => n.ToString()));
    }
}
