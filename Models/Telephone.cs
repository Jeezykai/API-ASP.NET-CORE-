namespace Pertemuan6.Models;

public class Telephone
{
    public int Id { get; set; }
    public string Nama_User { get; set; } = string.Empty;
    public string Alamat { get; set; } = string.Empty;
    public string No_Telp { get; set; } = string.Empty;
    public string Kode_Post { get; set; } = string.Empty;
    public DateTime? Date_Time { get; set; }
}

public class TelephoneRequest
{
    public string Nama_User { get; set; } = string.Empty;
    public string Alamat { get; set; } = string.Empty;
    public string No_Telp { get; set; } = string.Empty;
    public string Kode_Post { get; set; } = string.Empty;
    public DateTime? Date_Time { get; set; }
}