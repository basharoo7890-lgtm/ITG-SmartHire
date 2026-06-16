namespace Employment.Services
{
    public class LanguageDetectorService
    {
        public string DetectLanguage(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return "unknown";

            int arabicCount = 0;
            int englishCount = 0;

            foreach (char c in text)
            {
                if (c >= 0x0600 && c <= 0x06FF)
                    arabicCount++;
                else if ((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z'))
                    englishCount++;
            }

            int total = arabicCount + englishCount;
            if (total == 0) return "unknown";

            double arabicRatio = (double)arabicCount / total;

            if (arabicRatio >= 0.6)
                return "arabic";
            else if (arabicRatio <= 0.3)
                return "english";
            else
                return "mixed";
        }
    }
}