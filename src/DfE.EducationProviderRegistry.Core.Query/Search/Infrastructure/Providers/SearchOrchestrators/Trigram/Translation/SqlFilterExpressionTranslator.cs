using System.Linq.Expressions;
using System.Text;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Providers.SearchOrchestrators.EntityMetadataResolver;
using DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Providers.SearchOrchestrators.Trigram.Translation.Strategies;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace DfE.EducationProviderRegistry.Core.Query.Search.Infrastructure.Providers.SearchOrchestrators.Trigram.Translation;

/// <summary>
/// Translates a LINQ predicate expression into a SQL filter fragment suitable
/// for trigram‑based search queries. Uses EF Core metadata to correctly map
/// CLR property names to database column names.
/// </summary>
/// <typeparam name="TProjection">
/// The entity or projection type the expression operates on.
/// </typeparam>
public sealed class SqlFilterExpressionTranslator<TProjection>
    : ExpressionVisitor, ISqlFilterExpressionTranslator<TProjection>
    where TProjection : class
{
    private readonly StringBuilder _builder = new();
    private EntityMetadata _metadata = null!;

    private readonly Dictionary<ExpressionType, IBinaryOperatorTranslationStrategy> _operatorStrategies =
        new()
        {
            { ExpressionType.Equal,   new BinaryOperatorTranslationStrategy("=") },
            { ExpressionType.AndAlso, new BinaryOperatorTranslationStrategy("AND") },
            { ExpressionType.OrElse,  new BinaryOperatorTranslationStrategy("OR") }
        };

    private readonly IConstantTranslationStrategy[] _constantStrategies =
    {
        new NullConstantTranslationStrategy(),
        new StringConstantTranslationStrategy(),
        new ConstantTranslationStrategy()
    };

    /// <summary>
    /// Translates the supplied predicate expression into a SQL filter fragment.
    /// </summary>
    /// <param name="expression">The predicate expression to translate.</param>
    /// <param name="metadata">
    /// The resolved EF Core metadata for <typeparamref name="TProjection"/>.
    /// </param>
    /// <returns>A SQL fragment representing the translated filter expression.</returns>
    public string Translate(
        Expression<Func<TProjection, bool>> expression,
        EntityMetadata metadata)
    {
        _metadata = metadata;
        Visit(expression.Body);
        return _builder.ToString();
    }

    /// <summary>
    /// Translates binary expressions such as <c>AND</c>, <c>OR</c>, and <c>=</c>.
    /// </summary>
    protected override Expression VisitBinary(BinaryExpression node)
    {
        _builder.Append('(');

        Visit(node.Left);

        if (!_operatorStrategies.TryGetValue(node.NodeType, out IBinaryOperatorTranslationStrategy? op))
        {
            throw new NotSupportedException($"Unsupported operator: {node.NodeType}");
        }

        op.Write(_builder);

        Visit(node.Right);

        _builder.Append(')');

        return node;
    }

    /// <summary>
    /// Translates member access expressions (e.g., <c>x.Property</c>) into SQL
    /// column references using EF Core metadata.
    /// </summary>
    protected override Expression VisitMember(MemberExpression node)
    {
        if (node.Expression?.NodeType == ExpressionType.Parameter)
        {
            string clrName = node.Member.Name;

            IProperty property =
                _metadata.EntityType.GetProperties()
                    .FirstOrDefault(p => p.Name == clrName)
                ?? throw new InvalidOperationException(
                    $"Property '{clrName}' not found on entity '{_metadata.EntityType.Name}'.");

            string columnName = property.GetColumnName();

            _builder.Append($"t.\"{columnName}\"");
            return node;
        }

        throw new NotSupportedException($"Unsupported member expression: {node}");
    }

    /// <summary>
    /// Translates constant expressions using the first matching constant
    /// translation strategy.
    /// </summary>
    protected override Expression VisitConstant(ConstantExpression node)
    {
        object? value = node.Value;

        foreach (IConstantTranslationStrategy strategy in _constantStrategies)
        {
            if (strategy.CanHandle(value!))
            {
                strategy.Write(value!, _builder);
                return node;
            }
        }

        throw new NotSupportedException($"Unsupported constant: {value}");
    }
}
