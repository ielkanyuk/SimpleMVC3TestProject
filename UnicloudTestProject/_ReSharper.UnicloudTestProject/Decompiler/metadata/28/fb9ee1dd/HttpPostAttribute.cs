// Type: System.Web.Mvc.HttpPostAttribute
// Assembly: System.Web.Mvc, Version=3.0.0.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// Assembly location: C:\Program Files (x86)\Microsoft ASP.NET\ASP.NET MVC 3\Assemblies\System.Web.Mvc.dll

using System;
using System.Reflection;

namespace System.Web.Mvc
{
  /// <summary>
  /// Represents an attribute that is used to restrict an action method so that the method handles only HTTP POST requests.
  /// </summary>
  [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
  public sealed class HttpPostAttribute : ActionMethodSelectorAttribute
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="T:System.Web.Mvc.HttpPostAttribute"/> class.
    /// </summary>
    public HttpPostAttribute();
    /// <summary>
    /// Determines whether a request is a valid HTTP POST request.
    /// </summary>
    /// 
    /// <returns>
    /// true if the request is valid; otherwise, false.
    /// </returns>
    /// <param name="controllerContext">The context within which the controller operates. The context information includes the controller, HTTP content, request context, and route data.</param><param name="methodInfo">Encapsulates information about a method, such as its type, return type, and arguments.</param>
    public override bool IsValidForRequest(ControllerContext controllerContext, MethodInfo methodInfo);
  }
}
