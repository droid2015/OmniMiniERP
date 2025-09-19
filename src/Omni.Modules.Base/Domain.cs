namespace Omni.Modules.Base;

public class User
{
    public Guid Id { get; set; }
    public string Email { get; set; } = "";
    public string PasswordHash { get; set; } = "";
}
public class ConfigParam
{
    public Guid Id { get; set; }
    public string Key { get; set; } = "";
    public string Value { get; set; } = "";
}
