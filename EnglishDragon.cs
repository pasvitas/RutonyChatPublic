using System.IO;
using System;
using System.Collections.Generic;
using System.Threading;
using Newtonsoft.Json;

//by pasvitas. twitch.tv/pasvitas

namespace RutonyChat {
    public class Script {
        public void RunScript(string site, string username, string text, string param) {
			string filename = ProgramProps.dir_scripts + @"\dragon.txt";
			string file = ProgramProps.dir_scripts + @"\dragonwarriors.json";
			if (!File.Exists(filename))
			{
                RutonyBot.BotSay(site, "The dragon has not yet appeared! Ask the administrator about this!");
				return;
			}

			string[] hp = File.ReadAllLines(filename);
			int currenthp = Convert.ToInt32(hp[0]);
			RankControl.ChatterRank strlist = RankControl.ListChatters.Find(r => r.Nickname == username.Trim().ToLower());

			if (strlist == null) {
                RutonyBot.BotSay(site,
                    username + ", error! Your record is not in the database or it's corrupted!!");
                return;
            }

			int str = strlist.RankOrder;
			
			Random rnd = new Random();
			int rndstr = rnd.Next(1, 11);
			int krit = 1;
			
			switch (rndstr) 
			{	
				case 1: 
				
					currenthp -= (str+2)*3;
					krit = 3;
					break;
					
				case 2: 
				
					RutonyBot.BotSay(site, string.Format("{0} misses the dragon!", username));
					break;
				
				case 3:
				
					RutonyBot.BotSay(site, string.Format("The dragon blocks the blow {0}!", username));
					//RutonyBot.BotSay(site, "/timeout " + username + " 30"); 
					//Uncomment line above for twitch timeout
					break;
				case 4:
					currenthp += 5;
					RutonyBot.BotSay(site, string.Format("The dragon avoids the blow {0} and restores the forces!(+5 hp)!", username));
					try {
                        File.Delete(filename);
					} catch { }
					RutonyBotFunctions.FileAddString(filename, string.Format("{0}",currenthp));
					break;
				default:
					currenthp -= (str+2);
					break;
			}
			
			if (rndstr > 1 && rndstr <=4)
			{
				
				return;
			}
			AddWarrior(username, (str+2)*krit, site);
			
			if (currenthp > 0)
			{
				RutonyBot.BotSay(site, string.Format("{0} beats the dragon for {1} damage! The dragon has {2} health left!", username, (str+2)*krit, currenthp));
				try {
                        File.Delete(filename);
                } catch { }
				RutonyBotFunctions.FileAddString(filename, string.Format("{0}",currenthp));
				
			}
			else
			{
				RutonyBot.BotSay(site, string.Format("{0} finishes the dragon! All participants receive loans from his treasury!", username));
				RankControl.ChatterRank cr_lasthit = RankControl.ListChatters.Find(r => r.Nickname == username.Trim().ToLower());
				cr_lasthit.CreditsQty += 50;
				Warriors players = GetListWarriors();
				foreach (Warrior player in players.ListWarriors)
        		{
					

					RankControl.ChatterRank cr_win = RankControl.ListChatters.Find(r => r.Nickname == player.name);
					cr_win.CreditsQty += Convert.ToInt32(player.damage);
					//RutonyBot.BotSay(site, player.name + " получил " + player.damage + " кредитов!");
					//Uncomment line above to output how many each participant received 
				}

				
				
				try {
                        File.Delete(file);
				} catch { }

				try {
                        File.Delete(filename);
                } catch { }
				
			}
			
			
			
			
		}
		public Warriors GetListWarriors()
		{
			string file = ProgramProps.dir_scripts + @"\dragonwarriors.json";

			Warriors players = new Warriors();

            if (File.Exists(file))
			{
                string[] filetexts = File.ReadAllLines(file);

			    players = JsonConvert.DeserializeObject<Warriors>(filetexts[0]);
				
			}

			return players;
		}

		public void AddWarrior(string username, int vklad, string site)
		{
			string file = ProgramProps.dir_scripts + @"\dragonwarriors.json";
			Warriors players = GetListWarriors();

			Warrior thiswarrior = players.ListWarriors.Find(r => r.name == username.Trim().ToLower());

			if (thiswarrior == null) {
                
                players.ListWarriors.Add(new Warrior() {name=username.Trim().ToLower(), damage = vklad});
                thiswarrior = players.ListWarriors.Find(r => r.name == username.Trim().ToLower());
				

				try {
                        File.Delete(file);
             	 } catch { }

            	string serialized = JsonConvert.SerializeObject(players);
            	RutonyBotFunctions.FileAddString(file, serialized);


                
            }
			else
			{

				thiswarrior.damage += vklad;

				try {
                        File.Delete(file);
             	 } catch { }

            	string serialized = JsonConvert.SerializeObject(players);
            	RutonyBotFunctions.FileAddString(file, serialized);

			}

		}	
    }
	public class Warriors
    {
        public List<Warrior> ListWarriors = new List<Warrior>();
    }

    public class Warrior
	{

		public string name {get; set;}
        public int damage {get; set;}

	}
}