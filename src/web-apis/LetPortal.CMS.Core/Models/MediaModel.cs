namespace LetPortal.CMS.Core.Models
{
    public class MediaModel
    {
        public MediaType MediaType { get; set; }

        public string Link { get; set; }

        public string Alt { get; set; }

        public string TargetLink { get; set; }
    }

    public enum MediaType
    {
        Image,
        Video
    }
}
