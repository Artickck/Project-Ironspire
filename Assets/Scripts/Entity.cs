using UnityEngine;

public class Entity
{
    public float hp;
    public float baseSpeed;
    public float damage;
    public int level;
    
    public Entity(float hp, float baseSpeed, float damage, int level)
    {
        this.hp = hp;
        this.baseSpeed = baseSpeed;
        this.damage = damage;
        this.level = level;
    }
}
