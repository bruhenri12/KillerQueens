using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
  [SerializeField] QueensManipulator manipulator;
  private static int sizeOfPopulation = 100;
  public GeneticManager[] genes = new GeneticManager[sizeOfPopulation];
  // Taking only one element -> fix this to calculate the fitness for every gene
  private GeneticManager selectedGene;

  // void RunIteration() {
  //     int[] newPositions = genetic.runGeneticAlgorithm();
  //     manipulator.UpdateQueensPositions();        
  // }

  [ContextMenu("CheckCollisions")]
  int CheckCollisions(int[] geneticTape)
  {
    int numCollisions = 0;
    // int[] geneticTape = genetic.GetGeneticTape();

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

  public void OnCheck()
  {
    if (!selectedGene.Equals(null))
    {
      Debug.Log("Number of Collisions: " + CheckCollisions(selectedGene.GetGeneticTape()) / 2);
    }
  }

  public void OnRun()
  {
    int numColisions = -1;
    int iter = 0;
    for (int i = 0; i < sizeOfPopulation; i++)
    {
      genes[i] = new GeneticManager();
    }
    // Taking only one element -> fix this to calculate the fitness for every gene
    selectedGene = genes[5];
    while (numColisions != 0 && iter < 10)
    {
      iter++;
      manipulator.OnUpdateQueens(selectedGene);
      numColisions = CheckCollisions(selectedGene.GetGeneticTape());
    }
    Debug.Log("Acabou o loop!" + "\nCollisions: " + numColisions.ToString());
    Debug.Log("Iterations: " + iter.ToString());
  }
}
