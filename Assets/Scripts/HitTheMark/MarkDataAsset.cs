using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Create Scriptable/MarkDataAsset")]
public class MarkDataAsset : ScriptableObject
{
    [SerializeField] int score;
    [SerializeField] float time;
    [SerializeField] int remainder_bullet;
    [SerializeField] int maxbullet;

    public int Score
    {
        get => score;
        set => score = value;
    }
    public float Time
    {
        get => time;
        set => time = value;
    }
    public int Remainder_bullet
    {
        get => remainder_bullet;
        set => remainder_bullet = value;
    }
    public int Maxbullet => maxbullet;
}
