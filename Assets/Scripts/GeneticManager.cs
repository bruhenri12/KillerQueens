using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class GeneticManager : MonoBehaviour
{
    private int[] genTape = new int[24];

    public void RandomizeGeneticTape(){
        System.Random rnd = new System.Random();
        int[] tape = Enumerable.Range(0, 8).OrderBy(c => rnd.Next()).ToArray();
        GenerateBitTape(tape);
    }

    public int[] GetGeneticTape()
    {
        return BitTapeToDecimal();
    }

    public void GenerateBitTape(int[] tape)
    {
        int temp = 0;
        for (int i = 0; i < 8; i++)
        {
            string bitString = Convert.ToString(tape[i],2);

            int[] bitQueen = bitString.PadLeft(3,'0')
                             .Select(c => int.Parse(c.ToString()))
                             .ToArray();

            for (int j = 0; j < 3; j++) {genTape[temp+j] = bitQueen[j];}
            temp += 3;
        }
    }

    public void PrintBitTape()
    {
        #region Print Original Tape
        string tapeS = "";
        for (int i = 0; i < 8; i++) {tapeS += BitTapeToDecimal()[i].ToString() + " ";}

        Debug.Log(tapeS);
        #endregion

        #region Print Bit Tape
        string bitS = "";

        int temp = 0;
        for (int i = 0; i < 8; i++)
        {
            string tempS = "";
            for (int j = 0; j < 3; j++) {tempS += genTape[temp+j];}
            temp += 3;
            bitS += "[" + tempS + "] ";
        }
        Debug.Log(bitS);
        #endregion
    }

    public int[] BitTapeToDecimal()
    {
        int[] decTape = new int[8];

        int temp = 0;
        for (int i = 0; i < 8; i++)
        {
            string conv = "";
            for (int j = 0; j < 3; j++) {conv += genTape[temp+j];}
            decTape[i] = Convert.ToInt16(conv,2);
            temp += 3;
        }
        return decTape;
    }

    public void PrintConv()
    {
        string news = "";
        foreach (int item in BitTapeToDecimal())
        {
            news += item + " ";
        }
        Debug.Log(news);
    }
}
