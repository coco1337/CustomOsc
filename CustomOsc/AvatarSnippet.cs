namespace CustomOsc
{
  internal sealed class AvatarSnippet
  {
    internal string id { get; set; }
    internal string name { get; set; } 
  }

  internal sealed class Parameter
  {
    internal string name { get; set; } 
    internal IO input { get; set; }
    internal IO output { get; set; }
  }

  internal sealed class IO
  {
    internal string address { get; set; } 
    internal string type { get; set; }  
  }
}
