using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace StudentRegistrationApp;

public partial class MainWindow : Window
{
    private int indexEdit = -1;

    // Menyimpan seluruh data mahasiswa
    private readonly List<string> semuaMahasiswa = new();

    // Menyimpan index data asli ketika sedang melakukan pencarian
    private readonly List<int> indexHasilSearch = new();

    public MainWindow()
    {
        InitializeComponent();
        UpdateCounter();
    }

    private void BtnSimpan_Click(object? sender, RoutedEventArgs e)
    {
        // Validasi NIM
        if (string.IsNullOrWhiteSpace(txtNim.Text))
        {
            txtNim.Focus();
            return;
        }

        // Validasi nama
        if (string.IsNullOrWhiteSpace(txtNama.Text))
        {
            txtNama.Focus();
            return;
        }

        // Validasi program studi
        if (cmbProdi.SelectedItem == null)
        {
            cmbProdi.Focus();
            return;
        }

        // Validasi jenis kelamin
        if (rbLaki.IsChecked != true && rbPerempuan.IsChecked != true)
        {
            rbLaki.Focus();
            return;
        }

        // Validasi tanggal lahir
        if (dpTanggalLahir.SelectedDate == null)
        {
            dpTanggalLahir.Focus();
            return;
        }

        // Validasi alamat
        if (string.IsNullOrWhiteSpace(txtAlamat.Text))
        {
            txtAlamat.Focus();
            return;
        }

        // Validasi nomor HP
        if (string.IsNullOrWhiteSpace(txtNoHp.Text))
        {
            txtNoHp.Focus();
            return;
        }

        string nim = txtNim.Text?.Trim() ?? "";
        string nama = txtNama.Text?.Trim() ?? "";

        string prodi = "";

        if (cmbProdi.SelectedItem is ComboBoxItem item)
        {
            prodi = item.Content?.ToString() ?? "";
        }

        string jenisKelamin = "";

        if (rbLaki.IsChecked == true)
        {
            jenisKelamin = "Laki-laki";
        }
        else if (rbPerempuan.IsChecked == true)
        {
            jenisKelamin = "Perempuan";
        }

        DateTimeOffset tanggalLahir = dpTanggalLahir.SelectedDate.Value;

        string alamat = txtAlamat.Text?.Trim() ?? "";
        string noHp = txtNoHp.Text?.Trim() ?? "";

        string data =
            $"{nim} | {nama} | {prodi} | {jenisKelamin} | " +
            $"{tanggalLahir:dd-MM-yyyy} | {alamat} | {noHp}";

        // Jika tidak sedang edit -> TAMBAH
        if (indexEdit == -1)
        {
            semuaMahasiswa.Add(data);
        }
        // Jika sedang edit -> UPDATE
        else
        {
            semuaMahasiswa[indexEdit] = data;
            indexEdit = -1;
        }

        // Tampilkan kembali seluruh data
        TampilkanSemuaData();

        // Bersihkan form setelah simpan
        BersihkanForm();
    }

    private void BtnReset_Click(object? sender, RoutedEventArgs e)
    {
        BersihkanForm();

        txtCari.Text = "";

        indexEdit = -1;
        indexHasilSearch.Clear();

        TampilkanSemuaData();

        txtNim.Focus();
    }

    private void BtnHapus_Click(object? sender, RoutedEventArgs e)
    {
        if (lstMahasiswa.SelectedItem == null)
        {
            return;
        }

        int indexListBox = lstMahasiswa.SelectedIndex;
        int indexData;

        // Jika sedang menampilkan hasil pencarian,
        // gunakan index asli dari data mahasiswa.
        if (indexHasilSearch.Count > 0)
        {
            if (indexListBox < 0 || indexListBox >= indexHasilSearch.Count)
            {
                return;
            }

            indexData = indexHasilSearch[indexListBox];
        }
        else
        {
            indexData = indexListBox;
        }

        if (indexData >= 0 && indexData < semuaMahasiswa.Count)
        {
            semuaMahasiswa.RemoveAt(indexData);
        }

        indexEdit = -1;
        indexHasilSearch.Clear();

        txtCari.Text = "";

        TampilkanSemuaData();
    }

    private void BtnEdit_Click(object? sender, RoutedEventArgs e)
    {
        if (lstMahasiswa.SelectedItem == null)
        {
            return;
        }

        int indexListBox = lstMahasiswa.SelectedIndex;

        if (indexListBox < 0)
        {
            return;
        }

        // Tentukan index asli data
        if (indexHasilSearch.Count > 0)
        {
            if (indexListBox >= indexHasilSearch.Count)
            {
                return;
            }

            indexEdit = indexHasilSearch[indexListBox];
        }
        else
        {
            indexEdit = indexListBox;
        }

        string data = lstMahasiswa.SelectedItem.ToString() ?? "";

        string[] bagian = data.Split(" | ");

        // Pastikan data memiliki 7 bagian
        if (bagian.Length < 7)
        {
            indexEdit = -1;
            return;
        }

        // Isi NIM
        txtNim.Text = bagian[0];

        // Isi nama
        txtNama.Text = bagian[1];

        // Isi program studi
        foreach (var item in cmbProdi.Items)
        {
            if (item is ComboBoxItem comboItem &&
                comboItem.Content?.ToString() == bagian[2])
            {
                cmbProdi.SelectedItem = comboItem;
                break;
            }
        }

        // Isi jenis kelamin
        rbLaki.IsChecked = bagian[3] == "Laki-laki";
        rbPerempuan.IsChecked = bagian[3] == "Perempuan";

        // Isi tanggal lahir
        if (DateTimeOffset.TryParseExact(
            bagian[4],
            "dd-MM-yyyy",
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out DateTimeOffset tanggalLahir))
        {
            dpTanggalLahir.SelectedDate = tanggalLahir;
        }

        // Isi alamat
        txtAlamat.Text = bagian[5];

        // Isi nomor HP
        txtNoHp.Text = bagian[6];

        // Setelah data diedit, pencarian tidak lagi diperlukan
        txtCari.Text = "";
        indexHasilSearch.Clear();
    }

    private void BtnCari_Click(object? sender, RoutedEventArgs e)
    {
        string keyword = txtCari.Text?.Trim() ?? "";

        lstMahasiswa.Items.Clear();
        indexHasilSearch.Clear();

        if (string.IsNullOrWhiteSpace(keyword))
        {
            TampilkanSemuaData();
            return;
        }

        for (int i = 0; i < semuaMahasiswa.Count; i++)
        {
            string data = semuaMahasiswa[i];

            if (data.Contains(
                keyword,
                StringComparison.OrdinalIgnoreCase))
            {
                lstMahasiswa.Items.Add(data);
                indexHasilSearch.Add(i);
            }
        }

        UpdateSearchCounter(lstMahasiswa.Items.Count);
    }

    private void TampilkanSemuaData()
    {
        lstMahasiswa.Items.Clear();

        foreach (string mahasiswa in semuaMahasiswa)
        {
            lstMahasiswa.Items.Add(mahasiswa);
        }

        indexHasilSearch.Clear();

        UpdateCounter();
    }

    private void BersihkanForm()
    {
        txtNim.Clear();
        txtNama.Clear();

        cmbProdi.SelectedIndex = -1;

        rbLaki.IsChecked = false;
        rbPerempuan.IsChecked = false;

        dpTanggalLahir.SelectedDate = null;

        txtAlamat.Clear();
        txtNoHp.Clear();
    }

    private void UpdateCounter()
    {
        txtCounter.Text =
            $"Total Mahasiswa: {semuaMahasiswa.Count}";
    }

    private void UpdateSearchCounter(int jumlahHasil)
    {
        txtCounter.Text =
            $"Hasil Pencarian: {jumlahHasil} dari {semuaMahasiswa.Count}";
    }
}