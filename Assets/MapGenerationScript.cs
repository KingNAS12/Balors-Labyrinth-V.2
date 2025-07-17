using System.Drawing;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static Unity.Collections.AllocatorManager;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System;

public class MapGenerationScript : MonoBehaviour
{
    public GameObject blockedTile; // Used like a pointer to Null, but not Null
    public GameObject[] tilePrefabs = new GameObject[12]; // Contains prefabs of Hallway GameObject. 0 = hor1, 1 = hor2, 2 = vert, 3 = cornWN, 4 = cornWS, 5 = cornES, 6 = cornNE, 7 = 3wayWNE, 8 = 3wayWNS, 9 = 3wayWES, 10 = 3wayNES, 11 = 4way; 
    public BoxCollider2D hallwayBody; 
    public LayerMask bodyLayer;
    public float tileSizeX = 24;
    public float tileSizeY = 24;
    public int stackSize = 0; 

    void Start()
    {
        TileScript tileScript = transform.GetComponent<TileScript>();
        tileScript.CreateNode(null, null, null, blockedTile, "3wayWNE");
        GameObject origin = GameObject.Find("OriginalTile"); 
        GenerationRecursion(origin); 
    }

    public GameObject GenerationRecursion(GameObject recursingNode)
    {
        if (stackSize >= 100)
        {
            stackSize = 0;
            GenerateLoop(recursingNode);
            return recursingNode; 
        }
        else
        {
            stackSize++; 
        }
        TileScript recursingScript = recursingNode.GetComponent<TileScript>();
        if (recursingScript.GetWest() == null)
        {
            GameObject newNode = Generate(recursingNode, "W");
            newNode = GenerationRecursion(newNode);
            recursingScript.SetWest(newNode);
        }
        if (recursingScript.GetNorth() == null)
        {
            GameObject newNode = Generate(recursingNode, "N");
            newNode = GenerationRecursion(newNode);
            recursingScript.SetNorth(newNode);
        }
        if (recursingScript.GetEast() == null)
        {
            GameObject newNode = Generate(recursingNode, "E");
            newNode = GenerationRecursion(newNode);
            recursingScript.SetEast(newNode);
        }
        if (recursingScript.GetSouth() == null)
        {
            GameObject newNode = Generate(recursingNode, "S");
            newNode = GenerationRecursion(newNode);
            recursingScript.SetSouth(newNode);
        }
        return recursingNode;
    }

    public GameObject Generate(GameObject parentNode, string childDirection)
    {
        Component hitCollider = CollisionCheck(childDirection);
        if (hitCollider != null)
        {
            return CreateIntersection(hitCollider.gameObject, parentNode, childDirection); 
        }
        else
        {
            return CreateNewTile(parentNode, childDirection);
        }
    }

    public Component CollisionCheck(string direction)
    {
        RaycastHit2D hit; 
        if (direction == "W")
        {
            hit = Physics2D.BoxCast(new Vector2 ((hallwayBody.bounds.center.x - hallwayBody.size.x), hallwayBody.bounds.center.y), hallwayBody.size, 0, Vector2.left, 0, bodyLayer);
        }
        else if (direction == "N")
        {
            hit = Physics2D.BoxCast(new Vector2(hallwayBody.bounds.center.x, (hallwayBody.bounds.center.y + hallwayBody.size.y)), hallwayBody.size, 0, Vector2.left, 0, bodyLayer);
        }
        else if (direction == "E")
        {
            hit = Physics2D.BoxCast(new Vector2((hallwayBody.bounds.center.x + hallwayBody.size.x), hallwayBody.bounds.center.y), hallwayBody.size, 0, Vector2.left, 0, bodyLayer);
        }
        else // (direction == "S")
        {
            hit = Physics2D.BoxCast(new Vector2(hallwayBody.bounds.center.x, (hallwayBody.bounds.center.y - hallwayBody.size.y)), hallwayBody.size, 0, Vector2.left, 0, bodyLayer);
        }
        return (hit.collider);
    }

    public GameObject CreateIntersection(GameObject hitNode, GameObject parentNode, string childDirection)
    {
        /* <parentNode> is the object that is trying to create a new child/tile at the same position as <hitNode>. 
        <childDirection> is the direction of <hitNode> relative to <parentNode>.
        So convert <hitNode> into a new intersection. */
        if (childDirection == "W")                                 // hitNode parentNode
	    {
            return WestIntersectChecks(hitNode, parentNode);
        }
        else if (childDirection == "N")                            // hitNode
        {                                                          // parentNode
            return NorthIntersectChecks(hitNode, parentNode);
        }
        else if (childDirection == "E")                            // parentNode hitNode
        {
            return EastIntersectChecks(hitNode, parentNode);
        }
        else // (childDirection == "S")                            // parentNode
        {                                                          // hitNode
            return SouthIntersectChecks(hitNode, parentNode);
        }
    }

    public GameObject WestIntersectChecks(GameObject nodeToConvert, GameObject nodeUnchanged)
    {
        /* <nodeUnchanged> is the object that is trying to create a new child/tile at the same position as <nodeToConvert>. 
        <nodeToConvert> is the object that needs to be converted into an intersection. 
        <nodeToConvert> is the same as <hitNode> from CreateIntersection() and <> from ().
        <nodeUnchanged> is the same as <parentNode> from CreateIntersection() and <> from ().
        <nodeUnchanged>'s west = <nodeToConvert> / <nodeToConvert>'s east = <nodeUnchanged> */
        TileScript convertingTileScript = nodeToConvert.GetComponent<TileScript>();
        TileScript unchangingTileScript = nodeUnchanged.GetComponent<TileScript>();
        string convertTile = convertingTileScript.GetName();
        string unchangedTile = unchangingTileScript.GetName();
        GameObject newNode = nodeToConvert;
        if (convertingTileScript.GetEast() != blockedTile)
        {
            return newNode;
        }
        else
        {
            if (convertTile == "vert")
            {
                newNode = Instantiate(tilePrefabs[10], nodeToConvert.transform.position, Quaternion.identity, nodeToConvert.transform.parent);
                TileScript newTileScript = newNode.GetComponent<TileScript>();
                newTileScript.CreateNode(blockedTile, convertingTileScript.GetNorth(), nodeUnchanged, convertingTileScript.GetSouth(), "3wayNES");
            }
            else if (convertTile == "cornWN")
            {
                newNode = Instantiate(tilePrefabs[17], nodeToConvert.transform.position, Quaternion.identity, nodeToConvert.transform.parent);
                TileScript newTileScript = newNode.GetComponent<TileScript>();
                newTileScript.CreateNode(convertingTileScript.GetWest(), convertingTileScript.GetNorth(), nodeUnchanged, blockedTile, "3wayWNE");
            }
            else if (convertTile == "cornWS")
            {
                newNode = Instantiate(tilePrefabs[9], nodeToConvert.transform.position, Quaternion.identity, nodeToConvert.transform.parent);
                TileScript newTileScript = newNode.GetComponent<TileScript>();
                newTileScript.CreateNode(convertingTileScript.GetWest(), convertingTileScript.GetNorth(), nodeUnchanged, blockedTile, "3wayWES");
            }
            else if (convertTile == "3wayWNS")
            {
                newNode = Instantiate(tilePrefabs[11], nodeToConvert.transform.position, Quaternion.identity, nodeToConvert.transform.parent);
                TileScript newTileScript = newNode.GetComponent<TileScript>();
                newTileScript.CreateNode(convertingTileScript.GetWest(), convertingTileScript.GetNorth(), nodeUnchanged, convertingTileScript.GetSouth(), "4way");
            }
            Destroy(nodeToConvert);
            return newNode;
        }
    }

    public GameObject NorthIntersectChecks(GameObject nodeToConvert, GameObject nodeUnchanged)
    {
        /* <nodeUnchanged> is the object that is trying to create a new child/tile at the same position as <nodeToConvert>. 
        <nodeToConvert> is the object that needs to be converted into an intersection. 
        <nodeToConvert> is the same as <hitNode> from CreateIntersection() and <> from ().
        <nodeUnchanged> is the same as <parentNode> from CreateIntersection() and <> from ().
        <nodeUnchanged>'s north = <nodeToConvert> / <nodeToConvert>'s south = <nodeUnchanged> */
        TileScript convertingTileScript = nodeToConvert.GetComponent<TileScript>();
        TileScript unchangingTileScript = nodeUnchanged.GetComponent<TileScript>();
        string convertTile = convertingTileScript.GetName();
        string unchangedTile = unchangingTileScript.GetName();
        GameObject newNode = nodeToConvert;
        if (convertingTileScript.GetSouth() != blockedTile)
        {
            return newNode;
        }
        else
        {
            if (convertTile == "hor")
            {
                newNode = Instantiate(tilePrefabs[9], nodeToConvert.transform.position, Quaternion.identity, nodeToConvert.transform.parent);
                TileScript newTileScript = newNode.GetComponent<TileScript>();
                newTileScript.CreateNode(convertingTileScript.GetWest(), blockedTile, convertingTileScript.GetEast(), nodeUnchanged, "3wayWES");
            }
            else if (convertTile == "cornWN")
            {
                newNode = Instantiate(tilePrefabs[8], nodeToConvert.transform.position, Quaternion.identity, nodeToConvert.transform.parent);
                TileScript newTileScript = newNode.GetComponent<TileScript>();
                newTileScript.CreateNode(convertingTileScript.GetWest(), convertingTileScript.GetNorth(), blockedTile, nodeUnchanged, "3wayWNS");
            }
            else if (convertTile == "cornNE")
            {
                newNode = Instantiate(tilePrefabs[10], nodeToConvert.transform.position, Quaternion.identity, nodeToConvert.transform.parent);
                TileScript newTileScript = newNode.GetComponent<TileScript>();
                newTileScript.CreateNode(blockedTile, convertingTileScript.GetNorth(), convertingTileScript.GetEast(), nodeUnchanged, "3wayNES");
            }
            else if (convertTile == "3wayWNE")
            {
                newNode = Instantiate(tilePrefabs[11], nodeToConvert.transform.position, Quaternion.identity, nodeToConvert.transform.parent);
                TileScript newTileScript = newNode.GetComponent<TileScript>();
                newTileScript.CreateNode(convertingTileScript.GetWest(), convertingTileScript.GetNorth(), convertingTileScript.GetEast(), nodeUnchanged, "4way");
            }
            Destroy(nodeToConvert);
            return newNode;
        }
    }

    public GameObject EastIntersectChecks(GameObject nodeToConvert, GameObject nodeUnchanged)
    {
        /* <nodeUnchanged> is the object that is trying to create a new child/tile at the same position as <nodeToConvert>. 
        <nodeToConvert> is the object that needs to be converted into an intersection. 
        <nodeToConvert> is the same as <hitNode> from CreateIntersection() and <> from ().
        <nodeUnchanged> is the same as <parentNode> from CreateIntersection() and <> from ().
        <nodeUnchanged>'s east = <nodeToConvert> / <nodeToConvert>'s west = <nodeUnchanged> */
        TileScript convertingTileScript = nodeToConvert.GetComponent<TileScript>();
        TileScript unchangingTileScript = nodeUnchanged.GetComponent<TileScript>();
        string convertTile = convertingTileScript.GetName();
        string unchangedTile = unchangingTileScript.GetName();
        GameObject newNode = nodeToConvert;
        if (convertingTileScript.GetWest() != blockedTile)
        {
            return newNode;
        }
        else
        {
            if (convertTile == "vert")
            {
                newNode = Instantiate(tilePrefabs[8], nodeToConvert.transform.position, Quaternion.identity, nodeToConvert.transform.parent);
                TileScript newTileScript = newNode.GetComponent<TileScript>();
                newTileScript.CreateNode(nodeUnchanged, convertingTileScript.GetNorth(), blockedTile, convertingTileScript.GetSouth(), "3wayWNS");
            }
            else if (convertTile == "cornNE")
            {
                newNode = Instantiate(tilePrefabs[7], nodeToConvert.transform.position, Quaternion.identity, nodeToConvert.transform.parent);
                TileScript newTileScript = newNode.GetComponent<TileScript>();
                newTileScript.CreateNode(nodeUnchanged, convertingTileScript.GetNorth(), convertingTileScript.GetEast(), blockedTile, "3wayWNE");
            }
            else if (convertTile == "cornES")
            {
                newNode = Instantiate(tilePrefabs[9], nodeToConvert.transform.position, Quaternion.identity, nodeToConvert.transform.parent);
                TileScript newTileScript = newNode.GetComponent<TileScript>();
                newTileScript.CreateNode(nodeUnchanged, blockedTile, convertingTileScript.GetEast(), convertingTileScript.GetSouth(), "3wayWES");
            }
            else if (convertTile == "3wayNES")
            {
                newNode = Instantiate(tilePrefabs[11], nodeToConvert.transform.position, Quaternion.identity, nodeToConvert.transform.parent);
                TileScript newTileScript = newNode.GetComponent<TileScript>();
                newTileScript.CreateNode(nodeUnchanged, blockedTile, convertingTileScript.GetEast(), convertingTileScript.GetSouth(), "4way");
            }
            Destroy(nodeToConvert);
            return newNode;
        }
    }

    public GameObject SouthIntersectChecks(GameObject nodeToConvert, GameObject nodeUnchanged)
    {
        /* <nodeUnchanged> is the object that is trying to create a new child/tile at the same position as <nodeToConvert>. 
        <nodeToConvert> is the object that needs to be converted into an intersection. 
        <nodeToConvert> is the same as <hitNode> from CreateIntersection() and <> from ().
        <nodeUnchanged> is the same as <parentNode> from CreateIntersection() and <> from ().
        <nodeUnchanged>'s south = <nodeToConvert> / <nodeToConvert>'s north = <nodeUnchanged> */
        TileScript convertingTileScript = nodeToConvert.GetComponent<TileScript>();
        TileScript unchangingTileScript = nodeUnchanged.GetComponent<TileScript>();
        string convertTile = convertingTileScript.GetName();
        string unchangedTile = unchangingTileScript.GetName();
        GameObject newNode = nodeToConvert;
        if (convertingTileScript.GetNorth() != blockedTile)
        {
            return newNode;
        }
        else
        {
            if (convertTile == "hor")
            {
                newNode = Instantiate(tilePrefabs[7], nodeToConvert.transform.position, Quaternion.identity, nodeToConvert.transform.parent);
                TileScript newTileScript = newNode.GetComponent<TileScript>();
                newTileScript.CreateNode(convertingTileScript.GetWest(), nodeUnchanged, convertingTileScript.GetEast(), blockedTile, "3wayWNE");
            }
            else if (convertTile == "cornWS")
            {
                newNode = Instantiate(tilePrefabs[8], nodeToConvert.transform.position, Quaternion.identity, nodeToConvert.transform.parent);
                TileScript newTileScript = newNode.GetComponent<TileScript>();
                newTileScript.CreateNode(convertingTileScript.GetWest(), nodeUnchanged, blockedTile, convertingTileScript.GetSouth(), "3wayWNS");
            }
            else if (convertTile == "cornES")
            {
                newNode = Instantiate(tilePrefabs[10], nodeToConvert.transform.position, Quaternion.identity, nodeToConvert.transform.parent);
                TileScript newTileScript = newNode.GetComponent<TileScript>();
                newTileScript.CreateNode(blockedTile, nodeUnchanged, convertingTileScript.GetEast(), convertingTileScript.GetSouth(), "3wayNES");
            }
            else if (convertTile == "3wayWES")
            {
                newNode = Instantiate(tilePrefabs[11], nodeToConvert.transform.position, Quaternion.identity, nodeToConvert.transform.parent);
                TileScript newTileScript = newNode.GetComponent<TileScript>();
                newTileScript.CreateNode(convertingTileScript.GetWest(), nodeUnchanged, convertingTileScript.GetEast(), convertingTileScript.GetSouth(), "4way");
            }
            Destroy(nodeToConvert);
            return newNode;
        }
    }

    public GameObject CreateNewTile(GameObject parentNode, string childDirection)
    {
        int a = UnityEngine.Random.Range(0, 100);
        GameObject newNode = parentNode; // This is just to initialize it. <newNode> will not stay <parentNode>. 
        if (a < 40) // 40% chance to create a straight path. 
        {
            if (childDirection == "W" || childDirection == "E")
            {
                int version = 0; // There are 2 versions of the horizontal hallway. 50/50 chance for each. 
                if (a >= 20)
                {
                    version = 1;
                }
                if (childDirection == "W")
                {
                    newNode = Instantiate(tilePrefabs[version], new Vector3(parentNode.transform.position.x + tileSizeX, parentNode.transform.position.y, parentNode.transform.position.z), Quaternion.identity, parentNode.transform.parent);
                    TileScript newTileScript = newNode.GetComponent<TileScript>();
                    newTileScript.CreateNode(null, blockedTile, parentNode, blockedTile, "hor");
                }
                else // if (childDirection == "E")
                {
                    newNode = Instantiate(tilePrefabs[version], new Vector3(parentNode.transform.position.x - tileSizeX, parentNode.transform.position.y, parentNode.transform.position.z), Quaternion.identity, parentNode.transform.parent);
                    TileScript newTileScript = newNode.GetComponent<TileScript>();
                    newTileScript.CreateNode(parentNode, blockedTile, null, blockedTile, "hor");
                }
            }
            else // if (childDirection == "N" || childDirection == "S")
            {
                if (childDirection == "N")
                {
                    newNode = Instantiate(tilePrefabs[2], new Vector3(parentNode.transform.position.x, parentNode.transform.position.y - tileSizeY, parentNode.transform.position.z), Quaternion.identity, parentNode.transform.parent);
                    TileScript newTileScript = newNode.GetComponent<TileScript>();
                    newTileScript.CreateNode(null, blockedTile, parentNode, blockedTile, "vert");
                }
                else // if (childDirection == "S")
                {
                    newNode = Instantiate(tilePrefabs[2], new Vector3(parentNode.transform.position.x, parentNode.transform.position.y + tileSizeY, parentNode.transform.position.z), Quaternion.identity, parentNode.transform.parent);
                    TileScript newTileScript = newNode.GetComponent<TileScript>();
                    newTileScript.CreateNode(parentNode, blockedTile, null, blockedTile, "vert");
                }
            }
        }
        // 26% chance to create a corner. 50/50 chance for direction. 
        else if (40 <= a && a < 53) 
        {
            if (childDirection == "E")
            {
                newNode = Instantiate(tilePrefabs[3], new Vector3(parentNode.transform.position.x + tileSizeX, parentNode.transform.position.y, parentNode.transform.position.z), Quaternion.identity, parentNode.transform.parent);
                TileScript newTileScript = newNode.GetComponent<TileScript>();
                newTileScript.CreateNode(parentNode, null, blockedTile, blockedTile, "cornWN");
            }
            else if (childDirection == "S")
            {
                newNode = Instantiate(tilePrefabs[3], new Vector3(parentNode.transform.position.x, parentNode.transform.position.y + tileSizeY, parentNode.transform.position.z), Quaternion.identity, parentNode.transform.parent);
                TileScript newTileScript = newNode.GetComponent<TileScript>();
                newTileScript.CreateNode(null, parentNode, blockedTile, blockedTile, "cornWN");
            }
            else if (childDirection == "W")
            {
                newNode = Instantiate(tilePrefabs[5], new Vector3(parentNode.transform.position.x - tileSizeX, parentNode.transform.position.y, parentNode.transform.position.z), Quaternion.identity, parentNode.transform.parent);
                TileScript newTileScript = newNode.GetComponent<TileScript>();
                newTileScript.CreateNode(blockedTile, blockedTile, parentNode, null, "cornES");
            }
            else // if (childDirection == "N")
            {
                newNode = Instantiate(tilePrefabs[5], new Vector3(parentNode.transform.position.x, parentNode.transform.position.y - tileSizeY, parentNode.transform.position.z), Quaternion.identity, parentNode.transform.parent);
                TileScript newTileScript = newNode.GetComponent<TileScript>();
                newTileScript.CreateNode(blockedTile, blockedTile, null, parentNode, "cornES");
            }
        }
        else if (53 <= a && a <= 65) // 13% chance to create a different corner. 
        {
            if (childDirection == "E")
            {
                newNode = Instantiate(tilePrefabs[4], new Vector3(parentNode.transform.position.x + tileSizeX, parentNode.transform.position.y, parentNode.transform.position.z), Quaternion.identity, parentNode.transform.parent);
                TileScript newTileScript = newNode.GetComponent<TileScript>();
                newTileScript.CreateNode(parentNode, blockedTile, blockedTile, null, "cornWS");
            }
            else if (childDirection == "N")
            {
                newNode = Instantiate(tilePrefabs[4], new Vector3(parentNode.transform.position.x, parentNode.transform.position.y - tileSizeY, parentNode.transform.position.z), Quaternion.identity, parentNode.transform.parent);
                TileScript newTileScript = newNode.GetComponent<TileScript>();
                newTileScript.CreateNode(null, blockedTile, blockedTile, parentNode, "cornWS");
            }
            else if (childDirection == "W")
            {
                newNode = Instantiate(tilePrefabs[6], new Vector3(parentNode.transform.position.x - tileSizeX, parentNode.transform.position.y, parentNode.transform.position.z), Quaternion.identity, parentNode.transform.parent);
                TileScript newTileScript = newNode.GetComponent<TileScript>();
                newTileScript.CreateNode(blockedTile, null, blockedTile, parentNode, "cornNE");
            }
            else // if (childDirection == "S")
            {
                newNode = Instantiate(tilePrefabs[6], new Vector3(parentNode.transform.position.x, parentNode.transform.position.y + tileSizeY, parentNode.transform.position.z), Quaternion.identity, parentNode.transform.parent);
                TileScript newTileScript = newNode.GetComponent<TileScript>();
                newTileScript.CreateNode(blockedTile, parentNode, blockedTile, null, "cornNE");
            }
        }
        if (65 < a && a <= 90) // 25% chance of 3 way intersection. 1/3 chance for direction. 
        {
            int b = UnityEngine.Random.Range(0, 3);
            if (childDirection == "W")
            {
                if (b == 0)
                {
                    newNode = Instantiate(tilePrefabs[7], new Vector3(parentNode.transform.position.x, parentNode.transform.position.y + tileSizeY, parentNode.transform.position.z), Quaternion.identity, parentNode.transform.parent);
                    TileScript newTileScript = newNode.GetComponent<TileScript>();
                    newTileScript.CreateNode(null, null, parentNode, blockedTile, "3wayWNE");
                }
                else if (b == 1)
                {
                    newNode = Instantiate(tilePrefabs[9], new Vector3(parentNode.transform.position.x, parentNode.transform.position.y + tileSizeY, parentNode.transform.position.z), Quaternion.identity, parentNode.transform.parent);
                    TileScript newTileScript = newNode.GetComponent<TileScript>();
                    newTileScript.CreateNode(null, blockedTile, parentNode, null, "3wayWES");
                }
                else
                {
                    newNode = Instantiate(tilePrefabs[10], new Vector3(parentNode.transform.position.x, parentNode.transform.position.y + tileSizeY, parentNode.transform.position.z), Quaternion.identity, parentNode.transform.parent);
                    TileScript newTileScript = newNode.GetComponent<TileScript>();
                    newTileScript.CreateNode(blockedTile, null, parentNode, null, "3wayNES");
                }
            }
            else if (childDirection == "N")
            {
                if (b == 0)
                {
                    newNode = Instantiate(tilePrefabs[8], new Vector3(parentNode.transform.position.x, parentNode.transform.position.y + tileSizeY, parentNode.transform.position.z), Quaternion.identity, parentNode.transform.parent);
                    TileScript newTileScript = newNode.GetComponent<TileScript>();
                    newTileScript.CreateNode(null, null, blockedTile, parentNode, "3wayWNS");
                }
                else if (b == 1)
                {
                    newNode = Instantiate(tilePrefabs[9], new Vector3(parentNode.transform.position.x, parentNode.transform.position.y + tileSizeY, parentNode.transform.position.z), Quaternion.identity, parentNode.transform.parent);
                    TileScript newTileScript = newNode.GetComponent<TileScript>();
                    newTileScript.CreateNode(null, blockedTile, null, parentNode, "3wayWES");
                }
                else
                {
                    newNode = Instantiate(tilePrefabs[10], new Vector3(parentNode.transform.position.x, parentNode.transform.position.y + tileSizeY, parentNode.transform.position.z), Quaternion.identity, parentNode.transform.parent);
                    TileScript newTileScript = newNode.GetComponent<TileScript>();
                    newTileScript.CreateNode(blockedTile, null, null, parentNode, "3wayNES");
                }
            }
            else if (childDirection == "E")
            {
                if (b == 0)
                {
                    newNode = Instantiate(tilePrefabs[7], new Vector3(parentNode.transform.position.x, parentNode.transform.position.y + tileSizeY, parentNode.transform.position.z), Quaternion.identity, parentNode.transform.parent);
                    TileScript newTileScript = newNode.GetComponent<TileScript>();
                    newTileScript.CreateNode(parentNode, null, null, blockedTile, "3wayWNE");
                }
                else if (b == 1)
                {
                    newNode = Instantiate(tilePrefabs[8], new Vector3(parentNode.transform.position.x, parentNode.transform.position.y + tileSizeY, parentNode.transform.position.z), Quaternion.identity, parentNode.transform.parent);
                    TileScript newTileScript = newNode.GetComponent<TileScript>();
                    newTileScript.CreateNode(parentNode, null, blockedTile, null, "3wayWNS");
                }
                else
                {
                    newNode = Instantiate(tilePrefabs[9], new Vector3(parentNode.transform.position.x, parentNode.transform.position.y + tileSizeY, parentNode.transform.position.z), Quaternion.identity, parentNode.transform.parent);
                    TileScript newTileScript = newNode.GetComponent<TileScript>();
                    newTileScript.CreateNode(parentNode, blockedTile, null, null, "3wayWES");
                }
            }
            else // if (childDirection == "S")
            {
                if (b == 0)
                {
                    newNode = Instantiate(tilePrefabs[7], new Vector3(parentNode.transform.position.x, parentNode.transform.position.y + tileSizeY, parentNode.transform.position.z), Quaternion.identity, parentNode.transform.parent);
                    TileScript newTileScript = newNode.GetComponent<TileScript>();
                    newTileScript.CreateNode(null, parentNode, blockedTile, null, "3wayWNE");
                }
                else if (b == 1)
                {
                    newNode = Instantiate(tilePrefabs[8], new Vector3(parentNode.transform.position.x, parentNode.transform.position.y + tileSizeY, parentNode.transform.position.z), Quaternion.identity, parentNode.transform.parent);
                    TileScript newTileScript = newNode.GetComponent<TileScript>();
                    newTileScript.CreateNode(null, parentNode, blockedTile, null, "3wayWNS");
                }
                else
                {
                    newNode = Instantiate(tilePrefabs[10], new Vector3(parentNode.transform.position.x, parentNode.transform.position.y + tileSizeY, parentNode.transform.position.z), Quaternion.identity, parentNode.transform.parent);
                    TileScript newTileScript = newNode.GetComponent<TileScript>();
                    newTileScript.CreateNode(blockedTile, parentNode, null, null, "3wayNES");
                }
            }
        }
        else // if (a >= 90); 10% chance of creating a 4 way intersection. 
        {
            if (childDirection == "W")
            {
                newNode = Instantiate(tilePrefabs[11], new Vector3(parentNode.transform.position.x - tileSizeX, parentNode.transform.position.y, parentNode.transform.position.z), Quaternion.identity, parentNode.transform.parent);
                TileScript newTileScript = newNode.GetComponent<TileScript>();
                newTileScript.CreateNode(null, null, parentNode, null, "4way");
            }
            else if (childDirection == "N")
            {
                newNode = Instantiate(tilePrefabs[11], new Vector3(parentNode.transform.position.x, parentNode.transform.position.y - tileSizeY, parentNode.transform.position.z), Quaternion.identity, parentNode.transform.parent);
                TileScript newTileScript = newNode.GetComponent<TileScript>();
                newTileScript.CreateNode(null, null, null, parentNode, "4way");
            }
            else if (childDirection == "E")
            {
                newNode = Instantiate(tilePrefabs[11], new Vector3(parentNode.transform.position.x + tileSizeX, parentNode.transform.position.y, parentNode.transform.position.z), Quaternion.identity, parentNode.transform.parent);
                TileScript newTileScript = newNode.GetComponent<TileScript>();
                newTileScript.CreateNode(parentNode, null, null, null, "4way");
            }
            else // if (childDirection == "S")
            {
                newNode = Instantiate(tilePrefabs[11], new Vector3(parentNode.transform.position.x, parentNode.transform.position.y + tileSizeY, parentNode.transform.position.z), Quaternion.identity, parentNode.transform.parent);
                TileScript newTileScript = newNode.GetComponent<TileScript>();
                newTileScript.CreateNode(null, parentNode, null, null, "4way");
            }
        }
        return newNode; 
    }

    public void GenerateLoop(GameObject endNode)
    {
        TileScript endTileScript = endNode.GetComponent<TileScript>(); // 3-6
        int i = 0; 
        if (endTileScript.GetEast() == null)
        {
            i = 3; // Create cornWN
        }
        else if (endTileScript.GetNorth() == null)
        {
            i = 4; // create cornWS
        }
        else if (endTileScript.GetWest() == null)
        {
            i = 5; // create cornES
        }
        else // if (endTileScript.GetSouth() == null)
        {
            i = 6; // create cornNE
        }
        CreateLoop(endNode, i, endNode); 
    }

    public void CreateLoop(GameObject lastNode, int i, GameObject endNode)
    {
        GameObject newNode = lastNode; // This is just to initialize it. 
        TileScript lastTileScript = lastNode.GetComponent<TileScript>();
        TileScript endTileScript = newNode.GetComponent<TileScript>();
        if (i == 3)
        {
            Component hitCollider = CollisionCheck("E"); 
            if (hitCollider != null)
            {
                if (hitCollider.gameObject == endNode)
                {
                    endTileScript.SetWest(lastNode); 
                    return;
                }
                else
                {
                    newNode = CreateIntersection(hitCollider.gameObject, lastNode, "E"); 
                    lastTileScript.SetEast(newNode);
                    return;
                }
            }
            else
            {
                newNode = Instantiate(tilePrefabs[3], new Vector3(lastNode.transform.position.x + tileSizeX, lastNode.transform.position.y, lastNode.transform.position.z), Quaternion.identity, lastNode.transform.parent);
                TileScript newTileScript = newNode.GetComponent<TileScript>();
                newTileScript.CreateNode(lastNode, null, blockedTile, blockedTile, "cornWN");
                lastTileScript.SetEast(newNode); 
            }
        }
        else if (i == 4)
        {
            Component hitCollider = CollisionCheck("N");
            if (hitCollider != null)
            {
                if (hitCollider.gameObject == endNode)
                {
                    endTileScript.SetSouth(lastNode);
                    return;
                }
                else
                {
                    newNode = CreateIntersection(hitCollider.gameObject, lastNode, "N");
                    lastTileScript.SetNorth(newNode);
                    return;
                }
            }
            else
            {
                newNode = Instantiate(tilePrefabs[4], new Vector3(lastNode.transform.position.x, lastNode.transform.position.y - tileSizeY, lastNode.transform.position.z), Quaternion.identity, lastNode.transform.parent);
                TileScript newTileScript = newNode.GetComponent<TileScript>();
                newTileScript.CreateNode(null, blockedTile, blockedTile, lastNode, "cornWS");
                lastTileScript.SetNorth(newNode);
            }
        }
        else if (i == 5)
        {
            Component hitCollider = CollisionCheck("W");
            if (hitCollider != null)
            {
                if (hitCollider.gameObject == endNode)
                {
                    endTileScript.SetEast(lastNode);
                    return;
                }
                else
                {
                    newNode = CreateIntersection(hitCollider.gameObject, lastNode, "W");
                    lastTileScript.SetWest(newNode);
                    return;
                }
            }
            else
            {
                newNode = Instantiate(tilePrefabs[5], new Vector3(lastNode.transform.position.x - tileSizeX, lastNode.transform.position.y, lastNode.transform.position.z), Quaternion.identity, lastNode.transform.parent);
                TileScript newTileScript = newNode.GetComponent<TileScript>();
                newTileScript.CreateNode(blockedTile, blockedTile, null, lastNode, "cornES");
                lastTileScript.SetWest(newNode);
            }
        }
        else // if (i == 6)
        {
            Component hitCollider = CollisionCheck("S");
            if (hitCollider != null)
            {
                if (hitCollider.gameObject == endNode)
                {
                    endTileScript.SetNorth(lastNode);
                    return;
                }
                else
                {
                    newNode = CreateIntersection(hitCollider.gameObject, lastNode, "S");
                    lastTileScript.SetSouth(newNode);
                    return;
                }
            }
            else
            {
                newNode = Instantiate(tilePrefabs[5], new Vector3(lastNode.transform.position.x - tileSizeX, lastNode.transform.position.y, lastNode.transform.position.z), Quaternion.identity, lastNode.transform.parent);
                TileScript newTileScript = newNode.GetComponent<TileScript>();
                newTileScript.CreateNode(blockedTile, lastNode, null, blockedTile, "cornNE");
                lastTileScript.SetWest(newNode);
            }
        }
        i++;
        if (i == 7)
        {
            i = 3; 
        }
        CreateLoop(newNode, i, endNode); 
    }









/*

    SpawnerSpawner()

public list[GameObjects] BattlePrefabs = new list(8);
    public list[GameObjects] BattleArenaPrefabs = new list(4);
    public int createdArenas = 0;

    void start()
    {
        Recurse(transform);
        if (createdArenas != 8)
        {

        }
    }

    void recurse(node)
    {
        List intersects = check(node, 1);
        intersects.addRange(check(node, 2));
        for (int i = 0; I < intersects.count; i++)
        {
            if (intersect[i].isChecked == false)
            {
                Recurse(intersect[i]);
            }
        }
    }

    void Check(node, i)
    {
        intersects = []
    while (nodeGetTile(i) != blocked)
        {
            node = node.GetTile(i);
        }
        CreateBattleArena(node);
        count = 1;
        while (node.GetTile(i + 2) != blocked)
        {
            count++;
            node = node.GetTile(i + 2); ;
        }
        CreateBattleArena(node);
        if (count >= 3)
        {
            if (i == 0)
            {
                CreateHorSpawn(node, count);
            }
            else
            {
                CreateVertSpawn(node, count);
            }
        }
    }

    public void CreateBattleArena(node) // default created below map. Telerport to arena. 
    {
        a = math.random(50);
        backgrounds = ((8 - createdArenas) * 6) + 2;
        if (b <= a)
        {
            if (node.West == blocked)
            {
                battleArena = Instantiate(BattleArenaPrefabs[0], blah blah blah);
                EastIntersectChecks(node, battlArena); ;
            }
            else if (node.North == blocked)
            {
                battleArena = Instantiate(BattleArenaPrefabs[1], blah blah blah);
                SouthIntersectChecks(node, battleArena);
            }
            else if (node.East == blocked)
            {
                battleArena = Instantiate(BattleArenaPrefabs[2], blah blah blah);
                WestIntersectChecks(node, battleArena);
            }
            if (node.South == blocked)
            {
                battleArena = Instantiate(BattleArenaPrefabs[3], blah blah blah);
                NorthIntersectChecks(node, battleArena);
            }
            Instantiate(BattlePrefabs[createdArenas], battleArena);
            createdArenas++;
        }

    }

    public void CreateHorSpawn(node, size); 
{
}

public void CreateVertSpawn(node, size);
{
}
*/
}