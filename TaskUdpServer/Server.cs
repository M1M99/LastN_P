using System.Net;
using System.Net.Sockets;
using System.Text;
//run first server later run client 2 times and they begin to play.

namespace TicTacToeServer
{
    class Server
    {
        static TcpListener? listener;
        static char[] boardMove = new char[9];
        static char currentPlayer = 'X';

        static void Main(string[] args)
        {
            InitializeBoard();
            listener = new TcpListener(IPAddress.Any, 27001);
            listener.Start();
            Console.WriteLine("Server Started...\nWaiting Players...");

            while (true)
            {
                TcpClient player1 = listener.AcceptTcpClient();
                Console.WriteLine($"Player 1 is Connected.");
                TcpClient player2 = listener.AcceptTcpClient();
                Console.WriteLine("Player 2 is Connected.");

                var gameThread = new Thread(() => PlayGame(player1, player2));
                gameThread.Start();
            }
        }

        static void PlayGame(TcpClient player1, TcpClient player2)
        {
            NetworkStream stream1 = player1.GetStream();
            NetworkStream stream2 = player2.GetStream();
            int turns = 0;

            while (true)
            {
                SendBoardState(stream1);
                SendBoardState(stream2);

                NetworkStream currentStream = (currentPlayer == 'X') ? stream1 : stream2;
                int position;

                while (true)
                {
                    position = ReceiveMove(currentStream);
                    if (position >= 0 && position < 9 && boardMove[position] == ' ')
                    {
                        boardMove[position] = currentPlayer;
                        turns++;
                        break;
                    }
                    SendMessage(currentStream, "Invalid move.");
                }

                if (CheckWin())
                {
                    SendBoardState(stream1);
                    SendBoardState(stream2);
                    PrintBoard();
                    SendMessage(stream1, $"Player {currentPlayer} wins!");
                    SendMessage(stream2, $"Player {currentPlayer} wins!");
                    break;
                }

                if (turns == 9)
                {
                    SendBoardState(stream1);
                    SendBoardState(stream2);
                    PrintBoard();
                    SendMessage(stream1, "It's a draw!");
                    SendMessage(stream2, "It's a draw!");
                    break;
                }

                currentPlayer = (currentPlayer == 'X') ? 'O' : 'X';
            }

            player1.Close();
            player2.Close();
        }

        static void InitializeBoard()
        {
            for (int i = 0; i < boardMove.Length; i++)
            {
                boardMove[i] = ' ';
            }
        }

        static void PrintBoard()
        {
            Console.Clear();
            Console.WriteLine("Current boardMove:");
            Console.WriteLine($" {boardMove[0]} | {boardMove[1]} | {boardMove[2]} ");
            Console.WriteLine("---|---|---");
            Console.WriteLine($" {boardMove[3]} | {boardMove[4]} | {boardMove[5]} ");
            Console.WriteLine("---|---|---");
            Console.WriteLine($" {boardMove[6]} | {boardMove[7]} | {boardMove[8]} ");
        }

        static void SendBoardState(NetworkStream stream)
        {
            var boardState = new string(boardMove);
            var data = Encoding.ASCII.GetBytes(boardState);
            stream.Write(data, 0, data.Length);
        }

        static int ReceiveMove(NetworkStream stream)
        {
            byte[] data = new byte[1];
            stream.Read(data, 0, 1);
            int move;
            if (int.TryParse(((char)data[0]).ToString(), out move))
            {
                return move;
            }
            else
            {
                throw new InvalidOperationException("Invalid move.");
            }
        }

        static void SendMessage(NetworkStream stream, string message)
        {
            var data = Encoding.ASCII.GetBytes(message);
            stream.Write(data, 0, data.Length);
        }

        static bool CheckWin()
        {
            int[,] winCombination = new int[,]
            {
                { 0, 4, 8 }, { 3, 4, 5 }, { 6, 7, 8 },
                { 0, 3, 6 }, { 2, 4, 6 }, { 2, 5, 8 },
                { 0, 1, 2 }, { 1, 4, 7 }
            };

            for (int i = 0; i < winCombination.GetLength(0); i++)
            {
                if (boardMove[winCombination[i, 0]] == currentPlayer &&
                    boardMove[winCombination[i, 1]] == currentPlayer &&
                    boardMove[winCombination[i, 2]] == currentPlayer)
                {
                    return true;
                }
            }
            return false;
        }
    }
}

