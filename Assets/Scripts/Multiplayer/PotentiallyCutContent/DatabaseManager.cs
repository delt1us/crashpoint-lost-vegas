//*************************************************************************************************************
/*  Database Manager
 *  A script to handle all database operations
 *  This is cut content due to time constraints
 *  
 *  Created by Armin Raad 2023
 *  Change log:
 *      Armin - 05/07/23 - Brought over to the main git repo
 *      Armin - 28/07/23 - Added to cut content folder
 */
//*************************************************************************************************************

using UnityEngine;
using Mono.Data.Sqlite;
using Unity.Netcode;

public class DatabaseManager : NetworkBehaviour
{
    private string databaseURI;
    
    // Called after server or client is started
    private void _Initialize()
    {
        DontDestroyOnLoad(this);
        if (!NetworkManager.Singleton.IsServer) return;
        databaseURI = "URI=file:database.db";

        _CreatePlayersTable();
    }
    
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        _Initialize();
    }

    // private void Start()
    // {
    //     throw new NotImplementedException();
    // }

    private void _CreatePlayersTable()
    {
        using (SqliteConnection connection = new SqliteConnection(databaseURI))
        {
            connection.Open();

            string sql = "CREATE TABLE IF NOT EXISTS players (" +
                         "username TEXT PRIMARY KEY, " +
                         "password TEXT);";

            using (SqliteCommand command = new SqliteCommand(sql, connection))
            {
                command.ExecuteNonQuery();
            }

            connection.Close();
        }
    }

    // Adds a row to the players table
    [ServerRpc(RequireOwnership = false)]
    public void AddPlayerToDatabaseServerRpc(string username, string password)
    {
        Debug.Log("got to here");
        using (SqliteConnection connection = new SqliteConnection(databaseURI))
        {
            connection.Open();
        
            string sql = "INSERT INTO players (username, password) VALUES (@username, @password)";
        
            using (SqliteCommand command = new SqliteCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@username", username);
                command.Parameters.AddWithValue("@password", password);
                command.ExecuteNonQuery();
            }
        
            connection.Close();
        }
        Debug.Log($"Added player {username} to database with password {password}");
    }
}
