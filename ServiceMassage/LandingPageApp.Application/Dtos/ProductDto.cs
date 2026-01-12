namespace LandingPageApp.Application.Dtos;

#region Response DTOs

public class ProductDto
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int? Stock { get; set; }
    public long CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public string? UrlImage { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

#endregion

#region Request DTOs

public class CreateProductDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public long CategoryId { get; set; }
    public string? UrlImage { get; set; }
}

public class UpdateProductDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public long CategoryId { get; set; }
    public string? UrlImage { get; set; }
}

public class UpdateProductStockDto
{
    public int Quantity { get; set; }
}

#endregion
