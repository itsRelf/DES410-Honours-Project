using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Node : MonoBehaviour
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

    [field: SerializeField] private NodeType _roomType;
    [field: SerializeField] private CombinationType _comboType;
    [field: SerializeField] private TextMeshPro _lettering;

    [field: SerializeField] private SpriteRenderer _sprite;

    // Start is called before the first frame update
    void Start()
    {
        SetText();
        SetColour();
    }

    // Update is called once per frame
    void Update()
    {
        
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
                _lettering.text = "U";
                break;
            case NodeType.Start:
                _lettering.text = "S";
                break;
            case NodeType.Puzzle:
                _lettering.text = "P";
                break;
            case NodeType.Treasure:
                _lettering.text = "T";
                break;
            case NodeType.Encounter:
                _lettering.text = "E";
                break;
            case NodeType.MiniBoss:
                _lettering.text = "M";
                break;
            case NodeType.Combination:
                switch (_comboType)
                {
                    case CombinationType.PuzzleEncounter:
                        _lettering.text = "P+E";
                        break;
                    case CombinationType.PuzzleTreasure:
                        _lettering.text = "P+T";
                        break;
                    case CombinationType.TreasureEncounter:
                        _lettering.text = "T+E";
                        break;
                    case CombinationType.TreasureMiniBoss:
                        _lettering.text = "T+M";
                        break;
                }

                break;
            case NodeType.Boss:
                _lettering.text = "B";
                break;
            case NodeType.End:
                _lettering.text = "X";
                break;
        }
    }

    public void SetColour()
    {
        switch (_roomType)
        {
            case NodeType.Unassigned:
                _sprite.color = Color.black;
                _lettering.color = Color.white;
                break;
            case NodeType.Start:
                _sprite.color = Color.blue;
                _lettering.color = Color.white; 
                break;
            case NodeType.Puzzle:
                _sprite.color = Color.cyan;
                _lettering.color = Color.white;
                break;
            case NodeType.Treasure:
                _sprite.color = Color.yellow;
                _lettering.color = Color.black;
                break;
            case NodeType.Encounter:
                _sprite.color = Color.white;
                _lettering.color = Color.black;
                break;
            case NodeType.MiniBoss:
                _sprite.color = Color.magenta;
                _lettering.color = Color.white;
                break;
            case NodeType.Combination:
                switch (_comboType)
                {
                    case CombinationType.PuzzleEncounter:
                        _sprite.color = Color.gray;
                        _lettering.color = Color.red;
                        break;
                    case CombinationType.PuzzleTreasure:
                        _sprite.color = Color.gray;
                        _lettering.color = Color.yellow;
                        break;
                    case CombinationType.TreasureEncounter:
                        _sprite.color = Color.gray;
                        _lettering.color = Color.blue;
                        break;
                    case CombinationType.TreasureMiniBoss:
                        _sprite.color = Color.gray;
                        _lettering.color = Color.green;
                        break;
                }

                break;
            case NodeType.Boss:
                _sprite.color = Color.red;
                _lettering.color = Color.black;
                break;
            case NodeType.End:
                _sprite.color = Color.green;
                _lettering.color = Color.black;
                break;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position + Vector3.up * 0.25f, 0.1f);
        Gizmos.DrawSphere(transform.position + Vector3.down * 0.25f, 0.1f);
        Gizmos.DrawSphere(transform.position + Vector3.left * 0.25f, 0.1f);
        Gizmos.DrawSphere(transform.position + Vector3.right * 0.25f, 0.1f);
    }
}
