using System;

/// <summary>
/// Default methods for reactions to implement. Follows the Standard .Net event pattern
/// for the method signature.
/// 
/// Written by: Nicolas Herrera (nherrera@cra.com), 2019
/// </summary>
public interface IReaction 
{
    void StartReaction(object o, EventArgs e);

    void StopReaction(object o, EventArgs e);
}
