namespace JapaneseTrainer.Api.DTOs.Packages
{
    /// <summary>
    /// Result of importing content to a lesson
    /// </summary>
    public class LessonImportResultDto
    {
        public int TotalProcessed { get; set; }
        public int ItemsAdded { get; set; }
        public int KanjisAdded { get; set; }
        public int GrammarsAdded { get; set; }
        public int ItemsNotFound { get; set; }
        public int KanjisNotFound { get; set; }
        public int GrammarsNotFound { get; set; }
        public int ItemsAlreadyExists { get; set; }
        public int KanjisAlreadyExists { get; set; }
        public int GrammarsAlreadyExists { get; set; }
        public List<string> NotFoundItems { get; set; } = new();
        public List<string> NotFoundKanjis { get; set; } = new();
        public List<string> NotFoundGrammars { get; set; } = new();
        public string Message { get; set; } = string.Empty;
    }
}


