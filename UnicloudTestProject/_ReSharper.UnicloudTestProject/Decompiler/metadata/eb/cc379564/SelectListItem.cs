// Type: System.Web.Mvc.SelectListItem
// Assembly: System.Web.Mvc, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// Assembly location: C:\Program Files (x86)\Microsoft ASP.NET\ASP.NET MVC 3\Assemblies\System.Web.Mvc.dll

namespace System.Web.Mvc
{
  /// <summary>
  /// Represents the selected item in an instance of the <see cref="T:System.Web.Mvc.SelectList"/> class.
  /// </summary>
  public class SelectListItem
  {
    /// <summary>
    /// Gets or sets a value that indicates whether this <see cref="T:System.Web.Mvc.SelectListItem"/> is selected.
    /// </summary>
    /// 
    /// <returns>
    /// true if the item is selected; otherwise, false.
    /// </returns>
    public bool Selected { get; set; }
    /// <summary>
    /// Gets or sets the text of the selected item.
    /// </summary>
    /// 
    /// <returns>
    /// The text.
    /// </returns>
    public string Text { get; set; }
    /// <summary>
    /// Gets or sets the value of the selected item.
    /// </summary>
    /// 
    /// <returns>
    /// The value.
    /// </returns>
    public string Value { get; set; }
  }
}
