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
    }

    public enum TileType
    {
        water,
        wall
    }

    public class Tile : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField] TileType tileType;
        [Header("Selection sprite")]
        [SerializeField] SpriteRenderer spriteRenderer;
        [SerializeField] Sprite selectedSprite, unitSelectionSprite, movementSprite;
        
        SelectionState selectionState = SelectionState.none;

        public TileType TileType => tileType;
        

        [HideInInspector]
        public Tile upTile, rightTile, downTile, leftTile;
        [HideInInspector] public int stepCheck;

        public Unit CurrentUnit { get; private set; }

        #region selection
        private void Start()
        {
            spriteRenderer.gameObject.SetActive(false);
        }

        private void OnMouseUp()
        {
            Map.instance.OnClickTile(this, selectionState);
        }

        public void SetSelectionState(SelectionState _state)
        {
            selectionState = _state;
            spriteRenderer.gameObject.SetActive(true);
            switch (_state)
            {
                case SelectionState.none:
                    spriteRenderer.gameObject.SetActive(false);
                    break;
                case SelectionState.selected:
                    spriteRenderer.sprite = selectedSprite;
                    break;
                case SelectionState.ShowUnitSelection:
                    spriteRenderer.sprite = unitSelectionSprite;
                    break;
                case SelectionState.showMovement:
                    spriteRenderer.sprite = movementSprite;
                    break;
            }
        }
        #endregion

        #region unit
        public void SetUnit(Unit _unit) => CurrentUnit = _unit;

        public void RemoveUnit() => CurrentUnit = null;

        #endregion

        #region move options

        public List<Tile> CalculateUnitMovementTiles()
        {
            if (!CurrentUnit) return new List<Tile>();

            return CurrentUnit.CalculateMovementTiles();
        }

        #endregion
    }
}
