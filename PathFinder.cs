using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.Threading.Tasks;
using System;
using Unity.VisualScripting;

public enum NodeState { Untested, Closed, Open}

public class Node
{
    public Room room { get; set; }
    public Node parentNode = null;
    public float F = 0;
    public float G = 0;
    public float H = 0;
    public NodeState State { get; set; } = NodeState.Untested;
    public List<float> Weights = new List<float>();
    public List<Node> ConnectedNodes = new List<Node>();
    public void Calculate(Room Start, Room End)
    {
        if (room.People >= room.MaxPeople || room == Start)
        {
            State = NodeState.Closed;
        }
        G = Vector2.Distance(Start.location, room.location);
        H = Vector2.Distance(End.location, room.location);
        F = G + H;
        foreach (float w in Weights)
        {
            F += w;
        }
    }
    public float GetTraversalCost(Room Start, Room End)
    {
        float aG = Vector2.Distance(Start.location, room.location);
        float aH = Vector2.Distance(End.location, room.location);
        float aF = G + H;
        foreach (float w in Weights)
        {
            aF += w;
        }
        return aF;
    }
}

public class PathFinder : MonoBehaviour
{
    List<Node> AllNodes = new List<Node>();
    public List<Room> Rooms = new List<Room>();
    public bool IsReady { get; private set; } = false;
    Room SRoom = null, FRoom = null;
    Node Snode = null, Fnode = null;
    // Start is called before the first frame update

    Node GetNodeFromRoom(Room room)
    {
        foreach (Node node in AllNodes) {
        if (node.room == room) return node;
        }
        return null;
    }
    int FindIndex(object[,] arg, object target)
    {
        int length = arg.GetLength(0);
        for (int i = 0; i < length;)
        {
            if (arg[i,0] == target) return i;
        }
        return -1;
    }

    List<Node> GetAvaiables(Node fromNode, List<Node> nodes)
    {
        List<Node> walkableNodes = new List<Node>();
        bool skip = false;
        foreach (Node node in nodes)
        {
            if (skip || (node.room.People >= node.room.MaxPeople) ) continue;
            if (node.State == NodeState.Open)
            {
                float traversalCost = Vector2.Distance(node.room.location,node.parentNode.room.location);
                float gTemp = fromNode.G + traversalCost;
                if (gTemp < node.G)
                {
                    node.parentNode = fromNode;
                    walkableNodes.Add(node);
                }
            }
            else if (node.State == NodeState.Untested)
            {
                // If it's untested, set the parent and flag it as 'Open' for consideration
                node.parentNode = fromNode;
                node.State = NodeState.Open;
                walkableNodes.Add(node);
                Debug.DrawLine(fromNode.room.transform.position, node.room.transform.position, Color.yellow, 10, false);
            }else if (node.room == FRoom)
            {
                skip = true;
                walkableNodes = new List<Node>();
                walkableNodes.Add(node);
            }
        }
        return walkableNodes;
    }
    public void WarmUp(Room Start, Room End, object[,] Weights)
    {
        IsReady = false;
        if (Rooms.Count <= 0) { Debug.LogWarning("No rooms to set up"); return; }
        //if (AllNodes.Count > 0) { AllNodes.Clear(); }
        AllNodes.Clear();
        foreach (Room room in Rooms)
        {
            Node node = new Node();
            node.room = room;
            AllNodes.Add(node);
        }
        foreach (Node node in AllNodes)
        {
            foreach (Room Croom in node.room.RoomConnections)
            {
                Node Cnode = GetNodeFromRoom((Room) Croom);
                if (Cnode != null)
                {
                    node.ConnectedNodes.Add(Cnode);
                }
            }
            
            if (Weights != null)
            {
                int WI = FindIndex(Weights, node.room);
                if (WI != -1)
                {
                    foreach (float weight in (float[])Weights[WI,1])
                    {
                        node.Weights.Add(weight);
                    }
                }
            }
            node.Calculate(Start, End);
        }
        SRoom = Start; FRoom = End;
        Snode = GetNodeFromRoom(Start); Fnode = GetNodeFromRoom(End);
        //object[] De = {SRoom.name,FRoom.name,Snode.room.name,Fnode.room.name};
        Debug.Log(Snode.room.name);
        Debug.Log(Fnode.room.name);
        IsReady = true;
    }

    /*void Start()
    {
        
    }*/
    bool Search(Node currentNode)
    {
        currentNode.State = NodeState.Closed;
        List<Node> nextNodes = GetAvaiables(currentNode, currentNode.ConnectedNodes);
        nextNodes.Sort((node1, node2) => node1.F.CompareTo(node2.F));
        foreach (var nextNode in nextNodes)
        {
            if (nextNode.room == Fnode.room)
            {
                return true;
            }
            else
            {
                if (Search(nextNode))
                { // Note: Recurses back into Search(Node)
                    //Debug.DrawLine(currentNode.room.transform.position, nextNode.room.transform.position, Color.green, 10, false);
                    return true;
                }
                else
                {
                    Debug.DrawLine(currentNode.room.transform.position, nextNode.room.transform.position, Color.red, 10, false);

                }
            }
        }
        return false;
    }
    public List<Room> FindPath()
    {
        List<Room> path = new List<Room>();
        if (Snode == null || Fnode == null) { Debug.LogWarning("Start and Finish node not found!"); return path; }
        bool wasFound = Search(Snode);
        //Debug.Log(wasFound);
        if (wasFound)
        {
            Node node = Fnode;
            while (node.parentNode != null)
            {
                path.Add(node.room);
                Debug.DrawLine(node.room.transform.position, node.parentNode.room.transform.position, Color.green, 10, false);
                node = node.parentNode;
            }
            path.Reverse();
        }
        AllNodes.Clear();
        return path;
    }
    // Update is called once per frame
    /*void Update()
    {
        
    }*/
}
