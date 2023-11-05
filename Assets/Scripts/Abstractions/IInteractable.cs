public interface IInteractable
{
    public string InteractableName { get; }

    public void Highlight();

    public void RemoveHighlight();

    public void Interact();
}
