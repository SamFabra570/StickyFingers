using System.Collections.Generic;
using UnityEngine;

//A guard who spots you and tells nobody turns your level into a series of isolated one-on-one puzzles.
//This is the cheapest possible fix for that: when someone reaches Alert, everyone nearby learns WHERE,
//and gets a shove up their own awareness — enough to come and look, not enough to teleport them into
//certainty. They still have to see you themselves to commit.
public static class EnemyAlertNetwork
{
    private static readonly List<EnemyPerception> _members = new List<EnemyPerception>();

    //Statics survive between play sessions when the editor skips the domain reload, which would leave
    //this list full of ghosts from the previous run. Clear it on every subsystem boot.
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStatics()
    {
        _members.Clear();
    }

    public static void Register(EnemyPerception perception)
    {
        if (perception != null && !_members.Contains(perception))
            _members.Add(perception);
    }

    public static void Unregister(EnemyPerception perception)
    {
        _members.Remove(perception);
    }

    public static void Broadcast(EnemyPerception source, Vector3 position, float radius, float awarenessBump)
    {
        float radiusSqr = radius * radius;

        for (int i = _members.Count - 1; i >= 0; i--)
        {
            EnemyPerception member = _members[i];

            //Destroyed enemies compare equal to null through Unity's fake-null; drop them as we find them.
            if (member == null)
            {
                _members.RemoveAt(i);
                continue;
            }

            if (member == source)
                continue;

            if ((member.transform.position - position).sqrMagnitude > radiusSqr)
                continue;

            member.ReportStimulus(position, awarenessBump);
        }
    }

    //Anything in the world that makes a noise can call this: thrown objects, doors, the player sprinting.
    public static void ReportNoise(Vector3 position, float radius, float awarenessBump)
    {
        Broadcast(null, position, radius, awarenessBump);
    }
}
