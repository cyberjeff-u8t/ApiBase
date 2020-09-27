using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;

namespace Api.Configuration
{
    public class VersionHeader : IOperationProcessor
    {
        public bool Process(OperationProcessorContext context)
        {
            context.OperationDescription.Operation.Parameters.Add(
                new NSwag.OpenApiParameter
                {
                    Name = "api-version",
                    Kind = NSwag.OpenApiParameterKind.Header,
                    Type = NJsonSchema.JsonObjectType.String,
                    IsRequired = false,
                    Description = "api-version",
                    Default = "1.0"
                });

            return true;
        }

    }

}
