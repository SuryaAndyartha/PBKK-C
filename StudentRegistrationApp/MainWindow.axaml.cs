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
    public MainWindow()
    {
        InitializeComponent();
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
        }
        else
        {
            lstMahasiswa.Items[indexEdit] = data;
            indexEdit = -1;
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

        txtNim.Focus();
    }

    private void BtnHapus_Click(object? sender, RoutedEventArgs e)
    {
        if(lstMahasiswa.SelectedItem == null)
        {
            return;
        }

        lstMahasiswa.Items.Remove(lstMahasiswa.SelectedItem);
    }

    private void BtnEdit_Click(object? sender, RoutedEventArgs e)
    {
        if(lstMahasiswa.SelectedItem == null)
        {
            return;
        }

        indexEdit = lstMahasiswa.SelectedIndex;

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

        foreach(string data in semuaMahasiswa)
        {
            if(data.Contains(keyword, StringComparison.OrdinalIgnoreCase))
            {
                lstMahasiswa.Items.Add(data);
            }
        }
    }
}