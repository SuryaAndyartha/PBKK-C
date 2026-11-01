using System;
using System.Collections.Generic;
using MySqlConnector;
using StudentManager.Models;

namespace StudentManager.Data;

public class StudentRepository
{
    // Connection string dibaca dari environment variable agar password tidak masuk git
    private const string ConnectionVariable = "STUDENTDB_CONNECTION";

    private static MySqlConnection CreateConnection()
    {
        string? connectionString =
            Environment.GetEnvironmentVariable(ConnectionVariable);

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                $"Environment variable {ConnectionVariable} belum diatur.");
        }

        return new MySqlConnection(connectionString);
    }

    public List<Student> GetAll()
    {
        using MySqlConnection connection = CreateConnection();

        string sql =
            "SELECT Id, NIM, Nama, Jurusan, Gender, Email " +
            "FROM Students ORDER BY Id DESC";

        using MySqlCommand command = new MySqlCommand(sql, connection);

        connection.Open();

        return ReadStudents(command);
    }

    public void Insert(Student student)
    {
        using MySqlConnection connection = CreateConnection();

        string sql =
            "INSERT INTO Students " +
            "(NIM, Nama, Jurusan, Gender, Email) " +
            "VALUES (@NIM, @Nama, @Jurusan, @Gender, @Email)";

        using MySqlCommand command = new MySqlCommand(sql, connection);

        AddStudentParameters(command, student);

        connection.Open();
        command.ExecuteNonQuery();
    }

    public void Update(Student student)
    {
        using MySqlConnection connection = CreateConnection();

        string sql =
            "UPDATE Students SET NIM=@NIM, Nama=@Nama, " +
            "Jurusan=@Jurusan, Gender=@Gender, Email=@Email " +
            "WHERE Id=@Id";

        using MySqlCommand command = new MySqlCommand(sql, connection);

        command.Parameters.AddWithValue("@Id", student.Id);
        AddStudentParameters(command, student);

        connection.Open();
        command.ExecuteNonQuery();
    }

    public void Delete(int id)
    {
        using MySqlConnection connection = CreateConnection();

        using MySqlCommand command =
            new MySqlCommand("DELETE FROM Students WHERE Id=@Id", connection);

        command.Parameters.AddWithValue("@Id", id);

        connection.Open();
        command.ExecuteNonQuery();
    }

    public List<Student> Search(string keyword)
    {
        using MySqlConnection connection = CreateConnection();

        string sql =
            "SELECT Id, NIM, Nama, Jurusan, Gender, Email " +
            "FROM Students WHERE NIM LIKE @Keyword " +
            "OR Nama LIKE @Keyword OR Jurusan LIKE @Keyword " +
            "ORDER BY Id DESC";

        using MySqlCommand command = new MySqlCommand(sql, connection);

        command.Parameters.AddWithValue("@Keyword", "%" + keyword + "%");

        connection.Open();

        return ReadStudents(command);
    }

    private static void AddStudentParameters(MySqlCommand command, Student student)
    {
        command.Parameters.AddWithValue("@NIM", student.NIM);
        command.Parameters.AddWithValue("@Nama", student.Nama);
        command.Parameters.AddWithValue("@Jurusan", student.Jurusan);
        command.Parameters.AddWithValue("@Gender", student.Gender);
        command.Parameters.AddWithValue("@Email", student.Email);
    }

    private static List<Student> ReadStudents(MySqlCommand command)
    {
        var students = new List<Student>();

        using MySqlDataReader reader = command.ExecuteReader();

        while (reader.Read())
        {
            students.Add(new Student
            {
                Id = Convert.ToInt32(reader["Id"]),
                NIM = reader["NIM"].ToString()!,
                Nama = reader["Nama"].ToString()!,
                Jurusan = reader["Jurusan"].ToString()!,
                Gender = reader["Gender"].ToString()!,
                Email = reader["Email"].ToString()!
            });
        }

        return students;
    }
}
