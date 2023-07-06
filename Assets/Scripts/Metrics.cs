using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Metrics
{
    [Serializable]
    public class BoardSettingJson{
        public int[] binaryGenTape = new int[24];
        public int[] intGenTape = new int[8];
        public int fitness;

        public BoardSettingJson(BoardSetting boardSetting)
        {
            this.binaryGenTape = boardSetting.GetGeneticBinaryTape();
            this.intGenTape = boardSetting.GetGeneticIntTape();
            this.fitness = boardSetting.Fitness;
        }
    }

    public int[] nIterations;
    public bool[] iterConverged;
    public int[] nConvergedPops;
    public double[] avgFitness;

    public BoardSettingJson[] bestPops;

    public double convergencePerc;
    public double avgIterNum;
    public double stdIterNum;

    public double avgFit;
    public double stdFit;

    public Metrics(int[] nIterations, bool[] iterConverged, int[] nConvergedPops, double[] avgFitness,
            BoardSetting[] bestPop, double convergencePerc, double avgIterNum, double stdIterNum, double avgFit, double stdFit)
    {
        BoardSettingJson[] bestPopJsonArray = new BoardSettingJson[bestPop.Length];
        for(int i=0; i<bestPop.Length; i++)
        {
            bestPopJsonArray[i] = new BoardSettingJson(bestPop[i]);
        }

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
    }
}
