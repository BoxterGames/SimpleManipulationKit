namespace SimpleManipulationKit
{
    public static class InteractionContext
    {
        public static SelectionModel Selection { get; } = new();
        public static DragModel Drag { get; } = new();
        public static MarqueeModel Marquee { get; } = new();
    }
}
