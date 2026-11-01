using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using StudentManager.Data;
using StudentManager.Models;

namespace StudentManager.ViewModels;

public class StudentViewModel : INotifyPropertyChanged
{
    private readonly StudentRepository _repository = new();

    // 0 berarti form sedang membuat data baru
    private int _editId;

    public ObservableCollection<Student> Students { get; } = new();

    public List<string> JurusanOptions { get; } =
        new() { "Informatika", "Sistem Informasi" };

    public List<string> GenderOptions { get; } =
        new() { "Laki-laki", "Perempuan" };

    private Student? _selectedStudent;
    public Student? SelectedStudent
    {
        get => _selectedStudent;
        set
        {
            _selectedStudent = value;
            OnPropertyChanged();

            // Form memakai salinan agar tabel tidak berubah sebelum disimpan
            if (value != null)
            {
                FillForm(value);
            }
        }
    }

    private string _nim = "";
    public string NIM
    {
        get => _nim;
        set { _nim = value; OnPropertyChanged(); }
    }

    private string _nama = "";
    public string Nama
    {
        get => _nama;
        set { _nama = value; OnPropertyChanged(); }
    }

    private string? _jurusan;
    public string? Jurusan
    {
        get => _jurusan;
        set { _jurusan = value; OnPropertyChanged(); }
    }

    private string? _gender;
    public string? Gender
    {
        get => _gender;
        set { _gender = value; OnPropertyChanged(); }
    }

    private string _email = "";
    public string Email
    {
        get => _email;
        set { _email = value; OnPropertyChanged(); }
    }

    private string _searchText = "";
    public string SearchText
    {
        get => _searchText;
        set { _searchText = value; OnPropertyChanged(); }
    }

    private string _statusMessage = "";
    public string StatusMessage
    {
        get => _statusMessage;
        set { _statusMessage = value; OnPropertyChanged(); }
    }

    public int TotalStudents => Students.Count;
    public int TotalInformatika =>
        Students.Count(x => x.Jurusan == "Informatika");
    public int TotalSistemInformasi =>
        Students.Count(x => x.Jurusan == "Sistem Informasi");
    public int TotalLakiLaki =>
        Students.Count(x => x.Gender == "Laki-laki");
    public int TotalPerempuan =>
        Students.Count(x => x.Gender == "Perempuan");

    public ICommand SaveCommand { get; }
    public ICommand DeleteCommand { get; }
    public ICommand ResetCommand { get; }
    public ICommand SearchCommand { get; }

    public StudentViewModel()
    {
        SaveCommand = new RelayCommand(Save);
        DeleteCommand = new RelayCommand(Delete);
        ResetCommand = new RelayCommand(Reset);
        SearchCommand = new RelayCommand(Search);

        RunDatabaseAction(LoadData);
    }

    private void LoadData()
    {
        ShowStudents(_repository.GetAll());
    }

    private void Save()
    {
        if (string.IsNullOrWhiteSpace(NIM) ||
            string.IsNullOrWhiteSpace(Nama) ||
            Jurusan == null ||
            Gender == null)
        {
            StatusMessage = "NIM, Nama, Jurusan, dan Gender wajib diisi.";
            return;
        }

        var student = new Student
        {
            Id = _editId,
            NIM = NIM.Trim(),
            Nama = Nama.Trim(),
            Jurusan = Jurusan,
            Gender = Gender,
            Email = Email.Trim()
        };

        RunDatabaseAction(() =>
        {
            if (student.Id == 0)
                _repository.Insert(student);
            else
                _repository.Update(student);

            LoadData();
            Reset();
            StatusMessage = "Data berhasil disimpan.";
        });
    }

    private void Delete()
    {
        if (_editId == 0)
        {
            StatusMessage = "Pilih data pada tabel terlebih dahulu.";
            return;
        }

        RunDatabaseAction(() =>
        {
            _repository.Delete(_editId);

            LoadData();
            Reset();
            StatusMessage = "Data berhasil dihapus.";
        });
    }

    private void Search()
    {
        RunDatabaseAction(() =>
        {
            var result = string.IsNullOrWhiteSpace(SearchText)
                ? _repository.GetAll()
                : _repository.Search(SearchText.Trim());

            ShowStudents(result);
        });
    }

    private void Reset()
    {
        SelectedStudent = null;
        _editId = 0;
        NIM = "";
        Nama = "";
        Jurusan = null;
        Gender = null;
        Email = "";
        StatusMessage = "";
    }

    private void FillForm(Student student)
    {
        _editId = student.Id;
        NIM = student.NIM;
        Nama = student.Nama;
        Jurusan = student.Jurusan;
        Gender = student.Gender;
        Email = student.Email;
    }

    private void ShowStudents(List<Student> students)
    {
        Students.Clear();

        foreach (var student in students)
            Students.Add(student);

        RefreshStatistics();
    }

    private void RunDatabaseAction(Action action)
    {
        try
        {
            action();
        }
        catch (Exception ex)
        {
            StatusMessage = $"Gagal mengakses database: {ex.Message}";
        }
    }

    private void RefreshStatistics()
    {
        OnPropertyChanged(nameof(TotalStudents));
        OnPropertyChanged(nameof(TotalInformatika));
        OnPropertyChanged(nameof(TotalSistemInformasi));
        OnPropertyChanged(nameof(TotalLakiLaki));
        OnPropertyChanged(nameof(TotalPerempuan));
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged(
        [CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(propertyName));
    }
}
