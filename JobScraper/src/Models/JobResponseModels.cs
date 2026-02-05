namespace JobScraper;

public class JobResponseModels
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class AdvertStatus
    {
        public bool Deadline { get; set; }
        public bool ThreeDaysLeft { get; set; }
        public bool TwoDaysLeft { get; set; }
        public bool LastChance { get; set; }
        public bool HourLeft { get; set; }
    }

    public class Datum
    {
        public string PublishedIntranetDate { get; set; }
        public bool IsIntranet { get; set; }
        public bool IsInternet { get; set; }
        public string Id { get; set; }
        public string TenantId { get; set; }
        public string CompanyName { get; set; }
        public string Heading { get; set; }
        public string HeadingNotOverruled { get; set; }
        public string JobType { get; set; }
        public string JobCategory { get; set; }
        public string Language { get; set; }
        public string Presentation { get; set; }
        public string PublishedDate { get; set; }
        public DateTime ApplicationDeadline { get; set; }
        public string ApplyWithinDate { get; set; }
        public int HoursLeft { get; set; }
        public string PictureUrl { get; set; }
        public string Workplace3 { get; set; }
        public string Workplace2 { get; set; }
        public string Workplace { get; set; }
        public string WorkPlaceFacet { get; set; }
        public List<string> WorkPlaceFacet2 { get; set; }
        public List<string> WorkPlaceFacet3 { get; set; }
        public bool MultipleWorkplaces { get; set; }
        public string OpenAdvertUrl { get; set; }
        public int PictureHeight { get; set; }
        public int PictureWidth { get; set; }
        public AdvertStatus AdvertStatus { get; set; }
        public string UILanguage { get; set; }
        public string Culture { get; set; }
        public string ApplyUrl { get; set; }
    }

    public class Facets
    {
        public List<WorkPlaceFacet3> workPlaceFacet3 { get; set; }
        public List<SpecialFieldEn> specialFieldEn { get; set; }
        public List<JobTypeEn> jobTypeEn { get; set; }
        public List<JobCategory> jobCategory { get; set; }
    }

    public class JobCategory
    {
        public int count { get; set; }
        public string value { get; set; }
    }

    public class JobTypeEn
    {
        public int count { get; set; }
        public string value { get; set; }
    }

    public class Root
    {
        public Facets Facets { get; set; }
        public string LastFilterQuery { get; set; }
        public List<Datum> Data { get; set; }
        public int Total { get; set; }
        public object Aggregates { get; set; }
    }

    public class SpecialFieldEn
    {
        public int count { get; set; }
        public string value { get; set; }
    }

    public class WorkPlaceFacet3
    {
        public int count { get; set; }
        public string value { get; set; }
    }


}