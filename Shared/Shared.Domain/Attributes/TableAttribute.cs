namespace Shared.Domain.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class TableAttribute : Attribute
{
    public string TableName { get; }
    
    public TableAttribute(string tableName)
    {
        TableName = tableName;
    }
}

[AttributeUsage(AttributeTargets.Property)]
public class ColumnAttribute : Attribute
{
    public string ColumnName { get; }
    public int MaxLength { get; }
    public int Precision { get; }
    public int Scale { get; }
    public bool HasMaxLength { get; }
    public bool HasPrecision { get; }
    public bool HasScale { get; }
    
    public ColumnAttribute(string columnName)
    {
        ColumnName = columnName;
        MaxLength = 0;
        Precision = 0;
        Scale = 0;
        HasMaxLength = false;
        HasPrecision = false;
        HasScale = false;
    }
    
    public ColumnAttribute(string columnName, int maxLength)
    {
        ColumnName = columnName;
        MaxLength = maxLength;
        Precision = 0;
        Scale = 0;
        HasMaxLength = true;
        HasPrecision = false;
        HasScale = false;
    }
    
    public ColumnAttribute(string columnName, int maxLength, int precision, int scale)
    {
        ColumnName = columnName;
        MaxLength = maxLength;
        Precision = precision;
        Scale = scale;
        HasMaxLength = true;
        HasPrecision = true;
        HasScale = true;
    }
} 