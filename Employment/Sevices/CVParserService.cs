using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;
using DocumentFormat.OpenXml.Packaging;
using System.Text;

namespace Employment.Services
{
    public class CVParserService
    {
        public string ExtractText(string filePath)
        {
            var extension = Path.GetExtension(filePath).ToLower();

            if (extension == ".pdf")
                return ExtractFromPdf(filePath);
            else if (extension == ".docx")
                return ExtractFromDocx(filePath);
            else
                return string.Empty;
        }

        private string ExtractFromPdf(string filePath)
        {
            var sb = new StringBuilder();

            using var pdf = PdfDocument.Open(filePath);
            foreach (var page in pdf.GetPages())
            {
                sb.AppendLine(page.Text);
            }

            return sb.ToString().Trim();
        }

        private string ExtractFromDocx(string filePath)
        {
            var sb = new StringBuilder();

            using var doc = WordprocessingDocument.Open(filePath, false);
            var body = doc.MainDocumentPart?.Document?.Body;
            if (body == null) return string.Empty;

            foreach (var para in body.Descendants<DocumentFormat.OpenXml.Wordprocessing.Paragraph>())
            {
                sb.AppendLine(para.InnerText);
            }

            return sb.ToString().Trim();
        }

        public bool IsSupportedFormat(string fileName)
        {
            var ext = Path.GetExtension(fileName).ToLower();
            return ext == ".pdf" || ext == ".docx";
        }
    }
}