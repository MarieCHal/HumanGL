
namespace HumanGL.Mathematics
{
    public class Exemple2
    {
        public Exemple exemple;
        public async Task DoSomething()
        {
            // Implementation goes here
            await HelperMethod();
            await exemple.DoSomething();
        }

        private async Task HelperMethod()
        {
            // Helper method implementation
        }
    }
}