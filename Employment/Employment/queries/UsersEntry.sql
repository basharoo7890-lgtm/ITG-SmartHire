DBCC CHECKIDENT ('Users', RESEED, 0);

INSERT INTO Users (FullName, Email, Role, PasswordHash) VALUES 
('Farah Atef', 'farahatef@gmail.com', 'Admin', 'pass123'),
('Nagham Atef', 'naghamatef@gmail.com', 'HR', 'pass123'),
('Butheinah Atef', 'butheinahatef@gmail.com', 'Applicant', 'pass123'),
('Mahmoed Atef', 'mahmoedatef@gmail.com', 'Admin', 'pass123'),
('Assaf Ahmed', 'assafahmed@gmail.com', 'HR', 'pass123');

Select * from Users;

