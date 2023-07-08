using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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
  [SerializeField] private int executions = 4;

  public BoardSetting[] boardSettings;
  private BoardSetting bestSetting;

    //Metrics
    private int[] nIterations;
    private bool[] iterConverged;
    private int[] nIterAllPopConverge;
    private int[] nConvergedPops;
    private double[] avgFitness;
    private BoardSetting[] bestPop;

    //Per execution metrics
    private List<double>[] avgFitnessIter;
    private List<double>[] stdFitnessIter;
    private List<BoardSetting>[] bestSettingIter;

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

    ResetMetrics();

    for (int i=0; i<executions; i++)
    {
      Epoch(i);
    }

     SaveAndPrintMetrics();

        Debug.Log("Finished algorithm execution!");
  }

    private void Epoch(int epochNum)
    {
        int iter = 0;
        bestSetting = null;
        int bestFitness = int.MaxValue;
        double iterAvgFitness = double.MaxValue;
        boardSettings = new BoardSetting[sizeOfPopulation];
        bool converged = false;

        //Initialize the population with random individuals
        for (int i = 0; i < sizeOfPopulation; i++)
        {
            boardSettings[i] = new BoardSetting();
        }

        while (!converged)
        {
            var parents = GeneticManager.ChooseParents(boardSettings);
            var offspring = GeneticManager.GenerateOffspring(parents.parent1, parents.parent2, offspringSize,
                                                              splitsNumber, geneSlices, mutationProb, crossoverProb, perGeneMutation);

            boardSettings = GeneticManager.SurvivorSelection(boardSettings, offspring);

            //Update best setting
            bestSetting = GetBestSetting(boardSettings);
            bestFitness = bestSetting.Fitness;

            //Update per iteration metrics
            int[] fitnessArray = boardSettings.Select(board => board.Fitness).ToArray();
            iterAvgFitness = fitnessArray.Average();
            avgFitnessIter[epochNum].Add(iterAvgFitness);
            stdFitnessIter[epochNum].Add(Std(fitnessArray));
            bestSettingIter[epochNum].Add(bestSetting);

            // Check convergence
            if(bestFitness == 0 && !converged)
            {
                //Setting metric fields
                nIterations[epochNum] = iter;
                iterConverged[epochNum] = boardSettings.Any(board => board.Fitness == 0);
                nConvergedPops[epochNum] = boardSettings.Count(board => board.Fitness == 0);
                avgFitness[epochNum] = boardSettings.Average(board => board.Fitness);
                bestPop[epochNum] = bestSetting;

                // Disallow the above metrics be updated after the first pop converge
                converged = true;
            }

            //Update iteration counter
            iter++;
        }

        nIterAllPopConverge[epochNum] = iter;

        if (iterAvgFitness == 0)
        {
            Debug.Log($"All Pop Converged in {iter}th iter. Avg Fit: {iterAvgFitness}");
        }

        //Update board renderer
        manipulator.OnUpdateQueens(bestSetting);
    }

    private void ResetMetrics()
    {
        nIterations = new int[executions];
        iterConverged = new bool[executions];
        nConvergedPops = new int[executions];
        avgFitness = new double[executions];
        bestPop = new BoardSetting[executions];
        nIterAllPopConverge = new int[executions];

        avgFitnessIter = new List<double>[executions];
        stdFitnessIter = new List<double>[executions];
        bestSettingIter = new List<BoardSetting>[executions];

        for (int i = 0; i < executions; i++)
        {
            avgFitnessIter[i] = new List<double>();
        }

        for (int i = 0; i < executions; i++)
        {
            stdFitnessIter[i] = new List<double>();
        }

        for (int i = 0; i < executions; i++)
        {
            bestSettingIter[i] = new List<BoardSetting>();
        }
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
  }

  void SaveAndPrintMetrics()
    {
        double convergenceCount = iterConverged.Count(conv => conv==true);
        double convergencePerc = convergenceCount / executions;

        double avgIterNum = nIterations.Average();
        double stdIterNum = Std(nIterations);

        double avgFit = avgFitness.Average();
        double stdFit = Std(avgFitness);

        double avgIterAllPopConverged = nIterAllPopConverge.Average();
        double stdIterAllPopConverged = Std(nIterAllPopConverge);

        string printTxt = $"Convergence Rate: {convergencePerc}\n" +
            $"Nº Iterations: avg={avgIterNum} | std={stdIterNum}\n" +
            $"Fitness: avg={avgFit} | std={stdFit}\n" +
            $"Iter. Total Convergence: avg={avgIterAllPopConverged} | std: {stdIterAllPopConverged}";

        var metrics = new Metrics(maxIterations, sizeOfPopulation ,nIterations,iterConverged,nConvergedPops,avgFitness, bestPop, convergencePerc, avgIterNum, stdIterNum,
                            avgFit, stdFit,avgFitnessIter, stdFitnessIter, bestSettingIter, avgIterAllPopConverged, stdIterAllPopConverged);
        var jsonString = JsonUtility.ToJson(metrics);
        var resultsPath = @$"Assets/Metrics/Metrics {DateTime.Now.ToString("M / d / yy h: m:s tt").Replace('/', '_').Replace(':','_').Replace(" ", "")}.json";
        File.WriteAllText(resultsPath, jsonString);    
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
