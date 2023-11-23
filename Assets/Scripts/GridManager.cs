using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static Unity.Collections.AllocatorManager;
using UnityEngine.InputSystem;

public class GridManager : MonoBehaviour
{
    [Serializable]
    public struct BlockData
    {
        public Material Material;
        public Sprite Sprite;
        public RuntimeAnimatorController Animator;
    }

    private PowerupManager _powerupManager;
    private PlayerMovement _playerMovement;

    [SerializeField] private bool drawGrid = true;
    [SerializeField] private GameObject blockPrefab;
    [SerializeField] private Sprite blockSprite;
    [SerializeField] private GameObject[] players;

    [SerializeField] private GameManager gameManager;

    // TODO: move player placing range into player
    [SerializeField] private float placingRange = 3f;
    [SerializeField] private float placingCooldown = 5f;

    [SerializeField] private float destroyCooldown = 5f;
    [SerializeField] private float sacreficeCooldown = 5f;
    
    private bool canPlace = true;
    private bool canDestroy = true;
    private bool canSacrefice = true;

    [SerializeField] private PhysicsMaterial2D groundPhysics;

    [SerializeField] private GameObject highlighterPrefab;

    //[SerializeField] private GameObject grid;
    [SerializeField] private Grid grid;

    private GameObject currentHighlighter;

    private Sprite currentSprite;
    private Sprite nextSprite;
    private Dictionary<Material, BlockData> allSprites;

    //[SerializeField] private Sprite sealSprite;
    //[SerializeField] private Sprite turtleSprite;
    //[SerializeField] private Sprite crabSprite;
    //[SerializeField] private Sprite fishSprite;
    //[SerializeField] private Sprite birdSprite;

    [SerializeField] private BlockData[] _blockDatas;

    List<GameObject> littleBlocksHighlighter = new List<GameObject>();

    private Tuple<Block, Block> blocks;

    private SoundManager _soundManager;

    //testing
    //private int currentRotation = 0;
    Shape shape = Shape.oShape;

    //private Grid grid;
    // Start is called before the first frame update

    void Start()
    {
        _soundManager = GameObject.FindGameObjectWithTag("SoundManager").GetComponent<SoundManager>();
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        //grid = new Grid(32, 256, 16);

        allSprites = new Dictionary<Material, BlockData>();
        //allSprites.Add(Material.seal, sealSprite);
        //allSprites.Add(Material.turtle, turtleSprite);
        //allSprites.Add(Material.crab, crabSprite);
        //allSprites.Add(Material.fish, fishSprite);
        //allSprites.Add(Material.bird, birdSprite);

        grid = GameObject.FindGameObjectWithTag("Grid").GetComponent<Grid>();

        foreach (var data in _blockDatas)
        {
            allSprites.Add(data.Material, data);
        }

        blocks = DrawBlocks.DrawBlock(0);

        _powerupManager = GameObject.FindGameObjectWithTag("PowerUpManager").GetComponent<PowerupManager>();
        _playerMovement = gameObject.GetComponent<PlayerMovement>();
    }

    public int getPlayerId()
    {
        int id = -1;

        for (int i = 0; i < gameManager.playerReferences.Length; i++)
        {
            if (gameManager.playerReferences[i].Equals(gameObject))
            {
                id = i;
                break;
            }
        }

        return id;
    }

    Vector2 inputTest = new Vector2();

    // Update is called once per frame
    void Update()
    {
        if (drawGrid)
            DrawDebugGrid();

        Vector3 currentCell = new Vector3(0, 0);

        Vector2 input = new Vector2();
        input.x = inputTest.x;
        input.y = inputTest.y;
        Vector2 movementInput = input;
        Vector2 movement = new Vector2(movementInput.x, movementInput.y);

        GameObject stickPosition = gameObject.transform.GetChild(0).gameObject;
        stickPosition.transform.Translate(movement * 5f * Time.deltaTime);
        currentCell = stickPosition.transform.position;

        Vector2 playerPos = gameManager.playerReferences[gameObject.GetComponent<PlayerInformation>().playerID]
            .transform.position;

        if (currentCell.x >= playerPos.x + placingRange)
        {
            currentCell.x = (int)(playerPos.x + placingRange);
            stickPosition.transform.position =
                new Vector2(playerPos.x + placingRange, stickPosition.transform.position.y);
        }
        else if (currentCell.x <= playerPos.x - placingRange)
        {
            currentCell.x = (int)(playerPos.x - placingRange);
            stickPosition.transform.position =
                new Vector2(playerPos.x - placingRange, stickPosition.transform.position.y);
        }

        if (currentCell.y >= playerPos.y + placingRange)
        {
            currentCell.y = (int)(playerPos.y + placingRange);
            stickPosition.transform.position =
                new Vector2(stickPosition.transform.position.x, playerPos.y + placingRange);
        }
        else if (currentCell.y <= playerPos.y - placingRange)
        {
            currentCell.y = (int)(playerPos.x - placingRange);
            stickPosition.transform.position =
                new Vector2(stickPosition.transform.position.x, playerPos.y - placingRange);
        }

        currentCell = new Vector3(Mathf.Floor(currentCell.x), Mathf.Floor(currentCell.y));
        HighlightCell(currentCell, currentSprite);
    }

    public void MoveBlock(InputAction.CallbackContext context)
    {
        inputTest.x = context.ReadValue<Vector2>().x;
        inputTest.y = context.ReadValue<Vector2>().y;
    }

    public void RotateBlockRight(InputAction.CallbackContext context)
    {
        if(context.started)
        {
            blocks.Item1.RotateBlock(true);
        }
    }

    public void RotateBlockLeft(InputAction.CallbackContext context)
    {
        if(context.started)
        {
            blocks.Item1.RotateBlock(false);
        }
    }

    public void Sacrefice(InputAction.CallbackContext context)
    {

        if (canSacrefice)
        {
            Material block = blocks.Item1.GetMaterial();
            blocks = DrawBlocks.DrawBlock(getPlayerId());
            // block = blocks.Item1;
            Debug.Log(block);
            switch (block)
            {
                case Material.seal:
                    _powerupManager.Swap();
                    break;
                case Material.turtle:
                    _powerupManager.Freeze();
                    break;
                case Material.crab:
                    _playerMovement.setJumpBoost();
                    //Jump
                    break;
                case Material.fish:
                    _powerupManager.Stun();
                    //Stun
                    break;
                case Material.bird:
                    _playerMovement.Push();
                    //Push
                    break;
            }

            canSacrefice = false;
            StartCoroutine(ResetSacrificeCooldown());
        }
        
    }

    public void DestroyBlock(InputAction.CallbackContext context)
    {
        if (canDestroy)
        {
            Animator animator = gameObject.GetComponent<Animator>();

            float vertical = animator.GetFloat("v");
            float horizontal = animator.GetFloat("h");

            Vector2 lookDir = new Vector2(horizontal, vertical).normalized;

            float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg;

            // Ensure the angle is positive
            if (angle < 0)
            {
                angle += 360f;
            }

            // Define the angle ranges for each look direction
            float rightRange = 45f;
            float upRange = 135f;
            float leftRange = 225f;
            float downRange = 315f;

            float rayLength = 3f;

            Ray2D ray;

            // Determine the look direction based on the angle
            if (angle < rightRange || angle >= 360f - rightRange)
            {
                ray = new Ray2D(gameObject.transform.position, new Vector3(0, 1, 0));
            }
            else if (angle < upRange && angle >= rightRange)
            {
                ray = new Ray2D(gameObject.transform.position, new Vector3(1, 0, 0));
            }
            else if (angle < leftRange && angle >= upRange)
            {
                ray = new Ray2D(gameObject.transform.position, new Vector3(0, -1, 0));
            }
            else if (angle < downRange && angle >= leftRange)
            {
                ray = new Ray2D(gameObject.transform.position, new Vector3(-1, 0, 0));
            }
            else
            {
                ray = new Ray2D(gameObject.transform.position, new Vector3(0, 1, 0));
            }

            LayerMask layerMask = LayerMask.GetMask("Ground");
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, rayLength, layerMask);

            Debug.DrawRay(ray.origin, ray.direction * rayLength, Color.red);

            if (hit.collider != null)
            {
                GameObject parent = hit.collider.gameObject.transform.parent.gameObject;

                for (int i = 0; i < parent.transform.childCount; i++)
                {
                    Vector3 childPos = parent.transform.GetChild(i).position;
                    if (grid.GetCell((int)childPos.x, (int)childPos.y).GetHasBlockPlaced())
                    {
                        // block can be placed
                        grid.GetGrid()[(int)childPos.x][(int)childPos.y].RemoveBlock();
                    }
                    parent.transform.GetChild(i).gameObject.GetComponent<Animator>().SetBool("isDestroyed", true);
                }
                canDestroy = false;
                StartCoroutine(ResetDestroyCooldown());
            }
        }
    }

    public void PlaceBlock(InputAction.CallbackContext context)
    {
        // placing
        if (canPlace)
        {
            bool placingSuccess = false;
            GameObject stickPosition = gameObject.transform.GetChild(0).gameObject;

            // select block to be placed - for testing
            GameObject shapeBlock = new GameObject("oObjectBlock");
            shapeBlock.AddComponent<Block>();

            Block block = blocks.Item1;

            Destroy(shapeBlock);
            
            
            Vector3 currentCell = new Vector3(Mathf.Floor(stickPosition.transform.position.x), Mathf.Floor(stickPosition.transform.position.y));

            if (!grid.GetCell((int)currentCell.x, (int)currentCell.y).GetHasBlockPlaced())
            {
                List<GameObject> littleBlocks = block.SetLittleBlocks(currentCell, groundPhysics, grid, blockPrefab, allSprites, false, gameObject.GetComponent<PlayerInformation>().playerID);
                GameObject parentObject = new GameObject("ParentObjectOfBlocks")
                {
                    tag = "BlockGroup"
                };
                BoxCollider boxCollider = parentObject.AddComponent<BoxCollider>();
                boxCollider.isTrigger = false;
                parentObject.layer = 8;


                if (littleBlocks.Count > 0)
                {
                    for (int i = 0; i < littleBlocks.Count; i++)
                    {BoxCollider2D collider = littleBlocks[i].AddComponent<BoxCollider2D>();
                        Rigidbody2D rigidBody = littleBlocks[i].AddComponent<Rigidbody2D>();
                        rigidBody.sharedMaterial = groundPhysics;
                        rigidBody.isKinematic = true;
                        grid.GetCell((int)littleBlocks[i].transform.position.x,
                            (int)littleBlocks[i].transform.position.y).SetBlock(littleBlocks[i]);
                        littleBlocks[i].transform.SetParent(parentObject.transform);
                    }

                    placingSuccess = true;
                }
                else
                {
                    Destroy(parentObject);
                }
            }

            if (placingSuccess)
            {
                blocks = DrawBlocks.DrawBlock(getPlayerId());
                _soundManager.PlaySoundEffect(_soundManager.SoundEffects.BlockPlace);
                canPlace = false;
                StartCoroutine(ResetPlacingCooldown());
            }
        }
    }


    private void HighlightCell(Vector3 cellPosition, Sprite sprite)
    {
        Block block = blocks.Item1;

        foreach (var oldHighlighter in littleBlocksHighlighter)
        {
            Destroy(oldHighlighter);
        }

        littleBlocksHighlighter = block.SetLittleBlocks(cellPosition, groundPhysics, grid, blockPrefab, allSprites, true, gameObject.GetComponent<PlayerInformation>().playerID);
    }

    private IEnumerator ResetPlacingCooldown()
    {
        // Wait for one second
        yield return new WaitForSeconds(placingCooldown);

        // Set the flag to true, allowing the function to be called again
        canPlace = true;
    }

    private IEnumerator ResetDestroyCooldown()
    {
        // Wait for one second
        yield return new WaitForSeconds(destroyCooldown);

        // Set the flag to true, allowing the function to be called again
        canDestroy = true;
    }
    
    private IEnumerator ResetSacrificeCooldown()
    {
        // Wait for one second
        yield return new WaitForSeconds(sacreficeCooldown);

        // Set the flag to true, allowing the function to be called again
        canSacrefice = true;
    }

    private void DrawDebugGrid()
    {
        for (int i = 0; i <= grid.GetWidth(); i++)
        {
            for (int j = 0; j <= grid.GetHeight(); j++)
            {
                // Draw horizontal line
                if (i < grid.GetWidth())
                {
                    Vector3 startH = new Vector3(i, j, 0);
                    Vector3 endH = new Vector3(i + 1, j, 0);
                    Debug.DrawLine(startH, endH, Color.white);
                }

                // Draw vertical line
                if (j < grid.GetHeight())
                {
                    Vector3 startV = new Vector3(i, j, 0);
                    Vector3 endV = new Vector3(i, j + 1, 0);
                    Debug.DrawLine(startV, endV, Color.white);
                }
            }
        }
    }
}

