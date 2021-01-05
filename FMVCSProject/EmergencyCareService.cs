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
        public string Name
        {
            get
            {
                lock (nameLock)
                {
                    return name;
                }
            }
            private set { name = value; }
        }
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

        public Mutex b1ResourceRequest { get; private set; }
        public Mutex b2ResourceOffer { get; private set; }

        public EmergencyCareService(String name, ResourceProvider provider, int availableNurses, int availableRooms, int availablePhysicians)
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
            this.b1ResourceRequest = new Mutex();
            this.b2ResourceOffer = new Mutex();
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
                Console.WriteLine(this.Name + " ----- " + this.acceptedPatient + " patients -----");
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

        public void RequestRoom()
        {
            this.b1ResourceRequest.WaitOne();
            Console.WriteLine("---> Calling provider for room...");
            //call provider which will check if it can give
            if (!this.provider.ShareRoom())
            {
                Console.WriteLine(this.Name + " : Oops... Provider has no room available for now");
                return;
            }
            lock (roomLock)
            {
                this.roomNb++;
                Console.WriteLine(this.Name + " : \nTotal number of rooms: " + this.roomNb);

            }
            this.Rooms.Release();
            this.b1ResourceRequest.ReleaseMutex();
        }

        public void RequestPhysician()
        {
            this.b1ResourceRequest.WaitOne();
            Console.WriteLine("---> Calling provider for physician...");
            //call provider which will check if it can give
            if (!this.provider.SharePhysician())
            {
                Console.WriteLine(this.Name + " : Oops... Provider has no physician available for now");
                return;
            }
            lock (physicianLock)
            {
                this.physicianNb++;
                Console.WriteLine(this.Name + " : \nTotal number of physician: " + this.physicianNb);
            }

            this.Physicians.Release();
            this.b1ResourceRequest.ReleaseMutex();
        }

        public void ShareRoom()
        {
            this.b2ResourceOffer.WaitOne();
            lock (patientLock)
            {
                if (acceptedPatient > 0)
                {
                    Console.WriteLine(this.Name + " : Oops... who cannot share a room when you have patient in the service");
                    return;
                }
            }
            lock (roomLock)
            {
                if (roomNb <= 0)
                {
                    Console.WriteLine(this.Name + " : Oops... you have no available room for now, maybe you should call the provider");
                    return;
                }

                Rooms.WaitOne();
                this.provider.ReceiveRoom();
                this.roomNb--;
                Console.WriteLine(this.Name + " : \n Total number of rooms: " + this.roomNb);
            }
            this.b2ResourceOffer.ReleaseMutex();
        }

        public void SharePhysician()
        {
            this.b2ResourceOffer.WaitOne();
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
            this.b2ResourceOffer.ReleaseMutex();
        }
    }
}
