using JMayer.Data.Database.DataLayer.MemoryStorage;
using JMayer.Example.WebAssemblyBlazor.Shared.Data.Assets;

namespace JMayer.Example.WebAssemblyBlazor.Shared.Database.DataLayer.Assets;

/// <summary>
/// The class manages CRUD interactions with the database for an asset.
/// </summary>
public class AssetDataLayer : UserEditableMemoryDataLayer<Asset>, IAssetDataLayer
{
    /// <inheritdoc/>
    public override async Task<Asset> UpdateAsync(Asset dataObject, CancellationToken cancellationToken = default)
    {
        Asset? originalDataObject = await GetSingleAsync(obj => obj.Integer64ID == dataObject.Integer64ID, cancellationToken);

        //Update the parent path if the parent has changed.
        if (originalDataObject != null && originalDataObject.ParentID != dataObject.ParentID)
        {
            if (dataObject.ParentID == null)
            {
                dataObject.ParentPath = null;
            }
            else
            {
                Asset? parentAsset = await GetSingleAsync(obj => obj.Integer64ID == dataObject.ParentID, cancellationToken);
                dataObject.ParentPath = parentAsset?.MeAsParentPath;
            }
        }

        dataObject = await base.UpdateAsync(dataObject, cancellationToken);

        //All child under the asset must update their parent's path if the name has changed or the parent has changed.
        if (originalDataObject != null && (originalDataObject.Name != dataObject.Name || originalDataObject.ParentID != dataObject.ParentID))
        {
            await UpdateParentPathAsync(dataObject.MeAsParentPath, dataObject, cancellationToken);
        }

        return dataObject;
    }

    /// <summary>
    /// The method recursively updates all the parent paths for the children.
    /// </summary>
    /// <param name="parentPath">The parent path at this node in the asset tree.</param>
    /// <param name="parentAsset">The parent asset at this node in the asset tree.</param>
    /// <returns>A Task object for the async.</returns>
    private async Task UpdateParentPathAsync(string? parentPath, Asset parentAsset, CancellationToken cancellationToken)
    {
        List<Asset> childAssets = await GetAllAsync(obj => obj.ParentID == parentAsset.ParentID);

        if (childAssets.Count > 0)
        {
            foreach (Asset childAsset in childAssets)
            {
                childAsset.ParentPath = parentPath;
                await UpdateParentPathAsync(childAsset.MeAsParentPath, childAsset, cancellationToken);
            }

            _ = await UpdateAsync(childAssets, cancellationToken);
        }
    }
}
