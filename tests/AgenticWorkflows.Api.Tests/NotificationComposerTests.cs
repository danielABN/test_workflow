using AgenticWorkflows.Api.Models;
using AgenticWorkflows.Api.Services;

namespace AgenticWorkflows.Api.Tests;

public sealed class NotificationComposerTests
{
    [Fact]
    public void Notifications_truncate_descriptions_longer_than_ninety_characters()
    {
        var item = CreateItem(description: new string('a', 91), dueDate: new DateOnly(2026, 6, 15));

        var created = NotificationComposer.BuildCreatedNotification(item);
        var dueSoon = NotificationComposer.BuildDueSoonNotification(item);

        var expectedDescriptionLine = $"Description: {new string('a', 87)}...";
        Assert.Contains(expectedDescriptionLine, created);
        Assert.Contains(expectedDescriptionLine, dueSoon);
    }

    [Fact]
    public void Notifications_omit_due_date_line_when_due_date_is_null()
    {
        var item = CreateItem(description: "Short description", dueDate: null);

        var created = NotificationComposer.BuildCreatedNotification(item);
        var dueSoon = NotificationComposer.BuildDueSoonNotification(item);

        Assert.DoesNotContain("Due date:", created);
        Assert.DoesNotContain("Due date:", dueSoon);
    }

    [Fact]
    public void Created_and_due_soon_notifications_include_expected_next_steps()
    {
        var item = CreateItem(description: "Short description", dueDate: new DateOnly(2026, 6, 15));

        var created = NotificationComposer.BuildCreatedNotification(item);
        var dueSoon = NotificationComposer.BuildDueSoonNotification(item);

        Assert.Contains("Next step: Review the backlog and assign an owner.", created);
        Assert.Contains("Next step: Confirm the item still belongs in this sprint.", dueSoon);
    }

    private static WorkItem CreateItem(string description, DateOnly? dueDate) =>
        new(Guid.NewGuid(), "Title", description, 3, WorkItemStatus.Todo, dueDate);
}
