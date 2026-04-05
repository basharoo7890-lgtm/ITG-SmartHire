# ITG SmartHire

An AI-powered recruitment screening system that automates CV analysis, candidate scoring, and interview preparation — reducing HR manual review time by over 80%.

## Features

- AI-powered CV parsing and skill extraction (Google Gemini API)
- Smart matching score (0-100) based on skills, experience, salary, and education
- Auto-filtering (salary budget, minimum experience)
- AI-generated professional candidate summaries
- AI-generated custom interview questions per candidate
- AI-powered job description generator
- Side-by-side candidate comparison
- Statistics dashboard with charts
- HR dashboard with filters and ranking
- Role-based access (Admin, HR, Applicant)

## Tech Stack

- **Backend + Frontend:** ASP.NET Core 8 MVC
- **UI:** MVC Views + Bootstrap 5
- **Database:** SQL Server + Entity Framework Core 8
- **Authentication:** ASP.NET Identity (Cookie Auth)
- **AI Engine:** Google Gemini API (Free Tier — 1,500 requests/day)
- **PDF Processing:** PdfPig
- **Charts:** Chart.js
- **Hosting:** AWS (EC2 + RDS + S3)
- **Containerization:** Docker
- **Source Control:** GitHub

## Getting Started

### Prerequisites

- .NET 8 SDK
- Visual Studio 2022 or 2026
- SQL Server (LocalDB or Developer Edition)
- Git

### Run Locally

```bash
git clone https://github.com/basharOO99/ITG-SmartHire.git
cd ITG-SmartHire
cp .env.example .env
# Edit .env and add your Gemini API key
dotnet restore
dotnet ef database update --project SmartHire
dotnet run --project SmartHire
```

App runs at: http://localhost:5000

### Run with Docker

```bash
cd docker
docker-compose up -d
```

## Project Structure

```
SmartHire/
├── Controllers/         # Request handling
├── Models/              # Data entities
├── ViewModels/          # View-specific models
├── Services/            # Business logic + AI
├── Views/               # Razor views (UI)
├── Data/                # DbContext + Migrations
└── wwwroot/             # Static files (CSS, JS, Bootstrap)
```

## Team

| Name | Role |
|------|------|
| Bashar Mukaddam | PMO + DevOps |
| Omar | Backend Developer (Core) |
| Mazen | Full-Stack Developer (Dashboard + Admin) |
| Ghazal | Backend + Documentation |
| Sedeen | AI Developer |
| Farah Atef | Database + QA |
| Rama | UI/UX Design |

## License

Internal internship project — ITG Solutions © 2026
