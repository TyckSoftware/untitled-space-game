using System.Collections.Generic;
using System.Linq;
using Godot;

/// <summary>
/// An interaction resource that can be set as an interaction
/// in the <see cref="InteractableArea"/> node.
/// </summary>
public partial class InteractableAreaInteraction : Resource, IInteractableAreaInteraction
{
    /// <inheritdoc />
    [Export]
    public string[] Groups { get; set; } = new string[0];

    /// <inheritdoc />
    public InteractableArea InteractableArea { get; private set; } = default!;

    /// <inheritdoc />
    public IList<Node> ValidEntitiesInArea { get; private set; } = new List<Node>();

    /// <inheritdoc />
    public IList<Node> InvalidEntitiesInArea { get; private set; } = new List<Node>();

    /// <inheritdoc />
    public bool EntityIsPresent => ValidEntitiesInArea.Count + InvalidEntitiesInArea.Count > 0;

    /// <inheritdoc />
    public virtual void Initialize(InteractableArea interactableArea)
    {
        InteractableArea = interactableArea;
    }

    #region Process

    /// <inheritdoc />
    public void _Process(double delta)
    {
        if (EntityIsPresent)
        {
            Process(delta);
        }
    }

    /// <inheritdoc />
    /// <remarks>
    /// Empty by default, override to implement.
    /// </remarks>
    protected virtual void Process(double delta)
    {
        return;
    }

    #endregion

    #region Physics process

    /// <inheritdoc />
    public void _PhysicsProcess(double delta)
    {
        if (EntityIsPresent)
        {
            PhysicsProcess(delta);
        }
    }

    /// <inheritdoc />
    /// <remarks>
    /// Empty by default, override to implement.
    /// </remarks>
    protected virtual void PhysicsProcess(double delta)
    {
        return;
    }

    #endregion

    #region Entity detection

    /// <inheritdoc />
    public virtual void OnEntityEntered(Node node)
    {
        if (NodeIsValid(node))
        {
            ValidEntitiesInArea.Add(node);
        }
        else
        {
            InvalidEntitiesInArea.Add(node);
        }

        OnEntitiesChanged(node);
    }

    /// <inheritdoc />
    public virtual void OnEntityExited(Node node)
    {
        if (NodeIsValid(node))
        {
            ValidEntitiesInArea.Remove(node);
        }
        else
        {
            InvalidEntitiesInArea.Remove(node);
        }

        OnEntitiesChanged(node);
    }

    /// <summary>
    /// Called when the amount of entities in the
    /// interactable area changes.
    /// </summary>
    /// <remarks>
    /// Empty by default, override to implement.
    /// </remarks>
    /// <param name="node">
    /// The node that entered or exited the interactable area.
    /// </param>
    protected virtual void OnEntitiesChanged(Node node)
    {
        return;
    }

    /// <summary>
    /// Checks if the given node can trigger the interaction
    /// and is therefore considered valid.
    /// </summary>
    /// <param name="node">
    /// The node to check whether it can trigger the interaction.
    /// </param>
    /// <returns>
    /// Whether the given node can trigger the interaction
    /// and is therefore considered valid.
    /// </returns>
    protected bool NodeIsValid(Node node)
        => Groups.Length == 0 || Groups.Any(group => node.IsInGroup(group));

    #endregion
}
