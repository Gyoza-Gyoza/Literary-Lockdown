using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Database : MonoBehaviour
{
    public Dictionary<string, object[]> database = new();

    private void Start()
    {

    }
    private object[] ParseDatabase(string databaseString)
    {
        if (database != null) database.Clear(); 

        // Add database creation logic here

        return null;
    }
}
