using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace FoodOrdering;

public partial class MainWindow : Window
{
    private int indexEdit = -1;

    // Menyimpan seluruh data pesanan
    private readonly List<string> semuaPesanan = new();

    // Menyimpan index data asli ketika sedang melakukan pencarian
    private readonly List<int> indexHasilSearch = new();

    public MainWindow()
    {
        InitializeComponent();
        UpdateCounter();
    }

    private void BtnSimpan_Click(object? sender, RoutedEventArgs e)
    {
        // Sembunyikan pesan error dari percobaan simpan sebelumnya
        SembunyikanError();

        // Field kosong pertama yang akan mendapat focus
        Control? fieldKosongPertama = null;

        // Jumlah field yang kosong (untuk pesan umum jika semua kosong)
        int jumlahKosong = 0;
        const int totalField = 7;

        // Validasi kode pesanan
        if (string.IsNullOrWhiteSpace(txtKode.Text))
        {
            errKode.IsVisible = true;
            fieldKosongPertama ??= txtKode;
            jumlahKosong++;
        }

        // Validasi nama pelanggan
        if (string.IsNullOrWhiteSpace(txtPelanggan.Text))
        {
            errPelanggan.IsVisible = true;
            fieldKosongPertama ??= txtPelanggan;
            jumlahKosong++;
        }

        // Validasi menu
        if (cmbMenu.SelectedItem == null)
        {
            errMenu.IsVisible = true;
            fieldKosongPertama ??= cmbMenu;
            jumlahKosong++;
        }

        // Validasi tipe pesanan
        if (rbDineIn.IsChecked != true && rbTakeAway.IsChecked != true)
        {
            errTipe.IsVisible = true;
            fieldKosongPertama ??= rbDineIn;
            jumlahKosong++;
        }

        // Validasi tanggal pesanan
        if (dpTanggal.SelectedDate == null)
        {
            errTanggal.IsVisible = true;
            fieldKosongPertama ??= dpTanggal;
            jumlahKosong++;
        }

        // Validasi catatan
        if (string.IsNullOrWhiteSpace(txtCatatan.Text))
        {
            errCatatan.IsVisible = true;
            fieldKosongPertama ??= txtCatatan;
            jumlahKosong++;
        }

        // Validasi nomor HP
        if (string.IsNullOrWhiteSpace(txtNoHp.Text))
        {
            errNoHp.IsVisible = true;
            fieldKosongPertama ??= txtNoHp;
            jumlahKosong++;
        }

        // Jika semua field kosong, tampilkan juga pesan umum
        if (jumlahKosong == totalField)
        {
            bdrErrorUmum.IsVisible = true;
        }

        // Ada field kosong -> focus ke field kosong pertama, data tidak disimpan
        if (fieldKosongPertama != null)
        {
            fieldKosongPertama.Focus();
            return;
        }

        string kode = txtKode.Text?.Trim() ?? "";
        string pelanggan = txtPelanggan.Text?.Trim() ?? "";

        string menu = "";

        if (cmbMenu.SelectedItem is ComboBoxItem item)
        {
            menu = item.Content?.ToString() ?? "";
        }

        string tipePesanan = "";

        if (rbDineIn.IsChecked == true)
        {
            tipePesanan = "Dine In";
        }
        else if (rbTakeAway.IsChecked == true)
        {
            tipePesanan = "Take Away";
        }

        DateTimeOffset tanggalPesanan = dpTanggal.SelectedDate!.Value;

        string catatan = txtCatatan.Text?.Trim() ?? "";
        string noHp = txtNoHp.Text?.Trim() ?? "";

        string data =
            $"{kode} | {pelanggan} | {menu} | {tipePesanan} | " +
            $"{tanggalPesanan:dd-MM-yyyy} | {catatan} | {noHp}";

        // Jika tidak sedang edit -> TAMBAH
        if (indexEdit == -1)
        {
            semuaPesanan.Add(data);
        }
        // Jika sedang edit -> UPDATE
        else
        {
            semuaPesanan[indexEdit] = data;
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

        txtKode.Focus();
    }

    private void BtnHapus_Click(object? sender, RoutedEventArgs e)
    {
        if (lstPesanan.SelectedItem == null)
        {
            return;
        }

        int indexListBox = lstPesanan.SelectedIndex;
        int indexData;

        // Jika sedang menampilkan hasil pencarian,
        // gunakan index asli dari data pesanan.
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

        if (indexData >= 0 && indexData < semuaPesanan.Count)
        {
            semuaPesanan.RemoveAt(indexData);
        }

        indexEdit = -1;
        indexHasilSearch.Clear();

        txtCari.Text = "";

        TampilkanSemuaData();
    }

    private void BtnEdit_Click(object? sender, RoutedEventArgs e)
    {
        if (lstPesanan.SelectedItem == null)
        {
            return;
        }

        int indexListBox = lstPesanan.SelectedIndex;

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

        string data = lstPesanan.SelectedItem.ToString() ?? "";

        string[] bagian = data.Split(" | ");

        // Pastikan data memiliki 7 bagian
        if (bagian.Length < 7)
        {
            indexEdit = -1;
            return;
        }

        // Isi kode pesanan
        txtKode.Text = bagian[0];

        // Isi nama pelanggan
        txtPelanggan.Text = bagian[1];

        // Isi menu
        foreach (var item in cmbMenu.Items)
        {
            if (item is ComboBoxItem comboItem &&
                comboItem.Content?.ToString() == bagian[2])
            {
                cmbMenu.SelectedItem = comboItem;
                break;
            }
        }

        // Isi tipe pesanan
        rbDineIn.IsChecked = bagian[3] == "Dine In";
        rbTakeAway.IsChecked = bagian[3] == "Take Away";

        // Isi tanggal pesanan
        if (DateTimeOffset.TryParseExact(
            bagian[4],
            "dd-MM-yyyy",
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out DateTimeOffset tanggalPesanan))
        {
            dpTanggal.SelectedDate = tanggalPesanan;
        }

        // Isi catatan
        txtCatatan.Text = bagian[5];

        // Isi nomor HP
        txtNoHp.Text = bagian[6];

        // Form sudah terisi dari data, pesan error lama tidak relevan lagi
        SembunyikanError();

        // Setelah data diedit, pencarian tidak lagi diperlukan
        txtCari.Text = "";
        indexHasilSearch.Clear();
    }

    private void BtnCari_Click(object? sender, RoutedEventArgs e)
    {
        string keyword = txtCari.Text?.Trim() ?? "";

        lstPesanan.Items.Clear();
        indexHasilSearch.Clear();

        if (string.IsNullOrWhiteSpace(keyword))
        {
            TampilkanSemuaData();
            return;
        }

        for (int i = 0; i < semuaPesanan.Count; i++)
        {
            string data = semuaPesanan[i];

            if (data.Contains(
                keyword,
                StringComparison.OrdinalIgnoreCase))
            {
                lstPesanan.Items.Add(data);
                indexHasilSearch.Add(i);
            }
        }

        UpdateSearchCounter(lstPesanan.Items.Count);
    }

    private void TampilkanSemuaData()
    {
        lstPesanan.Items.Clear();

        foreach (string pesanan in semuaPesanan)
        {
            lstPesanan.Items.Add(pesanan);
        }

        indexHasilSearch.Clear();

        UpdateCounter();
    }

    private void BersihkanForm()
    {
        txtKode.Clear();
        txtPelanggan.Clear();

        cmbMenu.SelectedIndex = -1;

        rbDineIn.IsChecked = false;
        rbTakeAway.IsChecked = false;

        dpTanggal.SelectedDate = null;

        txtCatatan.Clear();
        txtNoHp.Clear();

        // Pesan error ikut dibersihkan bersama form
        SembunyikanError();
    }

    // Menyembunyikan seluruh pesan validasi
    private void SembunyikanError()
    {
        bdrErrorUmum.IsVisible = false;

        errKode.IsVisible = false;
        errPelanggan.IsVisible = false;
        errMenu.IsVisible = false;
        errTipe.IsVisible = false;
        errTanggal.IsVisible = false;
        errCatatan.IsVisible = false;
        errNoHp.IsVisible = false;
    }

    private void UpdateCounter()
    {
        txtCounter.Text =
            $"Total Pesanan: {semuaPesanan.Count}";
    }

    private void UpdateSearchCounter(int jumlahHasil)
    {
        txtCounter.Text =
            $"Hasil Pencarian: {jumlahHasil} dari {semuaPesanan.Count}";
    }
}