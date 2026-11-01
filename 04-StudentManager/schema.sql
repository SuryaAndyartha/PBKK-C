CREATE DATABASE IF NOT EXISTS StudentDB;
USE StudentDB;

CREATE TABLE IF NOT EXISTS Students
(
    Id INT AUTO_INCREMENT PRIMARY KEY,
    NIM VARCHAR(20) NOT NULL,
    Nama VARCHAR(100) NOT NULL,
    Jurusan VARCHAR(100) NOT NULL,
    Gender VARCHAR(20) NOT NULL,
    Email VARCHAR(100)
);

INSERT INTO Students (NIM, Nama, Jurusan, Gender, Email)
VALUES
('23001', 'Budi Santoso', 'Informatika', 'Laki-laki', 'budi@gmail.com'),
('23002', 'Siti Aminah', 'Sistem Informasi', 'Perempuan', 'siti@gmail.com'),
('23003', 'Andi Wijaya', 'Informatika', 'Laki-laki', 'andi@gmail.com'),
('23004', 'Rina Sari', 'Sistem Informasi', 'Perempuan', 'rina@gmail.com');
