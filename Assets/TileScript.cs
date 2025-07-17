using NUnit.Framework.Internal;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static Unity.Collections.AllocatorManager;
using UnityEngine.InputSystem.EnhancedTouch;

public class TileScript : MonoBehaviour
{
    public GameObject[] linkedTiles = new GameObject[4];
    public string tileName; 
    public bool isChecked;
    public GameObject blockedTile;
    public int isLooped;
    public int creation; 

    public void CreateNode(GameObject west, GameObject north, GameObject east, GameObject south, string name, int loopy, int create)
    {
        linkedTiles[0] = west;
        linkedTiles[1] = north;
        linkedTiles[2] = east;
        linkedTiles[3] = south;
        tileName = name; 
        isChecked = false;
        isLooped = loopy;
        creation = create;
    }

    public void loop()
    {
        isLooped = 3; 
    }

    public string GetLinkTypes()
    {
        string links = "";
        for (int i = 0; i < 4; i++)
        {
            if (linkedTiles[i] == null)
            {
                links += "X";
            }
            else if (linkedTiles[i] == blockedTile)
            {
                links += "0";
            }
            else
            {
                links += "1";
            }
        }
        return links;
    }

    public string GetName()
    {
        return tileName;
    }

    public GameObject GetWest()
    {
        return linkedTiles[0];
    }

    public GameObject GetNorth()
    {
        return linkedTiles[1];
    }

    public GameObject GetEast()
    {
        return linkedTiles[2];
    }

    public GameObject GetSouth()
    {
        return linkedTiles[3];
    }

    public void SetWest(GameObject newWest)
    {
        linkedTiles[0] = newWest;
    }

    public void SetNorth(GameObject newNorth)
    {
        linkedTiles[1] = newNorth;
    }

    public void SetEast(GameObject newEast)
    {
        linkedTiles[2] = newEast;
    }

    public void SetSouth(GameObject newSouth)
    {
        linkedTiles[3] = newSouth;
    }

    public void Checked()
    {
        isChecked = true;
    }

    public void Uncheck()
    {
        isChecked = false;
    }

}
