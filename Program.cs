using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Detachment
{
    internal class Program
    {
        static void Main(string[] args)
        {
            BattleGround battleGround = new BattleGround();
            battleGround.Fight();
        }
    }

    class BattleGround
    {
        private static Random _random = new Random();
        private Detachment _detachment1 = new Detachment("Бобрия", 0, 0, 28);
        private Detachment _detachment2 = new Detachment("Ослия", 60, 0, 28);

        public void Fight()
        {
            bool isContinue = true;

            while (isContinue)
            {
                Console.Clear();
                _detachment1.SetTargets(SelectTargets(_detachment1, _detachment2));
                _detachment2.SetTargets(SelectTargets(_detachment2, _detachment1));
                ShowInfo(_detachment1);
                ShowInfo(_detachment2);
                _detachment1.TakeDmageList(_detachment2.GetDamageList(_detachment1.Count));
                _detachment2.TakeDmageList(_detachment1.GetDamageList(_detachment2.Count));

                if (_detachment1.Count == 0 || _detachment2.Count == 0)
                {
                    Console.Clear();
                    isContinue = false;
                    ShowInfo(_detachment1);
                    ShowInfo(_detachment2);
                }

                Console.ReadKey(true);
            }

            ShowWinner();
        }

        private void ShowInfo(Detachment detachment)
        {
            detachment.ShowInfo();
        }

        private void ShowWinner()
        {
            Console.Clear();

            if (_detachment2.Count == 0)
            {
                Console.WriteLine($"Победила {_detachment1.CountryName}!");
            }
            else if (_detachment1.Count == 0)
            {
                Console.WriteLine($"Победила {_detachment2.CountryName}!");
            }
        }

        private List<int> SelectTargets(Detachment detachmentAttack, Detachment detachmentDefense)
        {
            List<int> targets = new List<int>(detachmentAttack.Count);

            for (int i = 0; i < detachmentAttack.Count; i++)
            {
                targets.Add(_random.Next(detachmentDefense.Count));
            }

            return targets;
        }
    }

    class Detachment
    {
        private static Random _random = new Random();
        private int _privatePercent = 50;
        private int _stormtrooperPercent = 35;
        private int _sniperPercent = 15;
        private int _positionX;
        private int _positionY;
        private List<int> _targets;
        private List<Soldier> _soldiers = new List<Soldier>();
        private List<Soldier> _killeds = new List<Soldier>();

        public string CountryName { get; }
        public int Count { get { return _soldiers.Count; } }

        public Detachment(string countryName, int positionX, int positionY, int count)
        {
            CountryName = countryName;
            _positionX = positionX;
            _positionY = positionY;

            for (int i = 0; i < count; i++)
            {
                _soldiers.Add(CreateSoldier());
            }
        }

        public void SetTargets(List<int> targets)
        {
            _targets = targets;
        }

        public List<int> GetDamageList(int enemyCount)
        {
            List<int> damages = new List<int>(enemyCount);

            for (int i = 0; i < enemyCount; i++)
            {
                damages.Add(0);
            }

            for (int i = 0; i < _soldiers.Count; i++)
            {
                damages[_targets[i]] += _soldiers[i].Damage;
            }

            return damages;
        }

        public void TakeDmageList(List<int> damages)
        {
            for (int i = 0; i < _soldiers.Count; i++)
            {
                _soldiers[i].TakeDamage(damages[i]);
            }

            PassKilleds();
        }

        public void ShowInfo()
        {
            int positionX = _positionX;
            int positionY = _positionY;

            Console.SetCursorPosition(positionX, positionY++);
            Console.Write($"Солдаты страны: {CountryName}");
            ShowSoldiers(_soldiers, ref positionX, ref positionY);
            ShowSoldiers(_killeds, ref positionX, ref positionY);
        }

        private void PassKilleds()
        {
            List<Soldier> passList = new List<Soldier>();

            foreach (Soldier soldier in _soldiers)
            {
                if (soldier.Health <= 0)
                {
                    passList.Add(soldier);
                }
            }

            foreach (Soldier soldier in passList)
            {
                _soldiers.Remove(soldier);
            }

            _killeds.AddRange(passList);
        }

        private void ShowSoldiers(List<Soldier> list, ref int positionX, ref int positionY)
        {

            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Health > 0)
                {
                    Console.SetCursorPosition(positionX, ++positionY);
                    Console.Write((i + 1) + ") ");
                    _soldiers[i].ShowInfo();
                    Console.Write($" -> {_targets[i] + 1}");
                }
                else
                {
                    Console.SetCursorPosition(positionX, ++positionY);
                    _killeds[i].ShowInfo();
                }
            }
        }

        private Soldier CreateSoldier()
        {
            int hundredPercent = _privatePercent + _stormtrooperPercent + _sniperPercent;
            int randonNumber = _random.Next(hundredPercent + 1);

            if (randonNumber >= 0 && randonNumber < _privatePercent)
            {
                return new Private();
            }
            else if (randonNumber >= _privatePercent && randonNumber < _privatePercent + _stormtrooperPercent)
            {
                return new Stormtrooper();
            }

            return new Sniper();
        }
    }

    class Soldier
    {
        protected string ClassName;
        protected int Armor;
        protected int MaxHealth;
        private static Random _random = new Random();
        private int _characteristicsSpread = 25;

        public int Health { get; private set; }
        public int Damage { get; private set; }

        public Soldier(string cassName, int health, int damage, int anmor)
        {
            ClassName = cassName;
            Health = GetSpread(health);
            MaxHealth = Health;
            Damage = GetSpread(damage);
            Armor = GetSpread(anmor);
        }

        public void TakeDamage(int damage)
        {
            Health -= damage;
        }


        public void ShowInfo()
        {
            if (Health > 0)
            {
                Console.Write($"{ClassName} |HP: {Health} / {MaxHealth}| |DMG:{Damage}| |Armor:{Armor}|");
            }
            else
            {
                Console.Write($"{ClassName} - мёртв");
            }
        }

        private int GetSpread(int value)
        {
            int hundredPercent = 100;
            value += value * _random.Next(-_characteristicsSpread, _characteristicsSpread + 1) / hundredPercent;
            return value;
        }
    }

    class Private : Soldier
    {
        public Private() : base("Рядовой", 1000, 100, 20)
        {

        }
    }

    class Stormtrooper : Soldier
    {
        public Stormtrooper() : base("Штурмовик", 1500, 100, 50)
        {

        }
    }

    class Sniper : Soldier
    {
        public Sniper() : base("Снайпер", 800, 400, 10)
        {

        }
    }
}
