using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System;

public interface IQuadTreeObject
{
    Guid ID { get; }
    Rect rect { get; }
    Collider2D Collider { get; }
    void SetDebugColor(Color col);
}

class QuadTree<T> where T : IQuadTreeObject
{
    public Rect boundry;
    T[] nodes;
    bool root = false;
    bool divided = false;
    int numberOfNodesInserted = 0;
    int maxSize;
    QuadTree<T> northEast, northWest, southEast, southWest;
    public int Count()
    {
        int result = numberOfNodesInserted;
        if (divided && root)
        {
            result += northEast.Count();
            result += northWest.Count();
            result += southEast.Count();
            result += southWest.Count();
        }
        return result;
    }
    public QuadTree(Rect boundry, int size)
    {
        if (boundry.width == 0 || boundry.height == 0)
            Debug.LogError("Radius of the boundry cannot be zero.");

        nodes = new T[size];
        maxSize = size;
        this.boundry = boundry;
    }

    #region Methods

    //Clear all the nodes in the Quad-Tree
    public void ClearAllNodes()
    {
        if (numberOfNodesInserted == 0 && !root) return;
        numberOfNodesInserted = 0;
        root = false;

        if (divided)
        {
            northEast.ClearAllNodes();
            northWest.ClearAllNodes();
            southEast.ClearAllNodes();
            southWest.ClearAllNodes();
        }
        divided = false;
    }
    /// <summary>Insert a node in the Quad-Tree</summary>
    public bool Insert(T node)
    {
        if (node.rect.width == 0 || node.rect.height == 0)
			Debug.LogError("boundry cannot be zero.");
		
		//Checking if the position is in the boundries of the node.
		//if(!boundry.Contains(node.rect.center))
  //          return false;
		if(numberOfNodesInserted < maxSize && !root) 
		{
			nodes[numberOfNodesInserted] = node;
			numberOfNodesInserted++;
			return true;
		}
		else if(root)
		{
            if (northEast.boundry.Overlaps(node.rect))
                northEast.Insert(node);
            if (northWest.boundry.Overlaps(node.rect))
                northWest.Insert(node);
            if (southEast.boundry.Overlaps(node.rect))
                southEast.Insert(node);
            if (southWest.boundry.Overlaps(node.rect))
                southWest.Insert(node);

			//if(northEast.Insert(node)) return true;			
			//if(northWest.Insert(node)) return true;		
			//if(southEast.Insert(node)) return true;
			//if(southWest.Insert(node)) return true;	
		}
		else if(!root && numberOfNodesInserted == maxSize)
		{
			root = true;
			numberOfNodesInserted = 0;
            
			if(!divided)
				SubDivide();
            
            //Moving current nodes to subnodes
			for (int i = 0; i < maxSize; i++)
			{
                if (northEast.boundry.Overlaps(nodes[i].rect))
                    northEast.Insert(nodes[i]);
                if (northWest.boundry.Overlaps(nodes[i].rect))
                    northWest.Insert(nodes[i]);
                if (southEast.boundry.Overlaps(nodes[i].rect))
                    southEast.Insert(nodes[i]);
                if (southWest.boundry.Overlaps(nodes[i].rect))
                    southWest.Insert(nodes[i]);

    //            if (!northEast.Insert(nodes[i]))			
				//if(!northWest.Insert(nodes[i]))		
				//if(!southEast.Insert(nodes[i]))
				//if(!southWest.Insert(nodes[i])) { Debug.LogError("It should not reach here"); }
			}

            if (northEast.boundry.Overlaps(node.rect))
                northEast.Insert(node);
            if (northWest.boundry.Overlaps(node.rect))
                northWest.Insert(node);
            if (southEast.boundry.Overlaps(node.rect))
                southEast.Insert(node);
            if (southWest.boundry.Overlaps(node.rect))
                southWest.Insert(node);

   //         if (!northEast.Insert(node))			
			//if(!northWest.Insert(node))		
			//if(!southEast.Insert(node))
			//if(!southWest.Insert(node)) { Debug.LogError("It should not reach here"); }
			return true;
		}
		return false;
	}

    private List<IQuadTreeObject> QueryObject(Rect searchingArea)
    {
        var founded = new List<IQuadTreeObject>();
        if (numberOfNodesInserted == 0 && !root)
            return founded;
        if (!boundry.Overlaps(searchingArea))
            return founded;

        if (!root && numberOfNodesInserted != 0)
        {
            for (int i = 0; i < numberOfNodesInserted; i++)
            {
                if (searchingArea.Overlaps(nodes[i].rect))
                    founded.Add(nodes[i]);
            }
            return founded;
        }
        else if (root && numberOfNodesInserted == 0)
        {
            founded.AddRange(northEast.QueryObject(searchingArea));
            founded.AddRange(northWest.QueryObject(searchingArea));
            founded.AddRange(southEast.QueryObject(searchingArea));
            founded.AddRange(southWest.QueryObject(searchingArea));
        }

        return founded;
    }

    public List<IQuadTreeObject> Query(Rect searchingArea)
    {
        var foundList = QueryObject(searchingArea);
        var distinctList = foundList.Distinct().ToList();
        return distinctList;
    }
    #endregion

    #region HelperMethods
    /// <summary>Divide the Quadtree into 4 equal parts and set it's boundries, NorthEast, NorthWest, SouthEast and SouthWest.</summary>
    private void SubDivide() 
	{
		//Size of the sub boundries 
		if(northEast == null) 
		{	
			float x = boundry.x;
			float y = boundry.y;
			float width = boundry.width * 0.5f;
			float height = boundry.height * 0.5f;
	
			northEast = new QuadTree<T>(new Rect(x + width, y + height, width, height), maxSize);
			northWest = new QuadTree<T>(new Rect(x, y + height, width, height), maxSize);
			southEast = new QuadTree<T>(new Rect(x + width, y, width, height), maxSize);
			southWest = new QuadTree<T>(new Rect(x, y, width, height), maxSize);
		} 
		divided = true; 
	}

	// Shows boundires of the quadtree and SubNodes
	// public void LogNodes() 
	// {
	// 	if(numberOfNodesInserted == 0 && !root) return;
	// 	else if(root)
	// 	{
	// 		northEast.LogNodes();
	// 		northWest.LogNodes();
	// 		southWest.LogNodes();
	// 		southEast.LogNodes();
	// 		return;
	// 	}
	// 	else if(numberOfNodesInserted > 0)
	// 	{
	// 		for (int i = 0; i < numberOfNodesInserted; i++)
	// 		{	
	// 			Debug.Log(nodes[i].rect.x + " " + nodes[i].rect.y + " id:" + nodes[i].ID);	
	// 		}
	// 		return;
	// 	}
	// }
	
	public void ShowBoundries()
	{ 
		float x = boundry.x;
		float y = boundry.y;
		float w = boundry.width;
		float h = boundry.height;

		Vector2 bottomLeftPoint = new Vector2(x, y);
		Vector2 bottomRightPoint = new Vector2(x + w, y);
		Vector2 topRightPoint = new Vector2(x + w, y + h);
		Vector2 topLeftPoint = new Vector2(x, y + h);
		
		Debug.DrawLine(bottomLeftPoint, bottomRightPoint, Color.red);	//bottomLine
		Debug.DrawLine(bottomLeftPoint, topLeftPoint, Color.red);		//leftLine
		Debug.DrawLine(bottomRightPoint, topRightPoint, Color.red);		//rightLine
		Debug.DrawLine(topLeftPoint, topRightPoint, Color.red);			//topLine

		if(divided)
		{
			northEast.ShowBoundries();
			northWest.ShowBoundries();
			southEast.ShowBoundries();
			southWest.ShowBoundries();
		}
	}
	#endregion
}
