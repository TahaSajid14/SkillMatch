using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using SkillMatch.API.Interfaces;
using UglyToad.PdfPig;
using UglyToad.PdfPig.DocumentLayoutAnalysis.TextExtractor;

namespace SkillMatch.API.Services;

public sealed class DocumentResumeParserService : IResumeParserService
{
    public string ExtractText(string absoluteFilePath)
    {
        return Path.GetExtension(absoluteFilePath).ToLowerInvariant() switch
        {
            ".pdf" => ExtractPdfText(absoluteFilePath),
            ".docx" => ExtractWordText(absoluteFilePath),
            _ => throw new NotSupportedException("This resume file type is not supported.")
        };
    }

    private static string ExtractPdfText(string path)
    {
        using var document = PdfDocument.Open(path);
        return string.Join(
            Environment.NewLine,
            document.GetPages().Select(page => ContentOrderTextExtractor.GetText(page)))
            .Trim();
    }

    private static string ExtractWordText(string path)
    {
        using var document = WordprocessingDocument.Open(path, false);
        var body = document.MainDocumentPart?.Document?.Body
            ?? throw new InvalidDataException("The Word document does not contain a readable body.");

        return string.Join(
            Environment.NewLine,
            body.Descendants<Paragraph>()
                .Select(paragraph => paragraph.InnerText.Trim())
                .Where(text => text.Length > 0))
            .Trim();
    }
}
