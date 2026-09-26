using System;
using System.Globalization;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace StudentRegistrationApp;

public partial class MainWindow : Window
{

    private int indexEdit = -1;

    private List<string> semuaMahasiswa = new();

    private List<int> indexHasilSearch = new();
    public MainWindow()
    {
        InitializeComponent();

        UpdateCounter();
    }

    private void BtnSimpan_Click(object? sender, RoutedEventArgs e)
    {
        if(string.IsNullOrWhiteSpace(txtNim.Text))
        {
            txtNim.Focus();
            return;
        }

        if(string.IsNullOrWhiteSpace(txtNama.Text))
        {
            txtNama.Focus();
            return;
        }

        if(string.IsNullOrWhiteSpace(txtAlamat.Text))
        {
            txtAlamat.Focus();
            return;
        }

        if(string.IsNullOrWhiteSpace(txtNoHp.Text))
        {
            txtNoHp.Focus();
            return;
        }

        if(dpTanggalLahir.SelectedDate == null)
        {
            dpTanggalLahir.Focus();
            return;
        }

        if(cmbProdi.SelectedItem == null)
        {
            return;
        }

        if(rbLaki.IsChecked != true && rbPerempuan.IsChecked != true)
        {   
            return;
        }

        string nim = txtNim.Text ?? "";
        string nama = txtNama.Text ?? "";

        string prodi = "";

        if(cmbProdi.SelectedItem is ComboBoxItem item)
        {
            prodi = item.Content?.ToString() ?? "";
        }

        string jenisKelamin = "";

        if(rbLaki.IsChecked == true)
        {
            jenisKelamin = "Laki-laki"; 
        }
        else if(rbPerempuan.IsChecked == true)
        {
            jenisKelamin = "Perempuan";
        }

        DateTimeOffset? tanggalLahir = dpTanggalLahir.SelectedDate;

        string alamat = txtAlamat.Text ?? "";

        string noHp = txtNoHp.Text ?? "";

        string data = $"{nim} | {nama} | {prodi} | {jenisKelamin} | {tanggalLahir:dd-MM-yyyy} | {alamat} | {noHp}";

        if(indexEdit == -1)
        {
            semuaMahasiswa.Add(data);
            lstMahasiswa.Items.Add(data);

            UpdateCounter();
        }
        else
        {
            semuaMahasiswa[indexEdit] = data;

            indexEdit = -1;

            txtCari.Text = "";

            lstMahasiswa.Items.Clear();

            foreach(string mahasiswa in semuaMahasiswa)
            {
                lstMahasiswa.Items.Add(mahasiswa);
            }

            indexHasilSearch.Clear();
        }
    }

    private void BtnReset_Click(object? sender, RoutedEventArgs e)
    {
        txtNim.Clear();
        txtNama.Clear();

        cmbProdi.SelectedIndex = -1;

        rbLaki.IsChecked = false;
        rbPerempuan.IsChecked = false;

        dpTanggalLahir.SelectedDate = null;

        txtAlamat.Clear();
        txtNoHp.Clear();

        indexEdit = -1;

        txtCari.Text = "";
        UpdateCounter();

        lstMahasiswa.Items.Clear();

        foreach(string mahasiswa in semuaMahasiswa)
        {
            lstMahasiswa.Items.Add(mahasiswa);
        }

        indexHasilSearch.Clear();

        txtNim.Focus();
    }

    private void BtnHapus_Click(object? sender, RoutedEventArgs e)
    {
        if(lstMahasiswa.SelectedItem == null)
        {
            return;
        }

        int indexListBox = lstMahasiswa.SelectedIndex;
        int indexData;

        if(indexHasilSearch.Count > 0)
        {
            indexData = indexHasilSearch[indexListBox];
        }
        else
        {
            indexData = indexListBox;
        }

        semuaMahasiswa.RemoveAt(indexData);
        UpdateCounter();

        lstMahasiswa.Items.Clear();

        foreach(string mahasiswa in semuaMahasiswa)
        {
            lstMahasiswa.Items.Add(mahasiswa);
        }

        indexHasilSearch.Clear();
    }

    private void BtnEdit_Click(object? sender, RoutedEventArgs e)
    {
        if(lstMahasiswa.SelectedItem == null)
        {
            return;
        }

        int indexListBox = lstMahasiswa.SelectedIndex;

        if(indexHasilSearch.Count > 0)
        {
            indexEdit = indexHasilSearch[indexListBox];
        }
        else
        {
            indexEdit = indexListBox;
        }

        string data = lstMahasiswa.SelectedItem.ToString() ?? "";
        string[] bagian = data.Split(" | ");

        txtNim.Text = bagian[0];
        txtNama.Text = bagian[1];

        if(cmbProdi.ItemsSource == null)
        {
            foreach(var item in cmbProdi.Items)
            {
                if(item is ComboBoxItem comboItem && comboItem.Content?.ToString() == bagian[2])
                {
                    cmbProdi.SelectedItem = comboItem;
                    break;
                }
            }
        }

        if(bagian[3] == "Laki-laki")
        {
            rbLaki.IsChecked = true;
        }
        else if(bagian[3] == "Perempuan"){
            rbPerempuan.IsChecked = true;
        }

        if(DateTimeOffset.TryParseExact(
            bagian[4], "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTimeOffset tanggalLahir
        ))
        {
            dpTanggalLahir.SelectedDate = tanggalLahir;
        }

        txtAlamat.Text = bagian[5];
        txtNoHp.Text = bagian[6];
    }

    private void BtnCari_Click(object? sender, RoutedEventArgs e)
    {
        string keyword = txtCari.Text ?? "";

        lstMahasiswa.Items.Clear();
        indexHasilSearch.Clear();

        for(int i = 0; i < semuaMahasiswa.Count; i++)
        {
            string data = semuaMahasiswa[i];

            if(data.Contains(keyword, StringComparison.OrdinalIgnoreCase))
            {
                lstMahasiswa.Items.Add(data);
                indexHasilSearch.Add(i);
            }
        }

        UpdateSearchCounter(lstMahasiswa.Items.Count);
    }

    private void UpdateCounter()
    {
        txtCounter.Text = $"Total Mahasiswa: {semuaMahasiswa.Count}";
    }

    private void UpdateSearchCounter(int jumlahHasil)
    {
        txtCounter.Text = $"Hasil Pencarian: {jumlahHasil} dari {semuaMahasiswa.Count}";
    }
}