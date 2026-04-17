namespace BMS.Application.Common.Audit;

public static class EntityChangeDetector
{
    public static List<(string Field, string? OldValue, string? NewValue)>
        GetPersonChanges(
            string oldFirstName,
            string oldLastName,
            string? oldEmail,
            string? oldMobile,
            string newFirstName,
            string newLastName,
            string? newEmail,
            string? newMobile)
    {
        var changes = new List<(string, string?, string?)>();

        if (oldFirstName != newFirstName)
            changes.Add(("FirstName", oldFirstName, newFirstName));

        if (oldLastName != newLastName)
            changes.Add(("LastName", oldLastName, newLastName));

        if (oldEmail != newEmail)
            changes.Add(("Email", oldEmail, newEmail));

        if (oldMobile != newMobile)
            changes.Add(("Mobile", oldMobile, newMobile));

        return changes;
    }
}
