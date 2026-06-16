Create Database BookingSystem;

use BookingSystem;

CREATE TABLE Roles
(
    RoleId INT PRIMARY KEY ,
    RoleName VARCHAR(50) NOT NULL UNIQUE
)

INSERT INTO Roles(RoleId,RoleName)
VALUES (1,'Admin'),
       (2, 'Staff'),
       (3,'User');

SELECT * FROM Roles;

DELETE FROM Roles; 
DROP TABLE Roles;


/* USER TABLE*/

CREATE TABLE Users
(
    UserId INT PRIMARY KEY IDENTITY,
    RoleId INT NOT NULL,
    FirstName VARCHAR(100) NOT NULL,
    LastName VARCHAR(100),
    Email VARCHAR(255) UNIQUE NOT NULL,
    PhoneNumber VARCHAR(20),
    PasswordHash NVARCHAR(MAX) NOT NULL,
    IsActive BIT DEFAULT 1,
    CreatedAt DATETIME DEFAULT GETDATE(),
    FOREIGN KEY(RoleId)
    REFERENCES Roles(RoleId)
)

INSERT INTO Users(RoleId,FirstName,LastName,Email,PhoneNumber,PasswordHash)
VALUES(1,'Delwin','Dennis','delwin@gmail.com',6369508338,'del@123');

SELECT * FROM Users;



/*COURTS STATUS*/

CREATE TABLE Courts
(
    CourtId INT PRIMARY KEY IDENTITY,

    CourtName VARCHAR(100) NOT NULL,

    HourlyRate DECIMAL(10,2) NOT NULL,

    CourtStatus VARCHAR(20) NOT NULL,

    CreatedAt DATETIME DEFAULT GETDATE()
)


INSERT INTO Courts(CourtName,HourlyRate,CourtStatus)
VALUES ('A',100,'Available');

SELECT * FROM Courts;

DELETE FROM Courts;
DROP TABLE Courts;


/*Court Images*/

CREATE TABLE CourtImages
(
    ImageId INT PRIMARY KEY IDENTITY,

    CourtId INT NOT NULL,

    ImagePath NVARCHAR(500),

    DisplayOrder INT DEFAULT 1,

    FOREIGN KEY(CourtId)
    REFERENCES Courts(CourtId)
)

/*TIME SLOT*/

CREATE TABLE TimeSlots
(
    TimeSlotId INT PRIMARY KEY IDENTITY,

    StartTime TIME NOT NULL,

    EndTime TIME NOT NULL,

    IsActive BIT DEFAULT 1
)

INSERT INTO TimeSlots(StartTime,EndTime)
VALUES('6:00:00','7:00:00'),
('7:00:00','8:00:00'),
('8:00:00','9:00:00'),
('9:00:00','10:00:00'),
('10:00:00','11:00:00'),
('11:00:00','12:00:00'),
('12:00:00','13:00:00'),
('13:00:00','14:00:00'),
('14:00:00','15:00:00'),
('15:00:00','16:00:00'),
('16:00:00','17:00:00'),
('17:00:00','18:00:00'),
('18:00:00','19:00:00'),
('19:00:00','20:00:00'),
('20:00:00','21:00:00'),
('21:00:00','22:00:00');


SELECT * FROM TimeSlots;


/*Bookings */


CREATE TABLE Bookings
(
    BookingId INT PRIMARY KEY IDENTITY,

    BookingReference UNIQUEIDENTIFIER
        DEFAULT NEWID(),

    UserId INT NOT NULL,

    CourtId INT UNIQUE NOT NULL,

    TimeSlotId INT UNIQUE NOT NULL,

    BookingDate DATE UNIQUE NOT NULL,

    Amount DECIMAL(10,2),

    BookingStatus VARCHAR(20),

    PaymentStatus VARCHAR(20),

    CheckInStatus BIT DEFAULT 0,

    Notes VARCHAR(500),

    CreatedAt DATETIME DEFAULT GETDATE(),

    FOREIGN KEY(UserId)
        REFERENCES Users(UserId),

    FOREIGN KEY(CourtId)
        REFERENCES Courts(CourtId),

    FOREIGN KEY(TimeSlotId)
        REFERENCES TimeSlots(TimeSlotId)
)

SELECT * FROM Bookings;

DROP TABLE Bookings;

/* REVIEW SECTION */

CREATE TABLE Reviews
(
    ReviewId INT PRIMARY KEY IDENTITY,

    UserId INT NOT NULL,

    CourtId INT NOT NULL,

    Rating INT NOT NULL,

    Comment VARCHAR(1000),

    CreatedAt DATETIME DEFAULT GETDATE(),

    FOREIGN KEY(UserId)
        REFERENCES Users(UserId),

    FOREIGN KEY(CourtId)
        REFERENCES Courts(CourtId)
)



SELECT * FROM Reviews;


/* NOTIFICATION */

CREATE TABLE Notifications
(
    NotificationId INT PRIMARY KEY IDENTITY,

    UserId INT NOT NULL,

    Title VARCHAR(200),

    Message VARCHAR(1000),

    IsRead BIT DEFAULT 0,

    CreatedAt DATETIME DEFAULT GETDATE(),

    FOREIGN KEY(UserId)
        REFERENCES Users(UserId)
)


SELECT * FROM Notifications;


SELECT TABLE_NAME
FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_TYPE = 'BASE TABLE';


SELECT UserId,
       Email,
       PasswordHash
FROM Users

DELETE FROM Users
WHERE Email = 'delwin@gmail.com';

SELECT Email, PasswordHash
FROM Users;

SELECT *
FROM Roles;

UPDATE Roles
SET RoleName = 'Customer'
WHERE RoleName = 'User';


INSERT INTO Users
(
    RoleId,
    FirstName,
    LastName,
    Email,
    PhoneNumber,
    PasswordHash,
    IsActive
)
VALUES
(
    1,
    'System',
    'Admin',
    'admin@badminton.com',
    '9999999999',
    'PASTE_HASH_HERE',
    1
);


SELECT UserId, Email, RoleId
FROM Users;

SELECT UserId,
       Email,
       PasswordHash
FROM Users;

DELETE FROM Users
WHERE Email = 'admin@badminton.com';


INSERT INTO Users
(
    RoleId,
    FirstName,
    LastName,
    Email,
    PhoneNumber,
    PasswordHash,
    IsActive,
    CreatedAt
)
VALUES
(
    1,
    'System',
    'Admin',
    'admin@badminton.com',
    '9999999999',
    'THE_REAL_HASH_YOU_COPIED',
    1,
    GETDATE()
);

SELECT Email, PasswordHash
FROM Users;

SELECT Email, PasswordHash
FROM Users
WHERE Email = 'admin@badminton.com';

UPDATE Users
SET PasswordHash = 'AQAAAAIAAYagAAAAEBdXwenM7RelRVKx8i0aqT0uL02PYgxjjPmsVmzvf8In5gtF1WJv6izFmLHVAwNxmQ=='
WHERE Email = 'admin@badminton.com';


INSERT INTO Users
(
    RoleId,
    FirstName,
    LastName,
    Email,
    PhoneNumber,
    PasswordHash,
    IsActive,
    CreatedAt
)
VALUES
(
    1,
    'System',
    'Admin',
    'admin@badminton.com',
    '9999999999',
    'AQAAAAIAAYagAAAAEOnxe122g5Jy9x1YHTP4eDNb4PORnFYHccsW9sU4RBoUtRi+/g+A3NX//d8u6BAQ/Q==',
    1,
    GETDATE()
);


SELECT * FROM Users;

SELECT * FROM Courts;

SELECT * FROM CourtImages;

DELETE FROM CourtImages
WHERE ImagePath='/uploads/courts/c16dc9de-5910-4a5d-bd49-54baba3984c8.png';


SELECT TOP 10 *
FROM Bookings;


SELECT
    i.name AS ConstraintName,
    c.name AS ColumnName
FROM sys.indexes i
JOIN sys.index_columns ic
    ON i.object_id = ic.object_id
    AND i.index_id = ic.index_id
JOIN sys.columns c
    ON ic.object_id = c.object_id
    AND ic.column_id = c.column_id
WHERE OBJECT_NAME(i.object_id) = 'Bookings'
AND i.is_unique = 1;


ALTER TABLE Bookings
DROP CONSTRAINT UQ__Bookings__34C8A99F01F9CDE0;

ALTER TABLE Bookings
DROP CONSTRAINT UQ__Bookings__41CC1F334301DBD3;


ALTER TABLE Bookings
DROP CONSTRAINT UQ__Bookings__C3A67C9B0E095F15;


SELECT
    i.name AS ConstraintName,
    c.name AS ColumnName
FROM sys.indexes i
JOIN sys.index_columns ic
    ON i.object_id = ic.object_id
    AND i.index_id = ic.index_id
JOIN sys.columns c
    ON ic.object_id = c.object_id
    AND ic.column_id = c.column_id
WHERE OBJECT_NAME(i.object_id) = 'Bookings'
AND i.is_unique = 1;


EXEC sp_helpindex 'Bookings';


SELECT COLUMN_NAME
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Bookings';

SELECT * FROM __EFMigrationsHistory;


SELECT TABLE_NAME
FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_TYPE = 'BASE TABLE'
ORDER BY TABLE_NAME;

SELECT TOP 1 *
FROM Bookings;

SELECT TOP 10
    BookingId,
    BookingStatus,
    ConfirmedAt,
    CheckInStatus,
    CheckedInAt
FROM Bookings;


SELECT TABLE_NAME
FROM INFORMATION_SCHEMA.TABLES
ORDER BY TABLE_NAME;

SELECT MigrationId
FROM __EFMigrationsHistory
ORDER BY MigrationId;

SELECT RoleId, RoleName
FROM Roles;