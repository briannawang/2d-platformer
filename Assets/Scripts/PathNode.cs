public class PathNode {
    public int x;
    public int y;

    public int gCost;
    public int hCost;
    public int fCost;

    public PathNode prevNode;

    public PathNode(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public PathNode(int x, int y, int gCost, PathNode prevNode)
    {
        this.x = x;
        this.y = y;
        this.gCost = gCost;
        this.prevNode = prevNode;
    }

    public void CalculateFCost()
    {
        fCost = gCost + hCost;
    }

    public override string ToString()
    {
        return x + ", " + y;
    }
}