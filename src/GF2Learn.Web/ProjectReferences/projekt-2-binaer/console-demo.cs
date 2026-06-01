string binaer = "10111011";
int sum = 0;
for (int i = 0; i < binaer.Length; i++)
{
    if (binaer[i] == '1')
        sum += (int)Math.Pow(2, binaer.Length - 1 - i);
}
Console.WriteLine($"{binaer} (binær) = {sum} (decimal)");
