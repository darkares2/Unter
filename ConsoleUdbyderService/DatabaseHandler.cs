using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using UnterLib;

namespace ConsoleUdbyderService
{
    public class DatabaseHandler
    {
        string connectString = "Server=localhost;Port=5432;User Id=unter;Password=unter;Database=unter_database;";
        public void init()
        {
            try
            {
                NpgsqlConnection conn = new NpgsqlConnection(connectString);
                conn.Open();
                createTables(conn);
                conn.Close();
            }
            catch (Exception msg)
            {
                Console.WriteLine($"failed to init db {msg.Message}");
                throw;
            }
        }

        private void createTables(NpgsqlConnection conn)
        {
            string locationTable = "CREATE TABLE IF NOT EXISTS location" +
                                    "(" +
                                    "  clientId UUID NOT NULL," +
                                    "  stamp TIMESTAMP WITH TIME ZONE NOT NULL," +
                                    "  location point," +
                                    "CONSTRAINT location_pkey PRIMARY KEY(clientId, stamp))";
            NpgsqlCommand command = new NpgsqlCommand(locationTable, conn);
            command.ExecuteNonQuery();
            string statusTable = "CREATE TABLE IF NOT EXISTS status" +
                                    "(" +
                                    "  clientId UUID NOT NULL," +
                                    "  stamp TIMESTAMP WITH TIME ZONE NOT NULL," +
                                    "  status integer," +
                                    "CONSTRAINT status_pkey PRIMARY KEY(clientId))";
            command = new NpgsqlCommand(statusTable, conn);
            command.ExecuteNonQuery();
        }

        internal bool setNewStatus(StatusMessage statusMessage)
        {
            bool done = false;
            try
            {
                NpgsqlConnection conn = new NpgsqlConnection(connectString);
                conn.Open();
                string insert = "INSERT INTO status (clientId, stamp, status) VALUES (:clientId, :timestamp, :status)" +
                    " ON CONFLICT (clientId) DO UPDATE SET stamp = EXCLUDED.stamp, status = EXCLUDED.status WHERE status.stamp < EXCLUDED.stamp;";
                var cmd = new NpgsqlCommand(insert, conn);
                cmd.Parameters.AddWithValue("clientId", statusMessage.clientId);
                cmd.Parameters.AddWithValue("timestamp", statusMessage.timestamp);
                cmd.Parameters.AddWithValue("status", statusMessage.status);
                cmd.Prepare();
                cmd.CommandType = CommandType.Text;
                int count = cmd.ExecuteNonQuery();
                if (count > 0)
                    done = true;
                conn.Close();
                Console.WriteLine($"Inserted status count {count}");
            }
            catch (Exception msg)
            {
                Console.WriteLine($"failed to init db {msg.Message}");
                throw;
            }
            return done;
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
