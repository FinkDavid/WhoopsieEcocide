using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.Collections.AllocatorManager;

public class Block : MonoBehaviour, IBlock
{
    private Material _material;

    private Shape _shape;

    private List<GameObject> _littleBlocks = new List<GameObject>();
    private Vector3 basePosition;
    private Vector3[] blockPositions = new Vector3[4];

    public int currentRotation = 0;

    private Dictionary<Material, Sprite> allSprites;

    private GameObject parentGameObjectOfBlocks;

    public void SetShape(Shape s)
    {
        _shape = s;
    }

    public void SetMaterial(Material m)
    {
        _material = m;
    }

    public Shape GetShape()
    {
        return _shape;
    }

    public Material GetMaterial()
    {
        return _material;
    }

    public List<GameObject> SetLittleBlocks(Vector3 basePosition, PhysicsMaterial2D physicsMaterial, Grid grid, GameObject prefab, Dictionary<Material, GridManager.BlockData> allSprites, bool isHighlighter, int playerID)
    {
        _littleBlocks = new();
        this.basePosition = basePosition;

        switch (_shape)
        {
            case Shape.oShape:
                SetupOShape();
                break;
            case Shape.iShape:
                SetupIShape();
                break;
            case Shape.lShape:
                SetupLShape();
                break;
            case Shape.tShape:
                SetupTShape();
                break;
            case Shape.zShape:
                SetupZShape();
                break;
        }

        bool hasOverlappingItems = false;

        foreach (var blockPosition in blockPositions)
        {
            //Debug.Log($"Position: {blockPosition}, Rotation: {currentRotation}");
            if (grid.GetCell((int)blockPosition.x, (int)blockPosition.y).GetHasBlockPlaced() && !isHighlighter)
            {
                return new List<GameObject>();
            }
        }

        foreach (var blockPosition in blockPositions)
        {
            //Debug.Log($"Position: {blockPosition}, Rotation: {currentRotation}");
            if (grid.GetCell((int)blockPosition.x, (int)blockPosition.y).GetHasBlockPlaced() && isHighlighter)
            {
                hasOverlappingItems = true;
            }
        }

        for (int i = 0; i < blockPositions.Length; i++)
        {
            // Instantiate the prefab for each little block
            GameObject littleBlock = Instantiate(prefab, blockPositions[i], Quaternion.identity);
            littleBlock.transform.position = blockPositions[i];
            SpriteRenderer spriteRenderer = littleBlock.GetComponent<SpriteRenderer>();

            // Set the sprite and color for each little block
            spriteRenderer.sprite = allSprites[_material].Sprite;
            // TODO: fix highlighting
            spriteRenderer.color = isHighlighter ? new Color(1f, 1f, 1f, 0.5f) : Color.white;
            
            if (isHighlighter)
            {
                Color c = Color.black;
                switch(playerID)
                {
                    case 0: c = Color.blue; break;
                    case 1: c = Color.green; break;
                    case 2: c = Color.red; break;
                    case 3: c = Color.yellow; break;
                }
                for(int j = 0; j < littleBlock.transform.childCount; j++)
                {
                    littleBlock.transform.GetChild(j).GetComponent<SpriteRenderer>().color = c;
                }

                spriteRenderer.sortingOrder = 10;
            }
            else
            {
                for(int j = 0; j < littleBlock.transform.childCount; j++)
                {
                    littleBlock.transform.GetChild(j).GetComponent<SpriteRenderer>().enabled = false;
                }

                spriteRenderer.sortingOrder = 3;
            }

            if (hasOverlappingItems && isHighlighter)
            {
                spriteRenderer.color = new Color(1f, 0f, 0f, 0.7f);
            }
            

            if (!isHighlighter)
            {
                Animator animator = littleBlock.GetComponent<Animator>();
                animator.runtimeAnimatorController = allSprites[_material].Animator;
                grid.GetCell((int)blockPositions[i].x, (int)blockPositions[i].y).SetBlock(littleBlock);
            } 

            _littleBlocks.Add(littleBlock);
        }

        return _littleBlocks;
    }


    private void SetupOShape()
    {
        Vector3 pos1;
        Vector3 pos2;
        Vector3 pos3;
        Vector3 pos4;

        pos1 = new Vector3(basePosition.x - 0.5f, basePosition.y + 0.5f, 0);
        pos2 = new Vector3(basePosition.x + 0.5f, basePosition.y + 0.5f, 0);
        pos3 = new Vector3(basePosition.x - 0.5f, basePosition.y - 0.5f, 0);
        pos4 = new Vector3(basePosition.x + 0.5f, basePosition.y - 0.5f, 0);

        blockPositions[0] = pos1;
        blockPositions[1] = pos2;
        blockPositions[2] = pos3;
        blockPositions[3] = pos4;
    }

    private void SetupIShape()
    {
        Vector3 pos1;
        Vector3 pos2;
        Vector3 pos3;
        Vector3 pos4;

        //basePosition += new Vector3(1, 0);
        switch (currentRotation)
        {
            case 0:
                pos1 = new Vector3(basePosition.x + 0.5f, basePosition.y + 1.5f, 0);
                pos2 = new Vector3(basePosition.x + 0.5f, basePosition.y + 0.5f, 0);
                pos3 = new Vector3(basePosition.x + 0.5f, basePosition.y - 0.5f, 0);
                pos4 = new Vector3(basePosition.x + 0.5f, basePosition.y - 1.5f, 0);
                blockPositions[0] = pos1;
                blockPositions[1] = pos2;
                blockPositions[2] = pos3;
                blockPositions[3] = pos4;
                break;
            case 1:
                pos1 = new Vector3(basePosition.x + 1.5f, basePosition.y - 0.5f, 0);
                pos2 = new Vector3(basePosition.x + 0.5f, basePosition.y - 0.5f, 0);
                pos3 = new Vector3(basePosition.x - 0.5f, basePosition.y - 0.5f, 0);
                pos4 = new Vector3(basePosition.x - 1.5f, basePosition.y - 0.5f, 0);
                blockPositions[0] = pos1;
                blockPositions[1] = pos2;
                blockPositions[2] = pos3;
                blockPositions[3] = pos4;
                break;
            case 2:
                pos1 = new Vector3(basePosition.x + 0.5f, basePosition.y - 1.5f, 0);
                pos2 = new Vector3(basePosition.x + 0.5f, basePosition.y - 0.5f, 0);
                pos3 = new Vector3(basePosition.x + 0.5f, basePosition.y + 0.5f, 0);
                pos4 = new Vector3(basePosition.x + 0.5f, basePosition.y + 1.5f, 0);
                blockPositions[0] = pos1;
                blockPositions[1] = pos2;
                blockPositions[2] = pos3;
                blockPositions[3] = pos4;
                break;
            case 3:
                pos1 = new Vector3(basePosition.x - 1.5f, basePosition.y + 0.5f, 0);
                pos2 = new Vector3(basePosition.x - 0.5f, basePosition.y + 0.5f, 0);
                pos3 = new Vector3(basePosition.x + 0.5f, basePosition.y + 0.5f, 0);
                pos4 = new Vector3(basePosition.x + 1.5f, basePosition.y + 0.5f, 0);
                blockPositions[0] = pos1;
                blockPositions[1] = pos2;
                blockPositions[2] = pos3;
                blockPositions[3] = pos4;
                break;
        }
        
    }
             
    private void SetupLShape()
    {
        Vector3 pos1;
        Vector3 pos2;
        Vector3 pos3;
        Vector3 pos4;
        switch (currentRotation)
        {
            case 0:
                pos1 = new Vector3(basePosition.x + 0.5f, basePosition.y + 1.5f, 0);
                pos2 = new Vector3(basePosition.x + 0.5f, basePosition.y + 0.5f, 0);
                pos3 = new Vector3(basePosition.x + 0.5f, basePosition.y - 0.5f, 0);
                pos4 = new Vector3(basePosition.x + 1.5f, basePosition.y - 0.5f, 0);
                blockPositions[0] = pos1;
                blockPositions[1] = pos2;
                blockPositions[2] = pos3;
                blockPositions[3] = pos4;
                break;
            case 1:
                pos1 = new Vector3(basePosition.x + 1.5f, basePosition.y + 0.5f, 0);
                pos2 = new Vector3(basePosition.x + 0.5f, basePosition.y + 0.5f, 0);
                pos3 = new Vector3(basePosition.x - 0.5f, basePosition.y + 0.5f, 0);
                pos4 = new Vector3(basePosition.x - 0.5f, basePosition.y - 0.5f, 0);
                blockPositions[0] = pos1;
                blockPositions[1] = pos2;
                blockPositions[2] = pos3;
                blockPositions[3] = pos4;
                break;
            case 2:
                pos1 = new Vector3(basePosition.x + 0.5f, basePosition.y - 0.5f, 0);
                pos2 = new Vector3(basePosition.x + 0.5f, basePosition.y + 0.5f, 0);
                pos3 = new Vector3(basePosition.x + 0.5f, basePosition.y + 1.5f, 0);
                pos4 = new Vector3(basePosition.x - 0.5f, basePosition.y + 1.5f, 0);
                blockPositions[0] = pos1;
                blockPositions[1] = pos2;
                blockPositions[2] = pos3;
                blockPositions[3] = pos4;
                break;
            case 3:
                pos1 = new Vector3(basePosition.x - 0.5f, basePosition.y + 0.5f, 0);
                pos2 = new Vector3(basePosition.x + 0.5f, basePosition.y + 0.5f, 0);
                pos3 = new Vector3(basePosition.x + 1.5f, basePosition.y + 0.5f, 0);
                pos4 = new Vector3(basePosition.x + 1.5f, basePosition.y + 1.5f, 0);
                blockPositions[0] = pos1;
                blockPositions[1] = pos2;
                blockPositions[2] = pos3;
                blockPositions[3] = pos4;
                break;
        }
    }
             
    private void SetupTShape()
    {
        Vector3 pos1;
        Vector3 pos2;
        Vector3 pos3;
        Vector3 pos4;
        switch (currentRotation)
        {
            case 0:
                pos1 = new Vector3(basePosition.x + 0.5f, basePosition.y + 1.5f, 0);
                pos2 = new Vector3(basePosition.x - 0.5f, basePosition.y + 0.5f, 0);
                pos3 = new Vector3(basePosition.x + 0.5f, basePosition.y + 0.5f, 0);
                pos4 = new Vector3(basePosition.x + 1.5f, basePosition.y + 0.5f, 0);
                blockPositions[0] = pos1;
                blockPositions[1] = pos2;
                blockPositions[2] = pos3;
                blockPositions[3] = pos4;
                break;
            case 1:
                pos1 = new Vector3(basePosition.x + 0.5f, basePosition.y + 0.5f, 0);
                pos2 = new Vector3(basePosition.x + 0.5f, basePosition.y - 0.5f, 0);
                pos3 = new Vector3(basePosition.x + 1.5f, basePosition.y + 0.5f, 0);
                pos4 = new Vector3(basePosition.x + 0.5f, basePosition.y + 1.5f, 0);
                blockPositions[0] = pos1;
                blockPositions[1] = pos2;
                blockPositions[2] = pos3;
                blockPositions[3] = pos4;
                break;
            case 2:
                pos1 = new Vector3(basePosition.x + 0.5f, basePosition.y + 0.5f, 0);
                pos2 = new Vector3(basePosition.x + 1.5f, basePosition.y + 0.5f, 0);
                pos3 = new Vector3(basePosition.x + 0.5f, basePosition.y - 0.5f, 0);
                pos4 = new Vector3(basePosition.x - 0.5f, basePosition.y + 0.5f, 0);
                blockPositions[0] = pos1;
                blockPositions[1] = pos2;
                blockPositions[2] = pos3;
                blockPositions[3] = pos4;
                break;
            case 3:
                pos1 = new Vector3(basePosition.x + 0.5f, basePosition.y + 0.5f, 0);
                pos2 = new Vector3(basePosition.x + 0.5f, basePosition.y + 1.5f, 0);
                pos3 = new Vector3(basePosition.x - 0.5f, basePosition.y + 0.5f, 0);
                pos4 = new Vector3(basePosition.x + 0.5f, basePosition.y - 0.5f, 0);
                blockPositions[0] = pos1;
                blockPositions[1] = pos2;
                blockPositions[2] = pos3;
                blockPositions[3] = pos4;
                break;
        }
        
    }
               
    private void SetupZShape()
    {
        Vector3 pos1;
        Vector3 pos2;
        Vector3 pos3;
        Vector3 pos4;
        switch (currentRotation)
        {
            case 0:
                pos1 = new Vector3(basePosition.x + 0.5f, basePosition.y + 1.5f, 0);
                pos2 = new Vector3(basePosition.x + 0.5f, basePosition.y + 0.5f, 0);
                pos3 = new Vector3(basePosition.x + 1.5f, basePosition.y + 0.5f, 0);
                pos4 = new Vector3(basePosition.x + 1.5f, basePosition.y - 0.5f, 0);
                blockPositions[0] = pos1;
                blockPositions[1] = pos2;
                blockPositions[2] = pos3;
                blockPositions[3] = pos4;
                break;
            case 1:
                pos1 = new Vector3(basePosition.x - 0.5f, basePosition.y - 0.5f, 0);
                pos2 = new Vector3(basePosition.x + 0.5f, basePosition.y - 0.5f, 0);
                pos3 = new Vector3(basePosition.x + 0.5f, basePosition.y + 0.5f, 0);
                pos4 = new Vector3(basePosition.x + 1.5f, basePosition.y + 0.5f, 0);
                blockPositions[0] = pos1;
                blockPositions[1] = pos2;
                blockPositions[2] = pos3;
                blockPositions[3] = pos4;
                break;
            case 2:
                pos1 = new Vector3(basePosition.x + 0.5f, basePosition.y + 1.5f, 0);
                pos2 = new Vector3(basePosition.x + 0.5f, basePosition.y + 0.5f, 0);
                pos3 = new Vector3(basePosition.x + 1.5f, basePosition.y + 0.5f, 0);
                pos4 = new Vector3(basePosition.x + 1.5f, basePosition.y - 0.5f, 0);
                blockPositions[0] = pos1;
                blockPositions[1] = pos2;
                blockPositions[2] = pos3;
                blockPositions[3] = pos4;
                break;
            case 3:
                pos1 = new Vector3(basePosition.x + 1.5f, basePosition.y + 0.5f, 0);
                pos2 = new Vector3(basePosition.x + 0.5f, basePosition.y + 0.5f, 0);
                pos3 = new Vector3(basePosition.x + 0.5f, basePosition.y - 0.5f, 0);
                pos4 = new Vector3(basePosition.x - 0.5f, basePosition.y - 0.5f, 0);
                blockPositions[0] = pos1;
                blockPositions[1] = pos2;
                blockPositions[2] = pos3;
                blockPositions[3] = pos4;
                break;
        }
        
    }

    public void RotateBlock(bool counterClockwise)
    {
        if (counterClockwise)
        {
            if (currentRotation > 0)
                currentRotation--;
            else
                currentRotation = 3;
        }
        else
        {
            if (currentRotation >= 3)
                currentRotation = 0;
            else
                currentRotation++;
        }
    }
}