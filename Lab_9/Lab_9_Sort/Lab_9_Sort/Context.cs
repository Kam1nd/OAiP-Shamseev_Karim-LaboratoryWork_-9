namespace Lab_9_Sort
{
    public class Context
    {
        public IStrategy ContextStrategy;
        public static int[] array;

        public Context(IStrategy Strategy)
        {
            this.ContextStrategy = Strategy;
        }

        public Context() { }

        public void ExecuteAlgorithm(bool flag = true)
        {
            ContextStrategy.Algorithm(array, flag);
        }
    }
}