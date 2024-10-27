using System.Net.Sockets;
using System.Text;
//run first server later run client 2 times and they begin to play.


namespace TicTacToeClient
{
    class Client
    {
        static void Main(string[] args)
        {
            TcpClient client = new TcpClient("127.0.0.1", 27001);
            NetworkStream stream = client.GetStream();
            Console.WriteLine("Connected to Server.");

            while (true)
            {
                byte[] data = new byte[9];
                stream.Read(data, 0, data.Length);
                string boardState = Encoding.ASCII.GetString(data);
                PrintBoard(boardState);

                Console.WriteLine("Your Move. \nChoose a position (0-8): arr index mentiqi ile isleyir 0 dan basla");
                int position = Convert.ToInt32(Console.ReadLine());
                stream.WriteByte((byte)(position + '0'));

                var resultData = new byte[512];
                int bytesRead = stream.Read(resultData, 0, resultData.Length);
                string resultMessage = Encoding.ASCII.GetString(resultData, 0, bytesRead);
                if (resultMessage.Contains("wins") || resultMessage.Contains("draw"))
                {
                    Console.WriteLine(resultMessage);
                    break;
                }
            }

            client.Close();
        }

        static void PrintBoard(string boardState)
        {
            Console.Clear();
            Console.WriteLine("Current board:");
            Console.WriteLine($" {boardState[0]} | {boardState[1]} | {boardState[2]} ");
            Console.WriteLine("---|---|---");
            Console.WriteLine($" {boardState[3]} | {boardState[4]} | {boardState[5]} ");
            Console.WriteLine("---|---|---");
            Console.WriteLine($" {boardState[6]} | {boardState[7]} | {boardState[8]} ");
        }
    }
}
