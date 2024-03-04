using System;
using UnityEngine;

[CreateAssetMenu(menuName = "VRHacking/ScriptableObjects/Hacker")]
public class HackerData : ScriptableObject
{
    //Narrative
    public string callsign;
    public string description;
    public Sprite icon;


    //Mechanical
    [Range(0.5f, 1.75f)]
    public float efficiency;
    [Range(0.5f, 1.75f)]
    public float aggressiveness;
    public HackerBehaviour behaviour;


    public enum HackerBehaviour {
        Null, Trickster, Daredevil 
    }
}