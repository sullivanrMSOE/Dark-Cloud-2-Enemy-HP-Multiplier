using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace DarkCloud2_EnemyHPMultiplier
{
    public class Randomizer
    {      
        static int prevFloor;
        static int currentAddress;     
        static bool exitError;
        static int[] dungeonAreaChestAddressUSA = { 0x20E656A0, 0x20E65D60, 0x20E67220, 0x20E67620, 0x20E681A0, 0x20E679A0, 0x20E68960 };
        static int[] dungeonAreaChestAddressPAL = { 0x20E8A520, 0x20E8ABE0, 0x20E8C0A0, 0x20E8C4A0, 0x20E8D020, 0x20E8C820, 0x20E8D7E0 };
        static int[] dungeonAreaChestAddress;

        static int currentFloorUSA = 0x21ECD638;
        static int currentFloorPAL = 0x21EFC658;
        static int currentFloor;

        static int currentDungeonUSA = 0x20376638;
        static int currentDungeonPAL = 0x2037C828;
        static int currentDungeon;

        static int dungeonCheckAddressUSA = 0x21E9F6E0;
        static int dungeonCheckAddressPAL = 0x21ECE1E0;
        static int dungeonCheckAddress;

        public static int gameVersion = 0;
        public static double multiplier;

        public static int[] floorEnemyHpValues;

        public static void ChestRandomizer(double mult)
        {
            multiplier = mult;
            Console.WriteLine("multiplier: " + multiplier);
            if (gameVersion == 1)
            {
                currentFloor = currentFloorPAL;
                dungeonAreaChestAddress = dungeonAreaChestAddressPAL;
                dungeonCheckAddress = dungeonCheckAddressPAL;
                currentDungeon = currentDungeonPAL;
            }
            else
            {
                currentFloor = currentFloorUSA;
                dungeonAreaChestAddress = dungeonAreaChestAddressUSA;
                dungeonCheckAddress = dungeonCheckAddressUSA;
                currentDungeon = currentDungeonUSA;
            }
            Console.WriteLine("Chest randomizer on");
            while (true)
            {              
                if (Memory.ReadByte(currentFloor) > 0)
                {
                    if (currentFloor != prevFloor)
                    {
                        Console.WriteLine("New floor");
                        Thread.Sleep(6000);
                        if (Memory.ReadByte(currentFloor) == 0 || Memory.ReadByte(dungeonCheckAddress) > 2)
                        {
                            Console.WriteLine("Error, went out of dungeon");
                            exitError = true;
                            break;
                        }
                        
                        Console.WriteLine("Exit error: " + exitError);
                        if (exitError == false)
                        {
                            Console.WriteLine("Multiplying Enemies HP");
                            Thread.Sleep(5000);

                            //Enemy Multiplier Code
                            currentAddress = 0x20D06FE4;
                            floorEnemyHpValues = new int[24]; // 24 Memory Addresses For Enemy Hp
                            for (int i = 0; i < 24; i++)
                            {
                                int prevEnemyHp = BitConverter.ToInt32(Memory.ReadByteArray(currentAddress, 4), 0);
                                Console.WriteLine("Prev enemy hp: " + prevEnemyHp);
                                
                                byte[] newHp = BitConverter.GetBytes(Convert.ToInt32(prevEnemyHp * multiplier));
                                Console.WriteLine("new hp: " + Encoding.Default.GetString(newHp));
                                Memory.Write(currentAddress, newHp);
                                Memory.Write(currentAddress - 0x00000004, newHp); // set new HP of new enemy
                                Console.WriteLine("New enemy hp: " + (prevEnemyHp * multiplier));
                                floorEnemyHpValues[i] = (Convert.ToInt32(prevEnemyHp * multiplier));
                                currentAddress += 0x000014A0;
                            }
                            prevFloor = currentFloor;
                        }
                        exitError = false;
                    }

                    // Newly spawned enemies (Mimics, Tores, Pirate Chariot cannonballs, etc) use earliest empty HP address at spawn
                    // Loop below checks if an originally 0 HP address is no longer 0, then multiplies it
                    if(floorEnemyHpValues != null)
                    {
                        int hpAddress = 0x20D06FE4;
                        for (int i = 0; i < floorEnemyHpValues.Length; i++)
                        {
                            if (floorEnemyHpValues[i] == 0) // If hp was 0 originally
                            {
                                int currentEnemyHp = BitConverter.ToInt32(Memory.ReadByteArray(hpAddress, 4), 0); // get new enemy's HP
                                Console.WriteLine("Multiplying newly spawned enemy hp");
                                byte[] newHp = BitConverter.GetBytes(Convert.ToInt32(currentEnemyHp * multiplier)); // value of new enemy's multiplied HP
                                Memory.Write(hpAddress - 0x00000004, newHp); // set new HP of new enemy
                                Memory.Write(hpAddress, newHp); // set new HP of new enemy
                                floorEnemyHpValues[i] = Convert.ToInt32(currentEnemyHp * multiplier);
                            }
                            hpAddress += 0x000014A0;
                        }
                    }

                    // If enemy dies, update floorEnemyHpValues
                    if (floorEnemyHpValues != null)
                    {
                        int hpAddress = 0x20D06FE4;
                        for(int i = 0; i < floorEnemyHpValues.Length; i++)
                        {
                            int currentEnemyHp = BitConverter.ToInt32(Memory.ReadByteArray(hpAddress, 4), 0);
                            if(currentEnemyHp == 0)
                            {
                                floorEnemyHpValues[i] = 0;
                            }
                            hpAddress += 0x000014A0;
                        }
                    }

                    Console.WriteLine("Sleeping");
                    Thread.Sleep(500);
                }
                else
                {
                    prevFloor = 200;
                }

                if (gameVersion == 1)
                {
                    if (Memory.ReadInt(0x203694D0) != 1701667175)
                    {
                        Thread.Sleep(1000);
                        if (Memory.ReadInt(0x203694D0) != 1701667175)
                        {
                            Program.ExitProgram();
                        }
                    }
                }
                else if (gameVersion == 2) //PAL 
                {

                    if (Memory.ReadInt(0x20364BD0) != 1701667175)
                    {
                        Thread.Sleep(1000);
                        if (Memory.ReadInt(0x20364BD0) != 1701667175)
                        {
                            Program.ExitProgram();
                        }
                    }
                }

                Thread.Sleep(1);
            }
        }
    }
}
