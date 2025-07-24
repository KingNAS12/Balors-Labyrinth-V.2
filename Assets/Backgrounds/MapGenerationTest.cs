using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;
using static Unity.Collections.AllocatorManager;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class MapGenerationTest : MonoBehaviour
{
    public GameObject blockedTile; // Used like a pointer to Null, but not Null. Shows that hallway is blocked. 
    public GameObject[] tilePrefabs = new GameObject[12]; // Contains prefabs of Hallway GameObject. 0 = hor1, 1 = hor2, 2 = vert, 3 = cornWN, 4 = cornWS, 5 = cornES, 6 = cornNE, 7 = 3wayWNE, 8 = 3wayWNS, 9 = 3wayWES, 10 = 3wayNES, 11 = 4way; 
    public GameObject origin;
    public LayerMask bodyLayer;
    public float tileSizeX = 24;
    public float tileSizeY = 24;
    public int stackSize = 0;
    public int stackLimit = 500;
    public int created = 0; 

    void Start()
    {
        origin = transform.gameObject;
        TileScript originTileScript = transform.GetComponent<TileScript>();
        originTileScript.CreateNode(null, null, null, blockedTile, "3wayWNE", 0, created);
        //GenerateLoop(origin);
        GenerationRecursion(origin, 0);
    }

    public void GenerationRecursion(GameObject recursingNode, int i)
    {
        // Check if the recursing node has any empty links. If yes, generate a new tile there and then recurse on that new node before checking the next link. 
        // Stop recursing once all links are not null. 
        print(i + " " + recursingNode); 
        TileScript recursingTileScript = recursingNode.GetComponent<TileScript>();
        bool newWest = false;
        bool newNorth = false;
        bool newEast = false;
        bool newSouth = false;
        if (i <= 10)
        {
            i++;
            if (recursingTileScript.GetWest() == null)
            {
                GameObject newNode = Generate(recursingNode, "W", i);
                newWest = true; 
            }
            if (recursingTileScript.GetNorth() == null)
            {
                GameObject newNode = Generate(recursingNode, "N", i);
                newNorth = true; 
            }
            if (recursingTileScript.GetEast() == null)
            {
                GameObject newNode = Generate(recursingNode, "E", i);
                newEast = true; 
            }
            if (recursingTileScript.GetSouth() == null)
            {
                GameObject newNode = Generate(recursingNode, "S", i);
                newSouth = true; 
            }
            if (newWest)
            {
                GenerationRecursion(recursingTileScript.GetWest(), i);
            }
            if (newNorth)
            {
                GenerationRecursion(recursingTileScript.GetNorth(), i);
            }
            if (newEast)
            {
                GenerationRecursion(recursingTileScript.GetEast(), i);
            }
            if (newSouth)
            {
                GenerationRecursion(recursingTileScript.GetSouth(), i);
            }

        }
        else
        {
            i--; 
            if (recursingTileScript.GetWest() == null)
            {
                recursingTileScript.SetWest(blockedTile); 
            }
            if (recursingTileScript.GetNorth() == null)
            {
                recursingTileScript.SetNorth(blockedTile);
            }
            if (recursingTileScript.GetEast() == null)
            {
                recursingTileScript.SetEast(blockedTile); 
            }
            if (recursingTileScript.GetSouth() == null)
            {
                recursingTileScript.SetSouth(blockedTile);
            }
        }
    }

    public GameObject Generate(GameObject parentNode, string childDirection, int i)
    {
        Component hitCollider = CollisionCheck(parentNode, childDirection); // First check if you are trying to create a new node where another node already exists. 
        if (hitCollider != null)
        {
            return CreateIntersection(hitCollider.gameObject, parentNode, childDirection, i); // If yes, then convert that tile into an intersection. 
        }
        else
        {
            return CreateNewTile(parentNode, childDirection, i); // If no, then create a new random tile. 
        }
    }

    public Component CollisionCheck(GameObject parentNode, string childDirection)
    {
        BoxCollider2D hallwayBody = parentNode.GetComponent<BoxCollider2D>(); 
        RaycastHit2D hit;
        if (childDirection == "W")
        {
            hit = Physics2D.BoxCast(new Vector2((hallwayBody.bounds.center.x - tileSizeX), hallwayBody.bounds.center.y), hallwayBody.size, 0, Vector2.left, 0, bodyLayer);
        }
        else if (childDirection == "N")
        {
            hit = Physics2D.BoxCast(new Vector2(hallwayBody.bounds.center.x, (hallwayBody.bounds.center.y + tileSizeY)), hallwayBody.size, 0, Vector2.left, 0, bodyLayer);
        }
        else if (childDirection == "E")
        {
            hit = Physics2D.BoxCast(new Vector2((hallwayBody.bounds.center.x + tileSizeX), hallwayBody.bounds.center.y), hallwayBody.size, 0, Vector2.left, 0, bodyLayer);
        }
        else // (childDirection == "S")
        {
            hit = Physics2D.BoxCast(new Vector2(hallwayBody.bounds.center.x, (hallwayBody.bounds.center.y - tileSizeY)), hallwayBody.size, 0, Vector2.left, 0, bodyLayer);
        }
        return (hit.collider);
    }

    public GameObject CreateIntersection(GameObject hitNode, GameObject parentNode, string childDirection, int i)
    {
        /* <parentNode> is the object that is trying to create a new child/tile at the same position as <hitNode>. 
        <childDirection> is the direction of <hitNode> relative to <parentNode>.
        So convert <hitNode> into a new intersection. */
        if (childDirection == "W")                                 // hitNode parentNode
        {
            return WestIntersectChecks(hitNode, parentNode, i);
        }
        else if (childDirection == "N")                            // hitNode
        {                                                          // parentNode
            return NorthIntersectChecks(hitNode, parentNode, i);
        }
        else if (childDirection == "E")                            // parentNode hitNode
        {
            return EastIntersectChecks(hitNode, parentNode, i);
        }
        else // (childDirection == "S")                            // parentNode
        {                                                          // hitNode
            return SouthIntersectChecks(hitNode, parentNode, i);
        }
    }

    public GameObject WestIntersectChecks(GameObject nodeToConvert, GameObject nodeUnchanged, int i)
    {
        /* <nodeUnchanged> is the object that is trying to create a new child/tile at the same position as <nodeToConvert>. 
        <nodeToConvert> is the object that needs to be converted into an intersection. 
        <nodeToConvert> is the same as <hitNode> from CreateIntersection() and <hitCollider> from CreateLoop().
        <nodeUnchanged> is the same as <parentNode> from CreateIntersection() and <lastNode> from CreateLoop().
        <nodeUnchanged>'s west = <nodeToConvert> / <nodeToConvert>'s east = <nodeUnchanged> */
        TileScript convertingTileScript = nodeToConvert.GetComponent<TileScript>();
        TileScript unchangingTileScript = nodeUnchanged.GetComponent<TileScript>();
        string convertTile = convertingTileScript.GetName();
        string unchangedTile = unchangingTileScript.GetName();
        GameObject newNode = nodeToConvert;
        if (convertingTileScript.GetEast() != blockedTile)
        {
            convertingTileScript.SetEast(nodeUnchanged);
            unchangingTileScript.SetWest(nodeToConvert); 
            return newNode;
        }
        else
        {
            if (convertTile == "vert")
            {
                newNode = Instantiate(tilePrefabs[10], nodeToConvert.transform.position, Quaternion.identity, nodeToConvert.transform.parent);
                convertingTileScript.GetNorth().GetComponent<TileScript>().SetSouth(newNode);
                convertingTileScript.GetSouth().GetComponent<TileScript>().SetNorth(newNode);
                TileScript newTileScript = newNode.GetComponent<TileScript>();
                newTileScript.CreateNode(blockedTile, convertingTileScript.GetNorth(), nodeUnchanged, convertingTileScript.GetSouth(), "3wayNES", 5, i);
            }
            else if (convertTile == "cornWN")
            {
                newNode = Instantiate(tilePrefabs[7], nodeToConvert.transform.position, Quaternion.identity, nodeToConvert.transform.parent);
                convertingTileScript.GetWest().GetComponent<TileScript>().SetEast(newNode);
                convertingTileScript.GetNorth().GetComponent<TileScript>().SetSouth(newNode);
                TileScript newTileScript = newNode.GetComponent<TileScript>();
                newTileScript.CreateNode(convertingTileScript.GetWest(), convertingTileScript.GetNorth(), nodeUnchanged, blockedTile, "3wayWNE", 5, i);
            }
            else if (convertTile == "cornWS")
            {
                newNode = Instantiate(tilePrefabs[9], nodeToConvert.transform.position, Quaternion.identity, nodeToConvert.transform.parent);
                convertingTileScript.GetWest().GetComponent<TileScript>().SetEast(newNode);
                convertingTileScript.GetSouth().GetComponent<TileScript>().SetNorth(newNode);
                TileScript newTileScript = newNode.GetComponent<TileScript>();
                newTileScript.CreateNode(convertingTileScript.GetWest(), blockedTile, nodeUnchanged, convertingTileScript.GetSouth(), "3wayWES", 5, i);
            }
            else if (convertTile == "3wayWNS")
            {
                newNode = Instantiate(tilePrefabs[11], nodeToConvert.transform.position, Quaternion.identity, nodeToConvert.transform.parent);
                convertingTileScript.GetWest().GetComponent<TileScript>().SetEast(newNode);
                convertingTileScript.GetNorth().GetComponent<TileScript>().SetSouth(newNode);
                convertingTileScript.GetSouth().GetComponent<TileScript>().SetNorth(newNode);
                TileScript newTileScript = newNode.GetComponent<TileScript>();
                newTileScript.CreateNode(convertingTileScript.GetWest(), convertingTileScript.GetNorth(), nodeUnchanged, convertingTileScript.GetSouth(), "4way", 5, i);
            }
            Destroy(nodeToConvert);
            unchangingTileScript.SetWest(newNode);
            return newNode;
        }
    }

    public GameObject NorthIntersectChecks(GameObject nodeToConvert, GameObject nodeUnchanged, int i)
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
            convertingTileScript.SetSouth(nodeUnchanged);
            unchangingTileScript.SetNorth(nodeToConvert);
            return newNode;
        }
        else
        {
            if (convertTile == "hor")
            {
                newNode = Instantiate(tilePrefabs[9], nodeToConvert.transform.position, Quaternion.identity, nodeToConvert.transform.parent);
                convertingTileScript.GetWest().GetComponent<TileScript>().SetEast(newNode);
                convertingTileScript.GetEast().GetComponent<TileScript>().SetWest(newNode);
                TileScript newTileScript = newNode.GetComponent<TileScript>();
                newTileScript.CreateNode(convertingTileScript.GetWest(), blockedTile, convertingTileScript.GetEast(), nodeUnchanged, "3wayWES", 5, i);
            }
            else if (convertTile == "cornWN")
            {
                newNode = Instantiate(tilePrefabs[8], nodeToConvert.transform.position, Quaternion.identity, nodeToConvert.transform.parent);
                convertingTileScript.GetWest().GetComponent<TileScript>().SetEast(newNode);
                convertingTileScript.GetNorth().GetComponent<TileScript>().SetSouth(newNode);
                TileScript newTileScript = newNode.GetComponent<TileScript>();
                newTileScript.CreateNode(convertingTileScript.GetWest(), convertingTileScript.GetNorth(), blockedTile, nodeUnchanged, "3wayWNS", 5, i);
            }
            else if (convertTile == "cornNE")
            {
                newNode = Instantiate(tilePrefabs[10], nodeToConvert.transform.position, Quaternion.identity, nodeToConvert.transform.parent);
                convertingTileScript.GetNorth().GetComponent<TileScript>().SetSouth(newNode);
                convertingTileScript.GetEast().GetComponent<TileScript>().SetWest(newNode);
                TileScript newTileScript = newNode.GetComponent<TileScript>();
                newTileScript.CreateNode(blockedTile, convertingTileScript.GetNorth(), convertingTileScript.GetEast(), nodeUnchanged, "3wayNES", 5, i);
            }
            else if (convertTile == "3wayWNE")
            {
                newNode = Instantiate(tilePrefabs[11], nodeToConvert.transform.position, Quaternion.identity, nodeToConvert.transform.parent);
                convertingTileScript.GetWest().GetComponent<TileScript>().SetEast(newNode);
                convertingTileScript.GetNorth().GetComponent<TileScript>().SetSouth(newNode);
                convertingTileScript.GetEast().GetComponent<TileScript>().SetWest(newNode);
                TileScript newTileScript = newNode.GetComponent<TileScript>();
                newTileScript.CreateNode(convertingTileScript.GetWest(), convertingTileScript.GetNorth(), convertingTileScript.GetEast(), nodeUnchanged, "4way", 5, i);
            }
            Destroy(nodeToConvert);
            unchangingTileScript.SetNorth(newNode);
            return newNode;
        }
    }

    public GameObject EastIntersectChecks(GameObject nodeToConvert, GameObject nodeUnchanged, int i)
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
            convertingTileScript.SetWest(nodeUnchanged);
            unchangingTileScript.SetEast(nodeToConvert);
            return newNode;
        }
        else
        {
            if (convertTile == "vert")
            {
                newNode = Instantiate(tilePrefabs[8], nodeToConvert.transform.position, Quaternion.identity, nodeToConvert.transform.parent);
                convertingTileScript.GetNorth().GetComponent<TileScript>().SetSouth(newNode);
                convertingTileScript.GetSouth().GetComponent<TileScript>().SetNorth(newNode);
                TileScript newTileScript = newNode.GetComponent<TileScript>();
                newTileScript.CreateNode(nodeUnchanged, convertingTileScript.GetNorth(), blockedTile, convertingTileScript.GetSouth(), "3wayWNS", 5, i);
            }
            else if (convertTile == "cornES")
            {
                newNode = Instantiate(tilePrefabs[9], nodeToConvert.transform.position, Quaternion.identity, nodeToConvert.transform.parent);
                convertingTileScript.GetEast().GetComponent<TileScript>().SetWest(newNode);
                convertingTileScript.GetSouth().GetComponent<TileScript>().SetNorth(newNode);
                TileScript newTileScript = newNode.GetComponent<TileScript>();
                newTileScript.CreateNode(nodeUnchanged, blockedTile, convertingTileScript.GetEast(), convertingTileScript.GetSouth(), "3wayWES", 5, i);
            }
            else if (convertTile == "cornNE")
            {
                newNode = Instantiate(tilePrefabs[7], nodeToConvert.transform.position, Quaternion.identity, nodeToConvert.transform.parent);
                convertingTileScript.GetNorth().GetComponent<TileScript>().SetSouth(newNode);
                convertingTileScript.GetEast().GetComponent<TileScript>().SetWest(newNode);
                TileScript newTileScript = newNode.GetComponent<TileScript>();
                newTileScript.CreateNode(nodeUnchanged, convertingTileScript.GetNorth(), convertingTileScript.GetEast(), blockedTile, "3wayWNE", 5, i);
            }
            else if (convertTile == "3wayNES")
            {
                newNode = Instantiate(tilePrefabs[11], nodeToConvert.transform.position, Quaternion.identity, nodeToConvert.transform.parent);
                convertingTileScript.GetNorth().GetComponent<TileScript>().SetSouth(newNode);
                convertingTileScript.GetEast().GetComponent<TileScript>().SetWest(newNode);
                convertingTileScript.GetSouth().GetComponent<TileScript>().SetNorth(newNode);
                TileScript newTileScript = newNode.GetComponent<TileScript>();
                newTileScript.CreateNode(nodeUnchanged, convertingTileScript.GetNorth(), convertingTileScript.GetEast(), convertingTileScript.GetSouth(), "4way", 5, i);
            }
            Destroy(nodeToConvert);
            unchangingTileScript.SetEast(newNode);
            return newNode;
        }
    }

    public GameObject SouthIntersectChecks(GameObject nodeToConvert, GameObject nodeUnchanged, int i)
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
            convertingTileScript.SetNorth(nodeUnchanged);
            unchangingTileScript.SetSouth(nodeToConvert);
            return newNode;
        }
        else
        {
            if (convertTile == "hor")
            {
                newNode = Instantiate(tilePrefabs[7], nodeToConvert.transform.position, Quaternion.identity, nodeToConvert.transform.parent);
                convertingTileScript.GetWest().GetComponent<TileScript>().SetEast(newNode);
                convertingTileScript.GetEast().GetComponent<TileScript>().SetWest(newNode);
                TileScript newTileScript = newNode.GetComponent<TileScript>();
                newTileScript.CreateNode(convertingTileScript.GetWest(), nodeUnchanged, convertingTileScript.GetEast(), blockedTile, "3wayWNE", 5, i);
            }
            else if (convertTile == "cornWS")
            {
                newNode = Instantiate(tilePrefabs[8], nodeToConvert.transform.position, Quaternion.identity, nodeToConvert.transform.parent);
                convertingTileScript.GetWest().GetComponent<TileScript>().SetEast(newNode);
                convertingTileScript.GetSouth().GetComponent<TileScript>().SetNorth(newNode);
                TileScript newTileScript = newNode.GetComponent<TileScript>();
                newTileScript.CreateNode(convertingTileScript.GetWest(), nodeUnchanged, blockedTile, convertingTileScript.GetSouth(), "3wayWNS", 5, i);
            }
            else if (convertTile == "cornES")
            {
                newNode = Instantiate(tilePrefabs[10], nodeToConvert.transform.position, Quaternion.identity, nodeToConvert.transform.parent);
                convertingTileScript.GetEast().GetComponent<TileScript>().SetWest(newNode);
                convertingTileScript.GetSouth().GetComponent<TileScript>().SetNorth(newNode);
                TileScript newTileScript = newNode.GetComponent<TileScript>();
                newTileScript.CreateNode(blockedTile, nodeUnchanged, convertingTileScript.GetEast(), convertingTileScript.GetSouth(), "3wayNES", 5, i);
            }
            else if (convertTile == "3wayWES")
            {
                newNode = Instantiate(tilePrefabs[11], nodeToConvert.transform.position, Quaternion.identity, nodeToConvert.transform.parent);
                convertingTileScript.GetWest().GetComponent<TileScript>().SetEast(newNode);
                convertingTileScript.GetEast().GetComponent<TileScript>().SetWest(newNode);
                convertingTileScript.GetSouth().GetComponent<TileScript>().SetNorth(newNode);
                TileScript newTileScript = newNode.GetComponent<TileScript>();
                newTileScript.CreateNode(convertingTileScript.GetWest(), nodeUnchanged, convertingTileScript.GetEast(), convertingTileScript.GetSouth(), "4way", 5, i);
            }
            Destroy(nodeToConvert);
            unchangingTileScript.SetSouth(newNode);
            return newNode;
        }
    }

    public GameObject CreateNewTile(GameObject parentNode, string childDirection, int i)
    {
        bool careful = false;
        /*if (parentNode.transform.position.y == origin.transform.position.y && (childDirection == "W" || childDirection == "E"))
        {
            careful = true; 
        }
        else if ((parentNode.transform.position.y + tileSizeY == origin.transform.position.y) && childDirection == "S")
        {
            careful = true; 
        } */
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
                    newNode = Instantiate(tilePrefabs[version], new Vector3(parentNode.transform.position.x - tileSizeX, parentNode.transform.position.y, parentNode.transform.position.z), Quaternion.identity, parentNode.transform.parent);
                    TileScript newTileScript = newNode.GetComponent<TileScript>();
                    newTileScript.CreateNode(null, blockedTile, parentNode, blockedTile, "hor", 2, i);
                }
                else // if (childDirection == "E")
                {
                    newNode = Instantiate(tilePrefabs[version], new Vector3(parentNode.transform.position.x + tileSizeX, parentNode.transform.position.y, parentNode.transform.position.z), Quaternion.identity, parentNode.transform.parent);
                    TileScript newTileScript = newNode.GetComponent<TileScript>();
                    newTileScript.CreateNode(parentNode, blockedTile, null, blockedTile, "hor", 2, i);
                }
            }
            else // if (childDirection == "N" || childDirection == "S")) 
            {
                if (careful)
                {
                    a = 53; // Do not create a vertical hallway. You cannot go down. Instead, make a corner. 
                }
                else
                {
                    if (childDirection == "N")
                    {
                        newNode = Instantiate(tilePrefabs[2], new Vector3(parentNode.transform.position.x, parentNode.transform.position.y + tileSizeY, parentNode.transform.position.z), Quaternion.identity, parentNode.transform.parent);
                        TileScript newTileScript = newNode.GetComponent<TileScript>();
                        newTileScript.CreateNode(blockedTile, null, blockedTile, parentNode, "vert", 2, i);
                    }
                    else // if (childDirection == "S")
                    {
                        newNode = Instantiate(tilePrefabs[2], new Vector3(parentNode.transform.position.x, parentNode.transform.position.y - tileSizeY, parentNode.transform.position.z), Quaternion.identity, parentNode.transform.parent);
                        TileScript newTileScript = newNode.GetComponent<TileScript>();
                        newTileScript.CreateNode(blockedTile, parentNode, blockedTile, null, "vert", 2, i);
                    }
                }
            }
        }
        // 26% chance to create a corner. 50/50 chance for direction. 
        if (40 <= a && a < 53)
        {
            if (careful)
            {
                a = 53; // Do not create a corner going down. You cannot go down. Instead, make a corner going up/sideways. 
            }
            if (childDirection == "E")
            {
                newNode = Instantiate(tilePrefabs[4], new Vector3(parentNode.transform.position.x + tileSizeX, parentNode.transform.position.y, parentNode.transform.position.z), Quaternion.identity, parentNode.transform.parent);
                TileScript newTileScript = newNode.GetComponent<TileScript>();
                newTileScript.CreateNode(parentNode, blockedTile, blockedTile, null, "cornWS", 2, i);
            }
            else if (childDirection == "N")
            {
                newNode = Instantiate(tilePrefabs[4], new Vector3(parentNode.transform.position.x, parentNode.transform.position.y + tileSizeY, parentNode.transform.position.z), Quaternion.identity, parentNode.transform.parent);
                TileScript newTileScript = newNode.GetComponent<TileScript>();
                newTileScript.CreateNode(null, blockedTile, blockedTile, parentNode, "cornWS", 2, i);
            }
            else if (childDirection == "W")
            {
                newNode = Instantiate(tilePrefabs[5], new Vector3(parentNode.transform.position.x - tileSizeX, parentNode.transform.position.y, parentNode.transform.position.z), Quaternion.identity, parentNode.transform.parent);
                TileScript newTileScript = newNode.GetComponent<TileScript>();
                newTileScript.CreateNode(blockedTile, blockedTile, parentNode, null, "cornES", 2, i);
            }
            else // if (childDirection == "S")
            {
                newNode = Instantiate(tilePrefabs[6], new Vector3(parentNode.transform.position.x, parentNode.transform.position.y - tileSizeY, parentNode.transform.position.z), Quaternion.identity, parentNode.transform.parent);
                TileScript newTileScript = newNode.GetComponent<TileScript>();
                newTileScript.CreateNode(blockedTile, parentNode, null, blockedTile, "cornNE", 2, i);
            }
        }
        else if (53 <= a && a <= 65) // 13% chance to create a different corner. 
        {
            if (childDirection == "E")
            {
                newNode = Instantiate(tilePrefabs[3], new Vector3(parentNode.transform.position.x + tileSizeX, parentNode.transform.position.y, parentNode.transform.position.z), Quaternion.identity, parentNode.transform.parent);
                TileScript newTileScript = newNode.GetComponent<TileScript>();
                newTileScript.CreateNode(parentNode, null, blockedTile, blockedTile, "cornWN", 2, i);
            }
            else if (childDirection == "S")
            {
                newNode = Instantiate(tilePrefabs[3], new Vector3(parentNode.transform.position.x, parentNode.transform.position.y - tileSizeY, parentNode.transform.position.z), Quaternion.identity, parentNode.transform.parent);
                TileScript newTileScript = newNode.GetComponent<TileScript>();
                newTileScript.CreateNode(null, parentNode, blockedTile, blockedTile, "cornWN", 2, i);
            }
            else if (childDirection == "W")
            {
                newNode = Instantiate(tilePrefabs[6], new Vector3(parentNode.transform.position.x - tileSizeX, parentNode.transform.position.y, parentNode.transform.position.z), Quaternion.identity, parentNode.transform.parent);
                TileScript newTileScript = newNode.GetComponent<TileScript>();
                newTileScript.CreateNode(blockedTile, null, parentNode, blockedTile, "cornNE", 2, i);
            }
            else // if (childDirection == "N")
            {
                newNode = Instantiate(tilePrefabs[5], new Vector3(parentNode.transform.position.x, parentNode.transform.position.y + tileSizeY, parentNode.transform.position.z), Quaternion.identity, parentNode.transform.parent);
                TileScript newTileScript = newNode.GetComponent<TileScript>();
                newTileScript.CreateNode(blockedTile, blockedTile, null, parentNode, "cornES", 2, i);
            }
        }
        else if (a >= 90) // 10 % chance of creating a 4 way intersection. 
        {
            if (careful)
            {
                a = 66;
            }
            if (childDirection == "W")
            {
                newNode = Instantiate(tilePrefabs[11], new Vector3(parentNode.transform.position.x - tileSizeX, parentNode.transform.position.y, parentNode.transform.position.z), Quaternion.identity, parentNode.transform.parent);
                TileScript newTileScript = newNode.GetComponent<TileScript>();
                newTileScript.CreateNode(null, null, parentNode, null, "4way", 2, i);
            }
            else if (childDirection == "N")
            {
                newNode = Instantiate(tilePrefabs[11], new Vector3(parentNode.transform.position.x, parentNode.transform.position.y + tileSizeY, parentNode.transform.position.z), Quaternion.identity, parentNode.transform.parent);
                TileScript newTileScript = newNode.GetComponent<TileScript>();
                newTileScript.CreateNode(null, null, null, parentNode, "4way", 2, i);
            }
            else if (childDirection == "E")
            {
                newNode = Instantiate(tilePrefabs[11], new Vector3(parentNode.transform.position.x + tileSizeX, parentNode.transform.position.y, parentNode.transform.position.z), Quaternion.identity, parentNode.transform.parent);
                TileScript newTileScript = newNode.GetComponent<TileScript>();
                newTileScript.CreateNode(parentNode, null, null, null, "4way", 2, i);
            }
            else // if (childDirection == "S")
            {
                newNode = Instantiate(tilePrefabs[11], new Vector3(parentNode.transform.position.x, parentNode.transform.position.y - tileSizeY, parentNode.transform.position.z), Quaternion.identity, parentNode.transform.parent);
                TileScript newTileScript = newNode.GetComponent<TileScript>();
                newTileScript.CreateNode(null, parentNode, null, null, "4way", 2, i);
            }
        }
        if (65 < a && a < 90) // 25% chance of 3 way intersection. 1/3 chance for direction. 
        {
            int b = UnityEngine.Random.Range(0, 3);
            if (childDirection == "W")
            {
                if (b == 0 || careful == true)
                {
                    newNode = Instantiate(tilePrefabs[7], new Vector3(parentNode.transform.position.x - tileSizeX, parentNode.transform.position.y, parentNode.transform.position.z), Quaternion.identity, parentNode.transform.parent);
                    TileScript newTileScript = newNode.GetComponent<TileScript>();
                    newTileScript.CreateNode(null, null, parentNode, blockedTile, "3wayWNE", 2, i);
                }
                else if (b == 1)
                {
                    newNode = Instantiate(tilePrefabs[9], new Vector3(parentNode.transform.position.x - tileSizeX, parentNode.transform.position.y, parentNode.transform.position.z), Quaternion.identity, parentNode.transform.parent);
                    TileScript newTileScript = newNode.GetComponent<TileScript>();
                    newTileScript.CreateNode(null, blockedTile, parentNode, null, "3wayWES", 2, i);
                }
                else
                {
                    newNode = Instantiate(tilePrefabs[10], new Vector3(parentNode.transform.position.x - tileSizeX, parentNode.transform.position.y, parentNode.transform.position.z), Quaternion.identity, parentNode.transform.parent);
                    TileScript newTileScript = newNode.GetComponent<TileScript>();
                    newTileScript.CreateNode(blockedTile, null, parentNode, null, "3wayNES", 2, i);
                }
            }
            else if (childDirection == "N")
            {
                if (b == 0)
                {
                    newNode = Instantiate(tilePrefabs[8], new Vector3(parentNode.transform.position.x, parentNode.transform.position.y + tileSizeY, parentNode.transform.position.z), Quaternion.identity, parentNode.transform.parent);
                    TileScript newTileScript = newNode.GetComponent<TileScript>();
                    newTileScript.CreateNode(null, null, blockedTile, parentNode, "3wayWNS", 2, i);
                }
                else if (b == 1)
                {
                    newNode = Instantiate(tilePrefabs[9], new Vector3(parentNode.transform.position.x, parentNode.transform.position.y + tileSizeY, parentNode.transform.position.z), Quaternion.identity, parentNode.transform.parent);
                    TileScript newTileScript = newNode.GetComponent<TileScript>();
                    newTileScript.CreateNode(null, blockedTile, null, parentNode, "3wayWES", 2, i);
                }
                else
                {
                    newNode = Instantiate(tilePrefabs[10], new Vector3(parentNode.transform.position.x, parentNode.transform.position.y + tileSizeY, parentNode.transform.position.z), Quaternion.identity, parentNode.transform.parent);
                    TileScript newTileScript = newNode.GetComponent<TileScript>();
                    newTileScript.CreateNode(blockedTile, null, null, parentNode, "3wayNES", 2, i);
                }
            }
            else if (childDirection == "E")
            {
                if (b == 0 || careful == true)
                {
                    newNode = Instantiate(tilePrefabs[7], new Vector3(parentNode.transform.position.x + tileSizeX, parentNode.transform.position.y, parentNode.transform.position.z), Quaternion.identity, parentNode.transform.parent);
                    TileScript newTileScript = newNode.GetComponent<TileScript>();
                    newTileScript.CreateNode(parentNode, null, null, blockedTile, "3wayWNE", 2, i);
                }
                else if (b == 1)
                {
                    newNode = Instantiate(tilePrefabs[8], new Vector3(parentNode.transform.position.x + tileSizeX, parentNode.transform.position.y, parentNode.transform.position.z), Quaternion.identity, parentNode.transform.parent);
                    TileScript newTileScript = newNode.GetComponent<TileScript>();
                    newTileScript.CreateNode(parentNode, null, blockedTile, null, "3wayWNS", 2, i);
                }
                else
                {
                    newNode = Instantiate(tilePrefabs[9], new Vector3(parentNode.transform.position.x + tileSizeX, parentNode.transform.position.y, parentNode.transform.position.z), Quaternion.identity, parentNode.transform.parent);
                    TileScript newTileScript = newNode.GetComponent<TileScript>();
                    newTileScript.CreateNode(parentNode, blockedTile, null, null, "3wayWES", 2, i);
                }
            }
            else // if (childDirection == "S")
            {
                if (b == 0 || careful == true)
                {
                    newNode = Instantiate(tilePrefabs[7], new Vector3(parentNode.transform.position.x, parentNode.transform.position.y - tileSizeY, parentNode.transform.position.z), Quaternion.identity, parentNode.transform.parent);
                    TileScript newTileScript = newNode.GetComponent<TileScript>();
                    newTileScript.CreateNode(null, parentNode, null, blockedTile, "3wayWNE", 2, i);
                }
                else if (b == 1)
                {
                    newNode = Instantiate(tilePrefabs[8], new Vector3(parentNode.transform.position.x, parentNode.transform.position.y - tileSizeY, parentNode.transform.position.z), Quaternion.identity, parentNode.transform.parent);
                    TileScript newTileScript = newNode.GetComponent<TileScript>();
                    newTileScript.CreateNode(null, parentNode, blockedTile, null, "3wayWNS", 2, i);
                }
                else
                {
                    newNode = Instantiate(tilePrefabs[10], new Vector3(parentNode.transform.position.x, parentNode.transform.position.y - tileSizeY, parentNode.transform.position.z), Quaternion.identity, parentNode.transform.parent);
                    TileScript newTileScript = newNode.GetComponent<TileScript>();
                    newTileScript.CreateNode(blockedTile, parentNode, null, null, "3wayNES", 2, i);
                }
            }
        }
        TileScript parentTileScript = parentNode.GetComponent<TileScript>(); 
        if (childDirection == "W")
        {
            parentTileScript.SetWest(newNode);
        }
        else if (childDirection == "N")
        {
            parentTileScript.SetNorth(newNode);
        }
        else if (childDirection == "E")
        {
            parentTileScript.SetEast(newNode);
        }
        else if (childDirection == "S")
        {
            parentTileScript.SetSouth(newNode);
        }
        return newNode;
    }

    public void GenerateLoop(GameObject endNode)
    {
        TileScript endTileScript = endNode.GetComponent<TileScript>();
        if (endTileScript.GetWest() == null)
        {
            endNode = CreateLoop(endNode, 5, endNode);
        }
        endTileScript = endNode.GetComponent<TileScript>();
        if (endTileScript.GetNorth() == null)
        {
            endNode = CreateLoop(endNode, 4, endNode);
        }
        endTileScript = endNode.GetComponent<TileScript>();
        if (endTileScript.GetEast() == null)
        {
            endNode = CreateLoop(endNode, 3, endNode);
        }
        endTileScript = endNode.GetComponent<TileScript>();
        if (endTileScript.GetSouth() == null)
        {
            endNode = CreateLoop(endNode, 6, endNode);
        }
    }

    public GameObject CreateLoop(GameObject lastNode, int i, GameObject endNode)
    {
        GameObject newNode = lastNode; // This is just to initialize it. <newNode> will not stay <lastNode>. 
        TileScript lastTileScript = lastNode.GetComponent<TileScript>();
        TileScript endTileScript = endNode.GetComponent<TileScript>();
        if (i == 3)
        {
            Component hitCollider = CollisionCheck(lastNode, "E");
            if (hitCollider != null)
            {/*
                if (hitCollider.gameObject == endNode)
                {
                    print("endNode"); 
                    endTileScript.SetWest(lastNode);
                    lastTileScript.SetEast(endNode);
                    lastTileScript.loop();
                    endTileScript.loop();
                }
                else
                {*/
                    newNode = CreateIntersection(hitCollider.gameObject, lastNode, "E", 5);
                    lastTileScript.SetEast(newNode);
                    lastTileScript.loop();
                    newNode.GetComponent<TileScript>().loop();
                //}
                return newNode;
            }
            else
            {
                newNode = Instantiate(tilePrefabs[3], new Vector3(lastNode.transform.position.x + tileSizeX, lastNode.transform.position.y, lastNode.transform.position.z), Quaternion.identity, lastNode.transform.parent);
                TileScript newTileScript = newNode.GetComponent<TileScript>();
                newTileScript.CreateNode(lastNode, null, blockedTile, blockedTile, "cornWN", 3, created++);
                lastTileScript.SetEast(newNode);
                lastTileScript.loop();
                newTileScript.loop();
            }
        }
        else if (i == 4)
        {
            Component hitCollider = CollisionCheck(lastNode, "N");
            if (hitCollider != null)
            {
                /*if (hitCollider.gameObject == endNode)
                {
                    print("endNode");
                    endTileScript.SetSouth(lastNode);
                    lastTileScript.SetNorth(endNode);
                    lastTileScript.loop();
                    endTileScript.loop();
                }
                else
                {*/
                    newNode = CreateIntersection(hitCollider.gameObject, lastNode, "N", 5);
                    lastTileScript.SetNorth(newNode);
                    lastTileScript.loop();
                    newNode.GetComponent<TileScript>().loop();
                //}
                return newNode;
            }
            else
            {
                newNode = Instantiate(tilePrefabs[4], new Vector3(lastNode.transform.position.x, lastNode.transform.position.y + tileSizeY, lastNode.transform.position.z), Quaternion.identity, lastNode.transform.parent);
                TileScript newTileScript = newNode.GetComponent<TileScript>();
                newTileScript.CreateNode(null, blockedTile, blockedTile, lastNode, "cornWS", 3, created++);
                lastTileScript.SetNorth(newNode);
                lastTileScript.loop();
                newTileScript.loop();
            }
        }
        else if (i == 5)
        {
            Component hitCollider = CollisionCheck(lastNode, "W");
            if (hitCollider != null)
            {/*
                if (hitCollider.gameObject == endNode)
                {
                    print("endNode");
                    endTileScript.SetEast(lastNode);
                    lastTileScript.SetWest(endNode);
                    lastTileScript.loop();
                    endTileScript.loop();
                }
                else
                {*/
                    newNode = CreateIntersection(hitCollider.gameObject, lastNode, "W", 5);
                    lastTileScript.SetWest(newNode);
                    lastTileScript.loop();
                    newNode.GetComponent<TileScript>().loop();
                //}
                return newNode;
            }
            else
            {
                newNode = Instantiate(tilePrefabs[5], new Vector3(lastNode.transform.position.x - tileSizeX, lastNode.transform.position.y, lastNode.transform.position.z), Quaternion.identity, lastNode.transform.parent);
                TileScript newTileScript = newNode.GetComponent<TileScript>();
                newTileScript.CreateNode(blockedTile, blockedTile, lastNode, null, "cornES", 3, created++);
                lastTileScript.SetWest(newNode);
                lastTileScript.loop();
                newTileScript.loop();
            }
        }
        else if (i == 6)
        {
            Component hitCollider = CollisionCheck(lastNode, "S");
            if (hitCollider != null)
            {/*
                if (hitCollider.gameObject == endNode)
                {
                    print("endNode");
                    endTileScript.SetNorth(lastNode);
                    lastTileScript.SetSouth(endNode);
                    lastTileScript.loop();
                    endTileScript.loop();
                }
                else
                {*/
                    print("intersection");
                    newNode = CreateIntersection(hitCollider.gameObject, lastNode, "S", 5);
                    lastTileScript.SetSouth(newNode);
                    lastTileScript.loop();
                    newNode.GetComponent<TileScript>().loop();
                //}
                return newNode;
            }
            else
            {
                newNode = Instantiate(tilePrefabs[6], new Vector3(lastNode.transform.position.x, lastNode.transform.position.y - tileSizeY, lastNode.transform.position.z), Quaternion.identity, lastNode.transform.parent);
                TileScript newTileScript = newNode.GetComponent<TileScript>();
                newTileScript.CreateNode(blockedTile, lastNode, null, blockedTile, "cornNE", 3, created++);
                lastTileScript.SetSouth(newNode);
                lastTileScript.loop();
                newTileScript.loop();
            }
        }
        i++;
        if (i == 7)
        {
            i = 3;
        }
        endNode = CreateLoop(newNode, i, endNode);
        return endNode;
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