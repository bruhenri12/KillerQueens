using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Test : MonoBehaviour
{
    public int splitsNumber = 3;
    public int childCount = 2;
    public int[] splitsIndexes = new int[] {2, 4, 6};
    public int[][] parentsGenes = new int[][] {
        new int[] { 0,0,0, 0,1,1, 0,1,0, 1,1,1, 0,0,1, 1,0,1, 1,1,0, 1,0,0 },
        new int[] { 0,1,1, 1,1,1, 0,0,1, 0,0,0, 1,0,0, 1,1,0, 0,1,0, 1,0,1 },
    };
    public void Crossover()
    {
        string p1 = ConvertToBinaryTape(GeneticManager.ConvertToIntTape(parentsGenes[0]));
        string p2 = ConvertToBinaryTape(GeneticManager.ConvertToIntTape(parentsGenes[1]));
        
        Debug.Log("Parent 1: "+p1);
        Debug.Log("Parent 2: "+p2);
        
        string fstChildGene = "";
        string sndChildGene = "";

        List<int> fstChildNullsIndexes = new List<int>();
        List<int> sndChildNullsIndexes = new List<int>();

        int currentSplitIndex = 0;

        for (int i = 0; i < splitsNumber; i++)
        {
            if (i % 2 == 0)
            {
                for (int j = currentSplitIndex; j < splitsIndexes[i]*3; j += 3)
                {
                    if (ContainsGene(fstChildGene,p1[j..(j+3)]))
                    {
                        fstChildGene += "---";
                        fstChildNullsIndexes.Add(j);
                    }
                    else
                    {
                        fstChildGene += p1[j..(j+3)];
                    }

                    if (ContainsGene(sndChildGene,p2[j..(j+3)]))
                    {
                        sndChildGene += "---";
                        sndChildNullsIndexes.Add(j);
                    }
                    else
                    {
                        sndChildGene += p2[j..(j+3)];
                    }
                }
            }
            else
            {
                for (int j = currentSplitIndex; j < splitsIndexes[i]*3; j += 3)
                {
                    if (ContainsGene(fstChildGene,p2[j..(j+3)]))
                    {
                        fstChildGene += "---";
                        fstChildNullsIndexes.Add(j);
                    }
                    else
                    {
                        fstChildGene += p2[j..(j+3)];
                    }

                    if (ContainsGene(sndChildGene,p1[j..(j+3)]))
                    {
                        sndChildGene += "---";
                        sndChildNullsIndexes.Add(j);
                    }
                    else
                    {
                        sndChildGene += p1[j..(j+3)];
                    }
                }
            }

            currentSplitIndex = splitsIndexes[i]*3;
        }

        if (splitsNumber-1 % 2 == 0)
        {
            for (int j = currentSplitIndex; j < 24; j += 3)
            {
                if (ContainsGene(fstChildGene,p1[j..(j+3)]))
                {
                    fstChildGene += "---";
                    fstChildNullsIndexes.Add(j);

                }
                else
                {
                    fstChildGene += p1[j..(j+3)];
                }

                if (ContainsGene(sndChildGene,p2[j..(j+3)]))
                {
                    sndChildGene += "---";
                    sndChildNullsIndexes.Add(j);
                }
                else
                {
                    sndChildGene += p2[j..(j+3)];
                }
            }
        }
        else
        {
            for (int j = currentSplitIndex; j < 24; j += 3)
            {
                if (ContainsGene(fstChildGene,p2[j..(j+3)]))
                {
                    fstChildGene += "---";
                    fstChildNullsIndexes.Add(j);
                }
                else
                {
                    fstChildGene += p2[j..(j+3)];
                }

                if (ContainsGene(sndChildGene,p1[j..(j+3)]))
                {
                    sndChildGene += "---";
                    sndChildNullsIndexes.Add(j);
                }
                else
                {
                    sndChildGene += p1[j..(j+3)];
                }
            }
        }

        Debug.Log("Child 1: "+fstChildGene);
        Debug.Log("Child 2: "+sndChildGene);

        foreach (int index in fstChildNullsIndexes)
        {
            StringBuilder fstChildStringBuilder = new StringBuilder(fstChildGene);
            for (int i = 0; i < sndChildGene.Length; i += 3)
            {
                if (!ContainsGene(fstChildGene,sndChildGene[i..(i+3)]))
                {
                    fstChildGene = fstChildStringBuilder.Remove(index,3).Insert(index,sndChildGene[i..(i+3)]).ToString();
                }
            }
        }
        foreach (int index in sndChildNullsIndexes)
        {
            for (int i = 0; i < fstChildGene.Length; i += 3)
            {
                if (!ContainsGene(sndChildGene,fstChildGene[i..(i+3)]))
                {
                    sndChildGene = sndChildGene.Remove(index,3).Insert(index,fstChildGene[i..(i+3)]).ToString();
                }
            }
        }

        for (int i = 0; i < 24; i += 3)
        {
            if (fstChildGene[i] == '-')
            {
                for (int j = 0; j < 24; j += 3)
                {
                    if (!ContainsGene(fstChildGene, p1[j..(j+3)]))
                    {
                        fstChildGene = fstChildGene.Remove(i,3).Insert(i,p1[j..(j+3)]).ToString();
                    }
                }
            }
            if (sndChildGene[i] == '-')
            {
                for (int j = 0; j < 24; j += 3)
                {
                    if (!ContainsGene(sndChildGene, p2[j..(j+3)]))
                    {
                        sndChildGene = sndChildGene.Remove(i,3).Insert(i,p2[j..(j+3)]).ToString();
                    }
                }
            }
        }
        
        Debug.Log("Child 1 (corrected): "+fstChildGene);
        Debug.Log("Child 2 (corrected): "+sndChildGene);

        
        Debug.Log("Converting: "+TapeString(ConvertToIntTape(fstChildGene)));
        Debug.Log("Converting: "+TapeString(ConvertToIntTape(sndChildGene)));
        
    }

    public string ConvertToBinaryTape(int[] tape)
    {
        Debug.Log("Converting: "+TapeString(tape));
        string genTape = "";

        for (int i = 0; i < 8; i++)
        {
            string bitString = Convert.ToString(tape[i], 2).PadLeft(3, '0');

            genTape += bitString;
        }

        return genTape;
    }

    public int[] ConvertToIntTape(string tape)
    {
        int[] genTape = new int[8];

        for (int i = 0; i < tape.Length; i += 3)
        {
            genTape[i/3] = Convert.ToInt32(tape[i..(i+3)],2);
        }

        return genTape;
    }

    string TapeString(int[] tape)
    {
        string tapeOut = "";

        if (tape.Length == 24)
        {
            for (int i = 0; i < 8; i++)
            {
                string bitInt = "";
                for (int j = 0; j < 3; j++)
                {
                    bitInt += tape[i*3+j];
                }
                tapeOut += bitInt+" ";
            }
        }
        else
        {
            for (int i = 0; i < tape.Length; i++)
            {
                tapeOut += tape[i]+" ";
            }

        }
        return tapeOut;
    }

    bool ContainsGene(string genoma, string gene)
    {
        bool contains = false;
        for (int j = 0; j < genoma.Length; j += 3)
        {
            if (Enumerable.SequenceEqual(genoma[j..(j + 3)], gene))
            {
                contains = true;
            }
        }

        return contains;
    }
}
