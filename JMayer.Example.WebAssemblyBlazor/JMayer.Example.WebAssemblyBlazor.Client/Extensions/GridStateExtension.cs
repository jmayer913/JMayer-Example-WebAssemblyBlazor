using JMayer.Data.Data.Query;
using MudBlazor;

namespace JMayer.Example.WebAssemblyBlazor.Client.Extensions;

/// <summary>
/// The static class contains extension methods for the GridState class.
/// </summary>
public static class GridStateExtension
{
    /// <summary>
    /// The method converts the grid state into a query definition which is what the server understands.
    /// </summary>
    /// <typeparam name="T">Can be any class.</typeparam>
    /// <param name="gridState">The grid state to convert.</param>
    /// <returns>A query definition.</returns>
    public static QueryDefinition ToQueryDefinition<T>(this GridState<T> gridState)
    {
#warning Mudblazor has multiple filter operators; I need to support all.

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
                value = filterDefinition.Value.ToString() ?? string.Empty;
            }

            //Do not include an empty value else an empty table will be displayed.
            if (value != string.Empty)
            {
                queryDefinition.FilterDefinitions.Add(new FilterDefinition()
                {
                    FilterOn = filterDefinition.Column?.PropertyName ?? string.Empty,
                    Operator = filterDefinition.Operator ?? FilterDefinition.ContainsOperator,
                    Value = value,
                });
            }
        }

        return queryDefinition;
    }
}
