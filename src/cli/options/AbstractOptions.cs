﻿using CommandLine;

namespace cli.options;

public abstract class AbstractOptions
{

    #region Constants

    private const int DefaultLimit = 10;

    #endregion
    
    #region Properties

    [Option('f', "filter", HelpText = "Filters by sub, author, date (e.g., -f author=foomanchu&sub=news.")]
    public string Filter { get; set; }
    
    [Option('q', "query", HelpText = "Search for a specific word.")]
    public string Query { get; set; }
    
    [Option('u', "user", HelpText = "Specify a user (if blank your personal account will be used).")]
    public string User { get; set; }

    #endregion

    #region Properties

    private Dictionary<string, string> FilterMap => CreateFilterMap();
    public int Limit => GetLimitFromFilter();

    #endregion
    
    #region Public Methods

    public string GetFilterValue(string key)
    {
        if (!FilterMap.Any())
        {
            return string.Empty;
        }
        
        return FilterMap.TryGetValue(key, out string value) ? value : string.Empty;
    }

    #endregion

    #region Helper Methods

    private Dictionary<string,string> CreateFilterMap()
    {
        return Filter.Split('&')
            .Select(part => part.Split('='))
            .Where(parts => parts.Length == 2)
            .ToDictionary(parts => parts[0], parts => parts[1]);
    }

    private int GetLimitFromFilter()
    {
        string limitValue = GetFilterValue("limit");
        return int.TryParse(limitValue, out var value) ? value : DefaultLimit;
    }

    #endregion

}
