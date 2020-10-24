using Piranha.AttributeBuilder;
using Piranha.Models;

namespace ClubSite.Models
{
    [PostType(Title = "Standard post")]
    public class StandardPost  : Post<StandardPost>
    {
    }
}