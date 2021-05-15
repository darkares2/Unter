using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using UnterLib;

namespace ConsoleMaeglerService
{
    public class DatabaseHandler
    {
        string connectString = "Server=localhost;Port=5432;User Id=unter;Password=unter;Database=unter_database;";

        internal Guid? getNearestSupplier(Double latitude, Double longitude)
        {
            Guid? clientId = null;
            try
            {
                NpgsqlConnection conn = new NpgsqlConnection(connectString);
                conn.Open();
                string select = "SELECT clientId FROM status WHERE status = 1 ORDER BY stamp DESC LIMIT 1;";
                var cmd = new NpgsqlCommand(select, conn);
                cmd.CommandType = CommandType.Text;
                clientId = (Guid)cmd.ExecuteScalar();
                conn.Close();
                string found = clientId.HasValue ? clientId.Value.ToString() : "None";
                Console.WriteLine($"Found avaliable client {found}");
            }
            catch (Exception msg)
            {
                Console.WriteLine($"failed to init db {msg.Message}");
                throw;
            }
            return clientId;
        }

        internal void insertLocation(LocationMessage locationMessage)
        {
            try
            {
                NpgsqlConnection conn = new NpgsqlConnection(connectString);
                conn.Open();
                string insert = "INSERT INTO location (clientId, stamp, location) VALUES (:clientId, :timestamp, point(:latitude, :longitude) );";
                var cmd = new NpgsqlCommand(insert, conn);
                cmd.Parameters.AddWithValue("clientId", locationMessage.clientId);
                cmd.Parameters.AddWithValue("timestamp", locationMessage.timestamp);
                cmd.Parameters.AddWithValue("latitude", locationMessage.location.latitude);
                cmd.Parameters.AddWithValue("longitude", locationMessage.location.longitude);
                cmd.Prepare();
                cmd.CommandType = CommandType.Text;
                int count = cmd.ExecuteNonQuery();
                conn.Close();
                Console.WriteLine($"Inserted location count {count}");
            }
            catch (Exception msg)
            {
                Console.WriteLine($"failed to init db {msg.Message}");
                throw;
            }
        }
    }
}
