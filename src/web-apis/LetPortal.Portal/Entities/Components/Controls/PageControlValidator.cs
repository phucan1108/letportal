namespace LetPortal.Portal.Entities.SectionParts.Controls
{
    public class PageControlValidator
    {
        public ValidatorType ValidatorType { get; set; }

        public bool IsActive { get; set; }

        public string ValidatorOption { get; set; }

        public string ValidatorMessage { get; set; }
    }

    public enum ValidatorType
    {
        Required,
        MinLength,
        MaxLength,
        NumberRange,
        Number,
        MinDate,
        MaxDate,
        DateTime,
        Equal,
        EqualTo,
        Regex,
        Email,
        Json,
        FileExtensions,
        FileMaximumSize,
        FileMaximumFiles
    }
}
