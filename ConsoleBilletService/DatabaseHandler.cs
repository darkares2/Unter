using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using UnterLib;

namespace ConsoleBilletService
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
            string ticketTable = "CREATE TABLE IF NOT EXISTS ticket" +
                                    "(" +
                                    "  id integer NOT NULL," +
                                    "  sequence bigint," +
                                    "CONSTRAINT ticket_pkey PRIMARY KEY(id))";
            NpgsqlCommand command = new NpgsqlCommand(ticketTable, conn);
            command.ExecuteNonQuery();
            string sequence = "INSERT INTO ticket (id, sequence) VALUES (1, 0) ON CONFLICT (id) DO NOTHING";
            command = new NpgsqlCommand(sequence, conn);
            command.ExecuteNonQuery();
        }

        internal UInt64 getNextSequence()
        {
            UInt64 sequence = 0;
            try
            {
                NpgsqlConnection conn = new NpgsqlConnection(connectString);
                conn.Open();
                string increment = "UPDATE ticket SET sequence = sequence + 1 WHERE id = 1 RETURNING sequence;";
                var cmd = new NpgsqlCommand(increment, conn);
                sequence = (ulong)(Int64)cmd.ExecuteScalar();
                conn.Close();
                Console.WriteLine($"Next sequence {sequence}");
            }
            catch (Exception msg)
            {
                Console.WriteLine($"failed to init db {msg.Message}");
                throw;
            }
            return sequence;
        }
    }
}
