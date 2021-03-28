using System;
using System.Collections.Generic;
using System.Linq;

namespace AStarPathFinder
{
    // This is a quick and dirty port of the Python code from:
    // https://medium.com/@nicholas.w.swift/easy-a-star-pathfinding-7e6689c7f7b2
    //
    // We aren't trying to do anything special or claim any brilliant insights
    // on A* algorithm implementations. We simply need a quick and dirty
    // implementation for a game so that we don't have to pull in any external
    // dependencies that in turn bring in their own view of the world which 
    // never meshes with ours.
    public class Point
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    public class AStarPathfinder
    {
        private int width;
        private int height;
        private readonly int[,] map;

        private class AStarNode
        {
            public AStarNode parent { get; set; }
            public Point position { get; set; }

            public int f { get; set; }
            public int g { get; set; }
            public int h { get; set; }

            public AStarNode(AStarNode p, Point pos)
            {
                parent = p;
                position = pos;
            }

            public bool eq(AStarNode x)
            {
                if (x == null) return false;

                return (position.X == x.position.X &&
                        position.Y == x.position.Y);
            }
        }

        public AStarPathfinder(int[,] m, int w, int h)
        {
            map = m;
            width = w;
            height = h;
        }

        public List<Point> FindPath(Point start, Point end)
        {
            var path = new List<Point>();
            var startNode = new AStarNode(null, start);
            startNode.g = startNode.h = startNode.f = 0;
            var endNode = new AStarNode(null, end);
            endNode.g = endNode.h = endNode.f = 0;
            var openList = new List<AStarNode>();
            var closedList = new List<AStarNode>();
            openList.Add(startNode);

            while (openList.Count > 0)
            {
                var currentNode = openList.First();
                var currentIndex = 0;

                var _index = 0;
                foreach (var item in openList)
                {
                    if (item.f < currentNode.f)
                    {
                        currentNode = item;
                        currentIndex = _index;
                    }
                    _index++;
                }

                openList.RemoveAt(currentIndex);
                closedList.Add(currentNode);

                if (currentNode.eq(endNode))
                {
                    var current = currentNode;
                    while (current != null)
                    {
                        path.Add(current.position);
                        current = current.parent;
                    }

                    path.Reverse();
                    return path;
                }

                var children = new List<AStarNode>();

                Point[] posArray = {
                    new Point(0, -1),
                    new Point(0, 1),
                    new Point(-1, 0),
                    new Point(1, 0)
                    // I don't personally want diagonals to be selected
                    //
                    //new Point(-1, -1), 
                    //new Point(-1, 1), 
                    //new Point(1, -1), 
                    //new Point(1, 1)
                };

                foreach (var newPosition in posArray)
                {
                    var nodePosition = new Point(currentNode.position.X + newPosition.X, currentNode.position.Y + newPosition.Y);

                    if (nodePosition.X > (width - 1) || nodePosition.X < 0 ||
                        nodePosition.Y > (height - 1) || nodePosition.Y < 0) continue;

                    if (map[nodePosition.Y, nodePosition.X] != 0) continue;

                    children.Add(new AStarNode(currentNode, nodePosition));
                }

                foreach (var child in children)
                {
                    if (closedList.FirstOrDefault(x => x.eq(child)) != null) continue;

                    child.g = currentNode.g + 1;
                    child.h = (int)Math.Pow((child.position.X - endNode.position.X), 2) + (int)Math.Pow(child.position.Y - endNode.position.Y, 2);
                    child.f = child.g + child.h;

                    foreach (var openNode in openList)
                        if (child.eq(openNode) && child.g > openNode.g) continue;

                    openList.Add(child);
                }
            }

            return path;
        }
    }


    class Program
    {
        static void Main(string[] args)
        {
            int[,] map =
            {
                { 0, 0, 0, 0, 1, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 1, 0, 0, 1, 0, 0 },
                { 0, 1, 1, 1, 1, 0, 0, 1, 0, 0 },
                { 0, 1, 0, 0, 1, 1, 1, 1, 0, 0 },
                { 0, 1, 0, 0, 0, 0, 1, 0, 0, 0 },
                { 0, 1, 0, 1, 1, 0, 1, 0, 0, 0 },
                { 0, 1, 0, 0, 1, 0, 1, 0, 0, 0 },
                { 0, 1, 1, 0, 1, 0, 1, 1, 1, 0 },
                { 0, 0, 0, 0, 1, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 1, 0, 0, 0, 0, 0 }
            };

            var astar = new AStarPathfinder(map, 10, 10);
            var path = astar.FindPath(new Point(1, 0), new Point(6, 2));

            //foreach (var p in path)
            //{
            //    Console.WriteLine($"({p.X},{p.Y})");
            //}

            for (int row = 0; row < 10; row++)
            {
                for (int col = 0; col < 10; col++)
                {
                    var p = path.FirstOrDefault(s => s.X == col && s.Y == row);

                    if (p != null) Console.Write("+");
                    else if (map[row, col] == 0) Console.Write(".");
                    else if (map[row, col] == 1) Console.Write("#");
                }
                Console.WriteLine();
            }
        }
    }
}
