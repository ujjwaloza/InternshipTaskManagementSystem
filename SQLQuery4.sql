use ITMS_DB;
Go

INSERT INTO Users (FullName, Email, Password, Role)
VALUES 
('Admin User', 'admin@itms.com', 'admin123', 'Admin'),
('Student One', 'student@itms.com', 'student123', 'Student'),
('Mentor One', 'mentor@itms.com', 'mentor123', 'Mentor');

Select Email , Role from Users;
Select * from Projects;
INSERT INTO Projects (Title, Description, StudentId, MentorId, StartDate, EndDate)
VALUES (
    'Internship Task Management System',
    '8th Semester Internship Project',
    1,  -- Student UserId
    3,  -- Mentor UserId
    '2025-01-01',
    '2025-04-30'
);

Select * from Tasks;
SELECT TaskId , Title, Status  FROM Tasks;
SELECT ProjectId, Title FROM Projects;
INSERT INTO Tasks
(Title, Description, Status, ProjectId)
VALUES
(
    'Requirement Analysis',
    'Analyze problem statement and prepare SRS document',
    'ToDo',
    1
);

INSERT INTO Tasks (Title, Description, Status, ProjectId)
VALUES 
('System Design', 'Prepare system architecture and diagrams', 'ToDo', 1),
('Database Design', 'Create ER diagram and database schema', 'ToDo', 1),
('Backend Development', 'Develop controllers and models', 'ToDo', 1),
('UI Development', 'Design Razor views', 'ToDo', 1),
('Testing', 'Test application functionality', 'ToDo', 1);

UPDATE Tasks
SET Status = 'InProgress'
WHERE TaskId = 1;


UPDATE Projects
SET StudentId = 2
WHERE ProjectId = 1;


SELECT ProjectId, StudentId
FROM Projects
WHERE StudentId = 1;
select * from Users;
SELECT 
    t.TaskId,
    t.Title,
    t.ProjectId,
    p.StudentId
FROM Tasks t
LEFT JOIN Projects p ON t.ProjectId = p.ProjectId;
SELECT ProjectId,  Title
FROM Projects;
select  * from Users;
ALTER TABLE Users
DROP CONSTRAINT DF__Users__CreatedAt__4BAC3F29;
ALTER TABLE Users
ALTER COLUMN CreatedAt DATETIME2 NOT NULL;
ALTER TABLE Users
ADD CONSTRAINT DF_Users_CreatedAt
DEFAULT SYSDATETIME() FOR CreatedAt;


SELECT COLUMN_NAME, DATA_TYPE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Users';

EXEC sp_help Projects;
