using System;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class Node
{
    private enum NodeType
    {
        Unassigned,
        Start,
        Puzzle,
        Treasure,
        Encounter,
        MiniBoss,
        Combination,
        Boss,
        End,
    }

    private enum CombinationType
    {
        PuzzleTreasure,
        PuzzleEncounter,
        TreasureEncounter,
        TreasureMiniBoss,
    }

    [field: SerializeField] public Vector2 Position;
    [field: SerializeField] private NodeType _roomType;
    [field: SerializeField] private CombinationType _comboType;
    [field: SerializeField] private string _label;
    [field: SerializeField] private Color _colour;
    private List<Node> ConnectedNodes = new List<Node>();

    public Node(Vector2 position)
    {
        Position = position;
        SetNodeType(0);
        ConnectedNodes = new List<Node>();
    }

    public void ConnectTo(Node otherNode)
    {
        if (!ConnectedNodes.Contains(otherNode))
        {
            ConnectedNodes.Add(otherNode);
            otherNode.ConnectTo(this);
        }
    }

    public void SetNodeType(int NodeTypeNumber)
    {
        _roomType = (NodeType) NodeTypeNumber;
        SetText();
        SetColour();
    }

    public void SetComboType(int ComboNumber)
    {
        _comboType = (CombinationType) ComboNumber;
    }

    public void SetText()
    {
        switch (_roomType)
        {
            case NodeType.Unassigned:
                _label = "U";
                break;
            case NodeType.Start:
                _label = "S";
                break;
            case NodeType.Puzzle:
                _label = "P";
                break;
            case NodeType.Treasure:
                _label = "T";
                break;
            case NodeType.Encounter:
                _label = "E";
                break;
            case NodeType.MiniBoss:
                _label = "M";
                break;
            case NodeType.Combination:
                switch (_comboType)
                {
                    case CombinationType.PuzzleEncounter:
                        _label = "P+E";
                        break;
                    case CombinationType.PuzzleTreasure:
                        _label = "P+T";
                        break;
                    case CombinationType.TreasureEncounter:
                        _label = "T+E";
                        break;
                    case CombinationType.TreasureMiniBoss:
                        _label = "T+M";
                        break;
                }

                break;
            case NodeType.Boss:
                _label = "B";
                break;
            case NodeType.End:
                _label = "X";
                break;
        }
    }

    public void SetColour()
    {
        switch (_roomType)
        {
            case NodeType.Unassigned:
                _colour = Color.white;
                break;
            case NodeType.Start:
                _colour = Color.white;
                break;
            case NodeType.Puzzle:
                _colour = Color.white;
                break;
            case NodeType.Treasure:
                _colour = Color.black;
                break;
            case NodeType.Encounter:
                _colour = Color.black;
                break;
            case NodeType.MiniBoss:
                _colour = Color.white;
                break;
            case NodeType.Combination:
                switch (_comboType)
                {
                    case CombinationType.PuzzleEncounter:
                        _colour = Color.red;
                        break;
                    case CombinationType.PuzzleTreasure:
                        _colour = Color.yellow;
                        break;
                    case CombinationType.TreasureEncounter:
                        _colour = Color.blue;
                        break;
                    case CombinationType.TreasureMiniBoss:
                        _colour = Color.green;
                        break;
                }

                break;
            case NodeType.Boss:
                _colour = Color.black;
                break;
            case NodeType.End:
                _colour = Color.black;
                break;
        }
    }
}
