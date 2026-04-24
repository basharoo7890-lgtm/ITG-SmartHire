DBCC CHECKIDENT ('Jobs', RESEED, 0);


INSERT INTO Jobs (Title, Description, Department, Location, MinExperience, RequiredEducation, CreatedBy, CreatedAt) VALUES 
('Backend Developer', 'Developing server-side logic and databases.', 'IT', 'Remote', 2, 'Bachelor', 1, GETDATE()),
('Frontend Developer', 'Creating interactive user interfaces.', 'IT', 'Amman', 1, 'Bachelor', 1, GETDATE()),
('Project Manager', 'Overseeing project timelines.', 'Management', 'Dubai', 5, 'Master', 1, GETDATE()),
('QA Engineer', 'Ensuring software quality.', 'Quality Assurance', 'Zarqa', 2, 'Bachelor', 1, GETDATE()),
('Data Analyst', 'Interpreting complex data sets.', 'Data Science', 'Remote', 3, 'Bachelor', 1, GETDATE());

Select * from Jobs;