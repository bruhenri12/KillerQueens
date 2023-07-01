using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
  [SerializeField] QueensManipulator manipulator;
  [Header("Parameters")]
  [SerializeField] private int sizeOfPopulation = 100;
  [SerializeField] private int maxIterations = 10;
  public BoardSetting[] boardSettings;
  private BoardSetting bestSetting;

  //Return the best setting among the various boardSettings
  BoardSetting GetBestSetting(BoardSetting[] boardSettings)
  {
    BoardSetting best = null;

    for (int i = 0; i < boardSettings.Length; i++) 
    {
      if(best == null || best.Fitness > boardSettings[i].Fitness) 
      {
        best = boardSettings[i];
      }
    }

    return best;
  }

  public void OnCheck()
  {
    if (!bestSetting.Equals(null))
    {
      Debug.Log("Number of Collisions: " + bestSetting.CheckCollisions() / 2);
    }
  }

  public void OnRun()
  {
    int iter = 0;
    bestSetting = null;
    int bestFitness = int.MaxValue;
    boardSettings = new BoardSetting[sizeOfPopulation];

    //Initialize the population with random individuals
    for (int i = 0; i < sizeOfPopulation; i++)
    {
      boardSettings[i] = new BoardSetting();
    }

    while (bestFitness != 0 && iter < maxIterations)
    {
      for (int i = 0; i < sizeOfPopulation; i++)
      {
        //int[] p1 = { 0,0,0, 0,1,1, 0,1,0, 1,1,1, 0,0,1, 1,0,1, 1,1,0, 1,0,0 };
        //int[] p2 = { 0,1,1, 1,1,1, 0,0,1, 0,0,0, 1,0,0, 1,1,0, 0,1,0, 1,0,1 };

        //GeneticManager.GenerateOffspring(boardSettings[0], boardSettings[1], 2);
        //GeneticManager.GenerateOffspring(new BoardSetting(p1), new BoardSetting(p2), 2);
        
        //Placeholder to simulate breeding
        boardSettings[i] = new BoardSetting();
      }

      var parents = GeneticManager.ChooseParents(boardSettings);
      Debug.Log("p1: " + parents.parent1.ToString());
      Debug.Log("p2: " + parents.parent2.ToString());

      //Update best setting
      bestSetting = GetBestSetting(boardSettings);
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
        tmpTxt += boardSettings[i].Fitness + " | ";
      }
      tmpTxt += "\nBest Fitness:  " + bestSetting.Fitness;
      tmpTxt += "\nBest boardSettings: [";

      foreach (var gene in bestSetting.GetGeneticIntTape())
      {
        tmpTxt += gene + ",";
      }
      tmpTxt += "]";

      Debug.Log(tmpTxt);
  }
}
