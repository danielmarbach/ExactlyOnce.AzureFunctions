namespace ExactlyOnce.AzureFunctions
{
    public abstract class SideEffect
    {
    }

    public class SideEffectWrapper
    {
        public string Type { get; set; }

        public string Content { get; set; }
    }
}