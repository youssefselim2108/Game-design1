using UnityEngine;
using UnityEngine.InputSystem;

// Demo bot for recording gameplay. Creates itself on Play; press B to toggle it on/off.
// It rolls the player toward the nearest PickUp and steers away from the Enemy.
public class AutoPlayer : MonoBehaviour
{
    // Distance at which the bot starts steering away from the enemy.
    const float fleeDistance = 4f;

    bool botOn;
    Rigidbody rb;
    PlayerController pc;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Spawn()
    {
        new GameObject("AutoPlayer").AddComponent<AutoPlayer>();
    }

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.bKey.wasPressedThisFrame)
        {
            botOn = !botOn;
        }
    }

    void FixedUpdate()
    {
        if (!botOn) return;

        if (rb == null)
        {
            var player = GameObject.Find("Player");
            if (player == null) return;
            rb = player.GetComponent<Rigidbody>();
            pc = player.GetComponent<PlayerController>();
        }

        Vector3 pos = rb.position;
        Vector3 dir = Vector3.zero;

        // Head for the nearest active PickUp.
        float best = float.MaxValue;
        foreach (var p in GameObject.FindGameObjectsWithTag("PickUp"))
        {
            float d = (p.transform.position - pos).sqrMagnitude;
            if (d < best)
            {
                best = d;
                dir = p.transform.position - pos;
            }
        }
        dir.y = 0;
        dir.Normalize();

        // Steer away from the enemy when it gets close.
        var enemy = GameObject.FindGameObjectWithTag("Enemy");
        if (enemy != null)
        {
            Vector3 away = pos - enemy.transform.position;
            away.y = 0;
            if (away.magnitude < fleeDistance)
            {
                dir = (dir + away.normalized * 2f).normalized;
            }
        }

        rb.AddForce(dir * pc.speed);
    }
}
