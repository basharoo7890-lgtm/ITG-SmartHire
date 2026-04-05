# Sprint Plan — ITG SmartHire

Start Date: ___/___/2026
Duration: 8 weeks (4 Sprints)

---

## Team

| Member | Role | Tools |
|--------|------|-------|
| Bashar | PMO + DevOps | VS Code, Docker Desktop, AWS CLI |
| Omar | Backend (Core) | Visual Studio 2022/2026 |
| Mazen | Full-Stack (Dashboard + Admin) | Visual Studio 2022/2026 |
| Ghazal | Backend + Documentation | Visual Studio 2022/2026 |
| Sedeen | AI Developer | Visual Studio 2022/2026 |
| Farah | Database + QA | SQL Server Management Studio |
| Rama | UI/UX Design | Figma + Visual Studio 2022/2026 |

---

## Sprint 0: Setup (Days 1-3)

### Bashar
- [ ] Create GitHub repository and add team as collaborators
- [ ] Upload initial files (README, Architecture, Git Guide, .gitignore, .env.example)
- [ ] Create GitHub Project Board (To Do, In Progress, Review, Done)
- [ ] Create develop branch
- [ ] Register Gemini API key (free)
- [ ] Send Git Guide to team
- [ ] Coordinate with ITG supervisor

### Omar
- [ ] Install tools (.NET 8 SDK, Visual Studio, Git)
- [ ] Create project: dotnet new mvc -n SmartHire
- [ ] Create Solution and push initial structure to GitHub

### Mazen
- [ ] Install tools (.NET 8 SDK, Visual Studio, Git)
- [ ] Read Architecture document — understand Dashboard and Admin pages
- [ ] Clone project from GitHub and verify it runs

### Ghazal
- [ ] Install tools (.NET 8 SDK, Visual Studio, Git)
- [ ] Write core Models: User, Job, JobSkill, Application, AIAnalysis, SkillSynonym, SystemSetting
- [ ] Coordinate with Farah on field names and types

### Sedeen
- [ ] Install tools (.NET 8 SDK, Visual Studio, Git)
- [ ] Research Gemini API integration in C# (HTTP POST)
- [ ] Test sending a simple request to Gemini API and getting a response

### Farah
- [ ] Install SQL Server + SSMS + Git
- [ ] Design ERD using draw.io or dbdiagram.io
- [ ] Coordinate with Ghazal to match tables with Models

### Rama
- [ ] Install Git
- [ ] Create wireframes in Figma for all pages:
  - Login, Register
  - Job Listings, Job Details
  - Application Form, My Applications
  - HR Dashboard, Candidate Details
  - Candidate Comparison, Statistics
  - Create Job, Admin Settings

---

## Sprint 1: Foundation (Weeks 1-2)

### Bashar
- [ ] Write Dockerfile and test Docker build
- [ ] Track team progress on Project Board
- [ ] Help team with any Git issues
- [ ] First progress report to ITG supervisor (end of week 2)

### Omar
- [ ] Set up ASP.NET Identity (Register + Login + 3 Roles)
- [ ] Create HomeController (landing page + job listings)
- [ ] Create AccountController (Login, Register, Logout)
- [ ] Create JobsController (job details page)
- [ ] Create ApplicationsController (apply form + save + CV upload)

### Mazen
- [ ] Create DashboardController (basic structure)
- [ ] Create AdminController (basic structure)
- [ ] Connect pages to test data from Farah

### Ghazal
- [ ] Create ViewModels for each page
- [ ] Create Service interfaces (IJobService, IApplicationService, IAIService)
- [ ] Help Omar and Mazen with Model changes if needed
- [ ] Start project documentation

### Sedeen
- [ ] Build CVParserService (extract text from PDF using PdfPig)
- [ ] Build basic AIService structure (connect to Gemini API)
- [ ] Test sending a real CV to Gemini and getting JSON back

### Farah
- [ ] Create AppDbContext with all tables
- [ ] Create first Migration and run it
- [ ] Add seed data: 5 jobs, 20 skills, 10 synonyms, admin user, HR user

### Rama
- [ ] Build _Layout.cshtml (Navbar + Footer + Bootstrap)
- [ ] Build Login page
- [ ] Build Register page
- [ ] Build Job Listings page (Index)
- [ ] Build Job Details page

---

## Sprint 2: Apply + Filtering (Weeks 3-4)

### Bashar
- [ ] Create AWS Free Tier account
- [ ] Test deploying project to EC2
- [ ] Create RDS database and S3 bucket
- [ ] Mid-project progress report to supervisor

### Omar
- [ ] Implement application submission + CV upload + storage
- [ ] Build "My Applications" page for applicants
- [ ] Build ApplicationService with auto-filtering logic

### Mazen
- [ ] Build full HR Dashboard (table + filters + accept/reject buttons)
- [ ] Build Candidate Details page
- [ ] Build Create Job page for Admin

### Ghazal
- [ ] Implement auto-filtering logic (reject over-budget salary, under-experience)
- [ ] Help Mazen connect Dashboard to data
- [ ] Update documentation

### Sedeen
- [ ] Complete AIService (send CV to Gemini + receive structured JSON)
- [ ] Build MatchingService (calculate 4 scores + total score)
- [ ] Connect AI analysis to application submission flow

### Farah
- [ ] Test application submission and storage
- [ ] Test file upload functionality
- [ ] Test auto-filtering with different examples

### Rama
- [ ] Build Application Form page (Apply) with file upload
- [ ] Build My Applications page
- [ ] Improve HR Dashboard styling with Mazen (score colors, sorting)

---

## Sprint 3: AI Features + Advanced (Weeks 5-6)

### Bashar
- [ ] Add Gemini API Key to AWS environment settings
- [ ] Test AI features on server
- [ ] Monitor API usage (stay within free tier)

### Omar
- [ ] Connect AIService to submission flow (auto-analyze after submit)
- [ ] Fix any Controller issues

### Mazen
- [ ] Build Candidate Comparison page (side-by-side)
- [ ] Build Statistics page with Chart.js
- [ ] Build System Settings page for Admin

### Ghazal
- [ ] Add data to SkillSynonyms table (20+ entries)
- [ ] Review documentation — ensure all features are covered
- [ ] Help Sedeen with JSON formatting

### Sedeen
- [ ] Build AI Summary generator (2-3 line summary per candidate)
- [ ] Build AI Interview Questions generator (5 custom questions)
- [ ] Build AI Job Description generator
- [ ] Handle Gemini API errors gracefully

### Farah
- [ ] Test AI analysis accuracy with different CVs
- [ ] Test score calculation with known expected results
- [ ] Test interview questions and summaries quality

### Rama
- [ ] Add interview questions display to Candidate Details page
- [ ] Build Comparison page layout (side-by-side columns)
- [ ] Build charts on Statistics page
- [ ] Add AI generator to Create Job page

---

## Sprint 4: Final Polish (Weeks 7-8)

### Bashar
- [ ] Deploy final version to AWS
- [ ] Security check (API keys hidden, file limits working)
- [ ] Prepare presentation with team
- [ ] Final report to supervisor
- [ ] Record backup demo video

### Omar + Mazen
- [ ] Fix remaining bugs
- [ ] Performance optimization

### Ghazal
- [ ] Complete full documentation
- [ ] Code review and cleanup

### Sedeen
- [ ] Improve AI prompt accuracy
- [ ] Ensure different CV formats are handled

### Farah
- [ ] Full end-to-end testing of all pages and features
- [ ] Prepare convincing demo data for presentation

### Rama
- [ ] Final UI polish (colors, spacing, consistency)
- [ ] Test on different screen sizes

---

## Progress Report Template (Every 2 weeks)

**Completed:**
-

**In Progress:**
-

**Blockers:**
-

**Overall Progress:** ___%
