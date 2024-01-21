using JMayer.Data.Database.DataLayer.MemoryStorage;
using JMayer.Example.WebAssemblyBlazor.Shared.Data.Assets;

namespace JMayer.Example.WebAssemblyBlazor.Shared.Database.DataLayer.Assets;

/// <summary>
/// The class manages CRUD interactions with the database for an asset.
/// </summary>
public class AssetDataLayer : UserEditableMemoryDataLayer<Asset>, IAssetDataLayer
{
    /// <inheritdoc/>
    /// <remarks>
    /// This is overriden so the parent path can be set before creation.
    /// </remarks>
    public override async Task<Asset> CreateAsync(Asset dataObject, CancellationToken cancellationToken = default)
    {
        //Set the parent path is a parent exists.
        if (dataObject.ParentID != null)
        {
            Asset? parent = await GetSingleAsync(obj => obj.Integer64ID == dataObject.ParentID, cancellationToken);

            if (parent != null)
            {
                dataObject.ParentPath = parent.MeAsParentPath;
            }
        }

        return await base.CreateAsync(dataObject, cancellationToken);
    }

    /// <inheritdoc/>
    /// <remarks>
    /// This is overriden so the children of the tree are deleted also; because this is memory storage, cascade 
    /// deletes must be built.
    /// </remarks>
    public override async Task DeleteAsync(Asset dataObject, CancellationToken cancellationToken = default)
    {
        List<Asset> children = await GetChildrenAsync(dataObject, cancellationToken);

        await base.DeleteAsync(dataObject, cancellationToken);

        if (children.Count > 0)
        {
            await base.DeleteAsync(children, cancellationToken);
        }
    }

    /// <summary>
    /// The method recursively returns the tree under the parent.
    /// </summary>
    /// <param name="parent">The parent at this node in the asset tree.</param>
    /// <param name="cancellationToken">A token used for task cancellations.</param>
    /// <returns>A list of children assets or an empty list is none exists.</returns>
    private async Task<List<Asset>> GetChildrenAsync(Asset parent, CancellationToken cancellationToken)
    {
        List<Asset> returnList = [];
        List<Asset> children = await GetAllAsync(obj => obj.ParentID == parent.Integer64ID, cancellationToken);

        if (children.Count > 0)
        {
            returnList = [.. children];

            foreach (Asset child in children)
            {
                if (child.ParentID != null)
                {
                    List<Asset> temp = await GetChildrenAsync(child, cancellationToken);

                    if (temp.Count > 0)
                    {
                        returnList.AddRange(temp);
                    }
                }
            }
        }

        return returnList;
    }

    /// <inheritdoc/>
    /// <remarks>
    /// This is overriden so the parent path can be updated if a change occurs; this will recursively update the
    /// children of the tree so its an expensive operation but the parent shouldn't change often.
    /// </remarks>
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
    /// <param name="cancellationToken">A token used for task cancellations.</param>
    /// <returns>A Task object for the async.</returns>
    private async Task UpdateParentPathAsync(string? parentPath, Asset parentAsset, CancellationToken cancellationToken)
    {
        List<Asset> children = await GetAllAsync(obj => obj.ParentID == parentAsset.Integer64ID, cancellationToken);

        if (children.Count > 0)
        {
            foreach (Asset childAsset in children)
            {
                childAsset.ParentPath = parentPath;
                await UpdateParentPathAsync(childAsset.MeAsParentPath, childAsset, cancellationToken);
            }

            _ = await UpdateAsync(children, cancellationToken);
        }
    }
}
