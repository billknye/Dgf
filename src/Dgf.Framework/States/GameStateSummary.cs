using System.Collections.Generic;

namespace Dgf.Framework.States;

/// <summary>
/// Contains a summary of the state that will be provided to the player
/// </summary>
public class GameStateSummary
{
    /// <summary>
    /// A short title string for the state
    /// </summary>
    public DisplayItem Title { get; set; }

    /// <summary>
    /// A long form description of the state
    /// </summary>
    public DisplayItem Description { get; set; }

    /// <summary>
    /// A set of attributes for the given state
    /// </summary>
    public IEnumerable<DisplayItem> Attributes { get; set; }

    /// <summary>
    /// Sound to play once when state is loaded, will be played each time the state is loaded
    /// </summary>
    public string SfxUri { get; set; }

    /// <summary>
    /// Song to be played, will be checked against current song and continue playing if matched
    /// </summary>
    public string MusicUri { get; set; }
}    
