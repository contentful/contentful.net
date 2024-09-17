using Contentful.Core.Errors;
using Newtonsoft.Json;

namespace Contentful.Core.Models.Management;

public class ScheduledAction
{
    //Here for serialization purposes
    public ScheduledAction() { }

    public ScheduledAction(string action, string entityId, string environment, ScheduledFor scheduledFor)
    {
        Entity = new BaseEntity
        {
            SystemProperties = new SystemProperties
            {
                Type = "Link",
                Id = entityId,
                LinkType = SystemLinkTypes.Entry
            }
        };
        ScheduledFor = scheduledFor;
        Action = action;
        Environment = new ContentfulEnvironment()
        {
            SystemProperties = new SystemProperties
            {
                Id = environment,
                Type = "Link",
                LinkType = SystemLinkTypes.Environment
            }
        };
    }

    [JsonProperty("sys")]
    public ScheduledActionSystemProperties SystemProperties { get; set; }

    public BaseEntity Entity { get; set; }

    public ContentfulEnvironment Environment { get; set; }

    public string Action { get; set; }

    public ScheduledFor ScheduledFor { get; set; }

    public ErrorDetails Error { get; set; }
}