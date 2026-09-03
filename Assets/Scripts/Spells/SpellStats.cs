using System;

[Serializable]
public class SpellStats
{
    public float damage = 10f;
    public float speed = 8f;
    public float size = 1f;
    public float lifetime = 2f;

    public int projectileCount = 1;

    public bool piercing = false;
    public bool bouncing = false;
    public bool explosive = false;
    public bool homing = false;
}