﻿namespace lib;

public interface ICommentOptions
{
    string Query { get; }
    string Filter { get; }
    int Limit { get; }
}
