using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using StackExchange.Redis;
using Newtonsoft.Json;
using ReflectionAndCaching;

namespace CompareObjects
{
    class Program
    {
        static void Main(string[] args)
        {
            Car car1 = new Car();
            car1.CarID = 777;
            car1.CarName = "2107";
            car1.Engine = new Engine();
            car1.Engine.Id = "12345";
            car1.Engine.Power = 87;

            Car car2 = new Car();
            car2.CarID = 555;
            car2.CarName = "2107";
            car2.Engine = new Engine();
            car2.Engine.Id = "54321";
            car2.Engine.Power = 87;

            Car car3 = new Car();
            car3.CarID = 777;
            car3.CarName = "2107";
            car3.Engine = new Engine();
            car3.Engine.Id = "12345";
            car3.Engine.Power = 87;

            Console.WriteLine(ReflectionEqualityComparer.Equality<Car>(car1, car2));
            Console.WriteLine(ReflectionEqualityComparer.Equality<Car>(car1, car3));

            var cacheRedisService = new CacheRedisService<Engine>();
            Engine engine = new Engine() { Id = "MSTPWRENGN", Power = 220 };
            Console.WriteLine("Before caching and serialization:" + engine.Id + " " + engine.Power);
            cacheRedisService.Put(engine, "SuperKey");
            Console.WriteLine("Get data from redis cache");
            Console.WriteLine("After caching and deserialization:" + cacheRedisService.Get("SuperKey").Id + " " + cacheRedisService.Get("SuperKey").Power);

            Console.ReadLine();
        }

    }

    public class Car
    {
        private int carID;
        private string carName;
        private Engine engine;

        public int CarID
        {
            set { carID = value; }
            get { return carID; }
        }
        public string CarName
        {
            set { carName = value; }
            get { return carName; }
        }
        public Engine Engine
        {
            set { engine = value; }
            get { return engine; }
        }
    }

    public class Engine
    {
        private string id;
        private int power;

        public string Id
        {
            set { id = value; }
            get { return id; }
        }

        public int Power
        {
            set { power = value; }
            get { return power; }
        }
    }
}
