using System;
using System.Threading;

namespace FMVCSProject
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("--- START ---\n");
			ResourceProvider provider = new ResourceProvider(2, 1);
			EmergencyCareService service = new EmergencyCareService(provider, 2, 4, 3);
			/*new Patient(service);
			Thread.Sleep(200);
			new Patient(service);
			Thread.Sleep(200);
			new Patient(service);
			Thread.Sleep(200);
			new Patient(service);
			Thread.Sleep(200);
			new Patient(service);*/

			string input = "";

			while (!input.Equals("exit"))
			{
				input = Console.ReadLine().ToLower().Trim();
				switch (input)
				{
					/*case "": //add un service type "new service / s1 ajouter dans un dict ???
						Console.Write("Recipient login : ");
						String recipient = Console.ReadLine();
						Console.Write("Message content : ");
						String content = Console.ReadLine();
						Net.SendMsg(client.GetStream(), input + "\n" + recipient + "\n" + content);
						break;*/
					case "new patient": // new patient /s1 (s1 nom du service ou arrive le patient)
						new Patient(service);
						break;
					case "call provider":
						service.CallProvider();
						break;
					case "share room":
						service.ShareRoom();
						break;
					case "share physician":
						service.SharePhysician();
						break;
					case "list commands":
						Console.WriteLine("New Patient | Call Provider | Share Room | Share Physicians | Exit");
						break;
					case "exit":
						break;
					default:
						Console.WriteLine("Sorry this command does not exist...\nYou can type 'list commands' to see all the existing commands");
						break;
				}
			}
		}
	}
}
