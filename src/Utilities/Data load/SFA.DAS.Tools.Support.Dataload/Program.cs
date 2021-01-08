using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.Tools.Support.Dataload
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var dataService = new DataService();
            var option = string.Empty;
            Action showOptions = () => 
            {
                Console.Clear();
                Console.WriteLine("Select option:");
                Console.WriteLine("1 - Load data");
                Console.WriteLine("Q - Quit");
            };
            
            while(option.ToLower() != "q")
            {
                showOptions();
                option = Console.ReadLine();

                if(option.ToLower() == "1")
                {
                    await dataService.Load();
                }
            }

        }
    }

    public class DataService
    {
        public async Task Load()
        {
            var accountIds = await CreateAccounts();
            var levy = CreateLevyDeclarations(accountIds);
            var commitments = CreateCommitments(accountIds);
            var payments = CreatePayments(accountIds);

            await Task.WhenAll(levy,commitments,payments);
        }

        private async Task<List<int>> CreateAccounts()
        {
            return await Task.FromResult(new List<int>());
        }

        private async Task CreateLevyDeclarations(List<int> accountIds)
        {

        }

        private async Task CreateCommitments(List<int> accountIds)
        {
            
        }

        private async Task CreatePayments(List<int> accountIds)
        {
            
        }
    }
}
