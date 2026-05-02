INSERT INTO Application (JobId, UserId, Phone, ExpectedSalary, YearsOfExperience, EducationLevel, CVFileName, CVText, Status, SubmittedAt) VALUES 
-- بثينة (UserId: 3) Backend Developer (JobId: 1)
(1, 3, '0790000001', 850.00, 2, 'Bachelor', 'CV_Butheinah.pdf', 'Expert in Python and Django development...', 'Pending', GETDATE()),

-- محمود (UserId: 4) Project Manager (JobId: 3)
(3, 4, '0780000002', 1200.00, 5, 'Master', 'CV_Mahmoed.pdf', 'Experienced in Agile and Team Leadership...', 'Pending', GETDATE()),

--عساف QA Engineer (JobId: 4)
(4, 5, '0770000003', 700.00, 2, 'Bachelor', 'CV_Assaf.pdf', 'Specialized in Manual and Automation Testing...', 'Pending', GETDATE());

