using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
  [SerializeField] QueensManipulator manipulator;
  private static int sizeOfPopulation = 100;
  public GeneticManager[] genes = new GeneticManager[sizeOfPopulation];
  // Taking only one element -> fix this to calculate the fitness for every gene
  public static GameManager Instance {get; private set;}

  private GeneticManager bestSetting;

  // void RunIteration() {
  //     int[] newPositions = genetic.runGeneticAlgorithm();
  //     manipulator.UpdateQueensPositions();        
  // }

  void Awake(){
    //Singleton pattern
    if(!Instance){
      Instance = this;
    } else {
      Destroy(gameObject);
    }
  }

  [ContextMenu("CheckCollisions")]
  public int CheckCollisions(int[] geneticTape)
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

  //Return the best setting among the various settings
  GeneticManager GetBestSetting(GeneticManager[] settings)
  {
    GeneticManager best = null;

    for (int i = 0; i < settings.Length; i++) 
    {
      if(best == null || best.Fitness > settings[i].Fitness) 
      {
        best = settings[i];
      }
    }

    return best;
  }

  public void OnCheck()
  {
    if (!bestSetting.Equals(null))
    {
      Debug.Log("Number of Collisions: " + CheckCollisions(bestSetting.GetGeneticTape()) / 2);
    }
  }

  public void OnRun()
  {
    int iter = 0;
    bestSetting = null;
    int bestFitness = int.MaxValue;

    //Initialize the population with random individuals
    for (int i = 0; i < sizeOfPopulation; i++)
    {
      genes[i] = new GeneticManager();
    }

    // Taking only one element -> fix this to calculate the fitness for every gene
    while (bestFitness != 0 && iter < 100)
    {
      //Placeholder to simulate breeding
      for (int i = 0; i < sizeOfPopulation; i++)
      {
        genes[i] = new GeneticManager();
      }

      //Update best setting
      bestSetting = GetBestSetting(genes);
      bestFitness = bestSetting.Fitness;

      //Update board renderer
      manipulator.OnUpdateQueens(bestSetting);

      PrintPopulationFitness();
      iter++; 
    }

    Debug.Log("Acabou o loop!" + "\nCollisions: " + bestSetting.Fitness.ToString());
    Debug.Log("(" + iter + ") best: " + bestSetting.Fitness);
    Debug.Log("Iterations: " + iter.ToString());
  }

  void PrintPopulationFitness(){
      //Debug method used to print the fitness of each individual from the population 
      string tmpTxt = "Fitness: ";
      for (int i = 0; i < sizeOfPopulation; i++)
      {
        tmpTxt += genes[i].Fitness + " | ";
      }
      tmpTxt += "\nBest Fitness:  " + bestSetting.Fitness;
      tmpTxt += "\nBest genes: [";

      foreach (var gene in bestSetting.GetGeneticTape())
      {
        tmpTxt += gene + ",";
      }
      tmpTxt += "]";

      Debug.Log(tmpTxt);
  }
}
