using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class Metrics
{
    [Serializable]
    public class BoardSettingJson{
        public string binaryGenTape;
        public string intGenTape;
        public int fitness = int.MaxValue;

        public BoardSettingJson(BoardSetting boardSetting)
        {
            if (boardSetting != null) 
            {
                this.binaryGenTape = string.Join("",boardSetting.GetGeneticBinaryTape());
                this.intGenTape = string.Join("", boardSetting.GetGeneticIntTape());
                this.fitness = boardSetting.Fitness;
            }
        }
    }

    [Serializable]
    public class Execution
    {
        public int executionNum;
        public double[] avgFitnessIter;
        public double[] stdFitnessIter;
        public int[] bestSettingFitnessIter;

        public Execution(int executionNum, double[] avgFitnessIter, double[] stdFitnessIter, int[] bestSettingIter)
        {
            this.executionNum = executionNum;
            this.avgFitnessIter = avgFitnessIter;
            this.stdFitnessIter = stdFitnessIter;
            this.bestSettingFitnessIter = bestSettingIter;
        }
    }

    public int maxIteration;
    public int populationSize;

    public int[] nIterations;
    public bool[] iterConverged;
    public int[] nConvergedPops;
    public double[] avgFitness;

    public BoardSettingJson[] bestPops;
    public Execution[] executions;

    public double convergencePerc;
    public double avgIterNum;
    public double stdIterNum;

    public double avgIterAllPopConverged;
    public double stdIterAllPopConverged;

    public double avgFit;
    public double stdFit;

    public Metrics(int maxIteration, int populationSize, int[] nIterations, bool[] iterConverged,int[] nConvergedPops, double[] avgFitness, BoardSetting[] bestPop,
        double convergencePerc, double avgIterNum, double stdIterNum, double avgFit, double stdFit,
        List<double>[] avgFitnessIter, List<double>[] stdFitnessIter, List<BoardSetting>[] bestSettingIter,
        double avgIterAllPopConverged, double stdIterAllPopConverged)
    {
        BoardSettingJson[] bestPopJsonArray = new BoardSettingJson[bestPop.Length];
        for(int i=0; i<bestPop.Length; i++)
        {
            bestPopJsonArray[i] = new BoardSettingJson(bestPop[i]);
        }

        this.maxIteration = maxIteration;
        this.populationSize = populationSize;

        this.nIterations = nIterations;
        this.iterConverged = iterConverged;
        this.nConvergedPops = nConvergedPops;
        this.avgFitness = avgFitness;
        this.bestPops = bestPopJsonArray;
        this.convergencePerc = convergencePerc;
        this.avgIterNum = avgIterNum;
        this.stdIterNum = stdIterNum;
        this.avgFit = avgFit;
        this.stdFit = stdFit;
        this.avgIterAllPopConverged = avgIterAllPopConverged;
        this.stdIterAllPopConverged = stdIterAllPopConverged;

        // Store per iter metrics
        executions = new Execution[bestSettingIter.GetLength(0)];
        for (int i = 0; i < bestSettingIter.GetLength(0); i++)
        {
            BoardSettingJson[] bestSettingIterJson = new BoardSettingJson[bestSettingIter.Count()];
            var avgFitnessLine = avgFitnessIter[i];
            var stdFitnessLine = stdFitnessIter[i];
            var bestSettingLine = bestSettingIter[i];

            int[] bestFitnessLine = bestSettingLine.Select(board => board.Fitness).ToArray();

            Execution currExec = new Execution(i, avgFitnessLine.ToArray(), stdFitnessLine.ToArray(), bestFitnessLine.ToArray());
            executions[i] = currExec;
        }
    }

    public T[] GetArrayLine<T>(T[,] array, int i)
    {
        T[] line = new T[array.GetLength(1)];

        for(int j=0; j< line.Length; j++)
        {
            line[j] = array[i,j];
        }

        return line;
    }
}
