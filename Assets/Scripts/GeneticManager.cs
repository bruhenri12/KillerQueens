using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using System.Text;

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
    static string TapeString(int[] tape)
    {
        string tapeOut = "";

        if (tape.Length == 24)
        {
            for (int i = 0; i < 8; i++)
            {
                string bitInt = "";
                for (int j = 0; j < 3; j++)
                {
                    bitInt += tape[i * 3 + j];
                }
                tapeOut += bitInt + " ";
            }
        }
        else
        {
            for (int i = 0; i < tape.Length; i++)
            {
                tapeOut += tape[i] + " ";
            }

        }
        return tapeOut;
    }

    public static string ConvertToStringBinaryTape(int[] tape)
    {
        Debug.Log("Converting: " + TapeString(tape));
        string genTape = "";

        for (int i = 0; i < 8; i++)
        {
            string bitString = Convert.ToString(tape[i], 2).PadLeft(3, '0');

            genTape += bitString;
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

    public static int[] ConvertToIntTape(string tape)
    {
        int[] genTape = new int[8];

        for (int i = 0; i < tape.Length; i += 3)
        {
            genTape[i / 3] = Convert.ToInt32(tape[i..(i + 3)], 2);
        }

        return genTape;
    }

    public static BoardSetting[] GenerateOffspring(
    BoardSetting parent1, BoardSetting parent2,
    int offspringSize, int geneSlices, int[] sliceIndexes,
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
        var curOffspring = Crossover(parent1, parent2, geneSlices, sliceIndexes);
        offspring[i] = new BoardSetting(curOffspring.firstChild);

        if(i+1 < offspringSize) { 
          offspring[i+1] = new BoardSetting(curOffspring.secondChild);
        }
      }
    }

    return offspring;
  }

  public static (string firstChild, string secondChild) Crossover(BoardSetting parent1, BoardSetting parent2,
                    int splitsNumber, int[] splitsIndexes)
  {
        string p1 = ConvertToStringBinaryTape(GeneticManager.ConvertToIntTape(parent1.GetGeneticBinaryTape()));
        string p2 = ConvertToStringBinaryTape(GeneticManager.ConvertToIntTape(parent2.GetGeneticBinaryTape()));

        string fstChildGene = "";
        string sndChildGene = "";

        List<int> fstChildNullsIndexes = new List<int>();
        List<int> sndChildNullsIndexes = new List<int>();

        int currentSplitIndex = 0;

        for (int i = 0; i < splitsNumber; i++)
        {
            if (i % 2 == 0)
            {
                for (int j = currentSplitIndex; j < splitsIndexes[i] * 3; j += 3)
                {
                    if (ContainsGene(fstChildGene, p1[j..(j + 3)]))
                    {
                        fstChildGene += "---";
                        fstChildNullsIndexes.Add(j);
                    }
                    else
                    {
                        fstChildGene += p1[j..(j + 3)];
                    }

                    if (ContainsGene(sndChildGene, p2[j..(j + 3)]))
                    {
                        sndChildGene += "---";
                        sndChildNullsIndexes.Add(j);
                    }
                    else
                    {
                        sndChildGene += p2[j..(j + 3)];
                    }
                }
            }
            else
            {
                for (int j = currentSplitIndex; j < splitsIndexes[i] * 3; j += 3)
                {
                    if (ContainsGene(fstChildGene, p2[j..(j + 3)]))
                    {
                        fstChildGene += "---";
                        fstChildNullsIndexes.Add(j);
                    }
                    else
                    {
                        fstChildGene += p2[j..(j + 3)];
                    }

                    if (ContainsGene(sndChildGene, p1[j..(j + 3)]))
                    {
                        sndChildGene += "---";
                        sndChildNullsIndexes.Add(j);
                    }
                    else
                    {
                        sndChildGene += p1[j..(j + 3)];
                    }
                }
            }

            currentSplitIndex = splitsIndexes[i] * 3;
        }

        if (splitsNumber - 1 % 2 == 0)
        {
            for (int j = currentSplitIndex; j < 24; j += 3)
            {
                string incomingFstChildGene = (splitsNumber == 1) ? p2[j..(j + 3)] : p1[j..(j + 3)];
                string incomingSndChildGene = (splitsNumber == 1) ? p1[j..(j + 3)] : p2[j..(j + 3)];

                if (ContainsGene(fstChildGene, incomingFstChildGene))
                {
                    fstChildGene += "---";
                    fstChildNullsIndexes.Add(j);

                }
                else
                {
                    fstChildGene += incomingFstChildGene;
                }

                if (ContainsGene(sndChildGene, incomingSndChildGene))
                {
                    sndChildGene += "---";
                    sndChildNullsIndexes.Add(j);
                }
                else
                {
                    sndChildGene += incomingSndChildGene;
                }
            }
        }
        else
        {
            for (int j = currentSplitIndex; j < 24; j += 3)
            {
                if (ContainsGene(fstChildGene, p2[j..(j + 3)]))
                {
                    fstChildGene += "---";
                    fstChildNullsIndexes.Add(j);
                }
                else
                {
                    fstChildGene += p2[j..(j + 3)];
                }

                if (ContainsGene(sndChildGene, p1[j..(j + 3)]))
                {
                    sndChildGene += "---";
                    sndChildNullsIndexes.Add(j);
                }
                else
                {
                    sndChildGene += p1[j..(j + 3)];
                }
            }
        }

        foreach (int index in fstChildNullsIndexes)
        {
            StringBuilder fstChildStringBuilder = new StringBuilder(fstChildGene);
            for (int i = 0; i < p2.Length; i += 3)
            {
                if (!ContainsGene(fstChildGene, p2[i..(i + 3)]))
                {
                    if (splitsNumber == 1)
                    {
                        fstChildGene = fstChildStringBuilder.Remove(index, 3).Append(p2[i..(i + 3)]).ToString();
                    }
                    else
                    {
                        fstChildGene = fstChildStringBuilder.Remove(index, 3).Insert(index, p2[i..(i + 3)]).ToString();
                    }
                }
            }
        }
        foreach (int index in sndChildNullsIndexes)
        {
            for (int i = 0; i < p1.Length; i += 3)
            {
                StringBuilder sndChildStringBuilder = new StringBuilder(sndChildGene);
                if (!ContainsGene(sndChildGene, p1[i..(i + 3)]))
                {
                    if (splitsNumber == 1)
                    {
                        sndChildGene = sndChildStringBuilder.Remove(index, 3).Append(p1[i..(i + 3)]).ToString();
                    }
                    else
                    {
                        sndChildGene = sndChildGene.Remove(index, 3).Insert(index, p1[i..(i + 3)]).ToString();
                    }
                }
            }
        }

        Debug.Log("Child 1: " + TapeString(ConvertToIntTape(fstChildGene)));
        Debug.Log("Child 2: " + TapeString(ConvertToIntTape(sndChildGene)));

        return (firstChild: fstChildGene, secondChild: sndChildGene);
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

    static bool ContainsGene(string genoma, string gene)
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
                if (bestFit1 < bestFit2)
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
