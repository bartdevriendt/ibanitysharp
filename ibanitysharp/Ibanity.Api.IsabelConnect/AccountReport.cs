namespace Ibanity.Api.IsabelConnect;

public class Attributes
{
    public List<string> accountReferences { get; set; }
    public string fileFormat { get; set; }
    public string fileName { get; set; }
    public int fileSize { get; set; }
    public string financialInstitutionName { get; set; }
    public DateTime receivedAt { get; set; }
}

public class Meta
{
    public string contentType { get; set; }
    public Paging paging { get; set; }
}

public class Self
{
    public string href { get; set; }
    public Meta meta { get; set; }
}

public class Links
{
    public Self self { get; set; }
}

public class AccountReport
{
    public Attributes attributes { get; set; }
    public string id { get; set; }
    public Links links { get; set; }
    public string type { get; set; }
}

public class Paging
{
    public int offset { get; set; }
    public int total { get; set; }
}

public class AccountReportList
{
    public List<AccountReport> data { get; set; }
    public Meta meta { get; set; }
}