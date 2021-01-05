using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace FMVCSProject
{
	public class ResourceProvider
	{
		private static int MAX_RESOURCE_NUMBER = 100;

		public Semaphore Rooms { get; private set; }
		private readonly object roomLock = new object();
		private int roomNb;
		public Semaphore Physicians { get; private set; }
		private readonly object physicianLock = new object();
		private int physicianNb;

		public ResourceProvider(int roomNb, int physicianNb)
		{
			this.Rooms = new Semaphore(roomNb, MAX_RESOURCE_NUMBER);
			this.roomNb = roomNb;
			this.Physicians = new Semaphore(physicianNb, MAX_RESOURCE_NUMBER);
			this.physicianNb = physicianNb;
		}

        public bool SharePhysician()
        {
            Console.WriteLine("PROVIDER: receiving physician request");

            lock (physicianLock)
            {
                if (physicianNb <= 0)
                    return false;
                this.Physicians.WaitOne();
                Console.WriteLine("PROVIDER: giving 1 physician");
                this.physicianNb--;
                Console.WriteLine("PROVIDER:\nTotal number of physician: " + this.physicianNb);
            }
            Thread.Sleep(1000);
            return true;
        }

        public bool ShareRoom()
        {
            Console.WriteLine("PROVIDER: receiving room request");

            lock (roomLock)
            {
                if (roomNb <= 0)
                    return false;
                this.Rooms.WaitOne();
                Console.WriteLine("PROVIDER: giving 1 room and 1 physician");
                this.roomNb--;
                Console.WriteLine("PROVIDER:\nTotal number of rooms: " + this.roomNb);

            }
            Thread.Sleep(1000);
            return true;
        }


        public void ReceiveRoom()
		{
			this.Rooms.Release();
			lock (roomLock)
			{
				this.roomNb++;
				Console.WriteLine("PROVIDER: Total number of rooms: " + this.roomNb);
			}
			Thread.Sleep(1000);
		}

		public void ReceivePhysician()
		{
			this.Physicians.Release();
			lock (physicianLock)
			{
				this.physicianNb++;
				Console.WriteLine("PROVIDER: Total number of physician: " + this.physicianNb);
			}
			Thread.Sleep(1000);
		}
	}
}
