using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Astar
{
    [SerializeField]
    public int i_MaxLoops = 10000;

    public List<Vector2Int> FindPathToTarget(Vector2Int startPos, Vector2Int endPos, Cell[,] grid)
    {
        List<Node> openNodes = new List<Node>();
        List<Node> closedNodes = new List<Node>();

        openNodes.Add(new Node(startPos, null, 0, 0));

        int loops = 0;

        Dictionary<Vector2Int, Wall> directionToDirection = new Dictionary<Vector2Int, Wall>()
        {
            { new Vector2Int(0, 1), Wall.UP },
            { new Vector2Int(0, -1), Wall.DOWN },
            { new Vector2Int(1, 0), Wall.RIGHT },
            { new Vector2Int(-1, 0), Wall.LEFT }
        };

        while (openNodes.Count > 0 && loops < i_MaxLoops)
        {
            loops++;

            openNodes = openNodes.OrderByDescending(x => x.FScore).ToList();

            Node current = openNodes[0];

            openNodes.Remove(current);

            closedNodes.Add(current);

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
                if (closedNodes.Any(x => x.EqualTo(node)))
                {
                    continue;
                }

                node.GScore = current.GScore + 1;

                node.HScore = Vector2Int.Distance(node.position, endPos);

                if (openNodes.Any(x => x.EqualTo(node)))
                {
                    Node inList = openNodes.Where(x => x.EqualTo(node)).ToList()[0];

                    if (inList.GScore < node.GScore)
                    {
                        openNodes.Remove(inList);

                        openNodes.Add(node);
                    }
                }
                else
                {
                    openNodes.Add(node);
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
