﻿using Reddit;
using Reddit.Inputs.Users;
using Reddit.Things;

namespace lib;

public class SavedService : ISavedService
{

    #region Constructors

    public SavedService(ApplicationConfig config)
    {
        _me = Environment.GetEnvironmentVariable("MY_REDDIT_USERNAME");
        _redditClient = new RedditClient(config.AppId, config.RefreshToken, accessToken: config.AccessToken);
    }

    #endregion

    #region Variables

    private readonly RedditClient _redditClient;
    private readonly string _me;

    #endregion

    #region Public Methods

    public async Task<Comment[]> GetFilteredItemsAsync(IOptions savedOptions)
    {
        // TODO:SNB - Abstract posts and comments under a single interface
        Comment[] comments = savedOptions.Comment ? await GetSavedComments() : await GetSavedPosts();
        var allComments = comments.OrderByDescending(c => c.CreatedUTC).ToArray();
        IEnumerable<Comment> filteredComments = FilterComments(allComments, savedOptions);
        return filteredComments.ToArray();
    }

    #endregion

    #region Helper Methods

    private static IEnumerable<Comment> FilterComments(IEnumerable<Comment> comments, IOptions options)
    {
        IEnumerable<Comment> filteredComments = comments;
        if (!string.IsNullOrEmpty(options.Query))
        {
            filteredComments = filteredComments.Where(c => c.Body.Contains(options.Query, StringComparison.OrdinalIgnoreCase));
        }

        if (string.IsNullOrEmpty(options.Filter))
        {
            return filteredComments;
        }

        string author = options.GetFilterValue("author");
        if (!string.IsNullOrEmpty(author))
        {
            filteredComments = filteredComments.Where(c => c.Author.Contains(author, StringComparison.OrdinalIgnoreCase));
        }

        string sub = options.GetFilterValue("sub");
        if (!string.IsNullOrEmpty(sub))
        {
            filteredComments = filteredComments.Where(c => c.Subreddit.Contains(sub, StringComparison.OrdinalIgnoreCase));
        }

        return filteredComments;
    }

    private async Task<Comment[]> GetSavedComments()
    {
        List<Comment> comments = new List<Comment>();

        var after = "";
        int totalTopComments;
        do
        {
            var topComments = await Task.Run(() =>
            {
                CommentContainer history = _redditClient.Models.Users.CommentHistory(_me, "saved",
                    new UsersHistoryInput("comments", after: after, sort: "top", context: 10, limit: 100));
                return history.Data.Children.Select(c => c.Data).ToArray();
            });
            if (!topComments.Any())
            {
                totalTopComments = 0;
                continue;
            }

            comments.AddRange(topComments);
            after = topComments.Last().Name;
            totalTopComments = topComments.Length;
        } while (totalTopComments > 0);

        return comments.ToArray();
    }

    private static async Task<Comment[]> GetSavedPosts()
    {
        return await Task.FromResult(Array.Empty<Comment>());
    }

    #endregion

}
