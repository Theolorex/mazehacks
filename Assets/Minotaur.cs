using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazePathfinder : MonoBehaviour
{
    [SerializeField] private Maze maze;

    private static readonly Vector2Int[] Directions =
    {
        Vector2Int.right,
        Vector2Int.left,
        Vector2Int.up,
        Vector2Int.down
    };

    public List<Vector2Int> FindPath(Vector2Int start, Vector2Int target)
    {
        if (!Walkable(start) || !Walkable(target))
            return null;

        var open = new List<Node> { new Node(start, 0, Distance(start, target), null) };
        var closed = new HashSet<Vector2Int>();

        while (open.Count > 0)
        {
            Node current = open[0];

            for (int i = 1; i < open.Count; i++)
                if (open[i].F < current.F)
                    current = open[i];

            open.Remove(current);
            closed.Add(current.Pos);

            if (current.Pos == target)
                return BuildPath(current);

            foreach (var dir in Directions)
            {
                Vector2Int pos = current.Pos + dir;

                if (!Walkable(pos) || closed.Contains(pos))
                    continue;

                int g = current.G + 1;
                Node existing = open.Find(n => n.Pos == pos);

                if (existing == null)
                {
                    open.Add(new Node(
                        pos,
                        g,
                        Distance(pos, target),
                        current
                    ));
                }
                else if (g < existing.G)
                {
                    existing.G = g;
                    existing.Parent = current;
                }
            }
        }

        return null;
    }

    private bool Walkable(Vector2Int p)
    {
        return p.x >= 0 &&
               p.y >= 0 &&
               p.x < maze.w &&
               p.y < maze.h &&
               maze.Get(p.x, p.y) != 1;
    }

    private int Distance(Vector2Int a, Vector2Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    private List<Vector2Int> BuildPath(Node node)
    {
        var path = new List<Vector2Int>();

        while (node != null)
        {
            path.Add(node.Pos);
            node = node.Parent;
        }

        path.Reverse();
        return path;
    }

    private class Node
    {
        public Vector2Int Pos;
        public int G;
        public int H;
        public Node Parent;

        public int F => G + H;

        public Node(Vector2Int pos, int g, int h, Node parent)
        {
            Pos = pos;
            G = g;
            H = h;
            Parent = parent;
        }
    }
}
