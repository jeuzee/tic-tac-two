namespace MenuSystem;

public class MenuItem
{
    private string _title = default!;
    private string _shortCut = default!;
    
    public Func<string>? MenuItemAction { get; set; }

    public string Title
    {
        get => _title;
        set
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException("Title cannot be empty.");
            }
            _title = value;
        }
    }
    
    public string Shortcut
    {
        get => _shortCut;
        set
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException("Shortcut cannot be empty.");
            }
            _shortCut = value;
        }
    }

    public override string ToString()
    {
        return _shortCut + ") " + _title;
    }
}