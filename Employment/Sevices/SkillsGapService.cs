using Employment.Data;
using Employment.Models;
using Microsoft.EntityFrameworkCore;

namespace Employment.Services
{
    public class SkillsGapService
    {
        private readonly GeminiService _gemini;
        private readonly ApplicationDbContext _context;

        public SkillsGapService(GeminiService gemini, ApplicationDbContext context)
        {
            _gemini = gemini;
            _context = context;
        }

        public async Task<string?> GenerateGapReportAsync(int applicationId)
        {
            var application = await _context.Applications
                .Include(a => a.Job)
                .FirstOrDefaultAsync(a => a.ApplicationId == applicationId);

            if (application == null || application.Job == null)
                return null;

            var analysis = await _context.AIAnalyses
                .FirstOrDefaultAsync(a => a.ApplicationId == applicationId);

            if (analysis == null)
                return null;

            var jobSkills = await _context.JobSkills
                .Where(s => s.JobId == application.Job.JobId)
                .Select(s => s.SkillName)
                .ToListAsync();

            var prompt = $"""
                You are an HR assistant. Write a short, professional, and encouraging message to a job applicant.

                Job Title: {application.Job.Title}
                Required Skills: {string.Join(", ", jobSkills)}
                Candidate's Skills: {analysis.ParsedSkills}
                Candidate's Strengths: {analysis.Strengths}
                Candidate's Weaknesses: {analysis.Weaknesses}
                Matching Score: {analysis.MatchingScore}%

                Write a message that:
                - Thanks them for applying
                - Mentions what they are strong in
                - Explains what skills they are missing
                - Encourages them to improve
                - Keep it under 150 words
                - Be professional and kind
                """;

            var gapReport = await _gemini.GenerateAsync(prompt);

            if (gapReport != null)
            {
                analysis.GapReport = gapReport;
                await _context.SaveChangesAsync();
            }

            return gapReport;
        }
    }
}