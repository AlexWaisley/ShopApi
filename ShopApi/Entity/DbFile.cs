namespace ShopApi.Entity;

public class DbFile
{
    public int Id { get; set; }    
    public int Size { get; set; }    
    public string Sha256 { get; set; }    
    public string Name { get; set; }    
    public string ContentType { get; set; }    
    public DateTime CreatedAt { get; set; }    
}