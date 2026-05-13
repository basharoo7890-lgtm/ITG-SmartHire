using System.Text.RegularExpressions;

namespace Employment.Services
{
    public class CVCompletenessService
    {
        public List<string> GetMissingItems(string cvText)
        {
            var missing = new List<string>();

            if (string.IsNullOrWhiteSpace(cvText))
            {
                missing.Add("CV is empty");
                return missing;
            }

            // Check minimum length
            if (cvText.Length < 300)
                missing.Add("CV is too short (minimum 300 characters)");

            // Check phone number
            var phoneRegex = new Regex(@"(\+?\d[\d\s\-]{7,}\d)");
            if (!phoneRegex.IsMatch(cvText))
                missing.Add("Phone number not found");

            // Check email
            var emailRegex = new Regex(@"[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}");
            if (!emailRegex.IsMatch(cvText))
                missing.Add("Email address not found");

            // Check education keywords
            var educationKeywords = new[]
            {
                "bachelor", "master", "phd", "degree", "university", "college",
                "بكالوريوس", "ماجستير", "دكتوراه", "جامعة"
            };

            bool hasEducation = educationKeywords.Any(k =>
                cvText.Contains(k, StringComparison.OrdinalIgnoreCase));

            if (!hasEducation)
                missing.Add("Education information not found");

            // Check experience keywords
            var experienceKeywords = new[]
            {
                "experience", "worked", "work", "job", "position", "company",
                "خبرة", "عمل", "شركة", "وظيفة"
            };

            bool hasExperience = experienceKeywords.Any(k =>
                cvText.Contains(k, StringComparison.OrdinalIgnoreCase));

            if (!hasExperience)
                missing.Add("Work experience not found");

            return missing;
        }

        public bool IsComplete(string cvText)
        {
            return !GetMissingItems(cvText).Any();
        }
    }
}