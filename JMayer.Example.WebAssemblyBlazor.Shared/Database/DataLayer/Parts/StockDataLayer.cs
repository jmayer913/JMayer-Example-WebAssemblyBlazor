﻿using JMayer.Data.Database.DataLayer.MemoryStorage;
using JMayer.Example.WebAssemblyBlazor.Shared.Data.Assets;
using JMayer.Example.WebAssemblyBlazor.Shared.Data.Parts;
using JMayer.Example.WebAssemblyBlazor.Shared.Database.DataLayer.Assets;
using System.ComponentModel.DataAnnotations;

namespace JMayer.Example.WebAssemblyBlazor.Shared.Database.DataLayer.Parts;

/// <summary>
/// The class manages CRUD interactions with the database for part stock.
/// </summary>
public class StockDataLayer : UserEditableDataLayer<Stock>, IStockDataLayer
{
    /// <summary>
    /// The data layer for interacting with parts.
    /// </summary>
    /// <remarks>
    /// This data layer needs to register the delete event on the
    /// part data layer. It's also used during server-side validation.
    /// </remarks>
    private readonly IPartDataLayer _partDataLayer;

    /// <summary>
    /// The data layer for interacting with storage locations.
    /// </summary>
    /// <remarks>
    /// This data layer needs to register the delete event on the
    /// storage location data layer. It's also used during server-side 
    /// validation.
    /// </remarks>
    private readonly IStorageLocationDataLayer _storageLocationDataLayer;

    /// <summary>
    /// The dependency injection constructor.
    /// </summary>
    /// <param name="partDataLayer"></param>
    public StockDataLayer(IPartDataLayer partDataLayer, IStorageLocationDataLayer storageLocationDataLayer)
    {
        _partDataLayer = partDataLayer;
        _partDataLayer.Deleted += PartDataLayer_Deleted;

        _storageLocationDataLayer = storageLocationDataLayer;
        _storageLocationDataLayer.Deleted += StorageLocationDataLayer_Deleted;
        _storageLocationDataLayer.Updated += StorageLocationDataLayer_Updated;
    }

    /// <summary>
    /// The method deletes any stocks associated with the deleted parts.
    /// </summary>
    /// <param name="sender">The part data layer.</param>
    /// <param name="e">The arguments which contain the deleted parts.</param>
    private async void PartDataLayer_Deleted(object? sender, JMayer.Data.Database.DataLayer.DeletedEventArgs e)
    {
        foreach (Part part in e.DataObjects.Cast<Part>())
        {
            List<Stock> stocks = await GetAllAsync(obj => obj.OwnerInteger64ID == part.Integer64ID);

            if (stocks.Count > 0)
            {
                await DeleteAsync(stocks);
            }
        }
    }

    /// <summary>
    /// The method deletes any stocks associated with the deleted storage locations.
    /// </summary>
    /// <param name="sender">The storage location data layer.</param>
    /// <param name="e">The arguments which contain the deleted storage locations.</param>
    private async void StorageLocationDataLayer_Deleted(object? sender, JMayer.Data.Database.DataLayer.DeletedEventArgs e)
    {
        foreach (StorageLocation storageLocation in e.DataObjects.Cast<StorageLocation>())
        {
            List<Stock> stocks = await GetAllAsync(obj => obj.StorageLocationID == storageLocation.Integer64ID);

            if (stocks.Count > 0)
            {
                await DeleteAsync(stocks);
            }
        }
    }

    /// <summary>
    /// The method updates the storage location name on the stock if the storage location name was updated on the storage location object.
    /// </summary>
    /// <param name="sender">The storage location data layer.</param>
    /// <param name="e">The arguments which contain the updated storage locations.</param>
    private async void StorageLocationDataLayer_Updated(object? sender, JMayer.Data.Database.DataLayer.UpdatedEventArgs e)
    {
        List<Stock> stockUpdateList = [];

        foreach (StorageLocation storageLocation in e.DataObjects.Cast<StorageLocation>())
        {
            List<Stock> stocks = await GetAllAsync(obj => obj.StorageLocationID == storageLocation.Integer64ID);

            if (stocks.Count > 0 && stocks[0].StorageLocationName != storageLocation.FriendlyName)
            {
                foreach (Stock stock in stocks)
                {
                    stock.StorageLocationName = storageLocation.FriendlyName;
                }

                stockUpdateList.AddRange(stocks);
            }
        }

        if (stockUpdateList.Count > 0)
        {
            await UpdateAsync(stockUpdateList);
        }
    }

    /// <inheritdoc/>
    /// <remarks>
    /// Overriding and not calling the base because the parent class forces the Name property 
    /// to be unique but the property is not used. Also, added additional server-side validation
    /// unique to the stock.
    /// </remarks>
    public override async Task<List<ValidationResult>> ValidateAsync(Stock dataObject, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dataObject);
        List<ValidationResult> validationResults = dataObject.Validate();

        if (await _partDataLayer.ExistAsync(obj => obj.Integer64ID == dataObject.OwnerInteger64ID, cancellationToken) == false)
        {
            validationResults.Add(new ValidationResult($"The {dataObject.OwnerInteger64ID} part was not found in the data store.", [nameof(Stock.OwnerInteger64ID)]));
        }

        if (await _storageLocationDataLayer.ExistAsync(obj => obj.Integer64ID == dataObject.StorageLocationID, cancellationToken) == false)
        {
            validationResults.Add(new ValidationResult($"The {dataObject.StorageLocationID} storage location was not found in the data store.", [nameof(Stock.StorageLocationID)]));
        }

        if (await ExistAsync(obj => obj.Integer64ID != dataObject.Integer64ID && obj.OwnerInteger64ID == dataObject.OwnerInteger64ID && obj.StorageLocationID == dataObject.StorageLocationID, cancellationToken) == true) 
        {
            validationResults.Add(new ValidationResult("The stock location already exists in the data store for the part.", [nameof(Stock.StorageLocationID)]));
        }

        return await Task.FromResult(validationResults);
    }
}
