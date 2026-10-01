using MySqlConnector;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace FoodOrdering;

public class DatabaseService
{
    private readonly string connectionString = "Server=[your_server];Port=[your_port];Database=[your_database];User ID=[your_username];Password=[your_password];";

    public async Task<bool> TestConnectionAsync()
    {
        try
        {
            await using var connection = new MySqlConnection(connectionString);

            await connection.OpenAsync();

            return true;
        }
        catch
        {
            return false;
        }
    }

    //This is for reading the record (R)
    public async Task<List<string>> GetAllPesananAsync()
    {
        var daftarPesanan = new List<string>();

        await using var connection = new MySqlConnection(connectionString);

        await connection.OpenAsync();

        string query = @"
            SELECT
                ID,
                KodePesanan,
                NamaPelanggan,
                Menu,
                Tipe,
                Tanggal,
                Catatan,
                NomorHp
            FROM Pesanan
            ORDER BY ID;
        ";

        await using var command = new MySqlCommand(query, connection);

        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            string data =
                $"{reader["KodePesanan"]} | " +
                $"{reader["NamaPelanggan"]} | " +
                $"{reader["Menu"]} | " +
                $"{reader["Tipe"]} | " +
                $"{reader["Tanggal"]:dd-MM-yyyy} | " +
                $"{reader["Catatan"]} | " +
                $"{reader["NomorHp"]}";

            daftarPesanan.Add(data);
        }

        return daftarPesanan;
    }

    // This is for inserting the record (C)
    public async Task InsertPesananAsync(
        string kodePesanan,
        string namaPelanggan,
        string menu,
        string tipe,
        DateTime tanggal,
        string catatan,
        string nomorHp)
    {
        await using var connection = new MySqlConnection(connectionString);
        await connection.OpenAsync();

        string query = @"
            INSERT INTO Pesanan
            (KodePesanan, NamaPelanggan, Menu, Tipe, Tanggal, Catatan, NomorHp)
            VALUES
            (@KodePesanan, @NamaPelanggan, @Menu, @Tipe, @Tanggal, @Catatan, @NomorHp);
        ";

        using var command = new MySqlCommand(query, connection);

        command.Parameters.AddWithValue("@KodePesanan", kodePesanan);
        command.Parameters.AddWithValue("@NamaPelanggan", namaPelanggan);
        command.Parameters.AddWithValue("@Menu", menu);
        command.Parameters.AddWithValue("@Tipe", tipe);
        command.Parameters.AddWithValue("@Tanggal", tanggal);
        command.Parameters.AddWithValue("@Catatan", catatan);
        command.Parameters.AddWithValue("@NomorHp", nomorHp);

        await command.ExecuteNonQueryAsync();
    }

    //This is for deleting the record (D)
    public async Task DeletePesananAsync(string kodePesanan)
    {
        await using var connection = new MySqlConnection(connectionString);
        await connection.OpenAsync();

        string query = @"
            DELETE FROM Pesanan
            WHERE KodePesanan = @KodePesanan;
        ";

        using var command = new MySqlCommand(query, connection);

        command.Parameters.AddWithValue("@KodePesanan", kodePesanan);

        await command.ExecuteNonQueryAsync();
    }

    //This is for updating/editing (U)
    public async Task UpdatePesananAsync(
        string kodePesanan,
        string namaPelanggan,
        string menu,
        string tipe,
        DateTime tanggal,
        string catatan,
        string nomorHp)
    {
        await using var connection = new MySqlConnection(connectionString);
        await connection.OpenAsync();

        string query = @"
            UPDATE Pesanan
            SET
                NamaPelanggan = @NamaPelanggan,
                Menu = @Menu,
                Tipe = @Tipe,
                Tanggal = @Tanggal,
                Catatan = @Catatan,
                NomorHp = @NomorHp
            WHERE KodePesanan = @KodePesanan;
        ";

    using var command = new MySqlCommand(query, connection);

    command.Parameters.AddWithValue("@KodePesanan", kodePesanan);
    command.Parameters.AddWithValue("@NamaPelanggan", namaPelanggan);
    command.Parameters.AddWithValue("@Menu", menu);
    command.Parameters.AddWithValue("@Tipe", tipe);
    command.Parameters.AddWithValue("@Tanggal", tanggal);
    command.Parameters.AddWithValue("@Catatan", catatan);
    command.Parameters.AddWithValue("@NomorHp", nomorHp);

    await command.ExecuteNonQueryAsync();
    }

    //This is for searching (reading too perhaps)
    public async Task<List<string>> SearchPesananAsync(string keyword)
    {
        var daftarPesanan = new List<string>();

        await using var connection = new MySqlConnection(connectionString);
        await connection.OpenAsync();

        string query = @"
            SELECT
                KodePesanan,
                NamaPelanggan,
                Menu,
                Tipe,
                Tanggal,
                Catatan,
                NomorHp
            FROM Pesanan
            WHERE
                KodePesanan LIKE @Keyword
                OR NamaPelanggan LIKE @Keyword
                OR Menu LIKE @Keyword
                OR Tipe LIKE @Keyword
                OR Catatan LIKE @Keyword
                OR NomorHp LIKE @Keyword
            ORDER BY ID;
        ";

        await using var command = new MySqlCommand(query, connection);

        command.Parameters.AddWithValue("@Keyword", $"%{keyword}%");

        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            string data =
                $"{reader["KodePesanan"]} | " +
                $"{reader["NamaPelanggan"]} | " +
                $"{reader["Menu"]} | " +
                $"{reader["Tipe"]} | " +
                $"{reader["Tanggal"]:dd-MM-yyyy} | " +
                $"{reader["Catatan"]} | " +
                $"{reader["NomorHp"]}";

            daftarPesanan.Add(data);
        }

        return daftarPesanan;
    }
}