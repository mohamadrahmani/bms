namespace BMS.Application.Common.Interfaces;

public interface IPmFilePolicy
{
    string? Validate(string fileName, string contentType, long fileSize);
}
