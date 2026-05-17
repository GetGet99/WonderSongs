global using QuickMarkupAttribute = QuickMarkup.SourceGen.QuickMarkupAttribute;
namespace QuickMarkup.SourceGen;

[AttributeUsage(AttributeTargets.Class)]
#pragma warning disable CS9113 // Parameter is unread.
class QuickMarkupAttribute(string markup) : Attribute;
#pragma warning restore CS9113 // Parameter is unread.
