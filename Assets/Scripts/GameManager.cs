using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] GeneticManager genetic;
    [SerializeField] QueensManipulator manipulator;

    // void RunIteration() {
    //     int[] newPositions = genetic.runGeneticAlgorithm();
    //     manipulator.UpdateQueensPositions();        
    // }

    [ContextMenu("CheckCollisions")]
    int CheckCollisions()
    {
        int numCollisions = 0;
        int[] geneticTape = genetic.GetGeneticTape();
        
        for (int i = 0; i < 8; i++) 
        {
            int lineValue = geneticTape[i];
            for (int j = 0; j < 8; j++) 
            {
                if (j == i) 
                {
                    continue;
                }
                
                int distanceFromCurrentIdx = Mathf.Abs(i-j);
                
                int downDiagValToColide = lineValue+distanceFromCurrentIdx;
                int upDiagValToColide = lineValue-distanceFromCurrentIdx;
                
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

    public void OnCheck()
    {
        Debug.Log("Number of Collisions: "+CheckCollisions()/2);
    }

    public void OnRun()
    {
        int numColisions = -1;
        int iter = 0;
        while (numColisions != 0 && iter < 10)
        {
            iter++;
            manipulator.OnUpdateQueens();
            numColisions = CheckCollisions();
        }
        Debug.Log("Acabou o loop!"+"\nCollisions: "+numColisions.ToString());
        Debug.Log("Iterations: "+iter.ToString());
    }
}
