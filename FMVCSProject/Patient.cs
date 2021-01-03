using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace FMVCSProject
{
	public class Patient
	{
		private EmergencyCareService Service;
		private int id;
		public Patient (EmergencyCareService service)
		{
			this.Service = service;
			Thread t1 = new Thread(new ThreadStart(process));
			t1.Start();
		}

		public void process()
		{
			int id = Checkin();
			if (id == -1)
			{
				Console.WriteLine("Oops... waiting time too long (patient refused)");
				return;
			}
			this.id = id;
			Console.WriteLine("Patient "+this.id+" has successfully checked in");

			FillPaper();
			//Paper are filled
			WaitForPaperToBeChecked();
			//paper are checked
			EnterRoom();
			Console.WriteLine("P" + this.id + ": waiting for physician");
			this.Service.Physicians.WaitOne();
			Console.WriteLine("P" + this.id + ": physician available");
			Examination();
			this.Service.Physicians.Release();
			Console.WriteLine("P" + this.id + ": Examination over");
			Checkout();
		}

		private void Checkout()
		{
			Console.WriteLine("P" + this.id + ": checking out");
			Thread.Sleep(1000);
			this.Service.PatientLeaves();
		}

		private void Examination()
		{
			//sleep
			Console.WriteLine("P" + this.id + ": Examining");
			Thread.Sleep(30000);
			this.Service.Rooms.Release();
		}

		private void EnterRoom()
		{
			Console.WriteLine("P" + this.id + ": waiting for room");
			this.Service.Rooms.WaitOne();
			Console.WriteLine("P" + this.id + ": room available");
			this.Service.Nurses.Release();
			//sleep
			Thread.Sleep(2000);
		}

		private void WaitForPaperToBeChecked()
		{
			Console.WriteLine("P" + this.id + ": waiting for nurse");
			this.Service.Nurses.WaitOne();
			Console.WriteLine("P" + this.id + ": nurse checking paper");
			Thread.Sleep(7000);
		}

		private void FillPaper()
		{
			//console log + sleep to mimic paper filling
			Console.WriteLine("P"+this.id + ": filling paper");
			Thread.Sleep(10000);
			Console.WriteLine("P"+this.id + ": paper filled");
		}

		private int Checkin()
		{
			this.Service.Receptionist.WaitOne();
			Console.WriteLine("Patient checking in...");
			int id = this.Service.CalculWaitingTime();
			Thread.Sleep(2000);
			// call service to process id and waiting time (if waiting time too high id = -1)
			this.Service.Receptionist.Release();
			return id;
		}
	}
}
