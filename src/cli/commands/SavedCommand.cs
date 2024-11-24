using lib;
using Serilog;

namespace cli.commands;

public class SavedCommand(IOptions options, ISavedService service) : AbstractCommand(options)
{

    #region Overriden Methods

    protected override Task<CommentModel[]> GetAllComments()
    {
        return service.GetAllItemsAsync();
    }

    protected override Task<(CommentPreview[], int)> GetFilteredComments(IOptions options)
    {
        Log.Information("Fetching locally saved comments with {@Options}", options);
        return service.GetFilteredItemsAsync(options);
    }

    #endregion

}
