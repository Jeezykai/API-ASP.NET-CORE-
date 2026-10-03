using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pertemuan6.Models;

[Table("tabel_mahasiswa")]
public class Mahasiswa
{
    [Column("id")]
    public int Id { get; set; }

    [Column("nama")]
    public string Nama { get; set; } = string.Empty;

    [Column("alamat")]
    public string Alamat { get; set; } = string.Empty;

    [Column("Pesan_pesan")]
    public string Pesanpesan { get; set; } = string.Empty;

    [Column("date_time")]
    public DateTime? Date_Time { get; set; }
}

public class MahasiswaRequest
{
    public string Nama { get; set; } = string.Empty;
    public string Alamat { get; set; } = string.Empty;
    public string Pesanpesan { get; set; } = string.Empty;
    public DateTime? Date_Time { get; set; }
}