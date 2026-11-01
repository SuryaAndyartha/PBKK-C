CREATE DATABASE IF NOT EXISTS StudentRegistrationDB;
USE StudentRegistrationDB;

CREATE TABLE IF NOT EXISTS Programs
(
    ProgramId INT AUTO_INCREMENT PRIMARY KEY,
    ProgramCode VARCHAR(20) NOT NULL UNIQUE,
    ProgramName VARCHAR(100) NOT NULL,
    IsActive BOOLEAN NOT NULL DEFAULT TRUE
);

CREATE TABLE IF NOT EXISTS Students
(
    StudentId INT AUTO_INCREMENT PRIMARY KEY,
    NIM VARCHAR(20) NOT NULL UNIQUE,
    Name VARCHAR(100) NOT NULL,
    ProgramId INT NOT NULL,
    BirthDate DATE NULL,
    Address VARCHAR(255) NULL,
    PhoneNumber VARCHAR(20) NULL,
    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt DATETIME NULL,
    CONSTRAINT FK_Students_Programs
        FOREIGN KEY (ProgramId) REFERENCES Programs (ProgramId)
);

INSERT IGNORE INTO Programs (ProgramId, ProgramCode, ProgramName)
VALUES
(1, 'TI', 'Teknik Informatika'),
(2, 'SI', 'Sistem Informasi'),
(3, 'MNJ', 'Manajemen'),
(4, 'ELK', 'Teknik Elektro');

INSERT IGNORE INTO Students (NIM, Name, ProgramId, BirthDate, Address, PhoneNumber)
VALUES
('20231001', 'Ahmad Rizki', 1, '2005-01-15', 'Surabaya', '081200000001'),
('20231002', 'Siti Aisyah', 2, '2005-03-22', 'Sidoarjo', '081200000002'),
('20231003', 'Budi Santoso', 1, '2004-11-08', 'Gresik', '081200000003'),
('20231004', 'Citra Lestari', 3, '2005-07-19', 'Malang', '081200000004'),
('20231005', 'Dimas Prayoga', 4, NULL, NULL, NULL);
