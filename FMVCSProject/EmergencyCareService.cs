using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace FMVCSProject
{
	public class EmergencyCareService
	{
		private readonly object nameLock = new object();
		private string name;
		public string Name { 
			get {
                lock (nameLock)
                {
					return name;
                }
			}
			private set { name = value; } }
        private static int MAX_RESOURCE_NUMBER = 100;

		private ResourceProvider provider;
		public Semaphore Receptionist { get; private set; }

		public Semaphore Nurses { get; private set; }
		private readonly object nurseLock = new object();
		private int nurseNb;
		public Semaphore Rooms { get; private set; }
		private readonly object roomLock = new object();
		private int roomNb;
		public Semaphore Physicians { get; private set; }
		private readonly object physicianLock = new object();
		private int physicianNb;

		private readonly object patientLock = new object();
		private int acceptedPatient = 0;

		public Mutex ResourceRequest { get; private set; }

		public EmergencyCareService(String name,ResourceProvider provider, int availableNurses, int availableRooms, int availablePhysicians)
		{
			this.Name = name;
			this.provider = provider;
			this.Receptionist = new Semaphore(1, 1);
			this.Nurses = new Semaphore(availableNurses, availableNurses);
			this.nurseNb = availableNurses;
			this.Rooms = new Semaphore(availableRooms, MAX_RESOURCE_NUMBER);
			this.roomNb = availableRooms;
			this.Physicians = new Semaphore(availablePhysicians, MAX_RESOURCE_NUMBER);
			this.physicianNb = availablePhysicians;
			//Only one request at a time
			this.ResourceRequest = new Mutex();

			Thread t1 = new Thread(new ThreadStart(Run));
			t1.Start();
		}

		public void Run()
        {
            while (true) { }
        }
		public string AcceptPatient()
		{
			lock (patientLock)
			{
				this.acceptedPatient++;
				Console.WriteLine(this.Name + " ----- " + this.acceptedPatient + " patients -----");
				string id = this.acceptedPatient.ToString() + this.Name;
				return id;
			}
		}

		public void PatientLeaves()
		{
			lock (patientLock)
			{
				this.acceptedPatient--;
				Console.WriteLine(this.Name + " ----- " + this.acceptedPatient+" patients -----");
			}
		}

		public string CalculWaitingTime()
		{
			int waitingTime = new Random().Next(7);
			if (waitingTime >= 6)
				return "-1";
			else
				return AcceptPatient();
		}

		public void CallProvider()
		{
			this.ResourceRequest.WaitOne();
			Console.WriteLine("---> Calling provider...");
			//call provider which will check if it can give
			if (!this.provider.ShareResources())
			{
				Console.WriteLine(this.Name + " : Oops... Provider has no resource available for now");
				return;
			}
			lock (roomLock)
			{
				this.roomNb++;
				lock (physicianLock)
				{
					this.physicianNb++;
					Console.WriteLine(this.Name + " : \nTotal number of physician: " + this.physicianNb+ "\nTotal number of rooms: " + this.roomNb);
				}
			}
			this.Rooms.Release();
			this.Physicians.Release();
			this.ResourceRequest.ReleaseMutex();
		}

		public void ShareRoom()
		{
			lock (patientLock)
			{
				if (acceptedPatient > 0)
				{
					Console.WriteLine(this.Name + " : Oops... who cannot share a room when you have patient in the service");
					return;
				}
			}
			lock(roomLock)
			{
				if(roomNb <= 0)
				{
					Console.WriteLine(this.Name + " : Oops... you have no available room for now, maybe you should call the provider");
					return;
				}

				Rooms.WaitOne();
				this.provider.ReceiveRoom();
				this.roomNb--;
				Console.WriteLine(this.Name + " : \n Total number of rooms: " + this.roomNb);
			}
		}

		public void SharePhysician()
		{
			lock (patientLock)
			{
				if (acceptedPatient > 0)
				{
					Console.WriteLine(this.Name + " : Oops... who cannot share a physician when you have patient in the service");
					return;
				}
			}
			lock (physicianLock)
			{
				if (physicianNb <= 0)
				{
					Console.WriteLine(this.Name + " : Oops... you have no available physician for now, maybe you should call the provider");
					return;
				}
				Physicians.WaitOne();
				this.provider.ReceivePhysician();
				this.physicianNb--;
				Console.WriteLine(this.Name + " : \nTotal number of physician: " + this.physicianNb);
			}
		}
	}
}
