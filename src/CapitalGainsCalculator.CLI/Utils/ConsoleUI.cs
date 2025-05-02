namespace CapitalGainsCalculator.CLI.Utils;

public static class ConsoleUI
{
    public static void Welcome()
    {
        Console.WriteLine("==========================================================================================");
        Console.WriteLine("🌈 Bem vindo(a) a melhor calculadora de imposto sobre ganhos de capital do universo! 🌈");
        Console.WriteLine("💖 Faça trades com amor e pague seus impostos com carinho! 💖");
        Console.WriteLine("==========================================================================================");
        Console.WriteLine("\n");
        Console.WriteLine("💰 Insira as operações no formato JSON e pressione ENTER para calcular os impostos.");
        Console.WriteLine("❌ Pressione CTRL + C para sair...");
        Console.WriteLine("==========================================================================================");
        Console.WriteLine("\n");
    }
}