using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentCard : MonoBehaviour
{
    //1 for helm, 2 for armor, 3 for ring, 4 for weapon, 5 for mount, 6 for misc, 7 for un-equippable
    //8 for goggles, 9 for mask, 10 for amulet, 11 for tome, 12 for toolbox, 13 for shovel
    public int equipmentType;

    //secondary type for misc items
    //1= tome, 2=toolbox, 3=shovel, 4= orb, 5= spyglass, 6= amulet
    public int miscType;

    //types: 0=energy, 1= attack, 2= defense, 3= arcane power, 4= resistance, 5= influence, 6= mechanics, 7= digging, 8= lore, 9= discovery, 15= movementBonus
    //17= maxHealth, 18=maxAP
    public int[] statBonusType;
    public int[] statBonusQty;

    //this should cover most of the other effects?
    public int specialEffect;

    public int attMultiplier;
    public int apMultiplier;
    public int bombMultiplier;
    public int defMultiplier;
    public int resMultiplier;

    public int healthRegen;
    public int energyRegen;
    public int bombAttack;
    public int holyAttack;
    public int holyMultiplier;

    public bool isStaff;
}
