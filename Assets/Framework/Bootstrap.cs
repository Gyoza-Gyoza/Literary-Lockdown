using UnityEngine;

// Use this to arrange the order of initialization
// Use awake to subscribe to the events and start to invoke them
// This is to ensure that all subscriptions are done before invocation
public class Bootstrap : MonoBehaviour
{
    public delegate void Tasks();

    public static event Tasks
        SingletonInitializations, // Enter all singleton initializations here
        AcceptLoadRegistrations, // Enter all things that need loading here
        StopLoadRegistrations,  // This stops all load registrations to begin checking for load completion
        PostDatabaseInitializations;

    private void Start()
    {
        SingletonInitializations?.Invoke();
        AcceptLoadRegistrations?.Invoke();
        StopLoadRegistrations?.Invoke();
        PostDatabaseInitializations?.Invoke();
    }
}
