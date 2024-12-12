using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Genome : MonoBehaviour
{
    public static int InnovationNumber = 0;
    public List<ConnectionGene> ConnectionGenes;
    public static List<ConnectionHistory> MainConnectionHistory = new List<ConnectionHistory>();
    public List<NodeGene> NodeGenes;
    public int CurrentNodeID;
    private int InputCount;
    private int OutputCount;
    public int BiasNode;
    private bool Loaded;

    public void SortNodes()
    {
        NodeGenes.Sort(SortyByNodeID);

        NodeGenes.Sort(SortByNodeLayer);
    }

    public Genome(Genome p1, Genome p2)
    {
        Crossover(p1, p2);
    }

    public Genome(Genome p1)
    {
        CopyParent(p1);
    }

    public Genome(int input, int output)
    {
        InputCount = input;
        OutputCount = output;
        ConnectionGenes = new List<ConnectionGene>();
        NodeGenes = new List<NodeGene>();
        StartNN(InputCount, OutputCount);
    }
    public void StartNN(int InputNode, int OutputNodes)
    {
        CurrentNodeID = 0;
        NodeGene ng;
        for (int i = 0; i < InputNode; i++)
        {
            ng = new NodeGene(CurrentNodeID, 0);
            NodeGenes.Add(ng);
            CurrentNodeID++;

        }

        for (int i = 0; i < OutputNodes; i++)
        {
            ng = new NodeGene(CurrentNodeID, 1);
            NodeGenes.Add(ng);
            CurrentNodeID++;
        }


        ng = new NodeGene(CurrentNodeID, 0);
        ng.Value = 1;
        NodeGenes.Add(ng);
        BiasNode = CurrentNodeID;
        CurrentNodeID++;

    }


    public void addNodeMutation(List<ConnectionHistory> connectionHistory)
    {
        if (ConnectionGenes.Count == 0 || (ConnectionGenes.Count == 1 && ConnectionGenes[0].inNode == BiasNode))
        {
            addConnecitonMutation(connectionHistory);
            return;
        }



        ConnectionGene CG = ConnectionGenes[Random.Range(0, ConnectionGenes.Count)];

        int loop = 0;
        while (CG.inNode == BiasNode)
        {
            loop++;
            CG = ConnectionGenes[Random.Range(0, ConnectionGenes.Count)];
            if (loop == 15)
            {
                addConnecitonMutation(connectionHistory);

                return;
            }
        }

        CG.Expressed = false;
        NodeGene ng = new NodeGene(CurrentNodeID, GetNode(CG.inNode).Layer + 1);
        int CurrentLayer = GetNode(CG.outNode).Layer;
        CurrentNodeID++;
        int NewInnovationNumber = GetInnovationNumber(connectionHistory, CG.inNode, ng.NodeID);

        ConnectionGene newConnection = new ConnectionGene(CG.inNode, ng.NodeID, 1, true, NewInnovationNumber);
        ConnectionGenes.Add(newConnection);
        NewInnovationNumber = GetInnovationNumber(connectionHistory, ng.NodeID, CG.outNode);
        newConnection = new ConnectionGene(ng.NodeID, CG.outNode, CG.weight, true, NewInnovationNumber);
        ConnectionGenes.Add(newConnection);

        NewInnovationNumber = GetInnovationNumber(connectionHistory, BiasNode, ng.NodeID);
        newConnection = new ConnectionGene(BiasNode, ng.NodeID, 0, true, NewInnovationNumber);
        ConnectionGenes.Add(newConnection);
        NodeGenes.Add(ng);

        if (CurrentLayer == ng.Layer)
        {
            for (int i = 0; i < NodeGenes.Count - 1; i++)
            {
                if (NodeGenes[i].Layer >= CurrentLayer)
                {
                    NodeGenes[i].Layer++;
                }
            }
        }
        //PrintMyStuff();

    }


    public void addConnecitonMutation(List<ConnectionHistory> connectionHistory)
    {
        NodeGene n1 = NodeGenes[Random.Range(0, NodeGenes.Count)];
        NodeGene n2 = NodeGenes[Random.Range(0, NodeGenes.Count)];
        int loop = 0;

        while (n1.Layer == n2.Layer || n1.NodeID == n2.NodeID)
        {
            n1 = NodeGenes[Random.Range(0, NodeGenes.Count)];
            n2 = NodeGenes[Random.Range(0, NodeGenes.Count)];
            loop++;
            if (loop == 100)
            {

                break;
            }
        }

        bool reversed = (n1.Layer > n2.Layer);
        bool ConnectionExists = false;

        foreach (ConnectionGene CG in ConnectionGenes)
        {
            if ((CG.inNode == n1.NodeID && CG.outNode == n2.NodeID) || (CG.inNode == n2.NodeID && CG.outNode == n1.NodeID))
            {
                ConnectionExists = true;
                MutateWeight(CG.InnovationNumber);
                return;
            }
        }

        int innov = GetInnovationNumber(connectionHistory, n1.NodeID, n2.NodeID);
        if (!ConnectionExists)
        {
            float weight = Random.Range(-1f, 1f);

            ConnectionGene newGene = new ConnectionGene(reversed ? n2.NodeID : n1.NodeID, reversed ? n1.NodeID : n2.NodeID, weight, true, innov);
            ConnectionGenes.Add(newGene);
        }
    }

    public string PrintMyStuff()
    {
        string nodes = "";
        NodeGenes.Sort(SortByNodeLayer);
        int prevnodes = 0;
        for (int i = 0; i < NodeGenes.Count; i++)
        {
            if (NodeGenes[i].Layer > prevnodes)
            {
                nodes += ",";
                prevnodes = NodeGenes[i].Layer;

            }
            nodes += "(" + NodeGenes[i].NodeID.ToString() + "," + NodeGenes[i].Layer.ToString() + ")";

        }
        string links = "";
        for (int i = 0; i < ConnectionGenes.Count; i++)
        {
            links += "(" + ConnectionGenes[i].inNode.ToString() + "," + ConnectionGenes[i].outNode + "," + ConnectionGenes[i].weight.ToString("F2") + "," + ConnectionGenes[i].InnovationNumber.ToString() + "," + ConnectionGenes[i].Expressed.ToString() + "),";
        }
        return "Nodes: " + nodes + " Links: " + links;
    }

    public void CreateNode(int id, int layer)
    {
        NodeGene ng = new NodeGene(id, layer);
        if (id == BiasNode)
            ng.Value = 1;
        NodeGenes.Add(ng);
    }

    public void CreateLink(int from, int to, float weight, bool e, int i)
    {
        ConnectionGene cg = new ConnectionGene(from, to, weight, e, i);
        ConnectionGenes.Add(cg);
    }

    void MutateWeight(int innovNumber)
    {
        if (ConnectionGenes.Count > 0)
        {
            ConnectionGene cg = GetConnection(innovNumber);
            float r = Random.value;
            if (r < 0.2)
            {
                cg.weight = Random.Range(-1f, 1f);
            }
            else if (r < 0.3)
            {
                cg.Expressed = !cg.Expressed;
            }
            else
            {
                cg.weight += Random.Range(-0.1f, 0.1f);
                cg.weight = Mathf.Clamp(cg.weight, -1, 1);
            }
        }
    }


    public int GetInnovationNumber(List<ConnectionHistory> connectionHistory, int From, int To)
    {
        bool isNew = true;
        int output = 0;
        for (int i = 0; i < connectionHistory.Count; i++)
        {
            ConnectionHistory ch = connectionHistory[i];
            if (ch.Matches(ConnectionGenes, From, To))
            {
                output = ch.InnovationNumber;
                isNew = false;
                break;
            }
        }

        if (isNew)
        {
            ConnectionHistory NH = new ConnectionHistory();

            List<int> NewConnectionINs = new List<int>();
            foreach (ConnectionGene cg in ConnectionGenes)
            {
                NewConnectionINs.Add(cg.InnovationNumber);
            }
            NH.From = From;
            NH.To = To;
            NH.InnovationNumber = InnovationNumber;
            output = InnovationNumber;
            NH.AllConnections = new List<int>(NewConnectionINs);
            connectionHistory.Add(NH);
            InnovationNumber++;
        }

        return output;
    }

    // Parent 1 has higher fitness than parent 2
    public void Crossover(Genome Parent1, Genome Parent2)
    {
        InputCount = Parent1.InputCount;
        OutputCount = Parent1.OutputCount;
        CurrentNodeID = Parent1.CurrentNodeID;
        BiasNode = Parent1.BiasNode;
        NodeGenes = new List<NodeGene>();
        ConnectionGenes = new List<ConnectionGene>();
        foreach (NodeGene NG in Parent1.NodeGenes)
        {
            NodeGene newG = new NodeGene(NG.NodeID, NG.Layer);
            NodeGenes.Add(newG);
        }

        foreach (ConnectionGene CG in Parent1.ConnectionGenes)
        {
            foreach (ConnectionGene CG2 in Parent2.ConnectionGenes)
            {
                if (CG.InnovationNumber == CG2.InnovationNumber)
                {
                    float RandomVal = (!CG.Expressed || !CG2.Expressed) ? 0.75f : 0;
                    ConnectionGene newCG = new ConnectionGene(CG.inNode, CG.outNode, CG.weight, Random.value < RandomVal ? false : true, CG.InnovationNumber);
                    ConnectionGenes.Add(new ConnectionGene(newCG.inNode, newCG.outNode, newCG.weight, newCG.Expressed, newCG.InnovationNumber));
                    break;
                }
                else
                {
                    ConnectionGene newCG = new ConnectionGene(CG.inNode, CG.outNode, CG.weight, CG.Expressed, CG.InnovationNumber); ;
                    ConnectionGenes.Add(new ConnectionGene(newCG.inNode, newCG.outNode, newCG.weight, newCG.Expressed, newCG.InnovationNumber));
                    break;
                }
            }
        }
    }

    public void CopyParent(Genome parent)
    {
        InputCount = parent.InputCount;
        OutputCount = parent.OutputCount;
        CurrentNodeID = parent.CurrentNodeID;
        BiasNode = parent.BiasNode;
        NodeGenes = new List<NodeGene>();
        ConnectionGenes = new List<ConnectionGene>();
        foreach (NodeGene NG in parent.NodeGenes)
        {
            NodeGene newG = new NodeGene(NG.NodeID, NG.Layer);
            NodeGenes.Add(newG);
        }

        foreach (ConnectionGene CG in parent.ConnectionGenes)
        {
            ConnectionGene newCG = new ConnectionGene(CG.inNode, CG.outNode, CG.weight, CG.Expressed, CG.InnovationNumber); ;
            ConnectionGenes.Add(new ConnectionGene(newCG.inNode, newCG.outNode, newCG.weight, newCG.Expressed, newCG.InnovationNumber));
        }
    }


    public float[] FeedForward(float[] Inputs)
    {
        float[] outputs = new float[OutputCount];

        if (NodeGenes.Count > 0)
        {
            NodeGenes.Sort(SortByNodeLayer);

            for (int i = 0; i < NodeGenes.Count; i++)
            {
                NodeGenes[i].Value = 0;
                if (NodeGenes[i].NodeID == BiasNode)
                {
                    NodeGenes[i].Value = 1;
                }
            }

            for (int i = 0; i < Inputs.Length; i++)
            {
                GetNode(i).Value = Inputs[i];
            }

            for (int i = 0; i < NodeGenes.Count; i++)
            {
                NodeGene ng = NodeGenes[i];
                int id = ng.NodeID;
                for (int j = 0; j < ConnectionGenes.Count; j++)
                {

                    ConnectionGene cg = ConnectionGenes[j];
                    if (cg.inNode == id && cg.Expressed && cg.inNode != cg.outNode)
                    {
                        float value = ng.Value;
                        if (GetNode(cg.inNode).Layer > 0) value = Sigmoid(value);
                        GetNode(cg.outNode).Value += value * cg.weight;

                    }
                }
            }


            for (int i = 0; i < OutputCount; i++)
            {
                outputs[i] = GetNode(InputCount + i).Value;
            }

        }
        return outputs;

    }
    public float Sigmoid(float x)
    {
        return 1 / (1 + Mathf.Exp(-2f * x));
    }
    public NodeGene GetNode(int id)//Get a node by it's ID
    {

        foreach (NodeGene ng in NodeGenes)
        {
            if (ng.NodeID == id)
            {
                return ng;
            }
        }
        return null;
    }

    public ConnectionGene GetConnection(int innovNum)//Get a connection by it's historical id
    {
        foreach (ConnectionGene cg in ConnectionGenes)
        {
            if (cg.InnovationNumber == innovNum)
            {
                return cg;
            }
        }
        return null;
    }

    public int SortByNodeLayer(NodeGene n1, NodeGene n2)
    {
        return n1.Layer.CompareTo(n2.Layer);
    }

    public int SortyByNodeID(NodeGene n1, NodeGene n2)
    {
        return n1.NodeID.CompareTo(n2.NodeID);
    }

    public void RandomExpress()
    {
        if (ConnectionGenes.Count > 0)
            ConnectionGenes[Random.Range(0, ConnectionGenes.Count)].Expressed = !ConnectionGenes[Random.Range(0, ConnectionGenes.Count)].Expressed;
    }

    public void Mutate()
    {
        int amount = Random.Range(0, 5);
        for (int i = 0; i < amount; i++)
        {
            float rand = Random.value;
            if (rand < 0.6f && ConnectionGenes.Count > 0)
            {
                MutateWeight(ConnectionGenes[Random.Range(0, ConnectionGenes.Count - 1)].InnovationNumber);
            }
            else if (rand < 0.95f)
            {
                addConnecitonMutation(MainConnectionHistory);
            }
            else
            {
                addNodeMutation(MainConnectionHistory);

            }
        }

    }



    public float CalculateDifference(Genome other, float c1, float c2, float c3, float N = 1)
    {
        float ExcessAmount = 0;
        float DisjointAmount = 0;
        float AverageWeightDifference = 0;
        List<int> FoundLinks = new List<int>();//This list contains all matching links, used to find excess linnks

        foreach (ConnectionGene cg in ConnectionGenes)
        {
            bool foundSame = false;
            foreach (ConnectionGene cg2 in other.ConnectionGenes)
            {
                if (cg.InnovationNumber == cg2.InnovationNumber)
                {
                    AverageWeightDifference += Mathf.Abs(cg.weight - cg2.weight);
                    FoundLinks.Add(cg.InnovationNumber);
                    foundSame = true;
                    break;
                }
            }
            if (!foundSame)
            {
                DisjointAmount++;
            }
        }



        foreach (ConnectionGene cg2 in other.ConnectionGenes)
        {
            if (!FoundLinks.Contains(cg2.InnovationNumber))
            {
                ExcessAmount++;
            }
        }
        if (FoundLinks.Count > 0)
            AverageWeightDifference = AverageWeightDifference / FoundLinks.Count;

        return ((c1 * ExcessAmount) / N) + ((c2 * DisjointAmount) / N) + (c3 * AverageWeightDifference);
    }
}


public class ConnectionGene
{
    public ConnectionGene()
    {

    }
    public ConnectionGene(int input, int output, float w, bool expressed, int inovation)
    {
        inNode = input;
        outNode = output;
        weight = w;
        Expressed = expressed;
        InnovationNumber = inovation;
    }
    public int inNode;
    public int outNode;
    public float weight;
    public bool Expressed;
    public int InnovationNumber;
}

public class NodeGene
{

    public NodeGene(int ID, int layer)
    {
        NodeID = ID;
        Value = 0;
        Layer = layer;

    }
    public int NodeID;
    public float Value;
    public int Layer;

}




public class ConnectionHistory
{
    public int From;
    public int To;
    public int InnovationNumber;
    public List<int> AllConnections = new List<int>();

    public bool Matches(List<ConnectionGene> Conections, int from, int to)
    {
        if (Conections.Count == AllConnections.Count)
        {
            for (int i = 0; i < AllConnections.Count; i++)
            {
                if (!AllConnections.Contains(Conections[i].InnovationNumber))
                {
                    return false;
                }
            }
            return true;
        }
        return false;
    }
}