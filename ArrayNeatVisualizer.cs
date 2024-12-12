using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ArrayNeatVisualizer : NeatViualizer
{
    [SerializeField]
    private GameObject SquarePrefab;
    [HideInInspector]
    public int Behind;
    [HideInInspector]
    public int Ahead;
    [HideInInspector]
    public int Above;
    [HideInInspector]
    public int Below;
    [SerializeField]
    public LevelCreator LC;
    private Dictionary<int, Color> ColorDict = new Dictionary<int, Color>();
    [SerializeField]
    private Color PlayerColor;
    private float player;
    
    [SerializeField]
    private Transform SqaureParent;
    private void Start()
    {
        ColorDict.Add(0, new Color(0, 0, 0, 0.1f));
        foreach (ColorToObject CtO in LC.AllObjects)
        {
            if (ColorDict.ContainsKey(CtO.Val))
                continue;
            ColorDict.Add(CtO.Val, CtO.Col);
        }
    }

   
    protected override IEnumerator DisplayPosCor(Genome g)
    {
        foreach (Transform t in NodesParent)
        {
            Destroy(t.gameObject);
        }
        foreach (Transform t in SqaureParent)
        {
            Destroy(t.gameObject);
        }
        foreach (Transform t in Lines)
        {
            Destroy(t.gameObject);
        }

        NodeDict.Clear();
        LinesDict.Clear();
        g.SortNodes();
        myG = g;
        int MaxLayer = g.NodeGenes[g.NodeGenes.Count - 1].Layer;
        int CurrentLayer = 0;

        while (CurrentLayer <= MaxLayer)
        {
            List<int> AllInthisLayer = new List<int>();
            bool foundnew = false;
            int templayer = 0;
            for (int i = 0; i < g.NodeGenes.Count; i++)
            {
                NodeGene ng = g.NodeGenes[i];
                if (ng.Layer == CurrentLayer)
                {
                    AllInthisLayer.Add(ng.NodeID);
                }
                else if (ng.Layer > CurrentLayer)
                {
                    templayer = ng.Layer;
                    foundnew = true;
                    break;
                }

            }

            int length = (Behind + Ahead + 1);
            int height = (Above + Below + 1);
            player = Behind * height + Below;
            float StartXArray = StartX - length * SquarePrefab.GetComponent<RectTransform>().sizeDelta.x;
            float StartYArray = (EndY + StartY) / 2 - (height * SquarePrefab.GetComponent<RectTransform>().sizeDelta.y) / 2;
            if (AllInthisLayer.Count > 0)
            {
                AllInthisLayer.Sort();
                int max = AllInthisLayer[AllInthisLayer.Count - 1];
                for (int i = 0; i < AllInthisLayer.Count; i++)
                {
                    GameObject tospawn = CurrentLayer == 0 ? SquarePrefab : NodePrefab;
                    Transform parent = CurrentLayer == 0 ? SqaureParent : NodesParent;
                    GameObject newCircle = Instantiate(tospawn, parent);
                    newCircle.name = AllInthisLayer[i].ToString();
                    Vector3 p = new Vector3();
                    if (CurrentLayer == 0)
                    {
                        int xpos = AllInthisLayer[i] / (height);
                        int ypos = (AllInthisLayer[i] - (height) * xpos);
                        p = new Vector3(StartXArray + SquarePrefab.GetComponent<RectTransform>().sizeDelta.x * xpos, StartYArray + SquarePrefab.GetComponent<RectTransform>().sizeDelta.y * ypos);

                    }
                    else
                    {

                        p = new Vector3(Mathf.Lerp(StartX, EndX, (float)CurrentLayer / MaxLayer), Mathf.Lerp(StartY, EndY, 1 - (float)(i + 1) / (AllInthisLayer.Count + 1)));
                    }
                    newCircle.transform.localPosition = p;
                    if (AllInthisLayer[i] == player)
                    {
                        newCircle.GetComponent<Image>().color = Color.red;

                    }
                    NodeDict[AllInthisLayer[i]] = newCircle.transform;
                }
            }


            if (!foundnew)
            {
                break;
            }
            else
            {
                CurrentLayer = templayer;
            }

        }


        for (int i = 0; i < g.ConnectionGenes.Count; i++)
        {
            ConnectionGene cg = g.ConnectionGenes[i];
            GameObject newConnection = Instantiate(LinePrefab, Lines);
            Transform t1, t2;
            t1 = NodeDict[cg.inNode];
            t2 = NodeDict[cg.outNode];
            newConnection.transform.position = (t1.transform.position + t2.transform.position) / 2;
            float mult = t2.position.y < t1.position.y ? -1 : 1;
            Image bar = newConnection.GetComponent<Image>();
            bar.rectTransform.sizeDelta = new Vector2(Vector3.Distance(t1.transform.position, t2.transform.position), Mathf.Abs(cg.weight) * LineWidth);
            bar.color = !cg.Expressed ? Color.grey : cg.weight < 0 ? Color.blue : Color.red;
            float angle = Vector2.Angle(Vector2.right, (t2.transform.position - t1.transform.position).normalized) * mult;
            bar.transform.localEulerAngles = new Vector3(0, 0, angle);
            LinesDict[cg.InnovationNumber] = newConnection.transform;
            if ((t2.transform.position - t1.transform.position).magnitude < 0.1f)
                Debug.Log(cg.inNode + " " + cg.outNode + " " + NodeDict[cg.inNode].name + " " + NodeDict[cg.outNode].name + NodeDict[cg.inNode].position + " " + NodeDict[cg.outNode].position);

        }

        //Transform bias1 = NodeDict[InputTexts.Length + OutputNames.Length];
        //GameObject btext = Instantiate(StartTextPrefab, Lines);
        //btext.transform.position = bias1.transform.position - new Vector3(15, 0, 0);
        //btext.GetComponent<TextMeshProUGUI>().text = "Bias";

        //for (int i = 0; i < InputTexts.Length; i++)
        //{
        //    Transform t1 = NodeDict[i];

        //    GameObject text = Instantiate(StartTextPrefab, Lines);
        //    text.transform.position = t1.position - new Vector3(15, 0, 0);
        //    text.GetComponent<TextMeshProUGUI>().text = InputTexts[i];
        //}

        //for (int i = 0; i < OutputNames.Length; i++)
        //{
        //    Transform t1 = NodeDict[InputTexts.Length + i];
        //    GameObject text = Instantiate(EndTextPrefab, Lines);
        //    text.transform.position = t1.transform.position + new Vector3(15, 0, 0);
        //    text.GetComponent<TextMeshProUGUI>().text = OutputNames[i];
        //}

        yield return null;
    }

    private void Update()
    {

        if (IAI != null)
        {

            foreach (ConnectionGene cg1 in myG.ConnectionGenes)
            {
                Transform cg = LinesDict[cg1.InnovationNumber];
                Color c = cg.GetComponent<Image>().color;
                c.a =  Mathf.Max(0.2f,Mathf.Abs(myG.GetNode(cg1.inNode).Value));
                cg.GetComponent<Image>().color = c;
            }

            foreach (NodeGene ng in myG.NodeGenes)
            {
                if (ng.Layer > 0)
                {
                    Transform cg = NodeDict[ng.NodeID];
                    Color c = ng.Value > 0 ? Color.red : Color.blue;
                    c.a = Mathf.Max(0.2f, Mathf.Abs(ng.Value));
                    cg.GetComponent<Image>().color = c;
                }
            }

            for (int i = 0; i < IAI.Features.Length; i++)
            {

                float v = IAI.Features[i];
                NodeDict[i].GetComponent<Image>().color = ColorDict[(int)v];
                if (i == player)
                    NodeDict[i].GetComponent<Image>().color = PlayerColor;
            }

        }
    }
}