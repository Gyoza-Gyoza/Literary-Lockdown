using System.Collections;
using UnityEngine;
using Unity.Netcode;
using System.Collections;
using System;

public class EnemyBehaviour : NetworkBehaviour
{
    [SerializeField] private float movementSpeed = 1.0f;
    [SerializeField] private float jiggleTime = 1.0f;
    [SerializeField] private float jiggleAmount = .3f;
    [SerializeField] private float jiggleFreq = 30f;
    [SerializeField] private Color flickerCol;

    [SerializeField] private int pages = 1;


    private bool jiggling = false;
    private float jiggleCount = 0f;

    private bool slowed = false;

    public GameObject deathFX;
    public SpriteRenderer renderer;
    private float initialMovementSpeed;


    //public NetworkVariable<float> movementSpeed = new NetworkVariable<float>(0);
    public NetworkVariable<int> health = new NetworkVariable<int> (5);
    private NetworkVariable<int> currentWaypointIndex = new NetworkVariable<int>(0);
    private Vector2 targetPosition
    { get { return WaypointManager.Instance.waypoints[currentWaypointIndex.Value].position; } }

    private void Update()
    {

        if (slowed)
        {
            renderer.color = Color.greenYellow;
        }
        else
        {
            renderer.color = Color.white;
        }

            Vector3 currentPos = transform.position;
        float move = movementSpeed * Time.deltaTime;
        if (Vector2.Distance(transform.position, targetPosition) >= 0.05f)
            transform.position = Vector3.MoveTowards(currentPos, targetPosition, move);
        else
        {
            if (IsHost)
                currentWaypointIndex.Value++;

            if (currentWaypointIndex.Value >= WaypointManager.Instance.waypoints.Length)
            {
                EscapeEnemyRpc();
            }
        }
    }

    [Rpc(SendTo.Server)]
    public void TakeDamageRpc(int damage)
    {
        health.Value -= damage;

        StartCoroutine(Flicker());

        //Take damage polish
        if (health.Value <= 0)
        {
            jiggling = false;
            DestroyEnemyRpc();
        }

        if (!jiggling)
        {
            StartCoroutine(Jiggle());
        }
        else
        {
            jiggleCount = 0f;
        }

    }

    [Rpc(SendTo.Server)]
    public void SlowDownRPC(float slowAmount, float duration)
    {
        initialMovementSpeed = movementSpeed;
        movementSpeed = movementSpeed * slowAmount;
        StartCoroutine(SlowDownRoutine(duration));
    }

    private IEnumerator SlowDownRoutine(float duration)
    {
        slowed = true;
        yield return new WaitForSeconds(duration);
        slowed = false;
        movementSpeed = initialMovementSpeed;
    }

    [Rpc(SendTo.Server)]
    public void DestroyEnemyRpc()
    {
        NetworkObject networkObject = GetComponent<NetworkObject>();
        if (networkObject != null && networkObject.IsSpawned)
        {
            networkObject.Despawn();
        }
        else
        {
            GameObject.Instantiate(deathFX, this.transform.position, this.transform.rotation);
            Destroy(gameObject);
        }
        if (!ObjectivesManager.Instance.gameEnded.Value) ObjectivesManager.Instance.CaptureBooks(pages);
    }

    [Rpc(SendTo.Server)]
    public void EscapeEnemyRpc()
    {
        NetworkObject networkObject = GetComponent<NetworkObject>();
        if (networkObject != null && networkObject.IsSpawned)
        {
            networkObject.Despawn();
        }
        else
        {
            Destroy(gameObject);
        }
        //ObjectivesManager.Instance.CaptureBooks();
        if (!ObjectivesManager.Instance.gameEnded.Value) ObjectivesManager.Instance.EscapeBooks();
    }


    IEnumerator Flicker()
    {

        Color origianl = renderer.color;

        renderer.color = flickerCol;

        yield return new WaitForSeconds(.1f);

        renderer.color = new Color(255,255,255);

        yield return new WaitForSeconds(.1f);

        renderer.color = Color.white;

        //renderer.color = origianl;

        yield break;
    }

    IEnumerator Jiggle()
    {
        jiggling = true;

        while (jiggling)
        {
            jiggleCount += Time.deltaTime;

            if (jiggleCount >= jiggleTime)
            {
                jiggling = false;
            }

            float amt = Mathf.Lerp(jiggleAmount, 0, jiggleCount / jiggleTime) * Mathf.Sin(jiggleCount * jiggleFreq);

            renderer.transform.localScale = new Vector3(1 + amt, 1 - amt, 1);

            yield return new WaitForSeconds(Time.deltaTime);
        }

        renderer.transform.localScale = new Vector3(1, 1, 1);
        jiggling = false;
        jiggleCount = 0;

        yield break;
    }
}
