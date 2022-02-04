using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class AIBrain {

    public Board Board;
    public PlayerColor Player;
    public int DepthSearch;
    
    private List<Tuple<int, Node>> Nodes = new List<Tuple<int, Node>>();

    public AIBrain(Board board, PlayerColor player, int depthSearch) {
        Board = board;
        Player = player;
        DepthSearch = depthSearch;
    }

    public void Think() {
        Nodes.Clear();
        float startingTime = Time.realtimeSinceStartup;
        foreach (Piece availablePiece in Board.AvailablePieces(Player)) {
            foreach (Coordinate availableMove in availablePiece.AvailableMoves(Board)) {
                Node node = new Node(Board, Player, Player, availablePiece.CurrentCoordinate, availableMove);
                
                int value = MinMax(node, DepthSearch, false);
                
                Nodes.Add(new Tuple<int, Node>(value, node));
            }
        }
        Debug.Log("Reflexion took about : " + (Time.realtimeSinceStartup - startingTime) + " seconds");
    }

    public void Act() {
        if (Nodes.Count == 0) throw new Exception("MinMax results is empty");
        int bestValue = Nodes.Max(node => node.Item1);
        Nodes.RemoveAll(node => node.Item1 < bestValue);
        Tuple<int, Node> selectedTuple = Nodes[Random.Range(0, Nodes.Count)];
        Board.GetPiece(selectedTuple.Item2.MoveOrigin).ExecuteMove(Board, selectedTuple.Item2.MoveDestination);
    }
    
    private int MinMax(Node node, int depth, bool isMax)
    {
        if (depth == DepthSearch || node.IsTerminal) return node.HeuristicValue;

        int value;
        
        if (isMax)
        {
            value = -1000000;
            foreach (var child in node.Children)
            {
                value = Mathf.Max(value, MinMax(child, depth + 1, false));
            }
        }
        else
        {
            value = +1000000;
            foreach (var child in node.Children)
            {
                value = Mathf.Max(value, MinMax(child, depth + 1, true));
            }
        }
        return value;
    }

    /*private int MinMaxAlphaBeta(Node node, int alpha, int beta)
    {
        if (node.IsTerminal) return node.HeuristicValue;
        else if (node)
        {
            int v = 10000;
            foreach (var child in node.Children)
            {
                v = Mathf.Min(v, MinMaxAlphaBeta(child, alpha, beta));
                if (alpha >= v)
                {
                    return v;
                }
                beta = Mathf.Min(beta, v);
            }
        }
        else
        {
            int v = -100000;
            foreach (var child in node.Children)
            {
                v = Mathf.Max(v, MinMaxAlphaBeta(child, alpha, beta));
                if (beta <= v)
                {
                    return v;
                }
                alpha = Mathf.Max(alpha, v);
            }
        }
        return v;
    }*/

    private int NegaMax(Node node, int depth, PlayerColor color)
    {
        if (depth == 0 || node.IsTerminal)
        {
            if (color == PlayerColor.Black) return node.HeuristicValue;
            if (color == PlayerColor.White) return -node.HeuristicValue;
        }

        int value = -1000000;

        foreach (var child in node.Children)
        {
            value = Mathf.Max(value, -NegaMax(child, depth - 1, color));
        }

        return value;
    }

    private int NegaMaxAlphaBeta(Node node, int i)
    {            
        if (node.IsTerminal) return node.HeuristicValue;
        else
        {
            int j = -1000000;
            foreach (var child in node.Children)
            {
                j = Mathf.Max(j, NegaMaxAlphaBeta(child, j));
                if (-j < i) return -j;
            }
            return -j;
        }
    }
    
}