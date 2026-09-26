using System;
using System.Globalization;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace StudentRegistrationApp;

public partial class MainWindow : Window
{
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

        lstMahasiswa.Items.Add(data);

        // await MessageBoxManager.GetMessageBoxStandard(
        //     "Informasi",
        //     "Data mahasiswa berhasil disimpan!"
        // ).ShowAsync();
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

        string data = lstMahasiswa.SelectedItem.ToString() ?? "";

        string[] bagian = data.Split(" | ");

        txtNim.Text = bagian[0];
        txtNama.Text = bagian[1];

        // if(cmbProdi.ItemsSource == null)
        // {
        //     foreach(ComboBoxItem item in cmbProdi.Items)
        //     {
        //         if(item.Content?.ToString() == bagian[2])
        //         {
        //             cmbProdi.SelectedItem = item;
        //             break;
        //         }
        //     }
        // }

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

        string prodi = "";

        if(cmbProdi.SelectedItem is ComboBoxItem selectedProdi)
        {
            prodi = selectedProdi.Content?.ToString() ?? "";
        }

        string jenisKelamin = "";

        if(bagian[3] == "Laki-laki")
        {
            rbLaki.IsChecked = true;
            jenisKelamin = "Laki-laki";
        }
        else if(bagian[3] == "Perempuan"){
            rbPerempuan.IsChecked = true;
            jenisKelamin = "Perempuan";
        }

        if(DateTimeOffset.TryParseExact(
            bagian[4], "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTimeOffset tanggalLahir
        ))
        {
            dpTanggalLahir.SelectedDate = tanggalLahir;
        }

        txtAlamat.Text = bagian[5];
        txtNoHp.Text = bagian[6];

        string dataBaru = $"{txtNim.Text} | {txtNama.Text} | {prodi} | {jenisKelamin} | {dpTanggalLahir.SelectedDate:dd-MM-yyyy} | {txtAlamat.Text} | {txtNoHp.Text}";

        int index = lstMahasiswa.SelectedIndex;

        lstMahasiswa.Items[index] = dataBaru;
    }
}