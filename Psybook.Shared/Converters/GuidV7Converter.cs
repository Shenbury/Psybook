using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Psybook.Shared.Converters
{
    public class GuidVersion7Converter(ConverterMappingHints? mappingHints = null)
        : ValueConverter<Guid, Guid>(guid => EnsureVersion7(guid), guid => guid, mappingHints)
    {
        private static Guid EnsureVersion7(Guid guid)
        {
            return guid == Guid.Empty ? GenerateGuidV7() : guid;
        }

        private static Guid GenerateGuidV7()
        {
            return Guid.CreateVersion7();
        }
    }
}
