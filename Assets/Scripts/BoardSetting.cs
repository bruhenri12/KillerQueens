using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardSetting
{
    private int[] genTape = new int[24];
    public int Fitness {get; private set;}

    public BoardSetting()
    {
        // Initialize a GeneticManager object already starting with random genes
        genTape = GeneticManager.GenerateRandomGeneticTape();

        // Set the fitness using the number of collisions
        Fitness = CheckCollisions();
    }

    //Return the tape with int genes
    public int[] GetGeneticTape()
    {
        return GeneticManager.ConvertToIntTape(genTape);
    }

    public int CheckCollisions()
    {
        int[] geneticTape = GetGeneticTape();
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
}
