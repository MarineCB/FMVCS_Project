using System;
using System.Collections.Generic;
using System.Threading;

namespace FMVCSProject
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("--- START ---\n");
			ResourceProvider provider = new ResourceProvider(2, 1);
			List<EmergencyCareService> services = new List<EmergencyCareService>();
			EmergencyCareService service = new EmergencyCareService("s1",provider, 2, 4, 3);
			services.Add(service);

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
			Console.WriteLine("COMMANDS: \nNew Service / [Name of the Service] / [Nb of Nurse] / [Nb of Room] / [Nb of Physician] \nNew Patient / [Name of the service] \nRequest Room / [Name of the service] \nRequest Physician / [Name of the service] \nShare Room / [Name of the service] \nShare Physicians / [Name of the service] \nExit");
			EmergencyCareService s;
			string name;
			int nbNurse, nbRoom, nbPhysician;
			while (!input.Equals("exit"))
			{
				input = Console.ReadLine().ToLower().Trim();
				string[] inputs = input.Split('/');
                for(int i =0; i<inputs.Length; i++)
                {
					inputs[i] = inputs[i].Trim();
                }



				switch (inputs[0])
				{
					case "new service":
						if (inputs.Length >= 5)
						{
							name = inputs[1];
							nbNurse = Int32.Parse(inputs[2]);
							nbRoom = Int32.Parse(inputs[3]);
							nbPhysician = Int32.Parse(inputs[4]);
						}
						else
						{
							Console.WriteLine("Invalid number of argument");
							break;
						}
						services.Add(new EmergencyCareService(name, provider, nbNurse, nbRoom, nbPhysician));
                        Console.WriteLine("Service \""+name+"\" created");
						break;
					case "new patient": // new patient /s1 (s1 nom du service ou arrive le patient)
						if(inputs.Length >=2)
							name = inputs[1];
                        else
                        {
							Console.WriteLine("Invalid number of argument");
							break;
                        }
						s = services.Find(s => s.Name.ToLower().Equals(name));
						if (s != null)
							new Patient(s);
						else
                            Console.WriteLine("This service does not exist");
						break;
					case "request room":
						if (inputs.Length >= 2)
							name = inputs[1];
						else
						{
							Console.WriteLine("Invalid number of argument");
							break;
						}
						 s = services.Find(s => s.Name.ToLower().Equals(name));
						if (s != null)
							s.RequestRoom();
						break;
					case "request physician":
						if (inputs.Length >= 2)
							name = inputs[1];
						else
						{
							Console.WriteLine("Invalid number of argument");
							break;
						}
						s = services.Find(s => s.Name.ToLower().Equals(name));
						if (s != null)
							s.RequestPhysician();
						break;
					case "share room":
						if (inputs.Length >= 2)
							name = inputs[1];
						else
						{
							Console.WriteLine("Invalid number of argument");
							break;
						}
						s = services.Find(s => s.Name.ToLower().Equals(name));
						if (s != null)
							service.ShareRoom();
						break;
					case "share physician":
						if (inputs.Length >= 2)
							name = inputs[1];
						else
						{
							Console.WriteLine("Invalid number of argument");
							break;
						}
						s = services.Find(s => s.Name.ToLower().Equals(name));
						if (s != null)
							service.SharePhysician();
						break;
					case "list commands":
						Console.WriteLine("New Service / [Name of the Service] / [Nb of Nurse] / [Nb of Room] / [Nb of Physician] \nNew Patient / [Name of the service] \nRequest Room / [Name of the service] \nRequest Physician / [Name of the service] \nShare Room / [Name of the service] \nShare Physicians / [Name of the service] \nExit");
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
