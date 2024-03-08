using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;
using Random = System.Random;

public static class DrawBlocks
{
    public static int RoundEqualisation = 10;

    // Start is called before the first frame update
    private static List<Material> weightedMaterials = new();
    private static List<Shape> shapes = new();
    private static bool init = false;
    private static int[] weights = { 1, 1, 6, 6, 6 };
    static GameObject[] player;
    private static Random r = new();

    private static Stack<Block>[] bags =
    {
        new(),
        new(),
        new(),
        new()
    };

    private static void Init()
    {
        player = GameObject.FindGameObjectsWithTag("Player");
        Refillbags();
        init = true;
    }

    //Returns the current block as first elemen
    public static Tuple<Block, Block> DrawBlock(int player)
    {
        if (!init)
            Init();

        //Blocks are drawn simultaniesly so if 1 bag is emtpty evey bag is empty
        if (bags[player].Count == 1)
        {
            Block nextBlock = bags[player].Pop();
            Refillbags();
            return new Tuple<Block, Block>(nextBlock, bags[player].Peek());
        }

        if (bags[player].Count == 0)
            Refillbags();
        return new(bags[player].Pop(), bags[player].Peek());
    }

    private static void Refillbags()
    {
        Block b;

        //For each player
        for (int playerCount = 0; playerCount < player.Length; playerCount++)
        {
            bags[playerCount].Clear();
            //For each material
            while (weightedMaterials.Count > 0)
            {
                b = player[playerCount].AddComponent<Block>();
                b.SetMaterial(GetRandomMaterial());
                b.SetShape(GetRandomShape());
                bags[playerCount].Push(b);
            }

            RefillShapes();
        }
      
    }

    private static void RefillShapes()
    {
        for (int i = 0; i < weights.Length; i++)
        for (int j = 0; j < weights[i]; j++)
        {
            weightedMaterials.Add((Material)i);
            shapes.Add((Shape)((i + j) % 5));
        }
    }

    private static Material GetRandomMaterial()
    {
        int number = r.Next(weightedMaterials.Count);
        Material m = weightedMaterials[number];
        weightedMaterials.RemoveAt(number);
        return m;
    }

    private static Shape GetRandomShape()
    {
        int number = r.Next(weightedMaterials.Count);
        Shape s = shapes[number];
        weightedMaterials.RemoveAt(number);
        return s;
    }
}