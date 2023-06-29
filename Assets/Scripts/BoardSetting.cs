using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardSetting
{
    private int[] genTape = new int[24];
    public int Fitness {get; private set;}

    public BoardSetting()
    {
        // Initialize a BoardSetting object already starting with random genes
        genTape = GeneticManager.GenerateRandomGeneticTape();

        // Set the fitness using the number of collisions
        Fitness = CheckCollisions();
    }

    public BoardSetting(int[] tape)
    {
        // Initialize a BoardSetting object with a given tape
        genTape = tape;

        // Set the fitness using the number of collisions
        Fitness = CheckCollisions();
    }

    //Return the tape with int genes
    public int[] GetGeneticIntTape()
    {
        return GeneticManager.ConvertToIntTape(genTape);
    }

    //Return the tape with binary genes
    public int[] GetGeneticBinaryTape()
    {
        return genTape;
    }

    public int CheckCollisions()
    {
        int[] geneticTape = GetGeneticIntTape();
        int numCollisions = 0;

        for (int i = 0; i < 8; i++)
        {
        int lineValue = geneticTape[i];
        for (int j = 0; j < 8; j++)
        {
            if (j == i)
            {
                continue;
            }

            int distanceFromCurrentIdx = Mathf.Abs(i - j);

            int downDiagValToColide = lineValue + distanceFromCurrentIdx;
            int upDiagValToColide = lineValue - distanceFromCurrentIdx;

            int comparisonColumnValue = geneticTape[j];
            bool willColideUp = comparisonColumnValue == upDiagValToColide;
            bool willColideDown = comparisonColumnValue == downDiagValToColide;
            bool willColide = willColideUp || willColideDown;
            if (willColide)
            {
            numCollisions++;
            }
        }
        }

        return numCollisions;
    }

    public override string ToString()
    {
        int[] intTape = GetGeneticIntTape();
        string tmp = "[";
        for(int i = 0; i < intTape.Length; i++)
        {
            var gene = intTape[i];
            tmp += gene.ToString();

            if (i != intTape.Length - 1)
            {
                tmp += ",";
            }
        }
        tmp += "]";
        return tmp;
    }
}
