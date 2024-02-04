using JMayer.Data.Data.Query;
using MudBlazor;

namespace JMayer.Example.WebAssemblyBlazor.Client.Extensions;

/// <summary>
/// The static class contains extension methods for the GridState class.
/// </summary>
public static class GridStateExtension
{
    /// <summary>
    /// The constant for the date time is after operator.
    /// </summary>
    private const string DateTimeIsAfterOperator = "is after";

    /// <summary>
    /// The constant for the date time is before operator.
    /// </summary>
    private const string DateTimeIsBeforeOperator = "is before";

    /// <summary>
    /// The constant for the date time is not operator.
    /// </summary>
    private const string DateTimeIsNotOperator = "is not";

    /// <summary>
    /// The constant for the date time is on or after operator.
    /// </summary>
    private const string DateTimeIsOnOrAfterOperator = "is on or after";

    /// <summary>
    /// The constant for the date time is on or before operator.
    /// </summary>
    private const string DateTimeIsOnOrBeforeOperator = "is on or before";

    /// <summary>
    /// The constant for the boolean is operator.
    /// </summary>
    private const string IsOperator = "is";

    /// <summary>
    /// The method converts the grid state into a query definition which is what the server understands.
    /// </summary>
    /// <typeparam name="T">Can be any class.</typeparam>
    /// <param name="gridState">The grid state to convert.</param>
    /// <returns>A query definition.</returns>
    public static QueryDefinition ToQueryDefinition<T>(this GridState<T> gridState)
    {
        QueryDefinition queryDefinition = new()
        {
            Skip = gridState.Page,
            Take = gridState.PageSize,
        };

        foreach (var sortDefinition in gridState.SortDefinitions)
        {
            queryDefinition.SortDefinitions.Add(new SortDefinition()
            {
                Descending = sortDefinition.Descending,
                SortOn = sortDefinition.SortBy,
            });
        }

        foreach (var filterDefinition in gridState.FilterDefinitions)
        {
            string value = string.Empty;

            if (filterDefinition.Value != null && !string.IsNullOrWhiteSpace(filterDefinition.Value.ToString()))
            {
                if (filterDefinition.Value is DateTime dateTime)
                {
                    value = $"{dateTime:yyyy-MM-ddTHH:mm:ss}";
                }
                else
                {
                    value = filterDefinition.Value.ToString() ?? string.Empty;
                }
            }

            //Do not include an empty value (table will be empty) unless on
            //certain operators which do not care about the value.
            if (value != string.Empty || filterDefinition.Operator is FilterDefinition.IsEmptyOperator or FilterDefinition.IsNotEmptyOperator)
            {
                queryDefinition.FilterDefinitions.Add(new FilterDefinition()
                {
                    FilterOn = filterDefinition.Column?.PropertyName ?? string.Empty,
                    Operator = TranslateOperator(filterDefinition.Operator ?? string.Empty),
                    Value = value,
                });
            }
        }

        return queryDefinition;
    }

    /// <summary>
    /// The method translates the mudblazor operator into a generic operator the server understands.
    /// </summary>
    /// <param name="mudblazorOperator">The mudblazor operator to be translated.</param>
    /// <returns>The original or translated operator.</returns>
    /// <remarks>
    /// Not all operators require translation.
    /// </remarks>
    private static string TranslateOperator(string mudblazorOperator)
    {
        return mudblazorOperator switch
        {
            DateTimeIsAfterOperator => FilterDefinition.GreaterThanOperator,
            DateTimeIsBeforeOperator => FilterDefinition.LessThanOperator,
            DateTimeIsNotOperator => FilterDefinition.NotEqualsOperator,
            DateTimeIsOnOrAfterOperator => FilterDefinition.GreaterThanOrEqualsOperator,
            DateTimeIsOnOrBeforeOperator => FilterDefinition.LessThanOrEqualsOperator,
            IsOperator => FilterDefinition.EqualsOperator,
            _ => mudblazorOperator //Return original operator if there's no translation.
        };
    }
}
