using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WorldGen : MonoBehaviour
{
    /*
        Start with an 8x8 grid. For each cell there is a chance it splits into 4 cells. This loop happens twice.
        So there will be three possible sizes.
        For each cell, call IslandGen and get an island of the corresponding size (as a game object).
        Move that object to the middle of the cell (+- a seeded random offset)

        TODO:
        - Eventually hook up island reference to some manager somewheres
        - Add ID to islands
        - Maybe apply a random scale to each island (will require filling out the gaps in the tiles)
    */

    private TreeNode root;
    public int numStartingCells;
    private int splits = 2;
    public float[] splitChances = new float[2]; //first element is chance for first split, etc...
    public float noIslandChance; //chance that a given cell will only contain ocean
    public int seed;
    private float baseCellSize = 1;
    public IslandGen islandGen;
    public int resolution;
    public Transform islandParent;
    private int islandCounter = 0;


    public class TreeNode
    {
        public int nodeID;
        public int splitCount;
        public Vector2 position;
        public List<TreeNode> children = new List<TreeNode>();
    }

    void Start()
    {
        baseCellSize = resolution / resolution;
        Random.InitState(seed);

        root = new TreeNode { nodeID = -1 };

        //create 16 children (4^2)
        for (int x = 0; x < numStartingCells; x ++)
        {
            for (int y = 0; y < numStartingCells; y ++)
            {
                TreeNode child = new TreeNode
                { 
                    position = new Vector2(baseCellSize * x, baseCellSize * y) //base cell size is just 1
                };
                root.children.Add(child);
            }
        }
        DFS(root);
    }

    public void DFS(TreeNode node)
    {
        if (node == null) return;

        //Proccess node, skipping root node (not a cell)
        if (node.nodeID != -1)
            ProcessNode(node);

        foreach (TreeNode child in node.children)
        {
            DFS(child);
        }
    }

    public void ProcessNode(TreeNode node)
    {
        if (node.splitCount < splits && Random.value < splitChances[node.splitCount])
        {
            // SPLIT
            for(int i = 0; i < 4; i ++)
            {
                //set each child's position based on parent position
                float childX;
                float childY;
        
                //Adding or subtracting to parent position depends on i, which represents the quadrant the island will be moved to
                //relative to the parent cell
                float curCellSize = baseCellSize / (4 * (node.splitCount + 1)); // base cell size is just 1

                if (i == 1 || i == 3) childX = node.position.x + curCellSize;  //right
                else childX = node.position.x - curCellSize;                   //left

                if (i == 0 || i == 1) childY = node.position.y + curCellSize;  //top
                else childY = node.position.y - curCellSize;                   //bottom

                TreeNode child = new TreeNode 
                { 
                    splitCount = node.splitCount + 1,
                    position = new Vector2(childX, childY)
                };
                
                node.children.Add(child);
            }
        }
        else
        {
            // DON'T SPLIT
            //Call IslandGen to get an island
            //size based on node.splitCount
            //adjust scale for new ppu
            GameObject newIsland;
            if (Random.value < noIslandChance)
                newIsland = islandGen.Generate(Random.Range(0, 10000), -1f, true, resolution, islandCounter); // just water (maybe with a lil something)
            else if (node.splitCount == 0)
            {
                newIsland = islandGen.Generate(Random.Range(0, 10000), 0.05f, false, resolution * 4, islandCounter); // large
                //newIsland.transform.localScale = newIsland.transform.localScale / 4;
            }
                
            else if (node.splitCount == 1)
            {
                newIsland = islandGen.Generate(Random.Range(0, 10000), 0.0f, false, resolution * 2, islandCounter); // medium
                //newIsland.transform.localScale = newIsland.transform.localScale / 2;
            }
                
            else
                newIsland = islandGen.Generate(Random.Range(0, 10000), -0.05f, false, resolution, islandCounter); // small

            //move island to node's position
            newIsland.transform.position = node.position;

            if (node.splitCount > 0)
                newIsland.transform.localScale = newIsland.transform.localScale / (2 * node.splitCount);

            newIsland.transform.SetParent(islandParent);
            islandCounter ++;
            //add newIsland to some manager list here
        }
    }
}
