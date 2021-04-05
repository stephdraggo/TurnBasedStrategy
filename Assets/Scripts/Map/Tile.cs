using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TurnBasedStrategy.Gameplay
{
    public enum SelectionState
    {
        none,
        selected,
        ShowUnitSelection,
        showMovement,
        showAttack,
        showEnemyMovement
    }

    public enum TileType
    {
        water,
        wall
    }

    /// <summary>
    /// Class for each tile in the map
    /// </summary>
    public class Tile : MonoBehaviour
    {
        [Header("Data")]
        //type of tile, determines what can move on it
        [SerializeField] TileType tileType;
        Vector2Int gridPosition;
        public Vector2Int GridPosition => gridPosition;
        [Header("Selection")]
        // SPRITE
        //reference to sprite renderer of selection
        [SerializeField] SpriteRenderer spriteRendererFront;
        [SerializeField] SpriteRenderer spriteRendererTop;
        //sprites for the different selection states
        [SerializeField] Sprite unitSelectionSprite, movementSprite, attackSprite;
        
        /*CUBE
        [SerializeField] MeshRenderer meshRenderer;
        [SerializeField] Material unitSelectionMat, movementMat, attackMat;
        */
        
        SelectionState selectionState = SelectionState.none;

        public TileType TileType => tileType;

        [HideInInspector]
        //variables for each of the tiles neighbours
        public Tile upTile, rightTile, downTile, leftTile;
        //counter used for finding tiles a unit can walk to
        [HideInInspector] public int stepCheck;
        //used for pathfinding only
        [HideInInspector] public Tile previousTile;


        //unit currently on this tile, null means no unit
        public Unit CurrentUnit { get; private set; }

        #region selection
        private void Start()
        {
            //disable the selection
            spriteRendererFront.gameObject.SetActive(false);
            spriteRendererTop.gameObject.SetActive(false);
        }

        public void SetGridPosition(Vector2Int _gridPosition) => gridPosition = _gridPosition;

        private void OnMouseUp()
        {
            //tell the map instance to select this tile
            Map.instance.OnClickTile(this, selectionState);
        }

        /// <summary>
        /// Set the selection state of this tile and change what sprite is showing over it
        /// </summary>
        /// <param name="_state">selection state to set this tile to</param>
        public void SetSelectionState(SelectionState _state)
        {
            //set the selection state of the tile
            selectionState = _state;
            //enable the sprite renderer for selection
            spriteRendererFront.gameObject.SetActive(true);
            spriteRendererTop.gameObject.SetActive(true);
            //set the selection sprite based on the state
            switch (_state)
            {
                case SelectionState.none:
                case SelectionState.selected:
                    spriteRendererFront.gameObject.SetActive(false);
                    spriteRendererTop.gameObject.SetActive(false);
                    break;
                case SelectionState.ShowUnitSelection:
                    spriteRendererFront.sprite = unitSelectionSprite;
                    spriteRendererTop.sprite = unitSelectionSprite;
                    //meshRenderer.material = unitSelectionMat;
                    break;
                case SelectionState.showMovement:
                    spriteRendererFront.sprite = movementSprite;
                    spriteRendererTop.sprite = movementSprite;
                    //meshRenderer.material = movementMat;
                    break;
                case SelectionState.showAttack:
                case SelectionState.showEnemyMovement:
                    spriteRendererFront.sprite = attackSprite;
                    spriteRendererTop.sprite = attackSprite;
                    break;
                    
            }
        }
        #endregion

        #region units
        /// <summary>
        /// Sets the unit stored on this tile
        /// </summary>
        /// <param name="_unit">unit on the tile</param>
        public void SetUnit(Unit _unit) => CurrentUnit = _unit;

        /// <summary>
        /// removes the unit from the tile
        /// </summary>
        public void RemoveUnit() => CurrentUnit = null;

        /// <summary>
        /// if there is a unit on this tile, calculate all of the locations it can move to
        /// </summary>
        /// <returns></returns>
        public List<Tile> CalculateUnitMovementTiles()
        {
            if (!CurrentUnit) return new List<Tile>();

            return CurrentUnit.CalculateMovementTiles();
        }

        public bool IsTileWalkable(Unit _unit) => _unit.WalkableTiles.Contains(tileType);

        #endregion


    }
}
