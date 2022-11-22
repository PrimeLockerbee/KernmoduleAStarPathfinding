using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Astar
{
    [SerializeField] public int i_MaxLoops = 10000;

    [HideInInspector] protected int i_Loops = 0;

    private List<Node> l_openNodes = new List<Node>();
    private List<Node> l_closedNodes = new List<Node>();

    public List<Vector2Int> FindPathToTarget(Vector2Int startPos, Vector2Int endPos, Cell[,] grid)
    {
        l_openNodes.Add(new Node(startPos, null, 0, 0));

        Dictionary<Vector2Int, Wall> directionToDirection = new Dictionary<Vector2Int, Wall>()
        {
            { new Vector2Int(0, 1), Wall.UP },
            { new Vector2Int(0, -1), Wall.DOWN },
            { new Vector2Int(1, 0), Wall.RIGHT },
            { new Vector2Int(-1, 0), Wall.LEFT }
        };

        while (l_openNodes.Count > 0 && i_Loops < i_MaxLoops)
        {
            i_Loops++;

            l_openNodes = l_openNodes.OrderByDescending(x => x.FScore).ToList();

            Node current = l_openNodes[0];

            l_openNodes.Remove(current);

            l_closedNodes.Add(current);

            if (current.position == endPos)
            {
                List<Vector2Int> path = new List<Vector2Int>();

                Node currentBack = current;

                while (currentBack != null)
                {
                    path.Add(currentBack.position);

                    currentBack = currentBack.parent;
                }

                path.Reverse();

                return path;
            }

            Cell currentCell = grid[current.position.x, current.position.y];

            List<Cell> neighbors = currentCell.GetNeighbours(grid);

            List<Node> adjacent = new List<Node>();

            foreach (var neighbor in neighbors)
            {
                if (!currentCell.HasWall(directionToDirection[neighbor.gridPosition - currentCell.gridPosition]))
                {
                    adjacent.Add(new Node(neighbor.gridPosition, current, 0, 0));
                }
            }

            foreach (var node in adjacent)
            {
                if (l_closedNodes.Any(x => x.EqualTo(node)))
                {
                    continue;
                }

                node.GScore = current.GScore + 1;

                node.HScore = Vector2Int.Distance(node.position, endPos);

                if (l_openNodes.Any(x => x.EqualTo(node)))
                {
                    Node inList = l_openNodes.Where(x => x.EqualTo(node)).ToList()[0];

                    if (inList.GScore < node.GScore)
                    {
                        l_openNodes.Remove(inList);

                        l_openNodes.Add(node);
                    }
                }
                else
                {
                    l_openNodes.Add(node);
                }
            }
        }

        return null;
    }

    public class Node
    {
        public Vector2Int position; //Position on the grid
        public Node parent; //Parent Node of this node

        public float FScore { //GScore + HScore
            get { return GScore + HScore; }
        }
        public float GScore; //Current Travelled Distance
        public float HScore; //Distance estimated based on Heuristic

        public bool EqualTo(Node a) => a.position == position;

        public Node() { }

        public Node(Vector2Int position, Node parent, int GScore, int HScore)
        {
            this.position = position;
            this.parent = parent;
            this.GScore = GScore;
            this.HScore = HScore;
        }
    }
}
