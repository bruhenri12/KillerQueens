using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
  [SerializeField] QueensManipulator manipulator;

  [Header("Parameters")]
  [SerializeField] private int sizeOfPopulation = 100;
  [SerializeField] private int maxIterations = 10;
  [SerializeField] private int offspringSize = 2;
  [SerializeField] [Range(0,1)]  private float mutationProb = 0.4f;
  [SerializeField] [Range(0, 1)] private float perGeneMutation = 0.05f;
  [SerializeField] [Range(0,1)]  private float crossoverProb = 0.9f;
  [SerializeField] int splitsNumber = 1; 
  [SerializeField] private int[] geneSlices = { 2, 4, 6 };
  [SerializeField] private int retries = 4;

  public BoardSetting[] boardSettings;
  private BoardSetting bestSetting;

    //Metrics
    private int[] nIterations;
    private bool[] iterConverged;
    private int[] nConvergedPops;
    private double[] avgFitness;
    private BoardSetting[] bestPop;

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
    //Reset Metrics
    nIterations = new int[retries];
    iterConverged = new bool[retries];
    nConvergedPops = new int[retries];
    avgFitness = new double[retries];
    bestPop = new BoardSetting[retries];

    for (int i=0; i<retries; i++)
    {
      Epoch(i);
    }

     PrintMetrics();
  }

    private void Epoch(int epochNum)
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
            var parents = GeneticManager.ChooseParents(boardSettings);
            Debug.Log("p1: " + parents.parent1.ToString());
            Debug.Log("p2: " + parents.parent2.ToString());

            var offspring = GeneticManager.GenerateOffspring(parents.parent1, parents.parent2, offspringSize,
                                                              splitsNumber, geneSlices, mutationProb, crossoverProb, perGeneMutation);

            Debug.Log("Offspring: " + offspring[0] + " \n " + offspring[1]);

            boardSettings = GeneticManager.SurvivorSelection(boardSettings, offspring);

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

        //Setting metric fields
        nIterations[epochNum] = iter;
        iterConverged[epochNum] = boardSettings.Any(board => board.Fitness == 0);
        nConvergedPops[epochNum] = boardSettings.Count(board => board.Fitness == 0);
        avgFitness[epochNum] = boardSettings.Average(board => board.Fitness);
        bestPop[epochNum] = bestSetting;
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

  void PrintMetrics()
    {
        double convergenceCount = iterConverged.Count(conv => conv==true);

        double avgIterNum = nIterations.Average();
        double stdIterNum = Std(nIterations);

        double avgFit = avgFitness.Average();
        double stdFit = Std(avgFitness);


        string printTxt = $"Convergence Rate: {convergenceCount/retries}\n" +
            $"Nº Iterations: avg={avgIterNum} | std={stdIterNum}\n" +
            $"Fitness: avg={avgFit} | std={stdFit}"; 

        Debug.Log(printTxt);
    }

    double Std(double[] array)
    {
        double avg = array.Average();
        int count = array.Length;
        double sqrDiff = 0;

        foreach(int x in array)
        {
            sqrDiff += Math.Pow((x - avg),2);
        }

        return Math.Sqrt(sqrDiff / (count - 1));
    }

    double Std(int[] array)
    {
        double[] convertedArray = new double[array.Length];
        for(int i=0; i < array.Length; i++)
        {
            convertedArray[i] = (double)array[i];
        }
        return Std(convertedArray);
    }
}
