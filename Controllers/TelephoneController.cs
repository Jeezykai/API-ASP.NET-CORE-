using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using Pertemuan6.Models;

namespace Pertemuan6.Controllers;

public class TelephoneRequest
{
    public string Nama_User { get; set; } = string.Empty;
    public string Alamat { get; set; } = string.Empty;
    public string No_Telp { get; set; } = string.Empty;
    public string Kode_Post { get; set; } = string.Empty;
    public DateTime? Date_Time { get; set; }
}

[ApiController]
[Route("api/[controller]")]
public class TelephoneController(IConfiguration configuration) : ControllerBase
{
    private readonly string _connectionString =
      configuration.GetConnectionString("DefaultConnection")
      ?? throw new InvalidOperationException(
        "Connection string 'DefaultConnection' tidak ditemukan.");

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Telephone>>> GetAll()
    {
        const string query = """
          SELECT id, Nama_User, Alamat, No_Telp, kode_post AS Kode_Post, date_time AS Date_Time
          FROM tabel_telephone ORDER BY id;
          """;

        var listTelephone = new List<Telephone>();
        await using var connection = new MySqlConnection(_connectionString);

        try
        {
            await connection.OpenAsync();
        }
        catch (MySqlException)
        {
            return StatusCode(
              StatusCodes.Status503ServiceUnavailable,
              new
              {
                  message = "Database MySQL tidak dapat dihubungi. Pastikan MySQL aktif dan connection string benar."
              });
        }

        await using var command = new MySqlCommand(query, connection);
        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
            listTelephone.Add(MapTelephone(reader));

        return Ok(listTelephone);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Telephone>> GetById(int id)
    {
        const string query = """
          SELECT id, Nama_User, Alamat, No_Telp, kode_post AS Kode_Post, date_time AS Date_Time
          FROM tabel_telephone WHERE id = @id;
          """;

        await using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();
        await using var command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@id", id);

        await using var reader = await command.ExecuteReaderAsync();
        if (!await reader.ReadAsync())
            return NotFound(new
            {
                message = $"Data telephone dengan id {id} tidak ditemukan."
            });

        return Ok(MapTelephone(reader));
    }

    [HttpPost]
    public async Task<ActionResult<Telephone>> Create(TelephoneRequest request)
    {
        const string query = """
          INSERT INTO tabel_telephone (Nama_User, Alamat, No_Telp, kode_post, date_time)
          VALUES (@Nama_User, @Alamat, @No_Telp, @kode_post, @date_time);
          SELECT LAST_INSERT_ID();
          """;

        await using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();
        await using var command = new MySqlCommand(query, connection);
        AddRequestParameters(command, request);

        var insertedId = Convert.ToInt32(await command.ExecuteScalarAsync());

        return CreatedAtAction(nameof(GetById), new { id = insertedId }, new
        {
            id = insertedId,
            request.Nama_User,
            request.Alamat,
            request.No_Telp,
            request.Kode_Post,
            Date_Time = request.Date_Time ?? DateTime.Now
        });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, TelephoneRequest request)
    {
        const string query = """
          UPDATE tabel_telephone
          SET Nama_User=@Nama_User, Alamat=@Alamat, No_Telp=@No_Telp, kode_post=@kode_post, date_time=@date_time
          WHERE id=@id;
          """;
        await using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();
        await using var command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@id", id);
        AddRequestParameters(command, request);

        if (await command.ExecuteNonQueryAsync() == 0)
            return NotFound(new
            {
                message = $"Data telephone dengan id {id} tidak ditemukan."
            });

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        const string query = "DELETE FROM tabel_telephone WHERE id = @id;";
        await using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();
        await using var command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@id", id);

        if (await command.ExecuteNonQueryAsync() == 0)
            return NotFound(new
            {
                message = $"Data telephone dengan id {id} tidak ditemukan."
            });

        return NoContent();
    }
    private static void AddRequestParameters(MySqlCommand command, TelephoneRequest request)
    {
        command.Parameters.AddWithValue("@Nama_User", request.Nama_User);
        command.Parameters.AddWithValue("@Alamat", request.Alamat);
        command.Parameters.AddWithValue("@No_Telp", request.No_Telp);
        command.Parameters.AddWithValue("@kode_post", request.Kode_Post);
        command.Parameters.AddWithValue("@date_time", request.Date_Time ?? DateTime.Now);
    }

    private static Telephone MapTelephone(MySqlDataReader reader) => new()
    {
        Id = reader.GetInt32("id"),
        Nama_User = reader.IsDBNull(reader.GetOrdinal("Nama_User")) ? string.Empty : reader.GetString("Nama_User"),
        Alamat = reader.IsDBNull(reader.GetOrdinal("Alamat")) ? string.Empty : reader.GetString("Alamat"),
        No_Telp = reader.IsDBNull(reader.GetOrdinal("No_Telp")) ? string.Empty : reader.GetString("No_Telp"),
        Kode_Post = reader.IsDBNull(reader.GetOrdinal("Kode_Post")) ? string.Empty : reader.GetString("Kode_Post"),
        Date_Time = reader.IsDBNull(reader.GetOrdinal("Date_Time")) ? null : reader.GetDateTime("Date_Time")
    };
}