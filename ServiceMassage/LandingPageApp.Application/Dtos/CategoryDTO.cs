namespace LandingPageApp.Application.Dtos;
public class CategoryDTO
{
    public long Id {set; get;}
    public string Name {set; get;} = string.Empty;
    public string Description {set; get;} = string.Empty;
    public bool Status {set; get; }

    public DateTime? CreatedAt {set; get; }
    public DateTime? UpdatedAt {set; get; }
}

public class CreateCategoryDto
{
    public string Name {set; get;} = string.Empty;
    public string Description {set; get;} = string.Empty;
    public bool Status {set; get; }
}
public class UpdateCategoryDto
{
    public string Name {set; get;} = string.Empty;
    public string Description {set; get;} = string.Empty;
    public bool Status {set; get; }
}
public class CategoryFilterDto
{
    public string? Name {set; get;}
    public bool? Status {set; get; }
}
