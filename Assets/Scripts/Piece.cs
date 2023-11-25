using System;
using UnityEngine;

[Serializable]
public class Piece : ICloneable
{
    public int move;
    public int attack;
    [NonSerialized] public bool isWhite;
    [NonSerialized] public Sprite sprite;
    [NonSerialized] public bool attacked = false;
    [NonSerialized] public bool movedThisTurn = false;
    public EquipmentType equipmentType = EquipmentType.None;
    
    public delegate void EquipmentChanged ();
    public event EquipmentChanged equippedChanged;

    public object Clone()
    {
        return new Piece
        {
            move = this.move,
            attack = this.attack,
            isWhite = this.isWhite,
            sprite = this.sprite,
            attacked = this.attacked,
            movedThisTurn = this.movedThisTurn,
            equipmentType = this.equipmentType
        };
    }
    
    public void Equip(EquipmentType equipmentType)
    {
        switch (equipmentType)
        {
            case EquipmentType.None:
            case EquipmentType.Barrel:
                break;
            case EquipmentType.Horse:
                move += 1;
                break;
            case EquipmentType.Gun:
                attack += 1;
                attacked = false;
                break;
            case EquipmentType.Sniper:
                attack += 2;
                attacked = false;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(equipmentType), equipmentType, null);
        }
        this.equipmentType = equipmentType;
        sprite = GameManager.instance.GetEquipmentSprite(equipmentType, isWhite);
        equippedChanged?.Invoke();
    }
    
    public void Unequip()
    {
        switch (equipmentType)
        {
            case EquipmentType.None:
            case EquipmentType.Barrel:
                break;
            case EquipmentType.Horse:
                move -= 1;
                break;
            case EquipmentType.Gun:
                attack -= 1;
                attacked = false;
                break;
            case EquipmentType.Sniper:
                attack -= 2;
                attacked = false;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(equipmentType), equipmentType, null);
        }
        equipmentType = EquipmentType.None;
        sprite = GameManager.instance.GetEquipmentSprite(equipmentType, isWhite);
        equippedChanged?.Invoke();
    }
}

public enum EquipmentType
{
    None,
    Barrel,
    Horse,
    Gun,
    Sniper
}