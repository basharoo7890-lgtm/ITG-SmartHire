namespace Employment.Services
{
    public class JobDescriptionService
    {
        private readonly GeminiService _gemini;

        public JobDescriptionService(GeminiService gemini)
        {
            _gemini = gemini;
        }

        public async Task<string?> GenerateJobDescriptionAsync(string jobTitle, string department, List<string> skills)
        {
            var skillsList = skills.Any()
                ? string.Join(", ", skills)
                : "general skills";

            var prompt = $"""
                Write a professional job description for the following position:

                Job Title: {jobTitle}
                Department: {department}
                Required Skills: {skillsList}

                The description should include:
                - A brief overview of the role (2-3 sentences)
                - Key responsibilities (5-6 bullet points)
                - What makes a great candidate (3-4 points)

                Keep it concise, professional, and engaging.
                Plain text only, no markdown symbols like * or #.
                """;

            return await _gemini.GenerateAsync(prompt);
        }
    }
}