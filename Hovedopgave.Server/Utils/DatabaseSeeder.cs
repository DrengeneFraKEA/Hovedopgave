using Hovedopgave.Server.Database;
using Npgsql;
using System.Diagnostics.Metrics;
using System.Net;
using static Hovedopgave.Server.Models.Roles;
using System.Numerics;
using System.Reflection;
using System.Xml.Linq;

namespace Hovedopgave.Server.Utils
{
    public class DatabaseSeeder
    {
        private List<string> Names = new List<string>() 
        {
            "John",
            "William",
            "James",
            "Charles",
            "George",
            "Frank",
            "Joseph",
            "Thomas",
            "Henry",
            "Robert",
            "Edward",
            "Harry",
            "Walter",
            "Arthur",
            "Fred",
            "Albert",
            "Samuel",
            "David",
            "Louis",
            "Joe",
            "Charlie",
            "Clarence",
            "Richard",
            "Andrew",
            "Daniel",
            "Ernest",
            "Will",
            "Jesse",
            "Oscar",
            "Lewis",
            "Peter",
            "Benjamin",
            "Frederick",
            "Willie",
            "Alfred",
            "Sam",
            "Roy",
            "Herbert",
            "Jacob",
            "Tom",
            "Elmer",
            "Carl",
            "Lee",
            "Howard",
            "Martin",
            "Michael",
            "Bert",
            "Herman",
            "Jim",
            "Francis",
            "Harvey",
            "Earl",
            "Eugene",
            "Ralph",
            "Ed",
            "Claude",
            "Edwin",
            "Ben",
            "Charley",
            "Paul",
            "Edgar",
            "Isaac",
            "Otto",
            "Luther",
            "Lawrence",
            "Ira",
            "Patrick",
            "Guy",
            "Oliver",
            "Theodore",
            "Hugh",
            "Clyde",
            "Alexander",
            "August",
            "Floyd",
            "Homer",
            "Jack",
            "Leonard",
            "Horace",
            "Marion",
            "Philip",
            "Cornelius",
            "Felix",
            "Reuben",
            "Wallace",
            "Claud",
            "Roscoe",
            "Sylvester",
            "Earnest",
            "Hiram",
            "Otis",
            "Simon",
            "Willard",
            "Irvin",
            "Mark",
            "Jose",
            "Wilbur",
            "Abraham",
            "Virgil",
            "Clinton",
            "Elbert",
            "Leslie",
            "Marshall",
            "Owen",
            "Wiley",
            "Anton",
            "Morris",
            "Manuel",
            "Phillip",
            "Augustus",
            "Emmett",
            "Eli",
            "Nicholas",
            "Wilson",
            "Alva",
            "Harley",
            "Newton",
            "Timothy",
            "Marvin",
            "Ross",
            "Curtis",
            "Edmund",
            "Jeff",
            "Elias",
            "Harrison",
            "Stanley",
            "Columbus",
            "Lon",
            "Ora",
            "Ollie",
            "Pearl",
            "Russell",
            "Solomon",
            "Arch",
            "Asa",
            "Clayton",
            "Enoch",
            "Irving",
            "Mathew",
            "Nathaniel",
            "Scott",
            "Hubert",
            "Lemuel",
            "Andy",
            "Ellis",
            "Emanuel",
            "Joshua",
            "Millard",
            "Vernon",
            "Wade",
            "Cyrus",
            "Miles",
            "Rudolph",
            "Sherman",
            "Austin",
            "Bill",
            "Chas",
            "Lonnie",
            "Monroe",
            "Byron",
            "Edd",
            "Emery",
            "Grant",
            "Jerome",
            "Max",
            "Mose",
            "Steve",
            "Gordon",
            "Abe",
            "Pete",
            "Chris",
            "Clark",
            "Gustave",
            "Orville",
            "Lorenzo",
            "Bruce",
            "Marcus",
            "Preston",
            "Bob",
            "Dock",
            "Donald",
            "Jackson",
            "Cecil",
            "Barney",
            "Delbert",
            "Edmond",
            "Anderson",
            "Christian",
            "Glenn",
            "Jefferson",
            "Luke",
            "Neal",
            "Burt",
            "Ike",
            "Myron",
            "Tony",
            "Conrad",
            "Joel",
            "Matt",
            "Riley",
            "Vincent",
            "Emory",
            "Isaiah",
            "Nick",
            "Ezra",
            "Green",
            "Juan",
            "Clifton",
            "Lucius",
            "Porter",
            "Arnold",
            "Bud",
            "Jeremiah",
            "Taylor",
            "Forrest",
            "Roland",
            "Spencer",
            "Burton",
            "Don",
            "Emmet",
            "Gustav",
            "Louie",
            "Morgan",
            "Ruben",
            "Hugo"
        };

        private List<string> Surnames = new List<string>()
        {
            "Smith",
            "Johnson",
            "Smithson",
            "Jefferson",
            "Blackson",
            "Yellowson",
            "Springson",
            "Garcia",
            "Martinez",
            "Lopez",
            "Thompson",
            "Richardson",
            "Harris",
            "Walker",
            "Mitchell",
            "Roberts",
            "Robinson",
            "Lewis",
            "White",
            "Black",
            "Brown",
            "Green",
            "Blue",
            "Red",
            "Pink",
            "Carter",
            "Nelson",
            "Perez",
            "Turner",
            "Parker",
            "Edwards",
            "Evans",
            "Howard",
            "Kelly",
            "Brooks",
            "Flores",
            "Ivanovitch",
            "Dimitri",
            "Donaldsob",
            "Wick",
            "Freeman",
            "Murray",
            "Cole",
            "Holmes",
            "Hunter",
            "Daniels",
            "Johnston",
            "Williamson",
            "Alvarez",
            "Ryan",
            "Schmidt",
            "Chavez",
            "Kawalski",
            "Ivanovski",
            "Hong-Mei",
            "Fang-Mai",
            "Lou-chu",
            "Chu",
            "Fai",
            "Fai-Mei"
        };

        private List<string> Adjectives = new List<string>() 
        {
            "Delightful",
            "Merciful",
            "Ridicilous",
            "Spontanous",
            "Helpful",
            "Disagreeable",
            "Dark",
            "Blue",
            "Green",
            "Red",
            "Yellow",
            "Brown",
            "Black",
            "Blue",
            "Warm",
            "Cold",
            "Angry",
            "Soft",
            "Calm",
            "Suspicious",
            "Round",
            "Circular",
            "Heavy",
            "Happy",
            "Mad",
            "Scared",
            "Agreeable",
            "Simple",
            "Difficult",
            "Hard",
            "Fast",
            "Slow",
            "Insane",
            "Spinning",
            "Rotating",
            "Blinding",
            "Square",
            "Rectangular",
            "Wet",
            "Dry",
            "Drenched",
            "Damp",
            "Soaked",
            "Dusty",
            "Clean",
            "Disgusting",
            "Nasty",
            "Fresh",
            "Orange",
            "Refreshing",
            "Sweet",
            "Sour",
            "Bitter",
            "Super",
            "Nice",
            "Nervous",
            "Obnoxious",
            "Jelous",
            "Eager",
            "Gentle",
            "Lively",
            "Kind",
            "Polite",
            "Silly",
            "Attractive",
            "Bald",
            "Dazzling",
            "Fancy",
            "Fit",
            "Handsome",
            "Muscular",
            "Skinny",
            "Big",
            "Gigantic",
            "Massive",
            "Petite",
            "Puny",
            "Scrawny",
            "Small",
            "Fat",
            "Short",
            "Gifted",
            "Healthy",
            "Horrible",
            "Itchy",
            "Odd",
            "Real",
            "Rich",
            "Sleepy",
            "Sore",
            "Perfect",
            "Modern",
            "Stupid",
            "Talented",
            "Wicked",
            "Drunk",
            "Loud",
            "Crazy",
            "Rusty",
            "Bloody",
            "Creepy",
            "Savage",
            "Smelly",
            "Poor",
            "Elegant",
            "Moody",
            "Silver",
            "Golden",
            "Wavy",
            "Windy",
            "Crispy",
            "Tasty",
            "Frozen",
            "Hot",
            "Lucky",
            "Spicy",
            "Wholesome",
        };

        private List<string> Objects = new List<string>()
        {
            "Chair",
            "Box",
            "Cardbox",
            "Glass",
            "Water",
            "Sky",
            "Car",
            "Racecar",
            "BMW",
            "Window",
            "Tree",
            "Leaf",
            "Branch",
            "Air",
            "Roof",
            "Bus",
            "Couch",
            "Table",
            "Mouse",
            "Giraffe",
            "Aligator",
            "Monk",
            "Spoon",
            "Fork",
            "Plate",
            "Bicycle",
            "Barber",
            "Hairdresser",
            "Bird",
            "Angel",
            "Devil",
            "Cloud",
            "Cable",
            "Wire",
            "Insect",
            "Spider",
            "Snake",
            "Forest",
            "Jungle",
            "Sand",
            "Pyramid",
            "Sun",
            "Star",
            "House",
            "Building",
            "Street",
            "Walkway",
            "Runner",
            "Walker",
            "Sprinter",
            "Jumper",
            "Lifter",
            "Squeezer",
            "Liar",
            "Man",
            "Woman",
            "Lion",
            "Tiger",
            "Cow",
            "Kid",
            "Monkey",
            "Trash",
            "Ape",
            "Gorilla",
            "Zebra",
            "Drum",
            "Guitar",
            "Stick",
            "Flute",
            "Pants",
            "Jacket",
            "Gloves",
            "Weapon",
            "Backpack",
            "Fireplace",
            "Barrel",
            "Helicopter",
            "Airplane",
            "Plane",
            "Grass",
            "Apple",
            "Banana",
            "Cucumber",
            "Tomato",
            "Fruit",
            "Carrot",
            "Bread",
            "Pizza",
            "Onion",
            "Salad",
            "Ketchup",
            "Soda",
            "Bench",
            "Sugar",
            "Salt",
            "Pepper",
            "Mint",
            "Ship",
            "Captain",
            "Pirate",
            "Soldier",
            "Warrior",
            "Thief",
            "Baker",
            "Lawyer",
            "Doctor",
            "Pilot",
            "Electrician",
            "Driver",
            "Sword",
            "Dagger",
            "Fish",
            "Squid",
            "Elephant",
            "Mosquito",
            "Drink",
            "Beverage",
            "Beer",
            "Wheel",
            "Helmet",
            "Kneepad",
            "Shoe",
            "Shoestring",
            "Nugget",
            "Girl",
            "Boy",
            "Machine",
            "Rat",
            "Cobra",
            "Ninja",
            "Samurai",
            "Cowboy",
            "King",
            "Queen",
            "Princess",
            "Prince",
            "Castle",
            "Cannon",
            "Spear",
            "Shield",
            "Criminal",
            "Builder",
            "Carpenter",
            "Teacher",
            "Milk",
            "Teeth",
            "Bear",
            "Wolf",
            "Human",
            "Enemy"
        };

        private List<string> PluralObjects = new List<string>()
        {
            "Kings",
            "Dragons",
            "Lions",
            "Ninjas",
            "Cobras",
            "Snakes",
            "Tigers",
            "Eagles",
            "Knights",
            "Warriors",
            "Survivors",
            "Fighters",
            "Hitters",
            "Hitmen",
            "Assassins",
            "Snipers",
            "Shooters",
            "Thiefs",
            "Robbers",
            "Soldiers",
            "Pirates",
            "Sailors",
            "Stars",
            "Planets",
            "Bears",
            "T1",
            "T2",
            "T3",
            "K1",
            "K2",
            "K3",
            "TR1",
            "TR2",
            "TR3",
            "JP1",
            "JP2",
            "JP3"
        };

        private List<string> Gender = new List<string>()
        {
            "male",
            "male",
            "male",
            "male",
            "male",
            "female",
            "male",
            "male",
            "male",
            "male",
            "male"
        };

        private List<string> Country = new List<string>()
        {
            "DK",
            "SE",
            "NO",
            "DE",
            "PL",
            "NL",
            "FR",
            "FI",
            "IN",
            "LT",
            "MA",
            "RU",
            "SA",
            "TH",
            "CN",
            "TR",
            "GR",
            "IT",
            "ES",
            "PT",
            "KR"
        };

        private List<string> Regions = new List<string>()
        {
            "EUNE",
            "EUW",
            "NA",
            "KR",
            "CN",
            "JP",
            "TR"
        };

        public void SeedUsers(int amount) 
        {
            Random rnd = new Random();
            HashSet<string> hashset = new HashSet<string>();

            for(int i = 0; i<amount; i++) 
            {
                int rndNameIndex = rnd.Next(Names.Count);
                int rndNameIndex2 = rnd.Next(Names.Count);
                int rndAdjectiveIndex = rnd.Next(Adjectives.Count);
                int rndObjectIndex = rnd.Next(Objects.Count);
                int rndNumber = rnd.Next(0, 99);
                int rndPhoneNumber = rnd.Next(100000, 999999);
                int rndCountryCode = rnd.Next(1, 99);
                int rndCountryIndex = rnd.Next(Country.Count);
                int rndGenderIndex = rnd.Next(Gender.Count);
                int rndDiscordId = rnd.Next(0, 999999);

                string id = Guid.NewGuid().ToString("n").Substring(0, 30);
                string name = $"{Names.ElementAt(rndNameIndex)} {Names.ElementAt(rndNameIndex2)}";
                string display_name = $"{Adjectives.ElementAt(rndAdjectiveIndex)}{Objects.ElementAt(rndObjectIndex)}{rndNumber}";
                string role = "user";
                string gender = $"{Gender.ElementAt(rndGenderIndex)}";
                string email = $"{display_name.ToLower()}@leagues.gg";
                int phoneExt = rndCountryCode;
                string phone = $"{rndPhoneNumber}";
                string country = $"{Country.ElementAt(rndCountryIndex)}";
                string discordId = $"{rndDiscordId}";
                string birthday = RandomDate(1985, 2010);
                string createdAt = RandomDate(2022, 2024);
                string updatedAt = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss").Replace('.', ':');

                string salt;
                string hashedPw = PasswordHandler.GenerateSaltAndHashedPassword(display_name.ToLower(), out salt);

                // If hashset contains display_name, skip and try again.
                if (hashset.Contains(display_name)) 
                {
                    i--;
                    continue;
                }

                hashset.Add(display_name);

                PostgreSQL psql = new PostgreSQL(true); // change to false once azure is up
                using NpgsqlDataSource conn = NpgsqlDataSource.Create(psql.connectionstring);

                using var command = conn.CreateCommand($"INSERT INTO users (id, full_name, display_name, role, gender, email, password_salt, password, phone_ext, phone, country, discord_id, birthday, created_at, updated_at)" +
                    $"VALUES ('{id}', '{name}', '{display_name}', '{role}', '{gender}', '{email}', '{salt}', '{hashedPw}', '{phoneExt}', '{phone}', '{country}', '{discordId}', '{birthday}', '{createdAt}', '{updatedAt}')");
                try
                {
                    command.ExecuteNonQuery();
                }
                catch (Exception e)
                {

                }
            }
        }

        public void SeedOrganizations(int amount) 
        {
            Random rnd = new Random();
            HashSet<string> hashset = new HashSet<string>();

            for (int i = 0; i < amount; i++) 
            {
                int rndAdjectiveIndex = rnd.Next(Adjectives.Count);
                int rndPluralObjectIndex = rnd.Next(PluralObjects.Count);
                int rndRegionIndex = rnd.Next(Regions.Count);
                int rndCountryIndex = rnd.Next(Country.Count);

                string id = Guid.NewGuid().ToString("n").Substring(0, 30);
                string orgName = $"{Adjectives.ElementAt(rndAdjectiveIndex)} {PluralObjects.ElementAt(rndPluralObjectIndex)}";
                string region = $"{Regions.ElementAt(rndRegionIndex)}";
                string country = $"{Country.ElementAt(rndCountryIndex)}";
                string summary = $"{orgName} is a company out of {country} that has been active at the Esports scene for years!";
                string description = $"{orgName} is a company out of {country} that has been active at the Esports scene for years!";
                string createdAt = RandomDate(2022, 2024);
                string updatedAt = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss").Replace('.', ':');


                // If hashset contains display_name, skip and try again.
                if (hashset.Contains(orgName))
                {
                    i--;
                    continue;
                }

                hashset.Add(orgName);

                PostgreSQL psql = new PostgreSQL(true); // change to false once azure is up
                using NpgsqlDataSource conn = NpgsqlDataSource.Create(psql.connectionstring);

                using var command = conn.CreateCommand($"INSERT INTO organizations (id, name, region, country, summary, description, created_at, updated_at)" +
                $"VALUES ('{id}', '{orgName}', '{region}', '{country}', '{summary}', '{description}', '{createdAt}', '{updatedAt}')");
                try
                {
                    command.ExecuteNonQuery();
                }
                catch (Exception e)
                {

                }
            }
        }

        public void SeedTeams(int amount) 
        {
            Random rnd = new Random();
            HashSet<string> hashset = new HashSet<string>();

            for (int i = 0; i < amount; i++)
            {
                int rndAdjectiveIndex = rnd.Next(Adjectives.Count);
                int rndPluralObjectIndex = rnd.Next(PluralObjects.Count);
                int rndCountryIndex = rnd.Next(Country.Count);

                string id = Guid.NewGuid().ToString("n").Substring(0, 30);
                string teamName = $"{Adjectives.ElementAt(rndAdjectiveIndex)} {PluralObjects.ElementAt(rndPluralObjectIndex)}";
                string country = $"{Country.ElementAt(rndCountryIndex)}";
                string initials = $"{Adjectives.ElementAt(rndAdjectiveIndex).Substring(0,1)}{PluralObjects.ElementAt(rndPluralObjectIndex).Substring(0,1)}";
                string game = "league-of-legends";
                string createdAt = RandomDate(2022, 2024);
                string updatedAt = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss").Replace('.', ':');


                // If hashset contains display_name, skip and try again.
                if (hashset.Contains(teamName))
                {
                    i--;
                    continue;
                }

                hashset.Add(teamName);

                PostgreSQL psql = new PostgreSQL(true); // change to false once azure is up
                using NpgsqlDataSource conn = NpgsqlDataSource.Create(psql.connectionstring);

                using var command = conn.CreateCommand($"INSERT INTO teams (id, name, initials, game, country, created_at, updated_at)" +
                $"VALUES ('{id}', '{teamName}', '{initials}', '{game}', '{country}', '{createdAt}', '{updatedAt}')");
                try
                {
                    command.ExecuteNonQuery();
                }
                catch (Exception e)
                {

                }
            }
        }

        private string RandomDate(int startYear, int endYear) 
        {
            Random rnd = new Random();
            DateTime start = new DateTime(startYear, 1, 1);
            DateTime end = new DateTime(endYear, 12, 31);
            int range = (end - start).Days;
            return start.AddDays(rnd.Next(range)).ToString("yyyy-MM-dd hh:mm:ss").Replace('.', ':');
        }
    }
}
