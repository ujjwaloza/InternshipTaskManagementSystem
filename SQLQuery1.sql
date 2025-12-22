CREATE TABLE Users (
    UserId INT IDENTITY(1,1) PRIMARY KEY,
    FullName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) UNIQUE NOT NULL,
    Password NVARCHAR(100) NOT NULL,
    Role NVARCHAR(20) NOT NULL, -- Admin, Student, Mentor
    CreatedAt DATETIME DEFAULT GETDATE()
);

CREATE TABLE Projects (
    ProjectId INT IDENTITY(1,1) PRIMARY KEY,
    Title NVARCHAR(100) NOT NULL,
    Description NVARCHAR(500),
    StudentId INT,
    MentorId INT,
    StartDate DATE,
    EndDate DATE,
    FOREIGN KEY (StudentId) REFERENCES Users(UserId),
    FOREIGN KEY (MentorId) REFERENCES Users(UserId)
);

CREATE TABLE Tasks (
    TaskId INT IDENTITY(1,1) PRIMARY KEY,
    Title NVARCHAR(200) NOT NULL,
    Description NVARCHAR(500),
    Status NVARCHAR(20), -- ToDo, InProgress, Done
    ProjectId INT,
    CreatedAt DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (ProjectId) REFERENCES Projects(ProjectId)
);

CREATE TABLE WeeklyReports (
    ReportId INT IDENTITY(1,1) PRIMARY KEY,
    StudentId INT,
    WeekNumber INT,
    Content NVARCHAR(MAX),
    SubmittedOn DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (StudentId) REFERENCES Users(UserId)
);

